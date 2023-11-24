using System.Collections.Generic;

namespace Hardware.Info.Linux
{
    public interface IHardwareInfo
    {
        OS OperatingSystem { get; }
        MemoryStatus MemoryStatus { get; }

        List<Battery> BatteryList { get; }
        List<BIOS> BiosList { get; }
        List<CPU> CpuList { get; }
        List<Drive> DriveList { get; }
        List<Keyboard> KeyboardList { get; }
        List<Memory> MemoryList { get; }
        List<Monitor> MonitorList { get; }
        List<Motherboard> MotherboardList { get; }
        List<Mouse> MouseList { get; }
        List<NetworkAdapter> NetworkAdapterList { get; }
        List<Printer> PrinterList { get; }
        List<SoundDevice> SoundDeviceList { get; }
        List<VideoController> VideoControllerList { get; }

        void RefreshAll();

        void RefreshOperatingSystem();
        void RefreshMemoryStatus();

        void RefreshBatteryList();
        void RefreshBIOSList();
        void RefreshCPUList(bool includePercentProcessorTime = true);
        void RefreshDriveList();
        void RefreshKeyboardList();
        void RefreshMemoryList();
        void RefreshMonitorList();
        void RefreshMotherboardList();
        void RefreshMouseList();
        void RefreshNetworkAdapterList(bool includeBytesPerSec = true, bool includeNetworkAdapterConfiguration = true);
        void RefreshPrinterList();
        void RefreshSoundDeviceList();
        void RefreshVideoControllerList();
    }
}