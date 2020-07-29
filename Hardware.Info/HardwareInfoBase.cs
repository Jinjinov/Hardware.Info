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
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    MACAddress = networkInterface.GetPhysicalAddress().ToString().Trim(),
                    Description = networkInterface.Description.Trim(),
                    Name = networkInterface.Name.Trim(),
                    Speed = (ulong)networkInterface.Speed
                };

                foreach (UnicastIPAddressInformation addressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        networkAdapter.IPAddressList.Add(addressInformation.Address);
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }
    }
}
