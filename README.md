# Hardware.Info

Basic info on the battery, BIOS, CPU, drives, keyboard, memory, monitors, motherboard, mouse, network adapters, printers, sound devices and video controllers. Hardware.Info is a .NET Standard 2.0 library and uses WMI on Windows, lshw on Linux and sysctl on macOS.

    class Program
    {
        static readonly HardwareInfo hardwareInfo = new HardwareInfo();

        static void Main(string[] _)
        {
            foreach (var address in HardwareInfo.GetLocalIPv4Address())
                Console.WriteLine(address);

            Console.WriteLine();

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
        }
    }