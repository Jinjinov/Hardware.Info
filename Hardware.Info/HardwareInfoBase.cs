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

            Drive drive = new Drive();

            Partition partition = new Partition();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                Volume volume = new Volume
                {
                    FileSystem = driveInfo.DriveFormat,
                    Description = driveInfo.DriveType.ToString(),
                    Name = driveInfo.Name,
                    Caption = driveInfo.RootDirectory.FullName,
                    FreeSpace = (ulong)driveInfo.TotalFreeSpace,
                    Size = (ulong)driveInfo.TotalSize,
                    VolumeName = driveInfo.VolumeLabel
                };

                partition.VolumeList.Add(volume);
            }

            drive.PartitionList.Add(partition);

            driveList.Add(drive);

            return driveList;
        }

        public virtual List<NetworkAdapter> GetNetworkAdapterList()
        {
            List<NetworkAdapter> networkAdapterList = GetNetworkAdapterList(NetworkInterfaceType.Wireless80211, false);

            if (networkAdapterList.Count == 0)
                networkAdapterList = GetNetworkAdapterList(NetworkInterfaceType.Ethernet, false);

            if (networkAdapterList.Count == 0)
                networkAdapterList = GetNetworkAdapterList(NetworkInterfaceType.Wireless80211, true);

            if (networkAdapterList.Count == 0)
                networkAdapterList = GetNetworkAdapterList(NetworkInterfaceType.Ethernet, true);

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
                                networkAdapter.IPAddressList.Add(ip.Address);
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
