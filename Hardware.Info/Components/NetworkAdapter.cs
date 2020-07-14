using System.Net;

namespace Hardware.Info
{
    public class NetworkAdapter
    {
        public string MACAddress { get; internal set; } = string.Empty;
        public string Model { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public IPAddress IPAddress { get; internal set; } = IPAddress.None;
    }
}
