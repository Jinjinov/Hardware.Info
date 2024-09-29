namespace Hardware.Info.Windows.Aot;

struct EnumerationOptions
{
    public bool ReturnImmediately { get; set; }
    public bool Rewindable { get; set; }
    public int Timeout { get; set; }
}