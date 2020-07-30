using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hardware.Info.Mac
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        static IntPtr SizeOfLineSize = (IntPtr)IntPtr.Size;

        [DllImport("libc")]
        static extern int sysctlbyname(string name, out IntPtr oldp, ref IntPtr oldlenp, IntPtr newp, IntPtr newlen);

        readonly MemoryStatus memoryStatus = new MemoryStatus();

        public MemoryStatus GetMemoryStatus()
        {
            if (sysctlbyname("hw.memsize", out IntPtr lineSize, ref SizeOfLineSize, IntPtr.Zero, IntPtr.Zero) == 0)
            {
                memoryStatus.TotalPhysical = (ulong)lineSize.ToInt64();
            }

            return memoryStatus;
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            return biosList;
        }

        public List<CPU> GetCpuList()
        {
            static Process StartProcess(string cmd, string args)
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

            List<CPU> cpuList = new List<CPU>();

            CPU cpu = new CPU();

            try
            {
                using var p = StartProcess("sysctl", "-n machdep.cpu.brand_string");
                using var sr = p.StandardOutput;
                p.WaitForExit();

                var info = sr.ReadToEnd().Trim().Split('@');

                info[1] = info[1].Trim();

                if (info[1].EndsWith("GHz"))
                {
                    info[1] = ((uint)(double.Parse(info[1].Replace("GHz", "").Replace(" ", "")) * 1000))
                        .ToString();
                }
                else if (info[1].EndsWith("KHz"))
                {
                    info[1] = ((uint)(double.Parse(info[1].Replace("KHz", "")) / 1000)).ToString();
                }
                else
                {
                    info[1] = info[1].Replace("MHz", "").Trim();
                }

                cpu.Name = info[0];
                cpu.CurrentClockSpeed = uint.Parse(info[1]);
            }
            catch
            {
                // Intentionally left blank
            }

            try
            {
                using var p = StartProcess("sysctl", "-n hw.physicalcpu");
                using var sr = p.StandardOutput;
                p.WaitForExit();

                var info = sr.ReadToEnd().Trim();

                cpu.NumberOfCores = uint.Parse(info);
            }
            catch
            {
                // Intentionally left blank
            }

            try
            {
                using var p = StartProcess("sysctl", "-n hw.logicalcpu");
                using var sr = p.StandardOutput;
                p.WaitForExit();

                var info = sr.ReadToEnd().Trim();

                cpu.NumberOfLogicalProcessors = uint.Parse(info);
            }
            catch
            {
                // Intentionally left blank
            }

            cpuList.Add(cpu);

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            return base.GetDriveList();
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

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

            return videoControllerList;
        }
    }
}
