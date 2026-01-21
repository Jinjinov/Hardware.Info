using System.Management;

namespace Hardware.Info.Windows
{
    internal sealed class ManagementObjectAdapter : IWmiPropertySource
    {
        private readonly ManagementBaseObject _inner;

        public ManagementObjectAdapter(ManagementBaseObject inner) => _inner = inner;

        public object this[string propertyName] => _inner[propertyName];

        public ManagementBaseObject GetManagementBaseObject() => _inner;

        public void Dispose() => _inner.Dispose();
    }
}
