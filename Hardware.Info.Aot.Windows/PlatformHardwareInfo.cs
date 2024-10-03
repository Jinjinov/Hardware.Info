using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.Com;
using Hardware.Info.Core;
using Monitor = Hardware.Info.Core.Monitor;

namespace Hardware.Info.Aot.Windows
{
    internal class PlatformHardwareInfo : PlatformHardwareInfoBase, IPlatformHardwareInfo
    {
        private readonly MemoryStatus _memoryStatus = new MemoryStatus();

        private readonly OS _os = new OS();

        public bool UseAsteriskInWMI { get; set; }

        private readonly string _managementScope = "root\\cimv2";
        private readonly string _managementScopeWmi = "root\\wmi";
        private readonly int _timeout;

        public PlatformHardwareInfo(TimeSpan? enumerationOptionsTimeout = null)
        {
            InitializeCom();

            _timeout = enumerationOptionsTimeout.HasValue ? (int)enumerationOptionsTimeout.Value.TotalMilliseconds : Constants.WBEM_INFINITE;


            GetOs();
        }

        public static Version? GetOsVersionByRtlGetVersion()
        {
            global::Windows.Win32.System.SystemInformation.OSVERSIONINFOW info = new ();
            info.dwOSVersionInfoSize = (uint)Marshal.SizeOf<global::Windows.Win32.System.SystemInformation.OSVERSIONINFOW>();

            var result = global::Windows.Wdk.PInvoke.RtlGetVersion(ref info);

            return (result.SeverityCode == NTSTATUS.Severity.Success)
                ? new Version((int)info.dwMajorVersion, (int)info.dwMinorVersion, (int)info.dwBuildNumber)
                : null;
        }

        private unsafe void InitializeCom()
        {
            HRESULT hr = PInvoke.CoInitializeEx((void*)0, COINIT.COINIT_MULTITHREADED);

            hr.ThrowOnFailure();
            if (hr.Failed)
            {
                Console.WriteLine($"Failed to initialize COM library. Error code = 0x{hr:X}");
                return;
            }

            // Set COM security levels
            hr = PInvoke.CoInitializeSecurity(
                new PSECURITY_DESCRIPTOR((void*)0),
                -1,
                (SOLE_AUTHENTICATION_SERVICE*)0,
                (void*)0,
                RPC_C_AUTHN_LEVEL.RPC_C_AUTHN_LEVEL_DEFAULT,
                RPC_C_IMP_LEVEL.RPC_C_IMP_LEVEL_IMPERSONATE,
                (void*)0,
                EOLE_AUTHENTICATION_CAPABILITIES.EOAC_NONE,
                (void*)0
            );

            if (hr.Failed)
            {
                Console.WriteLine($"Failed to initialize security. Error code = 0x{hr:X}");
                PInvoke.CoUninitialize();
                return;
            }
        }

        public void GetOs()
        {
            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_OperatingSystem"
                                                  : "SELECT Caption, Version FROM Win32_OperatingSystem";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                _os.Name = mo.GetProperty<string>("Caption");
                _os.VersionString = mo.GetProperty<string>("Version");

                if (Version.TryParse(_os.VersionString, out Version version))
                    _os.Version = version;
            }

            if (string.IsNullOrEmpty(_os.Name))
            {
                _os.Name = "Windows";
            }

            if (string.IsNullOrEmpty(_os.VersionString))
            {
                Version? version = GetOsVersionByRtlGetVersion();

                if (version != null)
                {
                    _os.Version = version;
                    _os.VersionString = version.ToString();
                }
            }
        }

        public OS GetOperatingSystem()
        {
            return _os;
        }

        public MemoryStatus GetMemoryStatus()
        {
            var _memoryStatusEx = new global::Windows.Win32.System.SystemInformation.MEMORYSTATUSEX();
            _memoryStatusEx.dwLength = (uint)Marshal.SizeOf<global::Windows.Win32.System.SystemInformation.MEMORYSTATUSEX>();

            if (PInvoke.GlobalMemoryStatusEx(ref _memoryStatusEx))
            {
                _memoryStatus.TotalPhysical = _memoryStatusEx.ullTotalPhys;
                _memoryStatus.AvailablePhysical = _memoryStatusEx.ullAvailPhys;
                _memoryStatus.TotalPageFile = _memoryStatusEx.ullTotalPageFile;
                _memoryStatus.AvailablePageFile = _memoryStatusEx.ullAvailPageFile;
                _memoryStatus.TotalVirtual = _memoryStatusEx.ullTotalVirtual;
                _memoryStatus.AvailableVirtual = _memoryStatusEx.ullAvailVirtual;
                _memoryStatus.AvailableExtendedVirtual = _memoryStatusEx.ullAvailExtendedVirtual;
            }

            return _memoryStatus;
        }

        public static T GetPropertyValue<T>(object obj) where T : struct
        {
            return (obj == null) ? default : (T)obj;
        }

        public static T[] GetPropertyArray<T>(object obj)
        {
            return (obj is T[] array) ? array : Array.Empty<T>();
        }

        public static string GetPropertyString(object obj)
        {
            return (obj is string str) ? str : string.Empty;
        }

        public static string GetStringFromArray(ushort[] array)
        {
            return Encoding.Unicode.GetString(MemoryMarshal.AsBytes(array.AsSpan()));
        }
        public static string GetStringFromArray(int[] array)
        {
            return Encoding.Unicode.GetString(MemoryMarshal.AsBytes(array.AsSpan()));
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Battery"
                                                  : "SELECT FullChargeCapacity, DesignCapacity, BatteryStatus, EstimatedChargeRemaining, EstimatedRunTime, ExpectedLife, MaxRechargeTime, TimeOnBattery, TimeToFullCharge FROM Win32_Battery";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Battery battery = new Battery
                {
                    FullChargeCapacity = mo.GetProperty<uint>("FullChargeCapacity"),
                    DesignCapacity = mo.GetProperty<uint>("DesignCapacity"),
                    BatteryStatus = (ushort) mo.GetProperty<int>("BatteryStatus"),
                    EstimatedChargeRemaining = (ushort) mo.GetProperty<int>("EstimatedChargeRemaining"),
                    EstimatedRunTime = mo.GetProperty<uint>("EstimatedRunTime"),
                    ExpectedLife = mo.GetProperty<uint>("ExpectedLife"),
                    MaxRechargeTime = mo.GetProperty<uint>("MaxRechargeTime"),
                    TimeOnBattery = mo.GetProperty<uint>("TimeOnBattery"),
                    TimeToFullCharge = mo.GetProperty<uint>("TimeToFullCharge")
                };

                batteryList.Add(battery);
            }

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_BIOS"
                                                  : "SELECT Caption, Description, Manufacturer, Name, ReleaseDate, SerialNumber, SoftwareElementID, Version FROM Win32_BIOS";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                BIOS bios = new BIOS
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    Name = mo.GetProperty<string>("Name"),
                    ReleaseDate = mo.GetProperty<string>("ReleaseDate"),
                    SerialNumber = mo.GetProperty<string>("SerialNumber"),
                    SoftwareElementID = mo.GetProperty<string>("SoftwareElementID"),
                    Version = mo.GetProperty<string>("Version")
                };

                biosList.Add(bios);
            }

            return biosList;
        }

        public List<ComputerSystem> GetComputerSystemList()
        {
            List<ComputerSystem> computerSystemList = new List<ComputerSystem>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_ComputerSystemProduct"
                                                  : "SELECT Caption, Description, IdentifyingNumber, Name, SKUNumber, UUID, Vendor, Version FROM Win32_ComputerSystemProduct";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                ComputerSystem computerSystem = new ComputerSystem
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    IdentifyingNumber = mo.GetProperty<string>("IdentifyingNumber"),
                    Name = mo.GetProperty<string>("Name"),
                    SKUNumber = mo.GetProperty<string>("SKUNumber"),
                    UUID = mo.GetProperty<string>("UUID"),
                    Vendor = mo.GetProperty<string>("Vendor"),
                    Version = mo.GetProperty<string>("Version")
                };

                computerSystemList.Add(computerSystem);
            }

            return computerSystemList;
        }

        public List<CPU> GetCpuList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500)
        {
            List<CPU> cpuList = new List<CPU>();

            List<CpuCore> cpuCoreList = new List<CpuCore>();

            ulong percentProcessorTime = 0ul;

            if (includePercentProcessorTime)
            {
                string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name != '_Total'"
                                                      : "SELECT Name, PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name != '_Total'";
                ManagementObjectSearcher percentProcessorTimeMOS = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

                queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'"
                                               : "SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'";
                ManagementObjectSearcher totalPercentProcessorTimeMOS = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

                try
                {
                    foreach (var mo in percentProcessorTimeMOS.Get())
                    {
                        CpuCore core = new CpuCore
                        {
                            Name = mo.GetProperty<string>("Name"),
                            PercentProcessorTime = ulong.Parse(mo.GetProperty<string>("PercentProcessorTime", "0"))
                        };

                        cpuCoreList.Add(core);
                    }

                    foreach (var mo in totalPercentProcessorTimeMOS.Get())
                    {
                        percentProcessorTime = ulong.Parse(mo.GetProperty<string>("PercentProcessorTime", "0"));
                    }
                }
                finally
                {
                    percentProcessorTimeMOS.Dispose();

                    totalPercentProcessorTimeMOS.Dispose();
                }

                if (percentProcessorTime == 0ul)
                {
                    queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Processor"
                                                   : "SELECT LoadPercentage FROM Win32_Processor";
                    using ManagementObjectSearcher loadPercentageMOS = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

                    foreach (var mo in loadPercentageMOS.Get())
                    {
                        percentProcessorTime = (uint) mo.GetProperty<int>("LoadPercentage");
                    }
                }
            }

            bool isAtLeastWin8 = (_os.Version.Major == 6 && _os.Version.Minor >= 2) || (_os.Version.Major > 6);

            string query = UseAsteriskInWMI ? "SELECT * FROM Win32_Processor"
                                            : isAtLeastWin8 ? "SELECT Caption, CurrentClockSpeed, Description, L2CacheSize, L3CacheSize, Manufacturer, MaxClockSpeed, Name, NumberOfCores, NumberOfLogicalProcessors, ProcessorId, SecondLevelAddressTranslationExtensions, SocketDesignation, VirtualizationFirmwareEnabled, VMMonitorModeExtensions FROM Win32_Processor"
                                                            : "SELECT Caption, CurrentClockSpeed, Description, L2CacheSize, L3CacheSize, Manufacturer, MaxClockSpeed, Name, NumberOfCores, NumberOfLogicalProcessors, ProcessorId, SocketDesignation FROM Win32_Processor";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, query, _timeout);

            float processorPerformance = 100f;

            try
            {
                using PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
                processorPerformance = cpuCounter.NextValue();
                System.Threading.Thread.Sleep(1); // the first call to NextValue() always returns 0
                processorPerformance = cpuCounter.NextValue();
            }
            catch
            {
                // Ignore performance counter errors and just assume that it's at 100 %
            }

            uint L1InstructionCacheSize = 0;
            uint L1DataCacheSize = 0;
            // L1 = 3
            // L2 = 4
            // L3 = 5
            query = UseAsteriskInWMI ? "SELECT * FROM Win32_CacheMemory WHERE Level = 3"
                                     : "SELECT CacheType, MaxCacheSize FROM Win32_CacheMemory WHERE Level = 3";
            using ManagementObjectSearcher Win32_CacheMemory = new ManagementObjectSearcher(_managementScope, query, _timeout);

            // Other = 1
            // Unknown = 2
            // Instruction = 3
            // Data = 4
            // Unified = 5
            foreach (var mo in Win32_CacheMemory.Get())
            {
                ushort CacheType = (ushort) mo.GetProperty<int>("CacheType");
                uint MaxCacheSize = 1024 * mo.GetProperty<uint>("MaxCacheSize");

                // if CacheType is Other or Unknown
                if (L1InstructionCacheSize == 0)
                    L1InstructionCacheSize = MaxCacheSize;

                // if CacheType is Other or Unknown
                if (L1DataCacheSize == 0)
                    L1DataCacheSize = MaxCacheSize;

                if (CacheType == 3) // Instruction
                    L1InstructionCacheSize = MaxCacheSize;

                if (CacheType == 4) // Data
                    L1DataCacheSize = MaxCacheSize;
            }

            foreach (var mo in mos.Get())
            {
                uint maxClockSpeed = mo.GetProperty<uint>("MaxClockSpeed");

                uint currentClockSpeed = (uint)(maxClockSpeed * (processorPerformance / 100));

                CPU cpu = new CPU
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    //CurrentClockSpeed = GetPropertyValue<uint>(mo["CurrentClockSpeed"]), https://stackoverflow.com/questions/61802420/unable-to-get-current-cpu-frequency-in-powershell-or-python
                    CurrentClockSpeed = currentClockSpeed,
                    Description = mo.GetProperty<string>("Description"),
                    L1InstructionCacheSize = L1InstructionCacheSize,
                    L1DataCacheSize = L1DataCacheSize,
                    L2CacheSize = 1024 * mo.GetProperty<uint>("L2CacheSize"),
                    L3CacheSize = 1024 * mo.GetProperty<uint>("L3CacheSize"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    MaxClockSpeed = maxClockSpeed,
                    Name = mo.GetProperty<string>("Name"),
                    NumberOfCores = mo.GetProperty<uint>("NumberOfCores"),
                    NumberOfLogicalProcessors = mo.GetProperty<uint>("NumberOfLogicalProcessors"),
                    ProcessorId = mo.GetProperty<string>("ProcessorId"),
                    SocketDesignation = mo.GetProperty<string>("SocketDesignation"),
                    PercentProcessorTime = percentProcessorTime,
                    CpuCoreList = cpuCoreList
                };

                if (isAtLeastWin8)
                {
                    cpu.SecondLevelAddressTranslationExtensions = mo.GetProperty<bool>("SecondLevelAddressTranslationExtensions");
                    cpu.VirtualizationFirmwareEnabled = mo.GetProperty<bool>("VirtualizationFirmwareEnabled");
                    cpu.VMMonitorModeExtensions = mo.GetProperty<bool>("VMMonitorModeExtensions");
                }

                cpuList.Add(cpu);
            }

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_DiskDrive"
                                                  : "SELECT Caption, Description, DeviceID, FirmwareRevision, Index, Manufacturer, Model, Name, Partitions, SerialNumber, Size FROM Win32_DiskDrive";
            using ManagementObjectSearcher Win32_DiskDrive = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var DiskDrive in Win32_DiskDrive.Get())
            {
                Drive drive = new Drive
                {
                    Caption = DiskDrive.GetProperty<string>("Caption"),
                    Description = DiskDrive.GetProperty<string>("Description"),
                    FirmwareRevision = DiskDrive.GetProperty<string>("FirmwareRevision"),
                    Index = DiskDrive.GetProperty<uint>("Index"),
                    Manufacturer = DiskDrive.GetProperty<string>("Manufacturer"),
                    Model = DiskDrive.GetProperty<string>("Model"),
                    Name = DiskDrive.GetProperty<string>("Name"),
                    Partitions = DiskDrive.GetProperty<uint>("Partitions"),
                    SerialNumber = DiskDrive.GetProperty<string>("SerialNumber"),
                    Size = ulong.Parse(DiskDrive.GetProperty<string>("Size", "0"))
                };

                string deviceId = DiskDrive.GetProperty<string>("DeviceID");
                string queryString1 = "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + deviceId + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                using ManagementObjectSearcher Win32_DiskPartition = new ManagementObjectSearcher(_managementScope, queryString1, _timeout);

                foreach (var DiskPartition in Win32_DiskPartition.Get())
                {
                    Partition partition = new Partition
                    {
                        Bootable = DiskPartition.GetProperty<bool>("Bootable"),
                        BootPartition = DiskPartition.GetProperty<bool>("BootPartition"),
                        Caption = DiskPartition.GetProperty<string>("Caption"),
                        Description = DiskPartition.GetProperty<string>("Description"),
                        DiskIndex = DiskPartition.GetProperty<uint>("DiskIndex"),
                        Index = DiskPartition.GetProperty<uint>("Index"),
                        Name = DiskPartition.GetProperty<string>("Name"),
                        PrimaryPartition = DiskPartition.GetProperty<bool>("PrimaryPartition"),
                        Size = ulong.CreateTruncating(UInt128.Parse(DiskPartition.GetProperty<string>("Size", "0"))),
                        StartingOffset = ulong.Parse(DiskPartition.GetProperty<string>("StartingOffset"))
                    };

                    var partitionDeviceId = DiskPartition.GetProperty<string>("DeviceID");
                    string queryString2 = "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partitionDeviceId + "'} WHERE AssocClass = Win32_LogicalDiskToPartition";
                    using ManagementObjectSearcher Win32_LogicalDisk = new ManagementObjectSearcher(_managementScope, queryString2, _timeout);

                    foreach (var LogicalDisk in Win32_LogicalDisk.Get())
                    {
                        Volume volume = new Volume
                        {
                            Caption = LogicalDisk.GetProperty<string>("Caption"),
                            Compressed = LogicalDisk.GetProperty<bool>("Compressed"),
                            Description = LogicalDisk.GetProperty<string>("Description"),
                            FileSystem = LogicalDisk.GetProperty<string>("FileSystem"),
                            FreeSpace = ulong.CreateTruncating(ulong.Parse(LogicalDisk.GetProperty<string>("FreeSpace"))),
                            Name = LogicalDisk.GetProperty<string>("Name"),
                            Size = ulong.CreateTruncating(ulong.Parse(LogicalDisk.GetProperty<string>("Size"))),
                            VolumeName = LogicalDisk.GetProperty<string>("VolumeName"),
                            VolumeSerialNumber = LogicalDisk.GetProperty<string>("VolumeSerialNumber")
                        };

                        partition.VolumeList.Add(volume);
                    }

                    drive.PartitionList.Add(partition);
                }

                driveList.Add(drive);
            }

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Keyboard"
                                                  : "SELECT Caption, Description, Name, NumberOfFunctionKeys FROM Win32_Keyboard";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Keyboard keyboard = new Keyboard
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    Name = mo.GetProperty<string>("Name"),
                    NumberOfFunctionKeys = (ushort) mo.GetProperty<int>("NumberOfFunctionKeys")
                };

                keyboardList.Add(keyboard);
            }

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PhysicalMemory"
                                                  : _os.Version.Major >= 10 ? "SELECT BankLabel, Capacity, FormFactor, Manufacturer, MaxVoltage, MinVoltage, PartNumber, SerialNumber, Speed FROM Win32_PhysicalMemory"
                                                                            : "SELECT BankLabel, Capacity, FormFactor, Manufacturer, PartNumber, SerialNumber, Speed FROM Win32_PhysicalMemory";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Memory memory = new Memory
                {
                    BankLabel = mo.GetProperty<string>("BankLabel"),
                    Capacity = ulong.Parse(mo.GetProperty<string>("Capacity")),
                    FormFactor = (FormFactor)mo.GetProperty<int>("FormFactor"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    PartNumber = mo.GetProperty<string>("PartNumber"),
                    SerialNumber = mo.GetProperty<string>("SerialNumber"),
                                Speed = mo.GetProperty<uint>("Speed")
                };

                if (_os.Version.Major >= 10)
                {
                    memory.MaxVoltage = mo.GetProperty<uint>("MaxVoltage");
                    memory.MinVoltage = mo.GetProperty<uint>("MinVoltage");
                }

                memoryList.Add(memory);
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            string win32PnpEntityQuery = UseAsteriskInWMI ? "SELECT * FROM Win32_PnPEntity WHERE PNPClass='Monitor'"
                                                          : "SELECT DeviceId FROM Win32_PnPEntity WHERE PNPClass='Monitor'";
            using ManagementObjectSearcher win32PnpEntityMos = new ManagementObjectSearcher(_managementScope, win32PnpEntityQuery, _timeout);

            foreach (var win32PnpEntityMo in win32PnpEntityMos.Get())
            {
                string deviceId = win32PnpEntityMo.GetProperty<string>("DeviceId");
                string win32DesktopMonitorQuery = UseAsteriskInWMI ? $"SELECT * FROM Win32_DesktopMonitor WHERE PNPDeviceId='{deviceId}'"
                                                                   : $"SELECT Caption, Description, MonitorManufacturer, MonitorType, Name, PixelsPerXLogicalInch, PixelsPerYLogicalInch FROM Win32_DesktopMonitor WHERE PNPDeviceId='{deviceId}'";
                using ManagementObjectSearcher win32DesktopMonitorMos = new ManagementObjectSearcher(_managementScope, win32DesktopMonitorQuery.Replace(@"\", @"\\"), _timeout);
                
                string wmiMonitorIdQuery = UseAsteriskInWMI ? $"SELECT * FROM WmiMonitorID WHERE InstanceName LIKE '{deviceId}%'"
                                                            : $"SELECT Active, ProductCodeID, SerialNumberID, ManufacturerName, UserFriendlyName, WeekOfManufacture, YearOfManufacture FROM WmiMonitorID WHERE InstanceName LIKE '{deviceId}%'";
                using ManagementObjectSearcher wmiMonitorIdMos = new ManagementObjectSearcher(_managementScopeWmi, wmiMonitorIdQuery.Replace(@"\", "_"), _timeout);

                Monitor monitor = new Monitor();

                foreach (var desktopMonitorMo in win32DesktopMonitorMos.Get())
                {
                    monitor.Caption = desktopMonitorMo.GetProperty<string>("Caption");
                    monitor.Description = desktopMonitorMo.GetProperty<string>("Description");
                    monitor.MonitorManufacturer = desktopMonitorMo.GetProperty<string>("MonitorManufacturer");
                    monitor.MonitorType = desktopMonitorMo.GetProperty<string>("MonitorType");
                    monitor.Name = desktopMonitorMo.GetProperty<string>("Name");
                    monitor.PixelsPerXLogicalInch = desktopMonitorMo.GetProperty<uint>("PixelsPerXLogicalInch");
                    monitor.PixelsPerYLogicalInch = desktopMonitorMo.GetProperty<uint>("PixelsPerYLogicalInch");

                    break;
                }

                foreach (var wmiMonitorIdMo in wmiMonitorIdMos.Get())
                {
                    monitor.Active = wmiMonitorIdMo.GetProperty<bool>("Active");
                    monitor.ProductCodeID = GetStringFromArray(wmiMonitorIdMo.GetArrayProperty<int>("ProductCodeID"));
                    if (wmiMonitorIdMo.TryGetArrayProperty<ushort>("UserFriendlyName", out var userFriendlyNameUint16,
                            out _))
                    {
                        monitor.UserFriendlyName = GetStringFromArray(userFriendlyNameUint16);
                    } else if (wmiMonitorIdMo.TryGetArrayProperty<int>("UserFriendlyName", out var userFriendlyNameInt4,
                                   out var errorReason))
                    {
                        monitor.UserFriendlyName = GetStringFromArray(userFriendlyNameInt4);
                    }
                    else
                    {
                        WmiSearchResultItem.HandleArrayPropertyError<ushort>("UserFriendlyName", errorReason);
                    }
                    monitor.UserFriendlyName = GetStringFromArray(wmiMonitorIdMo.GetArrayProperty<ushort>("UserFriendlyName"));
                    monitor.SerialNumberID = GetStringFromArray(wmiMonitorIdMo.GetArrayProperty<int>("SerialNumberID"));
                    monitor.ManufacturerName = GetStringFromArray(wmiMonitorIdMo.GetArrayProperty<int>("ManufacturerName"));
                    monitor.WeekOfManufacture = (ushort) wmiMonitorIdMo.GetProperty<byte>("WeekOfManufacture");
                    monitor.YearOfManufacture = (ushort) wmiMonitorIdMo.GetProperty<int>("YearOfManufacture");

                    break;
                }

                monitorList.Add(monitor);
            }

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_BaseBoard"
                                                  : "SELECT Manufacturer, Product, SerialNumber FROM Win32_BaseBoard";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Motherboard motherboard = new Motherboard
                {
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    Product = mo.GetProperty<string>("Product"),
                    SerialNumber = mo.GetProperty<string>("SerialNumber")
                };

                motherboardList.Add(motherboard);
            }

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PointingDevice"
                                                  : "SELECT Caption, Description, Manufacturer, Name, NumberOfButtons FROM Win32_PointingDevice";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Mouse mouse = new Mouse
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    Name = mo.GetProperty<string>("Name"),
                    NumberOfButtons = mo.GetProperty<byte>("NumberOfButtons")
                };

                mouseList.Add(mouse);
            }

            return mouseList;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000)        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True AND MACAddress IS NOT NULL"
                                                  : "SELECT AdapterType, Caption, Description, DeviceID, MACAddress, Manufacturer, Name, NetConnectionID, ProductName, Speed FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True AND MACAddress IS NOT NULL";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    AdapterType = mo.GetProperty<string>("AdapterType"),
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    MACAddress = mo.GetProperty<string>("MACAddress"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    Name = mo.GetProperty<string>("Name"),
                    NetConnectionID = mo.GetProperty<string>("NetConnectionID"),
                    ProductName = mo.GetProperty<string>("ProductName"),
                    Speed = ulong.Parse(mo.GetProperty<string>("Speed"))
                };

                if (includeBytesPersec)
                {
                    // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter.instancename

                    string name = networkAdapter.Name.Replace('(', '[').Replace(')', ']').Replace('#', '_').Replace('\\', '_').Replace('/', '_');

                    string query = UseAsteriskInWMI ? $"SELECT * FROM Win32_PerfFormattedData_Tcpip_NetworkAdapter WHERE Name = '{name}'"
                                                    : $"SELECT BytesSentPersec, BytesReceivedPersec, CurrentBandwidth FROM Win32_PerfFormattedData_Tcpip_NetworkAdapter WHERE Name = '{name}'";
                    using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_managementScope, query, _timeout);

                    foreach (var managementObject in managementObjectSearcher.Get())
                    {
                        networkAdapter.BytesSentPersec = ulong.Parse(managementObject.GetProperty<string>("BytesSentPersec"));
                        networkAdapter.BytesReceivedPersec = ulong.Parse(managementObject.GetProperty<string>("BytesReceivedPersec"));

                        if (networkAdapter.Speed == 0 || networkAdapter.Speed == long.MaxValue)
                        {
                            networkAdapter.Speed = managementObject.GetProperty<ulong>("CurrentBandwidth");
                        }
                    }
                }

                if (includeNetworkAdapterConfiguration)
                {
                    IPAddress address;

                    string query = $"ASSOCIATORS OF {{{mo.GetProperty<string>("__RelPath")}}} where resultclass = Win32_NetworkAdapterConfiguration";

                    using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_managementScope, query, _timeout);


                    foreach (var configuration in managementObjectSearcher.Get())
                    {
                        foreach (string str in configuration.GetArrayProperty<string>("DefaultIPGateway"))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.DefaultIPGatewayList.Add(address);

                        if (IPAddress.TryParse(configuration.GetProperty<string>("DHCPServer"), out address))
                            networkAdapter.DHCPServer = address;

                        foreach (string str in configuration.GetArrayProperty<string>("DNSServerSearchOrder"))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.DNSServerSearchOrderList.Add(address);

                        foreach (string str in configuration.GetArrayProperty<string>("IPAddress"))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.IPAddressList.Add(address);

                        foreach (string str in configuration.GetArrayProperty<string>("IPSubnet"))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.IPSubnetList.Add(address);
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Printer"
                                                  : "SELECT Caption, Default, Description, HorizontalResolution, Local, Name, Network, Shared, VerticalResolution FROM Win32_Printer";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                Printer printer = new Printer
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Default = mo.GetProperty<bool>("Default"),
                    Description = mo.GetProperty<string>("Description"),
                    HorizontalResolution = mo.GetProperty<uint>("HorizontalResolution"),
                    Local = mo.GetProperty<bool>("Local"),
                    Name = mo.GetProperty<string>("Name"),
                    Network = mo.GetProperty<bool>("Network"),
                    Shared = mo.GetProperty<bool>("Shared"),
                    VerticalResolution = mo.GetProperty<uint>("VerticalResolution")            };

                printerList.Add(printer);
            }

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_SoundDevice WHERE NOT Manufacturer='Microsoft'"
                                                  : "SELECT Caption, Description, Manufacturer, Name, ProductName FROM Win32_SoundDevice WHERE NOT Manufacturer='Microsoft'";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                SoundDevice soundDevice = new SoundDevice
                {
                    Caption = mo.GetProperty<string>("Caption"),
                    Description = mo.GetProperty<string>("Description"),
                    Manufacturer = mo.GetProperty<string>("Manufacturer"),
                    Name = mo.GetProperty<string>("Name"),
                    ProductName = mo.GetProperty<string>("ProductName")
                };

                soundDeviceList.Add(soundDevice);
            }

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_VideoController"
                                                  : "SELECT AdapterCompatibility, AdapterRAM, Caption, CurrentBitsPerPixel, CurrentHorizontalResolution, CurrentNumberOfColors, CurrentRefreshRate, CurrentVerticalResolution, Description, DriverDate, DriverVersion, MaxRefreshRate, MinRefreshRate, Name, PNPDeviceID, VideoModeDescription, VideoProcessor FROM Win32_VideoController";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _timeout);

            foreach (var mo in mos.Get())
            {
                VideoController videoController = new VideoController
                {
                    Manufacturer = mo.GetProperty<string>("AdapterCompatibility"),
                    AdapterRAM = mo.GetProperty<uint>("AdapterRAM"),
                    Caption = mo.GetProperty<string>("Caption"),
                    CurrentBitsPerPixel = mo.GetProperty<uint>("CurrentBitsPerPixel"),
                    CurrentHorizontalResolution = mo.GetProperty<uint>("CurrentHorizontalResolution"),
                    CurrentNumberOfColors = UInt64.Parse(mo.GetProperty<string>("CurrentNumberOfColors")),
                    CurrentRefreshRate = mo.GetProperty<uint>("CurrentRefreshRate"),
                    CurrentVerticalResolution = mo.GetProperty<uint>("CurrentVerticalResolution"),
                    Description = mo.GetProperty<string>("Description"),
                    DriverDate = mo.GetProperty<string>("DriverDate"),
                    DriverVersion = mo.GetProperty<string>("DriverVersion"),
                    MaxRefreshRate = mo.GetProperty<uint>("MaxRefreshRate"),
                    MinRefreshRate = mo.GetProperty<uint>("MinRefreshRate"),
                    Name = mo.GetProperty<string>("Name"),
                    VideoModeDescription = mo.GetProperty<string>("VideoModeDescription"),
                    VideoProcessor = mo.GetProperty<string>("VideoProcessor")
                };

                try
                {
                    string deviceID = mo.GetProperty<string>("PNPDeviceID");

                    if (string.IsNullOrEmpty(deviceID))
                        continue;

                    object? driverObject = Microsoft.Win32.Registry.GetValue(@$"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\{deviceID}", "Driver", default(string));

                    if (driverObject is string driver && !string.IsNullOrEmpty(driver))
                    {
                        object? qwMemorySizeObject = Microsoft.Win32.Registry.GetValue(@$"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{driver}", "HardwareInformation.qwMemorySize", default(long));

                        if (qwMemorySizeObject is long qwMemorySize && qwMemorySize != 0L)
                        {
                            videoController.AdapterRAM = (ulong)qwMemorySize;
                        }
                    }
                }
                catch (SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }

                videoControllerList.Add(videoController);
            }

            return videoControllerList;
        }
    }
}
