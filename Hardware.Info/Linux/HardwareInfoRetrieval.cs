using System;
using System.Collections.Generic;
using System.Linq;
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

                    if (Version.TryParse(_os.VersionString, out Version version))
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

        public List<CPU> GetCpuList(bool includePercentProcessorTime = true)
        {
            List<CPU> cpuList = new List<CPU>();

            string[] lines = TryReadLinesFromFile("/proc/cpuinfo");

            Regex vendorIdRegex = new Regex(@"^vendor_id\s+:\s+(.+)");
            Regex modelNameRegex = new Regex(@"^model name\s+:\s+(.+)");
            Regex cpuSpeedRegex = new Regex(@"^cpu MHz\s+:\s+(.+)");
            Regex cacheSizeRegex = new Regex(@"^cache size\s+:\s+(.+)\s+KB");
            Regex physicalCoresRegex = new Regex(@"^cpu cores\s+:\s+(.+)");
            Regex logicalCoresRegex = new Regex(@"^siblings\s+:\s+(.+)");

            CPU cpu = new CPU();

            foreach (string line in lines)
            {
                Match match = vendorIdRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    cpu.Manufacturer = match.Groups[1].Value.Trim();
                    continue;
                }

                match = modelNameRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    cpu.Name = match.Groups[1].Value.Trim();
                    continue;
                }

                match = cpuSpeedRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (double.TryParse(match.Groups[1].Value, out double currentClockSpeed))
                        cpu.CurrentClockSpeed = (uint)currentClockSpeed;
                    continue;
                }

                match = cacheSizeRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint cacheSize))
                        cpu.L3CacheSize = cacheSize;
                    continue;
                }

                match = physicalCoresRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint numberOfCores))
                        cpu.NumberOfCores = numberOfCores;
                    continue;
                }

                match = logicalCoresRegex.Match(line);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (uint.TryParse(match.Groups[1].Value, out uint numberOfLogicalProcessors))
                        cpu.NumberOfLogicalProcessors = numberOfLogicalProcessors;
                    continue;
                }
            }

            if (includePercentProcessorTime)
            {
                GetCpuUsage(cpu);
            }

            cpuList.Add(cpu);

            return cpuList;
        }

        private static void GetCpuUsage(CPU cpu)
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

            if (cpuUsageLineLast.Length > 0 && cpuUsageLineNow.Length > 0)
            {
                cpu.PercentProcessorTime = GetCpuPercentage(cpuUsageLineLast[0], cpuUsageLineNow[0]);

                for (int i = 0; i < cpu.NumberOfLogicalProcessors; i++)
                {
                    string? cpuStatLast = cpuUsageLineLast.FirstOrDefault(s => s.StartsWith($"cpu{i}"));
                    string? cpuStatNow = cpuUsageLineNow.FirstOrDefault(s => s.StartsWith($"cpu{i}"));

                    if (!string.IsNullOrEmpty(cpuStatLast) && !string.IsNullOrEmpty(cpuStatNow))
                    {
                        CpuCore core = new CpuCore
                        {
                            Name = i.ToString(),
                            PercentProcessorTime = GetCpuPercentage(cpuStatLast, cpuStatNow)
                        };

                        cpu.CpuCoreList.Add(core);
                    }
                }
            }
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

            Keyboard keyboard = new Keyboard();

            // /dev/input/by-path/*-kbd

            // /proc/bus/input/devices

            keyboardList.Add(keyboard);

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

            Mouse mouse = new Mouse();

            // /dev/input/by-path/*-kbd

            // /proc/bus/input/devices

            mouseList.Add(mouse);

            return mouseList;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
        {
            List<NetworkAdapter> networkAdapterList = base.GetNetworkAdapterList(includeBytesPersec, includeNetworkAdapterConfiguration);

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

            printerList.Add(printer);

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            SoundDevice soundDevice = new SoundDevice();

            // /proc/asound/cards

            soundDeviceList.Add(soundDevice);

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            string processOutput = ReadProcessOutput("lspci", string.Empty);

            string[] lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

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
                        else if (relevant.ToUpperInvariant().Contains("NVIDIA"))
                        {
                            vendor = "NVIDIA Corporation";
                        }

                        string name = relevant.Replace("[AMD/ATI]", string.Empty);

                        if (!string.IsNullOrEmpty(vendor))
                            name = name.Replace(vendor, string.Empty);

                        VideoController gpu = new VideoController { Description = relevant, Manufacturer = vendor, Name = name };

                        videoControllerList.Add(gpu);
                    }
                }
            }

            return videoControllerList;
        }
    }
}
