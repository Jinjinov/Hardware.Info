using System;
using System.Net;

namespace Hardware.Info
{
    public class NetworkAdapter
    {
        public string MACAddress { get; internal set; } = string.Empty;
        public string Model { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public IPAddress IPAddress { get; internal set; } = IPAddress.None;

        public string AdapterType;
        public string Caption;
        public string Description;
        //public string MACAddress;
        public string Manufacturer;
        public string Name;
        public string NetConnectionID;
        public string ProductName;
        public string Speed;

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
