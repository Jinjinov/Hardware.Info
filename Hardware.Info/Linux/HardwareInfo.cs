using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hardware.Info.Linux
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        const string KbToken = "kB";

        readonly MemoryStatus memoryStatus = new MemoryStatus();

        public MemoryStatus GetMemoryStatus()
        {
            string[] meminfo = File.ReadAllLines("/proc/meminfo");

            memoryStatus.TotalPhysical = GetBytesFromLine(meminfo, "MemTotal:");
            memoryStatus.AvailablePhysical = GetBytesFromLine(meminfo, "MemFree:");
            memoryStatus.TotalVirtual = GetBytesFromLine(meminfo, "SwapTotal:");
            memoryStatus.AvailableVirtual = GetBytesFromLine(meminfo, "SwapFree:");

            return memoryStatus;
        }

        private ulong GetBytesFromLine(string[] meminfo, string token)
        {
            string? memLine = meminfo.FirstOrDefault(x => x.StartsWith(token))?.Substring(token.Length);

            if (memLine != null && memLine.EndsWith(KbToken) && ulong.TryParse(memLine.Substring(0, memLine.Length - KbToken.Length), out ulong memKb))
                return memKb * 1024;

            return 0;
        }

        internal static Process StartProcess(string cmd, string args)
        {
            var psi = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            return Process.Start(psi);
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            BIOS bios = new BIOS();

            try
            {
                bios.Version = File.ReadAllText("/sys/class/dmi/id/bios_version").Trim();
            }
            catch
            {
                // Intentionally left blank
            }

            try
            {
                bios.Manufacturer = File.ReadAllText("/sys/class/dmi/id/bios_vendor").Trim();
            }
            catch
            {
                // Intentionally left blank
            }

            biosList.Add(bios);

            return biosList;
        }

        public List<CPU> GetCpuList()
        {
            List<CPU> cpuList = new List<CPU>();

            if (!File.Exists("/proc/cpuinfo"))
            {
                return cpuList;
            }

            try
            {
                File.OpenRead("/proc/cpuinfo").Dispose();
            }
            catch
            {
                return cpuList;
            }

            var info = File.ReadAllLines("/proc/cpuinfo");
            var modelNameRegex = new Regex(@"^model name\s+:\s+(.+)");
            var cpuSpeedRegex = new Regex(@"^cpu MHz\s+:\s+(.+)");
            var physicalCoresRegex = new Regex(@"^cpu cores\s+:\s+(.+)");
            var logicalCoresRegex = new Regex(@"^siblings\s+:\s+(.+)");

            CPU cpu = new CPU();

            foreach (var s in info)
            {
                try
                {
                    var match = modelNameRegex.Match(s);

                    if (match.Success)
                    {
                        cpu.Name = match.Groups[1].Value.Trim();

                        continue;
                    }

                    match = cpuSpeedRegex.Match(s);

                    if (match.Success)
                    {
                        cpu.CurrentClockSpeed = uint.Parse(match.Groups[1].Value);

                        continue;
                    }

                    match = physicalCoresRegex.Match(s);

                    if (match.Success)
                    {
                        cpu.NumberOfCores = uint.Parse(match.Groups[1].Value);

                        continue;
                    }

                    match = logicalCoresRegex.Match(s);

                    if (match.Success)
                    {
                        cpu.NumberOfLogicalProcessors = uint.Parse(match.Groups[1].Value);

                        continue;
                    }
                }
                catch
                {
                    // Intentionally left blank
                }
            }

            cpuList.Add(cpu);

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            try
            {
                var p = StartProcess("lshw", "-class disk");
                var sr = p.StandardOutput;
                p.WaitForExit();
                var lines = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                Drive? disk = null;

                foreach (var line in lines)
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
                            disk.Model = disk.Caption = line.Replace("product:", "").Trim();
                        }
                        else if (line.StartsWith("vendor:"))
                        {
                            disk.Manufacturer = line.Replace("vendor:", "").Trim();
                        }
                    }
                }

                if (disk != null)
                {
                    driveList.Add(disk);
                }
            }
            catch
            {
                // Intentionally left blank
            }

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            try
            {
                var p = StartProcess("lshw", "-short -C memory");
                var sr = p.StandardOutput;
                p.WaitForExit();
                var lines = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    try
                    {
                        var relevant = line.Split(new[] { "memory" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (relevant.Contains("DDR") || relevant.Contains("DIMM"))
                        {
                            var ram = new Memory();
                            var parts = relevant.Split(' ');

                            foreach (var part in parts)
                            {
                                var sizeRegex = new Regex("^([0-9]+)(K|M|G|T)iB");
                                var formFactor = Enum.GetNames(typeof(FormFactor)).FirstOrDefault(ff => ff == part);

                                if (formFactor != null)
                                {
                                    ram.FormFactor = (FormFactor)Enum.Parse(typeof(FormFactor), formFactor);
                                }
                                else if (new Regex("^[0-9]+$").IsMatch(part))
                                {
                                    ram.Speed = uint.Parse(part);
                                }
                                else if (sizeRegex.IsMatch(part))
                                {
                                    var match = sizeRegex.Match(part);
                                    var number = int.Parse(match.Groups[1].Value);
                                    var rawNumber = 0uL;
                                    var exponent = match.Groups[2].Value;

                                    if (exponent == "T")
                                    {
                                        rawNumber = (ulong)number * 1024uL * 1024uL * 1024uL * 1024uL;
                                    }
                                    else if (exponent == "G")
                                    {
                                        rawNumber = (ulong)number * 1024uL * 1024uL * 1024uL;
                                    }
                                    else if (exponent == "M")
                                    {
                                        rawNumber = (ulong)number * 1024uL * 1024uL;
                                    }
                                    else if (exponent == "K")
                                    {
                                        rawNumber = (ulong)number * 1024uL;
                                    }
                                    else
                                    {
                                        rawNumber = (ulong)number;
                                    }

                                    ram.Capacity = rawNumber;
                                }
                            }

                            memoryList.Add(ram);
                        }
                    }
                    catch
                    {
                        // Intentionally left blank
                    }
                }
            }
            catch
            {
                // Intentionally left blank
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            Motherboard motherboard = new Motherboard();

            try
            {
                motherboard.Product = File.ReadAllText("/sys/class/dmi/id/board_name").Trim();
            }
            catch
            {
                // Intentionally left blank
            }

            try
            {
                motherboard.Manufacturer = File.ReadAllText("/sys/class/dmi/id/board_vendor").Trim();
            }
            catch
            {
                // Intentionally left blank
            }

            motherboardList.Add(motherboard);

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            return mouseList;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList()
        {
            return base.GetNetworkAdapterList();
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            try
            {
                var p = StartProcess("lspci", "");
                using var sr = p.StandardOutput;
                p.WaitForExit();

                var lines = sr.ReadToEnd().Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.Contains("VGA compatible controller"))
                    {
                        try
                        {
                            var relevant = line.Split(':')[2];

                            if (!string.IsNullOrWhiteSpace(relevant))
                            {
                                var vendor = "";

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

                                var name = relevant.Replace(vendor, "").Replace("[AMD/ATI]", "");

                                var gpu = new VideoController { Description = relevant, Manufacturer = vendor, Name = name };

                                videoControllerList.Add(gpu);
                            }
                        }
                        catch
                        {
                            // Intentionally left blank
                        }
                    }
                }
            }
            catch
            {
                // Intentionally left blank
            }

            return videoControllerList;
        }
    }
}
