using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// https://www.binarytides.com/linux-commands-hardware-info/

namespace Hardware.Info.Linux
{
    internal class HardwareInfoRetrieval : HardwareInfoBase, IHardwareInfoRetrieval
    {
        private readonly MemoryStatus _memoryStatus = new MemoryStatus();

        private readonly OS _os = new OS();

        public OS GetOperatingSystem()
        {
            string[] lines = TryReadLinesFromFile("/etc/os-release");

            foreach (string line in lines)
            {
                if (line.StartsWith("NAME="))
                {
                    _os.Name = line.Replace("NAME=", string.Empty).Trim('"');
                }

                if (line.StartsWith("VERSION_ID="))
                {
                    _os.VersionString = line.Replace("VERSION_ID=", string.Empty).Trim('"');

                    if (Version.TryParse(_os.VersionString, out Version? version))
                        _os.Version = version;
                }
            }

            return _os;
        }

        public MemoryStatus GetMemoryStatus()
        {
            string[] meminfo = TryReadLinesFromFile("/proc/meminfo");

            _memoryStatus.TotalPhysical = GetBytesFromLine(meminfo, "MemTotal:");
            _memoryStatus.AvailablePhysical = GetBytesFromLine(meminfo, "MemAvailable:");
            _memoryStatus.TotalVirtual = GetBytesFromLine(meminfo, "SwapTotal:");
            _memoryStatus.AvailableVirtual = GetBytesFromLine(meminfo, "SwapFree:");

            return _memoryStatus;
        }

        private ulong GetBytesFromLine(string[] meminfo, string token)
        {
            const string KbToken = "kB";

            string? memLine = meminfo.FirstOrDefault(line => line.StartsWith(token) && line.EndsWith(KbToken));

            if (memLine != null)
            {
                string mem = memLine.Replace(token, string.Empty).Replace(KbToken, string.Empty).Trim();

                if (ulong.TryParse(mem, out ulong memKb))
                    return memKb * 1024;
            }

            return 0;
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            // https://stackoverflow.com/questions/26888636/how-to-calculate-the-time-remaining-until-the-end-of-the-battery-charge

            // https://stackoverflow.com/questions/4858657/how-can-i-obtain-battery-level-inside-a-linux-kernel-module

            // /sys/class/power_supply/BAT0/charge_now
            // /sys/class/power_supply/BAT0/charge_full

            // /sys/class/power_supply/BAT0/name = BAT0
            // /sys/class/power_supply/BAT0/status = Charging / Discharging
            // /sys/class/power_supply/BAT0/present = 1
            // /sys/class/power_supply/BAT0/technology = Li-ion
            // /sys/class/power_supply/BAT0/cycle_count = 0
            // /sys/class/power_supply/BAT0/voltage_min_design = 112530000
            // /sys/class/power_supply/BAT0/power_now = 21240000
            // /sys/class/power_supply/BAT0/energy_full_design = 50610000
            // /sys/class/power_supply/BAT0/energy_full = 50610000
            // /sys/class/power_supply/BAT0/energy_now  = 22460000
            // /sys/class/power_supply/BAT0/capacity = 44                   // 44 % full        // 50610000 / 22460000 = 0,4437858130804189
            // /sys/class/power_supply/BAT0/capacity_level = Normal
            // /sys/class/power_supply/BAT0/model_name = 
            // /sys/class/power_supply/BAT0/manufacturer = Sony Corp.
            // /sys/class/power_supply/BAT0/serial_number = 

            uint powerNow = TryReadIntegerFromFile("/sys/class/power_supply/BAT0/power_now", "/sys/class/power_supply/BAT0/voltage_now");
            uint designCapacity = TryReadIntegerFromFile("/sys/class/power_supply/BAT0/energy_full_design", "/sys/class/power_supply/BAT0/charge_full_design");
            uint fullChargeCapacity = TryReadIntegerFromFile("/sys/class/power_supply/BAT0/energy_full", "/sys/class/power_supply/BAT0/charge_full");
            uint energyNow = TryReadIntegerFromFile("/sys/class/power_supply/BAT0/energy_now", "/sys/class/power_supply/BAT0/charge_now");

            if (powerNow == 0)
                powerNow = 1;

            if (fullChargeCapacity == 0)
                fullChargeCapacity = 1;

            Battery battery = new Battery
            {
                DesignCapacity = designCapacity,
                FullChargeCapacity = fullChargeCapacity,
                BatteryStatusDescription = TryReadTextFromFile("/sys/class/power_supply/BAT0/status"),

                EstimatedChargeRemaining = (ushort)(energyNow * 100 / fullChargeCapacity), // current charge remaining in percentage
                EstimatedRunTime = energyNow / powerNow, // current remaining life in minutes
                ExpectedLife = fullChargeCapacity / powerNow, // total expected lifetime in minutes

                //TimeOnBattery = 1, // Elapsed time in seconds since the computer last switched to battery power

                MaxRechargeTime = fullChargeCapacity / powerNow, // total time, in minutes, to fully charge the battery
                TimeToFullCharge = (fullChargeCapacity - energyNow) / powerNow // Remaining time to charge the battery fully in minutes
            };

            batteryList.Add(battery);

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            BIOS bios = new BIOS
            {
                ReleaseDate = TryReadTextFromFile("/sys/class/dmi/id/bios_date"),
                Version = TryReadTextFromFile("/sys/class/dmi/id/bios_version"),
                Manufacturer = TryReadTextFromFile("/sys/class/dmi/id/bios_vendor")
            };

            biosList.Add(bios);

            return biosList;
        }

        private class Processor
        {
            public string ProcessorId = string.Empty;
            public string VendorId = string.Empty;
            public string ModelName = string.Empty;
            public uint CpuMhz;
            public uint CacheSize;
            public uint PhysicalId;
            public uint Siblings;
            public uint CoreId;
            public uint CpuCores;

            public uint L1DataCacheSize;
            public uint L1InstructionCacheSize;
            public uint L2CacheSize;
            public uint L3CacheSize;

            public ulong PercentProcessorTime;
        }

        public List<CPU> GetCpuList(bool includePercentProcessorTime = true)
        {
            string[] lines = TryReadLinesFromFile("/proc/cpuinfo");

            Regex processorRegex = new Regex(@"^processor\s+:\s+(\d+)"); // processor ID (from 0 to 7 in a PC with two quad core CPUs)
            Regex vendorIdRegex = new Regex(@"^vendor_id\s+:\s+(.+)");
            Regex modelNameRegex = new Regex(@"^model name\s+:\s+(.+)");
            Regex cpuSpeedRegex = new Regex(@"^cpu MHz\s+:\s+(.+)");
            Regex cacheSizeRegex = new Regex(@"^cache size\s+:\s+(.+)\s+KB"); // L2 cache size in KB
            Regex physicalIdRegex = new Regex(@"^physical id\s+:\s+(\d+)"); // physical CPU ID (a PC with two quad core CPUs, 4 cores will have one value, and the other 4 will have another value)
            Regex logicalCoresRegex = new Regex(@"^siblings\s+:\s+(.+)"); // number of logical cores (no hyperthreading = same as physical, with hyperthreading = 2 * physical)
            Regex coreIdRegex = new Regex(@"^core id\s+:\s+(.+)"); // core ID (from 0 to 3 in a PC with two quad core CPUs - for the first CPU, and then the same for second CPU)
            Regex physicalCoresRegex = new Regex(@"^cpu cores\s+:\s+(.+)"); // number of cores (in a quad core CPU = 4)

            List<Processor> processorList = new List<Processor>();

            Processor processor = null!;

            foreach (string line in lines)
            {
                Match match = processorRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    processor = new Processor();

                    if (uint.TryParse(match.Groups[1].Value, out uint processorId))
                    {
                        processor.ProcessorId = $"cpu{processorId}";

                        GetCpuCacheSize(processor);

                        processorList.Add(processor);
                    }
                    continue;
                }

                if (processor == null)
                {
                    continue;
                }

                match = vendorIdRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    processor.VendorId = match.Groups[1].Value.Trim();
                    continue;
                }

                match = modelNameRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    processor.ModelName = match.Groups[1].Value.Trim();
                    continue;
                }

                match = cpuSpeedRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (double.TryParse(match.Groups[1].Value, out double currentClockSpeed))
                        processor.CpuMhz = (uint)currentClockSpeed;
                    continue;
                }

                match = cacheSizeRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint cacheSize))
                        processor.CacheSize = 1024 * cacheSize;
                    continue;
                }

                match = physicalIdRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint physicalId))
                        processor.PhysicalId = physicalId;
                    continue;
                }

                match = logicalCoresRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint numberOfLogicalProcessors))
                        processor.Siblings = numberOfLogicalProcessors;
                    continue;
                }

                match = coreIdRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint coreId))
                        processor.CoreId = coreId;
                    continue;
                }

                match = physicalCoresRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint numberOfCores))
                        processor.CpuCores = numberOfCores;
                    continue;
                }
            }

            ulong percentProcessorTime = 0;

            if (includePercentProcessorTime)
            {
                percentProcessorTime = GetCpuUsage(processorList);
            }

            List<CPU> cpuList = new List<CPU>();

            foreach (var group in processorList.GroupBy(processor => processor.PhysicalId))
            {
                CPU cpu = new CPU();

                cpu.PercentProcessorTime = percentProcessorTime;

                cpu.ProcessorId = group.Key.ToString();

                Processor first = group.First();

                cpu.Manufacturer = first.VendorId;
                cpu.Name = first.ModelName;
                cpu.CurrentClockSpeed = first.CpuMhz;
                //cpu.L2CacheSize = first.CacheSize;
                cpu.NumberOfLogicalProcessors = first.Siblings;
                cpu.NumberOfCores = first.CpuCores;

                cpu.L1DataCacheSize = first.L1DataCacheSize;
                cpu.L1InstructionCacheSize = first.L1InstructionCacheSize;
                cpu.L2CacheSize = first.L2CacheSize;
                cpu.L3CacheSize = first.L3CacheSize;

                foreach (Processor proc in group)
                {
                    CpuCore core = new CpuCore
                    {
                        Name = proc.ProcessorId,
                        PercentProcessorTime = proc.PercentProcessorTime
                    };

                    cpu.CpuCoreList.Add(core);
                }

                cpuList.Add(cpu);
            }

            return cpuList;
        }

        private static void GetCpuCacheSize(Processor processor)
        {
            for (int i = 0; i <= 3; i++)
            {
                string level = TryReadTextFromFile($"/sys/devices/system/cpu/{processor.ProcessorId}/cache/index{i}/level");
                string type = TryReadTextFromFile($"/sys/devices/system/cpu/{processor.ProcessorId}/cache/index{i}/type");
                string size = TryReadTextFromFile($"/sys/devices/system/cpu/{processor.ProcessorId}/cache/index{i}/size");

                if (uint.TryParse(size.TrimEnd('K'), out uint cacheSize))
                {
                    cacheSize *= 1024;

                    if (level == "1")
                    {
                        if (type == "Data")
                            processor.L1DataCacheSize = cacheSize;

                        if (type == "Instruction")
                            processor.L1InstructionCacheSize = cacheSize;
                    }

                    if (level == "2")
                        processor.L2CacheSize = cacheSize;

                    if (level == "3")
                        processor.L3CacheSize = cacheSize;
                }
            }
        }

        private static ulong GetCpuUsage(List<Processor> processorList)
        {
            // Column   Name    Description
            // 1        user    Time spent with normal processing in user mode.
            // 2        nice    Time spent with niced processes in user mode.
            // 3        system  Time spent running in kernel mode.
            // 4        idle    Time spent in vacations twiddling thumbs.
            // 5        iowait  Time spent waiting for I / O to completed.This is considered idle time too.
            // 6        irq     Time spent serving hardware interrupts.See the description of the intr line for more details.
            // 7        softirq Time spent serving software interrupts.
            // 8        steal   Time stolen by other operating systems running in a virtual environment.
            // 9        guest   Time spent for running a virtual CPU or guest OS under the control of the kernel.

            // > cat /proc/stat 
            // cpu 1279636934 73759586 192327563 12184330186 543227057 56603 68503253 0 0
            // cpu0 297522664 8968710 49227610 418508635 72446546 56602 24904144 0 0
            // cpu1 227756034 9239849 30760881 424439349 196694821 0 7517172 0 0
            // cpu2 86902920 6411506 12412331 769921453 17877927 0 4809331 0 0
            // ... 

            string[] cpuUsageLineLast = TryReadLinesFromFile("/proc/stat");
            Task.Delay(500).Wait();
            string[] cpuUsageLineNow = TryReadLinesFromFile("/proc/stat");

            ulong percentProcessorTime = 0;

            if (cpuUsageLineLast.Length > 0 && cpuUsageLineNow.Length > 0)
            {
                percentProcessorTime = GetCpuPercentage(cpuUsageLineLast[0], cpuUsageLineNow[0]);

                foreach (Processor processor in processorList)
                {
                    string? cpuStatLast = cpuUsageLineLast.FirstOrDefault(s => s.StartsWith(processor.ProcessorId));
                    string? cpuStatNow = cpuUsageLineNow.FirstOrDefault(s => s.StartsWith(processor.ProcessorId));

                    if (!string.IsNullOrEmpty(cpuStatLast) && !string.IsNullOrEmpty(cpuStatNow))
                    {
                        processor.PercentProcessorTime = GetCpuPercentage(cpuStatLast, cpuStatNow);
                    }
                }
            }

            return percentProcessorTime;
        }

        private static UInt64 GetCpuPercentage(string cpuStatLast, string cpuStatNow)
        {
            char[] charSeparators = new char[] { ' ' };

            // Get all columns but skip the first (which is the "cpu" string) 
            List<string> cpuSumLineNow = cpuStatNow.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            cpuSumLineNow.RemoveAt(0);

            // Get all columns but skip the first (which is the "cpu" string) 
            List<string> cpuSumLineLast = cpuStatLast.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            cpuSumLineLast.RemoveAt(0);

            ulong cpuSumNow = 0;

            foreach (string cpuLineNow in cpuSumLineNow)
            {
                if (ulong.TryParse(cpuLineNow, out ulong cpuNow))
                    cpuSumNow += cpuNow;
            }

            ulong cpuSumLast = 0;

            foreach (string cpuLineLast in cpuSumLineLast)
            {
                if (ulong.TryParse(cpuLineLast, out ulong cpuLast))
                    cpuSumLast += cpuLast;
            }

            // Get the delta between two reads 
            ulong cpuDelta = cpuSumNow - cpuSumLast;

            // Get the idle time Delta 
            ulong cpuIdle = 0;

            if (cpuSumLineNow.Count > 3 && cpuSumLineLast.Count > 3)
            {
                if (ulong.TryParse(cpuSumLineNow[3], out ulong cpuIdleNow) && ulong.TryParse(cpuSumLineLast[3], out ulong cpuIdleLast))
                {
                    cpuIdle = cpuIdleNow - cpuIdleLast;
                }
            }

            // Calc percentage 
            ulong cpuUsed = cpuDelta - cpuIdle;

            return 100 * cpuUsed / cpuDelta;
        }

        public override List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            string processOutput = ReadProcessOutput("lshw", "-class disk");

            string[] lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Drive? disk = null;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("*-"))
                {
                    if (disk != null)
                    {
                        driveList.Add(disk);
                    }

                    disk = null;
                }

                if (trimmed.StartsWith("*-cdrom") || trimmed.StartsWith("*-disk"))
                {
                    disk = new Drive();
                    continue;
                }

                if (disk != null)
                {
                    if (trimmed.StartsWith("product:"))
                    {
                        disk.Model = disk.Caption = trimmed.Replace("product:", string.Empty).Trim();
                    }
                    else if (trimmed.StartsWith("vendor:"))
                    {
                        disk.Manufacturer = trimmed.Replace("vendor:", string.Empty).Trim();
                    }
                }
            }

            if (disk != null)
            {
                driveList.Add(disk);
            }

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            // /dev/input/by-path/*-kbd

            // xinput list // https://unix.stackexchange.com/questions/58117/determine-xinput-device-manufacturer-and-model

            string[] inputDevices = TryReadLinesFromFile("/proc/bus/input/devices");

            foreach (string inputDevice in inputDevices)
            {
                if (inputDevice.StartsWith("N: Name=") && inputDevice.IndexOf("keyboard", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string[] name = inputDevice.Split('\"');

                    if (name.Length > 1)
                    {
                        Keyboard keyboard = new Keyboard()
                        {
                            Caption = name[1],
                            Description = name[1],
                            Name = name[1]
                        };

                        keyboardList.Add(keyboard);
                    }
                }
            }

            /*
            I: Bus=0019 Vendor=0000 Product=0001 Version=0000
            N: Name="Power Button"
            P: Phys=LNXPWRBN/button/input0
            S: Sysfs=/devices/LNXSYSTM:00/LNXPWRBN:00/input/input0
            U: Uniq=
            H: Handlers=kbd event0 
            B: PROP=0
            B: EV=3
            B: KEY=10000000000000 0

            I: Bus=0019 Vendor=0000 Product=0003 Version=0000
            N: Name="Sleep Button"
            P: Phys=LNXSLPBN/button/input0
            S: Sysfs=/devices/LNXSYSTM:00/LNXSLPBN:00/input/input1
            U: Uniq=
            H: Handlers=kbd event1 
            B: PROP=0
            B: EV=3
            B: KEY=4000 0 0

            I: Bus=0011 Vendor=0001 Product=0001 Version=ab41
            N: Name="AT Translated Set 2 keyboard"
            P: Phys=isa0060/serio0/input0
            S: Sysfs=/devices/platform/i8042/serio0/input/input2
            U: Uniq=
            H: Handlers=sysrq kbd event2 leds 
            B: PROP=0
            B: EV=120013
            B: KEY=402000000 3803078f800d001 feffffdfffefffff fffffffffffffffe
            B: MSC=10
            B: LED=7

            I: Bus=0019 Vendor=0000 Product=0006 Version=0000
            N: Name="Video Bus"
            P: Phys=LNXVIDEO/video/input0
            S: Sysfs=/devices/LNXSYSTM:00/LNXSYBUS:00/PNP0A03:00/LNXVIDEO:00/input/input5
            U: Uniq=
            H: Handlers=kbd event3 
            B: PROP=0
            B: EV=3
            B: KEY=3e000b00000000 0 0 0

            I: Bus=0011 Vendor=0002 Product=0006 Version=0000
            N: Name="ImExPS/2 Generic Explorer Mouse"
            P: Phys=isa0060/serio1/input0
            S: Sysfs=/devices/platform/i8042/serio1/input/input4
            U: Uniq=
            H: Handlers=mouse0 event4 
            B: PROP=1
            B: EV=7
            B: KEY=1f0000 0 0 0 0
            B: REL=143

            I: Bus=0001 Vendor=80ee Product=cafe Version=0000
            N: Name="VirtualBox mouse integration"
            P: Phys=
            S: Sysfs=/devices/pci0000:00/0000:00:04.0/input/input7
            U: Uniq=
            H: Handlers=mouse2 event6 js1 
            B: PROP=0
            B: EV=b
            B: KEY=10000 0 0 0 0
            B: ABS=3

            I: Bus=0003 Vendor=80ee Product=0021 Version=0110
            N: Name="VirtualBox USB Tablet"
            P: Phys=usb-0000:00:06.0-1/input0
            S: Sysfs=/devices/pci0000:00/0000:00:06.0/usb2/2-1/2-1:1.0/0003:80EE:0021.0006/input/input12
            U: Uniq=
            H: Handlers=mouse1 event5 js0 
            B: PROP=0
            B: EV=1f
            B: KEY=1f0000 0 0 0 0
            B: REL=1940
            B: ABS=3
            B: MSC=10
            /**/

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            string processOutput = ReadProcessOutput("lshw", "-short -C memory");

            string[] lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string[] formFactorNames = Enum.GetNames(typeof(FormFactor));

            foreach (string line in lines)
            {
                string[] split = line.Split(new[] { "memory" }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length > 1)
                {
                    string relevant = split[1].Trim();

                    if (relevant.Contains("DDR") || relevant.Contains("DIMM") || relevant.Contains("System"))
                    {
                        Memory ram = new Memory();

                        string[] parts = relevant.Split(' ');

                        foreach (string part in parts)
                        {
                            Regex sizeRegex = new Regex("^([0-9]+)(K|M|G|T)iB");

                            if (formFactorNames.Any(name => name == part))
                            {
                                if (Enum.TryParse(part, out FormFactor formFactor))
                                    ram.FormFactor = formFactor;
                            }
                            else if (new Regex("^[0-9]+$").IsMatch(part))
                            {
                                if (uint.TryParse(part, out uint speed))
                                    ram.Speed = speed;
                            }
                            else if (sizeRegex.IsMatch(part))
                            {
                                Match match = sizeRegex.Match(part);

                                if (match.Groups.Count > 2)
                                {
                                    if (ulong.TryParse(match.Groups[1].Value, out ulong number))
                                    {
                                        string exponent = match.Groups[2].Value;

                                        ram.Capacity = exponent switch
                                        {
                                            "T" => number * 1024uL * 1024uL * 1024uL * 1024uL,
                                            "G" => number * 1024uL * 1024uL * 1024uL,
                                            "M" => number * 1024uL * 1024uL,
                                            "K" => number * 1024uL,
                                            _ => number,
                                        };
                                    }
                                }
                            }
                        }

                        memoryList.Add(ram);
                    }
                }
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            Monitor monitor = new Monitor();

            // https://superuser.com/questions/526497/how-do-you-find-out-a-laptop-screen-panel-manufacturer-model-with-linux-sams

            // /sys/class/graphics/fb0/mode

            // /sys/class/drm/ * /edid

            // https://github.com/linuxhw/EDID

            monitorList.Add(monitor);

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            Motherboard motherboard = new Motherboard
            {
                Product = TryReadTextFromFile("/sys/class/dmi/id/board_name"),
                Manufacturer = TryReadTextFromFile("/sys/class/dmi/id/board_vendor"),
                SerialNumber = TryReadTextFromFile("/sys/class/dmi/id/board_serial")
            };

            motherboardList.Add(motherboard);

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            // /dev/input/by-path/*-kbd

            // xinput list // https://unix.stackexchange.com/questions/58117/determine-xinput-device-manufacturer-and-model

            string[] inputDevices = TryReadLinesFromFile("/proc/bus/input/devices");

            foreach (string inputDevice in inputDevices)
            {
                if (inputDevice.StartsWith("N: Name=") && inputDevice.IndexOf("mouse", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string[] name = inputDevice.Split('\"');

                    if (name.Length > 1)
                    {
                        Mouse mouse = new Mouse()
                        {
                            Caption = name[1],
                            Description = name[1],
                            Name = name[1]
                        };

                        mouseList.Add(mouse);
                    }
                }
            }

            return mouseList;
        }

        private List<NetworkAdapter> GetNetworkAdapters()
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            string[] route = TryReadLinesFromFile("/proc/net/route");
            string[] fibTrieLines = TryReadLinesFromFile("/proc/net/fib_trie");

            foreach (string interfaceDirectory in Directory.EnumerateDirectories("/sys/class/net"))
            {
                string interfaceName = Path.GetFileName(interfaceDirectory);

                string macAddress = TryReadTextFromFile($"/sys/class/net/{interfaceName}/address");

                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    Caption = interfaceName,
                    Description = interfaceName,
                    Name = interfaceName,
                    MACAddress = macAddress,
                    NetConnectionID = interfaceName,
                    ProductName = interfaceName,
                };

                string speedString = TryReadTextFromFile($"/sys/class/net/{interfaceName}/speed");

                if (ulong.TryParse(speedString, out ulong speed))
                {
                    networkAdapter.Speed = speed;
                }

                foreach (string line in route)
                {
                    string[] parts = line.Split('\t');

                    if (parts.Length >= 8 && parts[0] == interfaceName)
                    {
                        if (parts[1] == "00000000" && parts[7] == "00000000")
                        {
                            string gatewayHex = parts[2];
                            byte[] gatewayBytes = BitConverter.GetBytes(Convert.ToInt32(gatewayHex, 16));
                            Array.Reverse(gatewayBytes);
                            IPAddress gatewayAddress = new IPAddress(gatewayBytes);

                            networkAdapter.DefaultIPGatewayList.Add(gatewayAddress);
                        }

                        if (parts[2] == "00000000" && parts[7] != "FFFFFFFF")
                        {
                            string network = HexToDecimalIP(parts[1]);
                            string mask = HexToDecimalIP(parts[7]);

                            if (TryGetInterfaceIp(fibTrieLines, network, out string interfaceIp))
                            {
                                networkAdapter.IPAddressList.Add(IPAddress.Parse(interfaceIp));
                                networkAdapter.IPSubnetList.Add(IPAddress.Parse(mask));
                            }
                        }
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }

        private string HexToDecimalIP(string hex)
        {
            return int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) + "." +
                   int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) + "." +
                   int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) + "." +
                   int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        }

        private bool TryGetInterfaceIp(string[] fibTrieLines, string network, out string ip)
        {
            ip = string.Empty;

            bool foundLocal = false;
            bool foundNetwork = false;
            bool foundIp = false;

            foreach (string line in fibTrieLines)
            {
                if (line.StartsWith("Local"))
                {
                    foundLocal = true;
                }
                else if (foundLocal)
                {
                    if (line.Contains(network))
                    {
                        foundNetwork = true;
                    }
                    else if (foundNetwork)
                    {
                        if (line.Contains("32 host"))
                        {
                            foundIp = true;
                            break;
                        }

                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ip = parts[1];
                    }
                }
            }

            return foundIp;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
        {
            List<NetworkAdapter> networkAdapterList;

            try
            {
                networkAdapterList = base.GetNetworkAdapterList(includeBytesPersec, includeNetworkAdapterConfiguration);
            }
            catch (NetworkInformationException)
            {
                networkAdapterList = GetNetworkAdapters();
            }

            if (includeBytesPersec)
            {
                char[] charSeparators = new char[] { ' ' };

                string[] procNetDevLast = TryReadLinesFromFile("/proc/net/dev");
                Task.Delay(1000).Wait();
                string[] procNetDevNow = TryReadLinesFromFile("/proc/net/dev");

                foreach (NetworkAdapter networkAdapter in networkAdapterList)
                {
                    List<string>? networkAdapterUsageLast = procNetDevLast.FirstOrDefault(l => l.Trim().StartsWith(networkAdapter.Name))?.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    List<string>? networkAdapterUsageNow = procNetDevNow.FirstOrDefault(l => l.Trim().StartsWith(networkAdapter.Name))?.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                    if (networkAdapterUsageLast != null && networkAdapterUsageNow != null)
                    {
                        if (networkAdapterUsageLast.Count > 1 && networkAdapterUsageNow.Count > 1)
                        {
                            if (ulong.TryParse(networkAdapterUsageNow[1], out ulong bytesReceivedPersecNow) && ulong.TryParse(networkAdapterUsageLast[1], out ulong bytesReceivedPersecLast))
                            {
                                networkAdapter.BytesReceivedPersec = bytesReceivedPersecNow - bytesReceivedPersecLast;
                            }
                        }

                        if (networkAdapterUsageLast.Count > 9 && networkAdapterUsageNow.Count > 9)
                        {
                            if (ulong.TryParse(networkAdapterUsageNow[9], out ulong bytesSentPersecNow) && ulong.TryParse(networkAdapterUsageLast[9], out ulong bytesSentPersecLast))
                            {
                                networkAdapter.BytesSentPersec = bytesSentPersecNow - bytesSentPersecLast;
                            }
                        }
                    }
                }
            }

            return networkAdapterList;
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            Printer printer = new Printer();

            // /dev/usb/lp0

            // lpstat -p

            printerList.Add(printer);

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            // lspci -v | grep -i audio
            // lspci | grep -i audio
            /*
            00:1b.0 Audio device: Intel Corporation 7 Series/C210 Series Chipset Family High Definition Audio Controller (rev 04)
            01:00.1 Audio device: NVIDIA Corporation GK104 HDMI Audio Controller (rev a1)
            /**/

            string processOutput = ReadProcessOutput("lspci", string.Empty);

            string[] lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.Contains("Audio device") || line.Contains("Multimedia audio controller"))
                {
                    string[] name = line.Split(new[] { "Audio device: ", "Multimedia audio controller: " }, StringSplitOptions.RemoveEmptyEntries);

                    if (name.Length > 1)
                    {
                        SoundDevice soundDevice = new SoundDevice()
                        {
                            Caption = name[1],
                            Description = name[1],
                            Name = name[1]
                        };

                        soundDeviceList.Add(soundDevice);
                    }
                }
            }

            if (soundDeviceList.Count > 0)
                return soundDeviceList;

            // cat /proc/asound/cards
            /*
            0 [Intel          ]: HDA-Intel - HDA Intel
                      HDA Intel at 0x93300000 irq 22
            1 [SAA7134        ]: SAA7134 - SAA7134
                      saa7133[0] at 0x9300c800 irq 21
            /**/

            string[] soundCards = TryReadLinesFromFile("/proc/asound/cards");

            foreach (string soundCard in soundCards)
            {
                if (soundCard.Contains(" at "))
                {
                    string[] name = soundCard.Split(new[] { " at " }, StringSplitOptions.RemoveEmptyEntries);

                    if (name.Length > 0)
                    {
                        string trimmed = name[0].Trim();

                        SoundDevice soundDevice = new SoundDevice()
                        {
                            Caption = trimmed,
                            Description = trimmed,
                            Name = trimmed
                        };

                        soundDeviceList.Add(soundDevice);
                    }
                }
            }

            if (soundDeviceList.Count > 0)
                return soundDeviceList;

            // aplay -l
            /*
            card 0: Intel[HDA Intel], device 0: AD198x Analog[AD198x Analog]
              Subdevices: 1 / 1
              Subdevice #0: subdevice #0
            card 0: Intel[HDA Intel], device 1: AD198x Digital[AD198x Digital]
              Subdevices: 1 / 1
              Subdevice #0: subdevice #0

            card 0: Intel [HDA Intel], device 0: STAC92xx Analog [STAC92xx Analog]
              Subdevices: 2/2
              Subdevice #0: subdevice #0
              Subdevice #1: subdevice #1
            card 1: SAA7134 [SAA7134], device 0: SAA7134 PCM [SAA7134 PCM]
              Subdevices: 1/1
              Subdevice #0: subdevice #0
            /**/

            processOutput = ReadProcessOutput("aplay", "-l");

            lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("card "))
                {
                    string[] name = line.Split(':');

                    if (name.Length > 0)
                    {
                        string trimmed = name[name.Length - 1].Trim();

                        SoundDevice soundDevice = new SoundDevice()
                        {
                            Caption = trimmed,
                            Description = trimmed,
                            Name = trimmed
                        };

                        soundDeviceList.Add(soundDevice);
                    }
                }
            }

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            uint currentHorizontalResolution = 0;
            uint currentVerticalResolution = 0;
            uint currentRefreshRate = 0;

            string processOutput = ReadProcessOutput("xrandr", "-q");

            string[] lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.Contains('*') && line.Contains('x'))
                {
                    string[] currentMode = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (currentMode.Length > 1)
                    {
                        string resolution = currentMode[0];

                        if (resolution.Contains('x'))
                        {
                            string[] resolutionXY = resolution.Split('x');

                            if (resolutionXY.Length > 1)
                            {
                                string resolutionX = resolutionXY[0];

                                if (uint.TryParse(resolutionX, out uint rx))
                                {
                                    currentHorizontalResolution = rx;
                                }

                                string resolutionY = resolutionXY[1];

                                if (uint.TryParse(resolutionY, out uint ry))
                                {
                                    currentVerticalResolution = ry;
                                }
                            }
                        }

                        string refreshRate = currentMode[1].Trim('*', '+');

                        if (double.TryParse(refreshRate, out double rr))
                        {
                            currentRefreshRate = (uint)Math.Round(rr);
                        }
                    }
                }
            }

            // xrandr -q

            /*
            Screen 0: minimum 320 x 200, current 1280 x 800, maximum 4096 x 4096
            VGA1 disconnected (normal left inverted right x axis y axis)
            LVDS1 connected 1280x800+0+0 inverted X and Y axis (normal left inverted right x axis y axis) 261mm x 163mm
               1280x800       59.8*+
               1024x768       60.0
               800x600        60.3     56.2
               640x480        59.9
            DVI1 disconnected (normal left inverted right x axis y axis)
            TV1 disconnected (normal left inverted right x axis y axis)

            Screen 0: minimum 320 x 200, current 1440 x 900, maximum 8192 x 8192
            VGA-1 disconnected (normal left inverted right x axis y axis)
            LVDS-1 connected 1440x900+0+0 (normal left inverted right x axis y axis) 304mm x 190mm
               1440x900       60.1*+
               1024x768       60.0
               800x600        60.3
               640x480        59.9

            Screen 0: minimum 320 x 200, current 3200 x 1080, maximum 8192 x 8192
            VGA-1 disconnected (normal left inverted right x axis y axis)
            HDMI-1 connected primary 1920x1080+0+0 (normal left inverted right x axis y axis) 531mm x 299mm
               1920x1080     59.93 +  60.00*   50.00    59.94  
               1920x1080i    60.00    50.00    59.94  
               1680x1050     59.88  
            /**/

            processOutput = ReadProcessOutput("lspci", string.Empty);

            lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines.Where(l => l.Contains("VGA compatible controller")))
            {
                string[] split = line.Split(':');

                if (split.Length > 2)
                {
                    string relevant = split[2].Trim();

                    if (!string.IsNullOrWhiteSpace(relevant))
                    {
                        string vendor = string.Empty;

                        if (relevant.Contains("Intel"))
                        {
                            vendor = "Intel Corporation";
                        }
                        else if (relevant.Contains("AMD") || relevant.Contains("Advanced Micro Devices") || relevant.Contains("ATI"))
                        {
                            vendor = "Advanced Micro Devices, Inc.";
                        }
                        else if (relevant.Contains("NVIDIA", StringComparison.InvariantCultureIgnoreCase))
                        {
                            vendor = "NVIDIA Corporation";
                        }

                        string name = relevant.Replace("[AMD/ATI]", string.Empty);

                        if (!string.IsNullOrEmpty(vendor))
                            name = name.Replace(vendor, string.Empty);

                        VideoController gpu = new VideoController
                        {
                            Description = relevant,
                            Manufacturer = vendor,
                            Name = name,
                            CurrentHorizontalResolution = currentHorizontalResolution,
                            CurrentVerticalResolution = currentVerticalResolution,
                            CurrentRefreshRate = currentRefreshRate
                        };

                        videoControllerList.Add(gpu);
                    }
                }
            }

            return videoControllerList;
        }
    }
}
