using PListNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

// https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man3/sysctlbyname.3.html
// https://wiki.freepascal.org/Accessing_macOS_System_Information
// https://stackoverflow.com/questions/6592578/how-to-to-print-motherboard-and-display-card-info-on-mac
// https://stackoverflow.com/questions/53117107/cocoa-nstask-ouput-extraction
// https://docs.python.org/3/library/plistlib.html

namespace Hardware.Info.Mac
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        [DllImport("libc")]
        static extern int sysctlbyname(string name, out IntPtr oldp, ref IntPtr oldlenp, IntPtr newp, IntPtr newlen);

        private readonly MemoryStatus memoryStatus = new MemoryStatus();

        private readonly PNode? system_profiler;

        public HardwareInfo()
        {
            try
            {
                using Process process = StartProcess("system_profiler", "-xml");
                using StreamReader streamReader = process.StandardOutput;
                process.WaitForExit();

                system_profiler = PList.Load(streamReader.BaseStream);
            }
            catch
            {
            }
        }

        public MemoryStatus GetMemoryStatus()
        {
            IntPtr SizeOfLineSize = (IntPtr)IntPtr.Size;

            if (sysctlbyname("hw.memsize", out IntPtr lineSize, ref SizeOfLineSize, IntPtr.Zero, IntPtr.Zero) == 0)
            {
                memoryStatus.TotalPhysical = (ulong)lineSize.ToInt64();
            }

            return memoryStatus;
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            Battery battery = new Battery();

            // https://stackoverflow.com/questions/29278961/check-mac-battery-percentage-in-swift

            // https://developer.apple.com/documentation/iokit/iopowersources_h

            batteryList.Add(battery);

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            BIOS bios = new BIOS();

            biosList.Add(bios);

            return biosList;
        }

        public List<CPU> GetCpuList()
        {
            List<CPU> cpuList = new List<CPU>();

            CPU cpu = new CPU();

            string processOutput = ReadProcessOutput("sysctl", "-n machdep.cpu.brand_string");
            string[] info = processOutput.Split('@');

            if (info.Length > 1)
            {
                string speedString = info[1].Trim();
                uint speed = 0;

                if (speedString.EndsWith("GHz"))
                {
                    string number = speedString.Replace("GHz", string.Empty).Trim();
                    if (uint.TryParse(number, out speed))
                        speed *= 1000;
                }
                else if (speedString.EndsWith("KHz"))
                {
                    string number = speedString.Replace("KHz", string.Empty).Trim();
                    if (uint.TryParse(number, out speed))
                        speed /= 1000;
                }
                else if (speedString.EndsWith("MHz"))
                {
                    string number = speedString.Replace("MHz", string.Empty).Trim();
                    uint.TryParse(number, out speed);
                }

                cpu.Name = info[0];
                cpu.CurrentClockSpeed = speed;
            }

            processOutput = ReadProcessOutput("sysctl", "-n hw.physicalcpu");

            if (uint.TryParse(processOutput, out uint numberOfCores))
                cpu.NumberOfCores = numberOfCores;

            processOutput = ReadProcessOutput("sysctl", "-n hw.logicalcpu");

            if (uint.TryParse(processOutput, out uint numberOfLogicalProcessors))
                cpu.NumberOfLogicalProcessors = numberOfLogicalProcessors;

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

            Keyboard keyboard = new Keyboard();

            keyboardList.Add(keyboard);

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            Memory memory = new Memory();

            memoryList.Add(memory);

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            Monitor monitor = new Monitor();

            // https://developer.apple.com/documentation/appkit/nsscreen

            // https://developer.apple.com/documentation/iokit/iographicslib_h

            // IODisplayConnect

            // IODisplayEDID

            //auto mainDisplayId = CGMainDisplayID();
            //width = CGDisplayPixelsWide(mainDisplayId);
            //height = CGDisplayPixelsHigh(mainDisplayId);

            monitorList.Add(monitor);

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            Motherboard motherboard = new Motherboard();

            motherboardList.Add(motherboard);

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            Mouse mouse = new Mouse();

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

            // https://stackoverflow.com/questions/57617998/how-to-retrieve-installed-printers-on-macos-in-c-sharp

            // https://developer.apple.com/documentation/appkit/nsprinter

            // https://developer.apple.com/documentation/iokit/1424817-printer_class_requests

            printerList.Add(printer);

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            SoundDevice soundDevice = new SoundDevice();

            soundDeviceList.Add(soundDevice);

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            VideoController videoController = new VideoController();

            // https://stackoverflow.com/questions/18077639/getting-graphic-card-information-in-objective-c

            // https://developer.apple.com/documentation/iokit/iographicslib_h

            videoControllerList.Add(videoController);

            return videoControllerList;
        }
    }
}
