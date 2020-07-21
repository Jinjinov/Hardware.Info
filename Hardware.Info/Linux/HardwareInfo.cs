using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            List<CPU> cpuList = new List<CPU>();

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
