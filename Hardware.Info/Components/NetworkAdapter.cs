using System;
using System.Collections.Generic;
using System.Net;

namespace Hardware.Info
{
    public class NetworkAdapter
    {
        public string AdapterType { get; internal set; } = string.Empty;
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string MACAddress { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string NetConnectionID { get; internal set; } = string.Empty;
        public string ProductName { get; internal set; } = string.Empty;
        public UInt64 Speed { get; internal set; }

        public List<IPAddress> DefaultIPGatewayList { get; internal set; } = new List<IPAddress>();
        public IPAddress DHCPServer { get; internal set; } = IPAddress.None;
        public List<IPAddress> DNSServerSearchOrderList { get; internal set; } = new List<IPAddress>();
        public List<IPAddress> IPAddressList { get; internal set; } = new List<IPAddress>();
        public List<IPAddress> IPSubnetList { get; internal set; } = new List<IPAddress>();

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
