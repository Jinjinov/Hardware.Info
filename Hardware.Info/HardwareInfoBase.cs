using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Hardware.Info
{
    internal class HardwareInfoBase
    {
        public virtual List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                Drive drive = new Drive
                {
                    AvailableFreeSpace = driveInfo.AvailableFreeSpace,
                    DriveFormat = driveInfo.DriveFormat,
                    DriveType = driveInfo.DriveType.ToString(),
                    IsReady = driveInfo.IsReady,
                    Name = driveInfo.Name,
                    RootDirectory = driveInfo.RootDirectory.FullName,
                    TotalFreeSpace = driveInfo.TotalFreeSpace,
                    TotalSize = driveInfo.TotalSize,
                    VolumeLabel = driveInfo.VolumeLabel
                };

                driveList.Add(drive);
            }

            return driveList;
        }

        public virtual List<NetworkAdapter> GetNetworkAdapterList()
        {
            List<NetworkAdapter> networkAdapterList = GetNetworkAdapterList(NetworkInterfaceType.Wireless80211, false);

            if (networkAdapterList.Count == 0)
                GetNetworkAdapterList(NetworkInterfaceType.Ethernet, false);

            if (networkAdapterList.Count == 0)
                GetNetworkAdapterList(NetworkInterfaceType.Wireless80211, true);

            if (networkAdapterList.Count == 0)
                GetNetworkAdapterList(NetworkInterfaceType.Ethernet, true);

            return networkAdapterList;
        }

        protected List<NetworkAdapter> GetNetworkAdapterList(NetworkInterfaceType networkInterfaceType, bool anyStatus)
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == networkInterfaceType)
                {
                    if (anyStatus || networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        NetworkAdapter networkAdapter = new NetworkAdapter
                        {
                            MACAddress = networkInterface.GetPhysicalAddress().ToString().Trim(),
                            Description = networkInterface.Description.Trim(),
                            Name = networkInterface.Name.Trim(),
                            Speed = (ulong)networkInterface.Speed
                        };

                        foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                networkAdapter.IPAddress = ip.Address;
                                break;
                            }
                        }

                        networkAdapterList.Add(networkAdapter);
                    }
                }
            }

            return networkAdapterList;
        }
    }
}
