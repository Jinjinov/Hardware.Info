using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// https://www.binarytides.com/linux-commands-hardware-info/

namespace Hardware.Info.Linux
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        private readonly MemoryStatus memoryStatus = new MemoryStatus();

        public MemoryStatus GetMemoryStatus()
        {
            string[] meminfo = TryReadFileLines("/proc/meminfo");

            memoryStatus.TotalPhysical = GetBytesFromLine(meminfo, "MemTotal:");
            memoryStatus.AvailablePhysical = GetBytesFromLine(meminfo, "MemAvailable:");
            memoryStatus.TotalVirtual = GetBytesFromLine(meminfo, "SwapTotal:");
            memoryStatus.AvailableVirtual = GetBytesFromLine(meminfo, "SwapFree:");

            return memoryStatus;
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

            string power_now = TryReadFileText("/sys/class/power_supply/BAT0/power_now");
            string energy_full_design = TryReadFileText("/sys/class/power_supply/BAT0/energy_full_design");
            string energy_full = TryReadFileText("/sys/class/power_supply/BAT0/energy_full");
            string energy_now = TryReadFileText("/sys/class/power_supply/BAT0/energy_now");

            uint.TryParse(power_now, out uint powerNow);
            uint.TryParse(energy_full_design, out uint designCapacity);
            uint.TryParse(energy_full, out uint fullChargeCapacity);
            uint.TryParse(energy_now, out uint energyNow);

            if (powerNow == 0)
                powerNow = 1;

            if (fullChargeCapacity == 0)
                fullChargeCapacity = 1;

            Battery battery = new Battery
            {
                DesignCapacity = designCapacity,
                FullChargeCapacity = fullChargeCapacity,
                BatteryStatusDescription = TryReadFileText("/sys/class/power_supply/BAT0/status"),

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
                ReleaseDate = TryReadFileText("/sys/class/dmi/id/bios_date"),
                Version = TryReadFileText("/sys/class/dmi/id/bios_version"),
                Manufacturer = TryReadFileText("/sys/class/dmi/id/bios_vendor")
            };

            biosList.Add(bios);

            return biosList;
        }

        public List<CPU> GetCpuList()
        {
            List<CPU> cpuList = new List<CPU>();

            string[] lines = TryReadFileLines("/proc/cpuinfo");

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

            GetCpuUsage(cpu);
            
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
            // 9	    guest   Time spent for running a virtual CPU or guest OS under the control of the kernel.

            // > cat /proc/stat 
            // cpu 1279636934 73759586 192327563 12184330186 543227057 56603 68503253 0 0
            // cpu0 297522664 8968710 49227610 418508635 72446546 56602 24904144 0 0
            // cpu1 227756034 9239849 30760881 424439349 196694821 0 7517172 0 0
            // cpu2 86902920 6411506 12412331 769921453 17877927 0 4809331 0 0
            // ... 

            string[] cpuUsageLineLast = TryReadFileLines("/proc/stat");
            Task.Delay(500).Wait();
            string[] cpuUsageLineNow = TryReadFileLines("/proc/stat");
            
            if (cpuUsageLineLast.Length > 0 && cpuUsageLineNow.Length > 0)
            {
                cpu.PercentProcessorTime = GetCpuPercentage(cpuUsageLineLast.First(), cpuUsageLineNow.First());
                
                for (int i = 0; i < cpu.NumberOfLogicalProcessors; i++)
                {
                    CpuCore core = new CpuCore
                    {
                        Name = i.ToString(),
                        PercentProcessorTime = GetCpuPercentage(cpuUsageLineLast.First(s => s.StartsWith($"cpu{i}")), cpuUsageLineNow.First(s => s.StartsWith($"cpu{i}")))
                    };

                    cpu.CpuCoreList.Add(core);
                }
            }
        }

        private static UInt64 GetCpuPercentage(string cpuStatLast, string cpuStatNow)
        {
            char[] charSeparators = new char[] { ' ' };

            // Get all columns but skip the first (which is the "cpu" string) 
            List<string> cpuSumLine = cpuStatNow.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            cpuSumLine.RemoveAt(0);

            // Get all columns but skip the first (which is the "cpu" string) 
            List<string> cpuLastSumLine = cpuStatLast.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            cpuLastSumLine.RemoveAt(0);

            ulong cpuSum = 0;
            cpuSumLine.ForEach(s => cpuSum += Convert.ToUInt64(s));

            ulong cpuLastSum = 0;
            cpuLastSumLine.ForEach(s => cpuLastSum += Convert.ToUInt64(s));

            // Get the delta between two reads 
            ulong cpuDelta = cpuSum - cpuLastSum;
            // Get the idle time Delta 
            ulong cpuIdle = Convert.ToUInt64(cpuSumLine[3]) - Convert.ToUInt64(cpuLastSumLine[3]);
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
                if (line.StartsWith("*-"))
                {
                    if (disk != null)
                    {
                        driveList.Add(disk);
                    }

                    disk = null;
                }

                if (line.StartsWith("*-disk:"))
                {
                    disk = new Drive();
                    continue;
                }

                if (disk != null)
                {
                    if (line.StartsWith("product:"))
                    {
                        disk.Model = disk.Caption = line.Replace("product:", string.Empty).Trim();
                    }
                    else if (line.StartsWith("vendor:"))
                    {
                        disk.Manufacturer = line.Replace("vendor:", string.Empty).Trim();
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

                    if (relevant.Contains("DDR") || relevant.Contains("DIMM"))
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
                Product = TryReadFileText("/sys/class/dmi/id/board_name"),
                Manufacturer = TryReadFileText("/sys/class/dmi/id/board_vendor"),
                SerialNumber = TryReadFileText("/sys/class/dmi/id/board_serial")
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

        public override List<NetworkAdapter> GetNetworkAdapterList()
        {
            List<NetworkAdapter> networkAdapterList =  base.GetNetworkAdapterList();

            char[] charSeparators = new char[] { ' ' };

            foreach (NetworkAdapter networkAdapter in networkAdapterList)
            {
                List<string>? networkAdapterUsageLast = TryReadFileLines("/proc/net/dev").FirstOrDefault(l => l.Trim().StartsWith(networkAdapter.Name))?.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                Task.Delay(1000).Wait();
                List<string>? networkAdapterUsageNow = TryReadFileLines("/proc/net/dev").FirstOrDefault(l => l.Trim().StartsWith(networkAdapter.Name))?.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (networkAdapterUsageLast != null && networkAdapterUsageLast.Count > 0 && networkAdapterUsageNow != null && networkAdapterUsageNow.Count > 0)
                {
                    networkAdapter.BytesReceivedPersec = Convert.ToUInt64(networkAdapterUsageNow[1]) - Convert.ToUInt64(networkAdapterUsageLast[1]);
                    networkAdapter.BytesSentPersec = Convert.ToUInt64(networkAdapterUsageNow[9]) - Convert.ToUInt64(networkAdapterUsageLast[9]);
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
