using System.Buffers;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.System.Wmi;
using static Hardware.Info.Aot.Windows.Constants;

namespace Hardware.Info.Aot.Windows
{
    delegate HRESULT ExtractValueDelegate<T>(in VARIANT variant, out T value);
    delegate HRESULT ExtractArrayValueDelegate<T>(in VARIANT variant, Span<T> value, out uint length);


    class ManagementObjectSearcher : IDisposable
    {
        private readonly int _timeout;
        private SafeHandle _managementScope;
        private SafeHandle _queryString;
        private readonly SafeHandle _strQueryLanguage;

        public ManagementObjectSearcher(string managementScope, string queryString, int timeout)
        {
            _timeout = timeout;

            _managementScope = PInvoke.SysAllocString(managementScope);
            _queryString = PInvoke.SysAllocString(queryString);

            _strQueryLanguage = PInvoke.SysAllocString("WQL");
        }

        public unsafe WmiSearch Get()
        {
            // Obtain the initial locator to WMI
            IWbemLocator* pLoc;
            var hr = PInvoke.CoCreateInstance(
                CLSID_WbemLocator,
                (IUnknown*)0,
                CLSCTX.CLSCTX_INPROC_SERVER,
                out pLoc
            );

            if (hr.Failed)
            {
                PInvoke.CoUninitialize();
                hr.ThrowOnFailure();
                return WmiSearch.Null;
            }

            // Connect to WMI through the IWbemLocator::ConnectServer method
            IWbemServices* pSvc;

            hr = pLoc->ConnectServer(
                _managementScope,
                new SysFreeStringSafeHandle(0), // User name
                new SysFreeStringSafeHandle(0), // Password
                new SysFreeStringSafeHandle(0), // Locale
                0, // Security flags
                new SysFreeStringSafeHandle(0), // Authority
                (IWbemContext*)0, // Context
                &pSvc // IWbemServices proxy
            );

            if (hr.Failed)
            {
                pLoc->Release();
                PInvoke.CoUninitialize();
                return WmiSearch.Null;
            }

            // Set security levels on the proxy
            hr = PInvoke.CoSetProxyBlanket(
                (IUnknown*)pSvc,
                RPC_C_AUTHN_WINNT,
                RPC_C_AUTHZ_NONE,
                new PWSTR((char*)0),
                RPC_C_AUTHN_LEVEL.RPC_C_AUTHN_LEVEL_CALL,
                RPC_C_IMP_LEVEL.RPC_C_IMP_LEVEL_IMPERSONATE,
                (void*)0,
                EOLE_AUTHENTICATION_CAPABILITIES.EOAC_NONE
            );

            if (hr.Failed)
            {
                pSvc->Release();
                pLoc->Release();
                PInvoke.CoUninitialize();
                return WmiSearch.Null;
            }

            // Perform WMI query to get the Win32_OperatingSystem class

            IEnumWbemClassObject* pEnumerator;
            hr = pSvc->ExecQuery(
                _strQueryLanguage,
                _queryString,
                WBEM_GENERIC_FLAG_TYPE.WBEM_FLAG_FORWARD_ONLY | WBEM_GENERIC_FLAG_TYPE.WBEM_RETURN_IMMEDIATELY,
                (IWbemContext*)0,
                &pEnumerator
            );

            if (hr.Failed)
            {
                pSvc->Release();
                pLoc->Release();
                PInvoke.CoUninitialize();
                return WmiSearch.Null;
            }

            return new WmiSearch(
                pSvc, pLoc, pEnumerator, _timeout);
        }

        public void Close()
        {
            PInvoke.CoUninitialize();
        }


        private void ReleaseUnmanagedResources()
        {
            _queryString.Dispose();
            _strQueryLanguage.Dispose();
            _managementScope.Dispose();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ManagementObjectSearcher()
        {
            Dispose(false);
        }
    }

    public unsafe ref struct WmiSearchResultItem
    {
        private readonly IWbemClassObject* _item;

        internal WmiSearchResultItem(IWbemClassObject* item)
        {
            _item = item;
        }

        public unsafe T GetProperty<T>(string name, T defaultValue = default!)
        {
            VARIANT vtProp = new VARIANT();

            var hr = _item->Get(name, 0, ref vtProp, (int*)0, (int*)0);

            if (hr.Value == WBEM_E_NOT_FOUND || vtProp.Anonymous.Anonymous.vt == VARENUM.VT_NULL)
            {
                if (typeof(T) == typeof(string))
                {
                    return defaultValue ?? (T)(object)string.Empty;
                }

                return defaultValue;
            }

            hr.ThrowOnFailure();

            try
            {
                if ((vtProp.Anonymous.Anonymous.vt & VARENUM.VT_ARRAY) == VARENUM.VT_ARRAY)
                {
                    throw new InvalidOperationException(
                        $"Property {name} is an array of values.");
                }

                if (typeof(T) == typeof(string))
                {
                    if (vtProp.Anonymous.Anonymous.vt != VARENUM.VT_BSTR)
                    {
                        throw new InvalidOperationException(
                            $"Property {name} is of type {vtProp.Anonymous.Anonymous.vt} and not BSTR.");
                    }

                    var strValue = vtProp.Anonymous.Anonymous.Anonymous.bstrVal.AsSpan().ToString();
                    
                    return (T)(object)strValue;
                }

                if (typeof(T) == typeof(ushort))
                {
                    var value = (T)(object)this.ExtractValue<ushort>(name, ref vtProp, VARENUM.VT_UI2, PInvoke.VariantToUInt16);
                    
                    return value;
                }

                if (typeof(T) == typeof(uint))
                {
                    var value = (T)(object)this.ExtractValue<uint>(name, ref vtProp, VARENUM.VT_I4, PInvoke.VariantToUInt32);
                    
                    return value;
                }

                if (typeof(T) == typeof(int))
                {
                    var value = (T)(object)this.ExtractValue<int>(name, ref vtProp, VARENUM.VT_I4, PInvoke.VariantToInt32);
                    
                    return value;
                }

                if (typeof(T) == typeof(UInt64))
                {
                    var value = (T)(object)this.ExtractValue<UInt64>(name, ref vtProp, VARENUM.VT_UI8, PInvoke.VariantToUInt64);
                    
                    return value;
                }

                if (typeof(T) == typeof(byte))
                {
                    var value = (T)(object)this.ExtractValue<byte>(name, ref vtProp, VARENUM.VT_UI1,
                        (in VARIANT v, out byte value) =>
                        {
                            value = v.Anonymous.Anonymous.Anonymous.bVal;
                            return new HRESULT(0);
                        });
                    
                    return value;
                }

                if (typeof(T) == typeof(bool))
                {
                    var boolValue = this.ExtractValue<BOOL>(name, ref vtProp, VARENUM.VT_BOOL, PInvoke.VariantToBoolean);
                    
                    return (T) (object) (bool) boolValue;
                }
                
                throw new NotSupportedException($"Type {typeof(T).FullName} is not supported.");
            }
            finally
            {
                PInvoke.VariantClear(ref vtProp);
            }
        }

        private T ExtractValue<T>(string propertyName, ref VARIANT variant, VARENUM expectedType, ExtractValueDelegate<T> extractFunc) where T:struct
        {
            if (variant.Anonymous.Anonymous.vt != expectedType)
            {
                throw new InvalidOperationException(
                    $"Property {propertyName} is of type {variant.Anonymous.Anonymous.vt} and not {typeof(T).FullName}.");
            }

            T value;
            extractFunc(in variant, out value);

            return (T)value;
        }

        private bool IsVariantOfType(in VARIANT variant, VARENUM expectedType)
        {
            return (variant.Anonymous.Anonymous.vt & expectedType) == expectedType;
        }

        private T[] ExtractArrayValue<T>(ref VARIANT variant, ExtractArrayValueDelegate<T> extractFunc) where T : unmanaged
        {
            Span<T> array = stackalloc T[10];
            extractFunc(in variant, array, out uint length);

            return array.Slice(0, (int)length).ToArray();
        }

        public enum ArrayPropertyError
        {
            None,
            NullProperty,
            NotArrayType,
            UnsupportedType,
            InvalidStringType,
            InvalidUShortType,
            InvalidIntType,
            InteropError
        }

        public bool TryGetArrayProperty<T>(string name, out T[] value, out ArrayPropertyError errorReason)
        {
            VARIANT vtProp = new VARIANT();
            
            var hr = _item->Get(name, 0, ref vtProp, (int*)0, (int*)0);

            // Default the output values
            value = Array.Empty<T>();
            errorReason = ArrayPropertyError.None;

            if (hr.Failed)
            {
                errorReason = ArrayPropertyError.InteropError;
                return false;
            }

            try
            {
                if (vtProp.Anonymous.Anonymous.vt == VARENUM.VT_NULL)
                {
                    errorReason = ArrayPropertyError.NullProperty;
                    return true;
                }

                if ((vtProp.Anonymous.Anonymous.vt & VARENUM.VT_ARRAY) != VARENUM.VT_ARRAY)
                {
                    errorReason = ArrayPropertyError.NotArrayType;
                    return false;
                }

                if (typeof(T) == typeof(string))
                {
                    if ((vtProp.Anonymous.Anonymous.vt & VARENUM.VT_BSTR) != VARENUM.VT_BSTR)
                    {
                        errorReason = ArrayPropertyError.InvalidStringType;
                        return false;
                    }

                    const uint BUFFER_SIZE = 10;
                    Span<PWSTR> buffer = stackalloc PWSTR[(int)BUFFER_SIZE];
                    uint usedBufferLength;

                    hr = PInvoke.VariantToStringArray(in vtProp, buffer, out usedBufferLength);

                    if (hr.Failed)
                    {
                        errorReason = ArrayPropertyError.InteropError;
                    
                        return false;
                    }

                    var usedArray = buffer.Slice(0, (int)usedBufferLength);
                    var strArray = new string[usedArray.Length];
                    for (var i = 0; i < usedArray.Length; i++)
                    {
                        strArray[i] = usedArray[i].AsSpan().ToString();
                    }
                
                    value = (T[])(object)strArray;
                    return true;
                }

                if (typeof(T) == typeof(ushort))
                {
                    if (!IsVariantOfType(in vtProp, VARENUM.VT_UI2))
                    {
                        errorReason = ArrayPropertyError.InvalidUShortType;
                        return false;
                    }
                
                    value = (T[])(object)this.ExtractArrayValue<ushort>(ref vtProp, PInvoke.VariantToUInt16Array);
                    return true;
                }

                if (typeof(T) == typeof(int))
                {
                    if (!IsVariantOfType(in vtProp, VARENUM.VT_I4))
                    {
                        errorReason = ArrayPropertyError.InvalidIntType;
                        return false;
                    }
                
                    value = (T[])(object)this.ExtractArrayValue<int>(ref vtProp, PInvoke.VariantToInt32Array);
                    return true;
                }

                errorReason = ArrayPropertyError.UnsupportedType;
                return false;
            }
            finally
            {
                PInvoke.VariantClear(ref vtProp);
            }
        }
        
        public static void HandleArrayPropertyError<T>(string name, ArrayPropertyError errorReason)
        {
            switch (errorReason)
            {
                case ArrayPropertyError.NullProperty:
                    return; // Null property is acceptable, just return (equivalent to returning an empty array).
                case ArrayPropertyError.NotArrayType:
                    throw new InvalidOperationException($"Property {name} is not an array of values.");
                case ArrayPropertyError.InvalidStringType:
                    throw new InvalidOperationException($"Property {name} is of type BSTR and not an array of strings.");
                case ArrayPropertyError.InvalidUShortType:
                    throw new InvalidOperationException($"Property {name} is not an array of ushort values.");
                case ArrayPropertyError.InvalidIntType:
                    throw new InvalidOperationException($"Property {name} is not an array of int values.");
                case ArrayPropertyError.UnsupportedType:
                    throw new NotSupportedException($"Type {typeof(T).FullName} is not supported.");
                case ArrayPropertyError.InteropError:
                    throw new NotSupportedException($"Property value from {name} could not be extracted.");
                default:
                    throw new InvalidOperationException($"Failed to get property {name}.");
            }
        }

        public T[] GetArrayProperty<T>(string name)
        {
            if (TryGetArrayProperty(name, out T[] value, out ArrayPropertyError errorReason))
            {
                return value;
            }

            // Call the new helper function for error handling
            HandleArrayPropertyError<T>(name, errorReason);

            // If no exception is thrown, return an empty array (for NullProperty case)
            return Array.Empty<T>();
        }


    }

    public unsafe ref struct WmiSearchResultsEnumerator
    {
        private Memory<IntPtr> _currentArray;
        private MemoryHandle _currentArrayHandle;
        private bool _isCurrentInUse;
        private IEnumWbemClassObject* _enumerator;
        private readonly int _timeout;


        internal WmiSearchResultsEnumerator(IEnumWbemClassObject* enumerator, int timeout)
        {
            _enumerator = enumerator;
            _timeout = timeout;
            _currentArray = new IntPtr[1];
            _currentArrayHandle = _currentArray.Pin();
            _isCurrentInUse = false;
        }

        public WmiSearchResultItem Current
        {
            get
            {
                IWbemClassObject** current = (IWbemClassObject**)_currentArrayHandle.Pointer;

                return new(current[0]);
            }
        }

        public bool MoveNext()
        {
            uint uReturn = 0;

            if (_isCurrentInUse)
            {
                IWbemClassObject** current = (IWbemClassObject**)_currentArrayHandle.Pointer;
                current[0]->Release();

                _isCurrentInUse = false;
            }

            var array = (IWbemClassObject**)_currentArrayHandle.Pointer;

            var hr = _enumerator->Next(_timeout, 1, array, &uReturn);

            hr.ThrowOnFailure();

            _isCurrentInUse = uReturn > 0;

            return uReturn > 0;
        }

        public void Dispose()
        {
            if (_isCurrentInUse)
            {
                IWbemClassObject** current = (IWbemClassObject**)_currentArrayHandle.Pointer;
                current[0]->Release();

                _isCurrentInUse = false;
            }

            _currentArrayHandle.Dispose();
        }
    }

    public unsafe ref struct WmiSearch: IDisposable
    {
        private IWbemServices* _wbemServices;
        private IWbemLocator* _wbemLocator;
        private IEnumWbemClassObject* _wbemEnumerator;
        private readonly int _timeout;

        internal WmiSearch(IWbemServices* wbemServices, IWbemLocator* wbemLocator, IEnumWbemClassObject* wbemEnumerator, int timeout)
        {
            _wbemServices = wbemServices;
            _wbemLocator = wbemLocator;
            _wbemEnumerator = wbemEnumerator;
            _timeout = timeout;
        }

        public WmiSearchResultsEnumerator GetEnumerator()
        {
            return new WmiSearchResultsEnumerator(_wbemEnumerator, _timeout);
        }

        public static WmiSearch Null => new WmiSearch();

        public void Dispose()
        {
            _wbemServices->Release();
            _wbemLocator->Release();
            _wbemEnumerator->Release();
        }
    }
}
