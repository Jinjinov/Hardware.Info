using System;

namespace Hardware.Info.Windows
{
    internal interface IWmiPropertySource : IDisposable
    {
        object this[string propertyName] { get; }
    }
}
