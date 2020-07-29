# Hardware.Info

Battery, BIOS, CPU, drives, keyboard, memory, monitors, motherboard, mouse, network adapters, printers, sound devices and video controllers. Hardware.Info is a .NET Standard 2.0 library and uses WMI on Windows, lshw on Linux and sysctl on macOS.

How to use:

1. Include NuGet package from https://www.nuget.org/packages/Hardware.Info

        <PackageReference Include="Hardware.Info" Version="0.0.1.0" />

2. Call `RefreshAll()` or one of the other `Refresh*()` methods:

        class Program
        {
            static readonly HardwareInfo hardwareInfo = new HardwareInfo();

            static void Main(string[] _)
            {
                hardwareInfo.RefreshAll();

                Console.WriteLine(hardwareInfo.MemoryStatus);

                foreach (var hardware in hardwareInfo.BatteryList)
                    Console.WriteLine(hardware);

                foreach (var hardware in hardwareInfo.BiosList)
                    Console.WriteLine(hardware);

                foreach (var hardware in hardwareInfo.CpuList)
                    Console.WriteLine(hardware);

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

                foreach (var address in HardwareInfo.GetLocalIPv4Address())
                    Console.WriteLine(address);

                Console.ReadLine();
            }
        }