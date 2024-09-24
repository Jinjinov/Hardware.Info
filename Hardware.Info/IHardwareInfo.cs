using System.Collections.Generic;

namespace Hardware.Info
{
    /// <summary>
    /// Main Hardware.Info interface
    /// </summary>
    public interface IHardwareInfo
    {
        /// <summary>
        /// Operating system
        /// </summary>
        OS OperatingSystem { get; }

        /// <summary>
        /// Memory status
        /// </summary>
        MemoryStatus MemoryStatus { get; }

        /// <summary>
        /// List of <see cref="Battery" />
        /// </summary>
        List<Battery> BatteryList { get; }

        /// <summary>
        /// List of <see cref="BIOS" />
        /// </summary>
        List<BIOS> BiosList { get; }

        /// <summary>
        /// List of <see cref="ComputerSystem" />
        /// </summary>
        List<ComputerSystem> ComputerSystemList { get; }

        /// <summary>
        /// List of <see cref="CPU" />
        /// </summary>
        List<CPU> CpuList { get; }

        /// <summary>
        /// List of <see cref="Drive" />
        /// </summary>
        List<Drive> DriveList { get; }

        /// <summary>
        /// List of <see cref="Keyboard" />
        /// </summary>
        List<Keyboard> KeyboardList { get; }

        /// <summary>
        /// List of <see cref="Memory" />
        /// </summary>
        List<Memory> MemoryList { get; }

        /// <summary>
        /// List of <see cref="Monitor" />
        /// </summary>
        List<Monitor> MonitorList { get; }

        /// <summary>
        /// List of <see cref="Motherboard" />
        /// </summary>
        List<Motherboard> MotherboardList { get; }

        /// <summary>
        /// List of <see cref="Mouse" />
        /// </summary>
        List<Mouse> MouseList { get; }

        /// <summary>
        /// List of <see cref="NetworkAdapter" />
        /// </summary>
        List<NetworkAdapter> NetworkAdapterList { get; }

        /// <summary>
        /// List of <see cref="Printer" />
        /// </summary>
        List<Printer> PrinterList { get; }

        /// <summary>
        /// List of <see cref="SoundDevice" />
        /// </summary>
        List<SoundDevice> SoundDeviceList { get; }

        /// <summary>
        /// List of <see cref="VideoController" />
        /// </summary>
        List<VideoController> VideoControllerList { get; }

        /// <summary>
        /// Refresh all hardware info
        /// </summary>
        void RefreshAll();

        /// <summary>
        /// Refresh operating system info
        /// </summary>
        void RefreshOperatingSystem();

        /// <summary>
        /// Refresh memory status info
        /// </summary>
        void RefreshMemoryStatus();

        /// <summary>
        /// Refresh battery info
        /// </summary>
        void RefreshBatteryList();

        /// <summary>
        /// Refresh BIOS info
        /// </summary>
        void RefreshBIOSList();

        /// <summary>
        /// Refresh computer system info
        /// </summary>
        void RefreshComputerSystemList();

        /// <summary>
        /// Refresh CPU info
        /// </summary>
        /// <param name="includePercentProcessorTime">Include PercentProcessorTime info. This makes the method a bit slower.</param>
        /// <param name="millisecondsDelayBetweenTwoMeasurements">Delay in milliseconds between two measurements in Linux</param>
        void RefreshCPUList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500);

        /// <summary>
        /// Refresh drive info
        /// </summary>
        void RefreshDriveList();

        /// <summary>
        /// Refresh keyboard info
        /// </summary>
        void RefreshKeyboardList();

        /// <summary>
        /// Refresh memory info
        /// </summary>
        void RefreshMemoryList();

        /// <summary>
        /// Refresh monitor info
        /// </summary>
        void RefreshMonitorList();

        /// <summary>
        /// Refresh motherboard info
        /// </summary>
        void RefreshMotherboardList();

        /// <summary>
        /// Refresh mouse info
        /// </summary>
        void RefreshMouseList();

        /// <summary>
        /// Refresh network adapter info
        /// </summary>
        /// <param name="includeBytesPerSec">Include BytesPerSec info. This makes the method a bit slower.</param>
        /// <param name="includeNetworkAdapterConfiguration">Include NetworkAdapterConfiguration info. This makes the method a bit slower.</param>
        /// <param name="millisecondsDelayBetweenTwoMeasurements">Delay in milliseconds between two measurements in Linux</param>
        void RefreshNetworkAdapterList(bool includeBytesPerSec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000);

        /// <summary>
        /// Refresh printer info
        /// </summary>
        void RefreshPrinterList();

        /// <summary>
        /// Refresh sound device info
        /// </summary>
        void RefreshSoundDeviceList();

        /// <summary>
        /// Refresh video controller info
        /// </summary>
        void RefreshVideoControllerList();
    }
}