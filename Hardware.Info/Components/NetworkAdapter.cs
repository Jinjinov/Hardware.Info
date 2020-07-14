using System;
using System.Net;

namespace Hardware.Info
{
    public class NetworkAdapter
    {
        //public string MACAddress { get; internal set; } = string.Empty;
        public string Model { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public IPAddress IPAddress { get; internal set; } = IPAddress.None;

        public string AdapterType { get; internal set; } = string.Empty;
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string MACAddress { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string NetConnectionID { get; internal set; } = string.Empty;
        public string ProductName { get; internal set; } = string.Empty;
        public UInt64 Speed { get; internal set; }

        public override string ToString()
        {
            return
                "AdapterType: " + AdapterType + Environment.NewLine +
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "MACAddress: " + MACAddress + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "NetConnectionID: " + NetConnectionID + Environment.NewLine +
                "ProductName: " + ProductName + Environment.NewLine +
                "Speed: " + Speed + Environment.NewLine;
        }
    }
}
