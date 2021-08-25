using System;
using System.Collections.Generic;
using System.Net;

namespace Hardware.Info
{
    public class NetworkAdapter
    {
        public string AdapterType { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MACAddress { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NetConnectionID { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public UInt64 Speed { get; set; }
        public UInt64 BytesSentPersec { get; set; }
        public UInt64 BytesReceivedPersec { get; set; }

        public List<IPAddress> DefaultIPGatewayList { get; set; } = new List<IPAddress>();
        public IPAddress DHCPServer { get; set; } = IPAddress.None;
        public List<IPAddress> DNSServerSearchOrderList { get; set; } = new List<IPAddress>();
        public List<IPAddress> IPAddressList { get; set; } = new List<IPAddress>();
        public List<IPAddress> IPSubnetList { get; set; } = new List<IPAddress>();

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
                "Speed: " + Speed + Environment.NewLine +
                "BytesSentPersec: " + BytesSentPersec + Environment.NewLine +
                "BytesReceivedPersec: " + BytesReceivedPersec + Environment.NewLine;
        }
    }
}
