using System;
using System.Net.NetworkInformation;

namespace Hardware.Info.Test
{
    class Program
    {
        static readonly HardwareInfo hardwareInfo = new HardwareInfo();

        static void Main(string[] _)
        {
            //hardwareInfo.RefreshMemoryStatus();
            //hardwareInfo.RefreshBatteryList();
            //hardwareInfo.RefreshBIOSList();
            //hardwareInfo.RefreshCPUList();
            //hardwareInfo.RefreshDriveList();
            //hardwareInfo.RefreshKeyboardList();
            //hardwareInfo.RefreshMemoryList();
            //hardwareInfo.RefreshMonitorList();
            //hardwareInfo.RefreshMotherboardList();
            //hardwareInfo.RefreshMouseList();
            //hardwareInfo.RefreshNetworkAdapterList();
            //hardwareInfo.RefreshPrinterList();
            //hardwareInfo.RefreshSoundDeviceList();
            //hardwareInfo.RefreshVideoControllerList();

            hardwareInfo.RefreshAll();

            Console.WriteLine(hardwareInfo.MemoryStatus);

            foreach (var hardware in hardwareInfo.BatteryList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.BiosList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.CpuList)
            {
                Console.WriteLine(hardware);
                foreach (var cpuCore in hardware.CoresUsage)
                {
                    Console.WriteLine(cpuCore.Name);
                    Console.WriteLine(cpuCore.CoreUsage);
                }
            }

            foreach (var drive in hardwareInfo.DriveList)
            {
                Console.WriteLine(drive);

                foreach (var partition in drive.PartitionList)
                {
                    Console.WriteLine(partition);

                    foreach (var volume in partition.VolumeList)
                        Console.WriteLine(volume);
                }
            }

            Console.ReadLine();

            foreach (var hardware in hardwareInfo.KeyboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MemoryList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MonitorList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MotherboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MouseList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.NetworkAdapterList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.PrinterList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.SoundDeviceList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.VideoControllerList)
                Console.WriteLine(hardware);

            Console.ReadLine();

            foreach (var address in HardwareInfo.GetLocalIPv4Addresses(NetworkInterfaceType.Ethernet, OperationalStatus.Up))
                Console.WriteLine(address);

            Console.WriteLine();

            foreach (var address in HardwareInfo.GetLocalIPv4Addresses(NetworkInterfaceType.Wireless80211))
                Console.WriteLine(address);

            Console.WriteLine();

            foreach (var address in HardwareInfo.GetLocalIPv4Addresses(OperationalStatus.Up))
                Console.WriteLine(address);

            Console.WriteLine();

            foreach (var address in HardwareInfo.GetLocalIPv4Addresses())
                Console.WriteLine(address);

            foreach (var service in hardwareInfo.Services)
                Console.WriteLine(service);
            
            Console.ReadLine();
        }
    }
}
