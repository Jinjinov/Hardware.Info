using System.Collections.Generic;

namespace Hardware.Info
{
    internal interface IHardwareInfoRetrieval
    {
        OS GetOperatingSystem();
        MemoryStatus GetMemoryStatus();

        List<Battery> GetBatteryList();
        List<BIOS> GetBiosList();
        List<ComputerSystem> GetComputerSystemList();
        List<CPU> GetCpuList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500);
        List<Drive> GetDriveList();
        List<Keyboard> GetKeyboardList();
        List<Memory> GetMemoryList();
        List<Monitor> GetMonitorList();
        List<Motherboard> GetMotherboardList();
        List<Mouse> GetMouseList();
        List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000);
        List<Printer> GetPrinterList();
        List<SoundDevice> GetSoundDeviceList();
        List<VideoController> GetVideoControllerList();
    }
}
