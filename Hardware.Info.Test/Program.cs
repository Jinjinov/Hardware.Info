using System;
using System.Net.NetworkInformation;

namespace Hardware.Info.Test
{
    class Program
    {
        static readonly IHardwareInfo _hardwareInfo = new HardwareInfo(useAsteriskInWMI: false);

        static void Main(string[] _)
        {
            _hardwareInfo.RefreshMemoryStatus();
            _hardwareInfo.RefreshBatteryList();
            _hardwareInfo.RefreshBIOSList();
            _hardwareInfo.RefreshCPUList(includePercentProcessorTime: false);
            _hardwareInfo.RefreshDriveList();
            _hardwareInfo.RefreshKeyboardList();
            _hardwareInfo.RefreshMemoryList();
            _hardwareInfo.RefreshMonitorList();
            _hardwareInfo.RefreshMotherboardList();
            _hardwareInfo.RefreshMouseList();
            _hardwareInfo.RefreshNetworkAdapterList(includeBytesPerSec: false, includeNetworkAdapterConfiguration: true);
            _hardwareInfo.RefreshPrinterList();
            _hardwareInfo.RefreshSoundDeviceList();
            _hardwareInfo.RefreshVideoControllerList();

            //hardwareInfo.RefreshAll();

            Console.WriteLine(_hardwareInfo.MemoryStatus);

            foreach (var hardware in _hardwareInfo.BatteryList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.BiosList)
                Console.WriteLine(hardware);

            foreach (var cpu in _hardwareInfo.CpuList)
            {
                Console.WriteLine(cpu);

                foreach (var cpuCore in cpu.CpuCoreList)
                    Console.WriteLine(cpuCore);
            }

            Console.ReadLine();

            foreach (var drive in _hardwareInfo.DriveList)
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

            foreach (var hardware in _hardwareInfo.KeyboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.MemoryList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.MonitorList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.MotherboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.MouseList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.NetworkAdapterList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.PrinterList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.SoundDeviceList)
                Console.WriteLine(hardware);

            foreach (var hardware in _hardwareInfo.VideoControllerList)
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

            Console.ReadLine();
        }
    }
}
