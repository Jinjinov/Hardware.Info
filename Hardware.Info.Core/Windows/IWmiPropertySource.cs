namespace Hardware.Info.Windows
{
    internal interface IWmiPropertySource
    {
        object this[string propertyName] { get; }
    }
}
