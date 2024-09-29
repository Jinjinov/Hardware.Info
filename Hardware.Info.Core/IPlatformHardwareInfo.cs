﻿using System.Collections.Generic;

namespace Hardware.Info.Core
{
    internal interface IPlatformHardwareInfo
    {
        OS GetOperatingSystem();
        MemoryStatus GetMemoryStatus();

        List<Battery> GetBatteryList();
        List<BIOS> GetBiosList();
        List<ComputerSystem> GetComputerSystemList();
        List<CPU> GetCpuList(bool includePercentProcessorTime = true);
        List<Drive> GetDriveList();
        List<Keyboard> GetKeyboardList();
        List<Memory> GetMemoryList();
        List<Monitor> GetMonitorList();
        List<Motherboard> GetMotherboardList();
        List<Mouse> GetMouseList();
        List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true);
        List<Printer> GetPrinterList();
        List<SoundDevice> GetSoundDeviceList();
        List<VideoController> GetVideoControllerList();
    }
}