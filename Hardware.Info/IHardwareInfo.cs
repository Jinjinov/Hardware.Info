using System.Collections.Generic;

namespace Hardware.Info
{
    internal interface IHardwareInfo
    {
        List<Battery> GetBatteryList();
        List<BIOS> GetBiosList();
        List<CPU> GetCpuList();
        List<Drive> GetDriveList();
        List<Keyboard> GetKeyboardList();
        List<Memory> GetMemoryList();
        List<Monitor> GetMonitorList();
        List<Motherboard> GetMotherboardList();
        List<Mouse> GetMouseList();
        List<NetworkAdapter> GetNetworkAdapterList();
        List<Printer> GetPrinterList();
        List<SoundDevice> GetSoundDeviceList();
        List<VideoController> GetVideoControllerList();
    }
}
