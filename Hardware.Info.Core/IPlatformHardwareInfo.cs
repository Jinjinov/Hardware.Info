using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Hardware.Info, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f9caba3b554c3c962fd85b3722680234d5f5ca2abec47a345a22e9f11e5abbb6c480c7b3506c1d493732056c439f9bee9f79e8f4d67ce6c1104ea88cc4c0273e6c85612e5a0f8d9aca97454da75c92817874a1d18d89fa97de1fab64c8e000f44350a0142486bf0b16e767fc4f2daefe1a6f062467f79e59d969899540a93bdf")]
[assembly: InternalsVisibleTo("Hardware.Info.Aot, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f9caba3b554c3c962fd85b3722680234d5f5ca2abec47a345a22e9f11e5abbb6c480c7b3506c1d493732056c439f9bee9f79e8f4d67ce6c1104ea88cc4c0273e6c85612e5a0f8d9aca97454da75c92817874a1d18d89fa97de1fab64c8e000f44350a0142486bf0b16e767fc4f2daefe1a6f062467f79e59d969899540a93bdf")]

namespace Hardware.Info
{
    public interface IPlatformHardwareInfo : IDisposable
    {
        OS GetOperatingSystem();
        MemoryStatus GetMemoryStatus();

        List<Battery> GetBatteryList();
        List<BIOS> GetBiosList();
        List<ComputerSystem> GetComputerSystemList();
        List<CPU> GetCpuList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500, bool includePerformanceCounter = true);
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
