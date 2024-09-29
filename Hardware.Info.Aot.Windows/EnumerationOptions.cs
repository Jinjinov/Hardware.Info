namespace Hardware.Info.Aot.Windows;

struct EnumerationOptions
{
    public bool ReturnImmediately { get; set; }
    public bool Rewindable { get; set; }
    public int Timeout { get; set; }
}