using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// https://www.binarytides.com/linux-commands-hardware-info/

namespace Hardware.Info.Linux
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        private readonly MemoryStatus memoryStatus = new MemoryStatus();

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
            const string KbToken = "kB";

            string? memLine = meminfo.FirstOrDefault(x => x.StartsWith(token))?.Substring(token.Length);

            if (memLine != null && memLine.EndsWith(KbToken) && ulong.TryParse(memLine.Substring(0, memLine.Length - KbToken.Length), out ulong memKb))
                return memKb * 1024;

            return 0;
        }

        internal static string TryReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            Battery battery = new Battery();

            // https://stackoverflow.com/questions/26888636/how-to-calculate-the-time-remaining-until-the-end-of-the-battery-charge

            // https://stackoverflow.com/questions/4858657/how-can-i-obtain-battery-level-inside-a-linux-kernel-module

            // /sys/class/power_supply/BAT0/charge_now

            // /sys/class/power_supply/BAT0/charge_full

            batteryList.Add(battery);

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            BIOS bios = new BIOS
            {
                ReleaseDate = TryReadFile("/sys/class/dmi/id/bios_date"),
                Version = TryReadFile("/sys/class/dmi/id/bios_version"),
                Manufacturer = TryReadFile("/sys/class/dmi/id/bios_vendor")
            };

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

            string[] info = File.ReadAllLines("/proc/cpuinfo");
            Regex modelNameRegex = new Regex(@"^model name\s+:\s+(.+)");
            Regex cpuSpeedRegex = new Regex(@"^cpu MHz\s+:\s+(.+)");
            Regex physicalCoresRegex = new Regex(@"^cpu cores\s+:\s+(.+)");
            Regex logicalCoresRegex = new Regex(@"^siblings\s+:\s+(.+)");

            CPU cpu = new CPU();

            foreach (string s in info)
            {
                try
                {
                    Match match = modelNameRegex.Match(s);

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

            foreach (string line in lines)
            {
                try
                {
                    string relevant = line.Split(new[] { "memory" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                    if (relevant.Contains("DDR") || relevant.Contains("DIMM"))
                    {
                        Memory ram = new Memory();
                        string[] parts = relevant.Split(' ');

                        foreach (string part in parts)
                        {
                            Regex sizeRegex = new Regex("^([0-9]+)(K|M|G|T)iB");
                            string formFactor = Enum.GetNames(typeof(FormFactor)).FirstOrDefault(ff => ff == part);

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
                                Match match = sizeRegex.Match(part);
                                int number = int.Parse(match.Groups[1].Value);
                                ulong rawNumber = 0uL;
                                string exponent = match.Groups[2].Value;

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
                Product = TryReadFile("/sys/class/dmi/id/board_name"),
                Manufacturer = TryReadFile("/sys/class/dmi/id/board_vendor"),
                SerialNumber = TryReadFile("/sys/class/dmi/id/board_serial")
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
            return base.GetNetworkAdapterList();
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
                try
                {
                    string relevant = line.Split(':')[2];

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

                        string name = relevant.Replace(vendor, string.Empty).Replace("[AMD/ATI]", string.Empty);

                        VideoController gpu = new VideoController { Description = relevant, Manufacturer = vendor, Name = name };

                        videoControllerList.Add(gpu);
                    }
                }
                catch
                {
                    // Intentionally left blank
                }
            }

            return videoControllerList;
        }
    }
}
