using WmiLight;

namespace Hardware.Info.Windows
{
    internal sealed class WmiLightObjectAdapter : IWmiPropertySource
    {
        private readonly WmiObject _inner;

        public WmiLightObjectAdapter(WmiObject inner) => _inner = inner;

        public object this[string propertyName] => _inner[propertyName];

        public WmiObject GetWmiObject() => _inner;

        public void Dispose() => _inner.Dispose();
    }
}
