using System;
using System.Collections.Generic;
using System.Net;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-networkadapter

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_NetworkAdapter
    /// </summary>
    public class NetworkAdapter
    {
        /// <summary>
        /// Network medium in use.
        /// </summary>
        public string AdapterType { get; set; } = string.Empty;

        /// <summary>
        /// Short description of the object (a one-line string).
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Media access control address for this network adapter. 
        /// A MAC address is a unique 48-bit number assigned to the network adapter by the manufacturer. 
        /// It uniquely identifies this network adapter and is used for mapping TCP/IP network communications.
        /// </summary>
        public string MACAddress { get; set; } = string.Empty;

        /// <summary>
        /// Name of the network adapter's manufacturer.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Name of the network connection.
        /// </summary>
        public string NetConnectionID { get; set; } = string.Empty;

        /// <summary>
        /// Product name of the network adapter.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Estimate of the current bandwidth in bits per second.
        /// </summary>
        public UInt64 Speed { get; set; }

        /// <summary>
        /// Bytes Sent/sec is the rate at which bytes are sent over each network adapter, including framing characters.
        /// </summary>
        public UInt64 BytesSentPersec { get; set; }

        /// <summary>
        /// Bytes Received/sec is the rate at which bytes are received over each network adapter, including framing characters.
        /// </summary>
        public UInt64 BytesReceivedPersec { get; set; }

        // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-networkadapterconfiguration

        // WMI class: Win32_NetworkAdapterConfiguration

        /// <summary>
        /// Array of IP addresses of default gateways that the computer system uses.
        /// </summary>
        public List<IPAddress> DefaultIPGatewayList { get; set; } = new List<IPAddress>();

        /// <summary>
        /// IP address of the dynamic host configuration protocol (DHCP) server.
        /// </summary>
        public IPAddress DHCPServer { get; set; } = IPAddress.None;

        /// <summary>
        /// Array of server IP addresses to be used in querying for DNS servers.
        /// </summary>
        public List<IPAddress> DNSServerSearchOrderList { get; set; } = new List<IPAddress>();

        /// <summary>
        /// Array of all of the IP addresses associated with the current network adapter. 
        /// This property can contain either IPv6 addresses or IPv4 addresses.
        /// </summary>
        public List<IPAddress> IPAddressList { get; set; } = new List<IPAddress>();

        /// <summary>
        /// Array of all of the subnet masks associated with the current network adapter.
        /// </summary>
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
