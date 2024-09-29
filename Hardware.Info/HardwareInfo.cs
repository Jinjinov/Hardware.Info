using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    /// <summary>
    /// Main Hardware.Info class
    /// </summary>
    public class HardwareInfo : IHardwareInfo
    {
        /// <summary>
        /// Operating system
        /// </summary>
        public OS OperatingSystem { get; private set; } = new OS();

        /// <summary>
        /// Memory status
        /// </summary>
        public MemoryStatus MemoryStatus { get; private set; } = new MemoryStatus();

        /// <summary>
        /// List of <see cref="Battery" />
        /// </summary>
        public List<Battery> BatteryList { get; private set; } = new List<Battery>();

        /// <summary>
        /// List of <see cref="BIOS" />
        /// </summary>
        public List<BIOS> BiosList { get; private set; } = new List<BIOS>();

        /// <summary>
        /// List of <see cref="ComputerSystem" />
        /// </summary>
        public List<ComputerSystem> ComputerSystemList { get; private set; } = new List<ComputerSystem>();

        /// <summary>
        /// List of <see cref="CPU" />
        /// </summary>
        public List<CPU> CpuList { get; private set; } = new List<CPU>();

        /// <summary>
        /// List of <see cref="Drive" />
        /// </summary>
        public List<Drive> DriveList { get; private set; } = new List<Drive>();

        /// <summary>
        /// List of <see cref="Keyboard" />
        /// </summary>
        public List<Keyboard> KeyboardList { get; private set; } = new List<Keyboard>();

        /// <summary>
        /// List of <see cref="Memory" />
        /// </summary>
        public List<Memory> MemoryList { get; private set; } = new List<Memory>();

        /// <summary>
        /// List of <see cref="Monitor" />
        /// </summary>
        public List<Monitor> MonitorList { get; private set; } = new List<Monitor>();

        /// <summary>
        /// List of <see cref="Motherboard" />
        /// </summary>
        public List<Motherboard> MotherboardList { get; private set; } = new List<Motherboard>();

        /// <summary>
        /// List of <see cref="Mouse" />
        /// </summary>
        public List<Mouse> MouseList { get; private set; } = new List<Mouse>();

        /// <summary>
        /// List of <see cref="NetworkAdapter" />
        /// </summary>
        public List<NetworkAdapter> NetworkAdapterList { get; private set; } = new List<NetworkAdapter>();

        /// <summary>
        /// List of <see cref="Printer" />
        /// </summary>
        public List<Printer> PrinterList { get; private set; } = new List<Printer>();

        /// <summary>
        /// List of <see cref="SoundDevice" />
        /// </summary>
        public List<SoundDevice> SoundDeviceList { get; private set; } = new List<SoundDevice>();

        /// <summary>
        /// List of <see cref="VideoController" />
        /// </summary>
        public List<VideoController> VideoControllerList { get; private set; } = new List<VideoController>();

        private readonly IHardwareInfoRetrieval _hardwareInfoRetrieval = null!;

        /// <summary>
        /// Main Hardware.Info class
        /// </summary>
        /// <param name="useAsteriskInWMI">causes WMI queries to use SELECT * FROM instead of SELECT with a list of property names</param>
        /// <param name="timeoutInWMI">sets the Timeout property of the EnumerationOptions in the ManagementObjectSearcher that executes the query. The default value is EnumerationOptions.InfiniteTimeout</param>
        public HardwareInfo(bool useAsteriskInWMI = true, TimeSpan? timeoutInWMI = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _hardwareInfoRetrieval = new Hardware.Info.Windows.HardwareInfoRetrieval(timeoutInWMI) { UseAsteriskInWMI = useAsteriskInWMI };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                _hardwareInfoRetrieval = new Hardware.Info.Mac.HardwareInfoRetrieval();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Environment.OSVersion.Platform == PlatformID.Unix)
            {
                _hardwareInfoRetrieval = new Hardware.Info.Linux.HardwareInfoRetrieval();
            }
        }

        public HardwareInfo(IHardwareInfoRetrieval implementation)
        {
            _hardwareInfoRetrieval = implementation;
        }

        /// <summary>
        /// Refresh all hardware info
        /// </summary>
        public void RefreshAll()
        {
            RefreshOperatingSystem();
            RefreshMemoryStatus();

            RefreshBatteryList();
            RefreshBIOSList();
            RefreshComputerSystemList();
            RefreshCPUList();
            RefreshDriveList();
            RefreshKeyboardList();
            RefreshMemoryList();
            RefreshMonitorList();
            RefreshMotherboardList();
            RefreshMouseList();
            RefreshNetworkAdapterList();
            RefreshPrinterList();
            RefreshSoundDeviceList();
            RefreshVideoControllerList();
        }

        /// <summary>
        /// Refresh operating system info
        /// </summary>
        public void RefreshOperatingSystem() => OperatingSystem = _hardwareInfoRetrieval.GetOperatingSystem();

        /// <summary>
        /// Refresh memory status info
        /// </summary>
        public void RefreshMemoryStatus() => MemoryStatus = _hardwareInfoRetrieval.GetMemoryStatus();

        /// <summary>
        /// Refresh battery info
        /// </summary>
        public void RefreshBatteryList() => BatteryList = _hardwareInfoRetrieval.GetBatteryList();

        /// <summary>
        /// Refresh BIOS info
        /// </summary>
        public void RefreshBIOSList() => BiosList = _hardwareInfoRetrieval.GetBiosList();

        /// <summary>
        /// Refresh computer system info
        /// </summary>
        public void RefreshComputerSystemList() => ComputerSystemList = _hardwareInfoRetrieval.GetComputerSystemList();

        /// <summary>
        /// Refresh CPU info
        /// </summary>
        /// <param name="includePercentProcessorTime">Include PercentProcessorTime info. This makes the method a bit slower.</param>
        /// <param name="millisecondsDelayBetweenTwoMeasurements">Delay in milliseconds between two measurements in Linux</param>
        public void RefreshCPUList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500) => CpuList = _hardwareInfoRetrieval.GetCpuList(includePercentProcessorTime, millisecondsDelayBetweenTwoMeasurements);

        /// <summary>
        /// Refresh drive info
        /// </summary>
        public void RefreshDriveList() => DriveList = _hardwareInfoRetrieval.GetDriveList();

        /// <summary>
        /// Refresh keyboard info
        /// </summary>
        public void RefreshKeyboardList() => KeyboardList = _hardwareInfoRetrieval.GetKeyboardList();

        /// <summary>
        /// Refresh memory info
        /// </summary>
        public void RefreshMemoryList() => MemoryList = _hardwareInfoRetrieval.GetMemoryList();

        /// <summary>
        /// Refresh monitor info
        /// </summary>
        public void RefreshMonitorList() => MonitorList = _hardwareInfoRetrieval.GetMonitorList();

        /// <summary>
        /// Refresh motherboard info
        /// </summary>
        public void RefreshMotherboardList() => MotherboardList = _hardwareInfoRetrieval.GetMotherboardList();

        /// <summary>
        /// Refresh mouse info
        /// </summary>
        public void RefreshMouseList() => MouseList = _hardwareInfoRetrieval.GetMouseList();

        /// <summary>
        /// Refresh network adapter info
        /// </summary>
        /// <param name="includeBytesPerSec">Include BytesPerSec info. This makes the method a bit slower.</param>
        /// <param name="includeNetworkAdapterConfiguration">Include NetworkAdapterConfiguration info. This makes the method a bit slower.</param>
        /// <param name="millisecondsDelayBetweenTwoMeasurements">Delay in milliseconds between two measurements in Linux</param>
        public void RefreshNetworkAdapterList(bool includeBytesPerSec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000) => NetworkAdapterList = _hardwareInfoRetrieval.GetNetworkAdapterList(includeBytesPerSec, includeNetworkAdapterConfiguration, millisecondsDelayBetweenTwoMeasurements);

        /// <summary>
        /// Refresh printer info
        /// </summary>
        public void RefreshPrinterList() => PrinterList = _hardwareInfoRetrieval.GetPrinterList();

        /// <summary>
        /// Refresh sound device info
        /// </summary>
        public void RefreshSoundDeviceList() => SoundDeviceList = _hardwareInfoRetrieval.GetSoundDeviceList();

        /// <summary>
        /// Refresh video controller info
        /// </summary>
        public void RefreshVideoControllerList() => VideoControllerList = _hardwareInfoRetrieval.GetVideoControllerList();

        #region Static

        private static bool _pingInProgress;
        private static Action<bool>? _onPingComplete;

        /// <summary>
        /// Ping
        /// </summary>
        /// <param name="hostNameOrAddress">Host name or address to ping</param>
        /// <param name="onPingComplete">On ping complete callback with "bool success" parameter</param>
        public static void Ping(string hostNameOrAddress, Action<bool> onPingComplete)
        {
            if (_pingInProgress)
                return;

            _pingInProgress = true;

            _onPingComplete = onPingComplete;

            using Ping pingSender = new Ping();
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompleted);

            byte[] buffer = Enumerable.Repeat<byte>(97, 32).ToArray();

            int timeout = 12000;

            PingOptions options = new PingOptions(64, true);

            pingSender.SendAsync(hostNameOrAddress, timeout, buffer, options, null);
        }

        private static void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            _pingInProgress = false;

            bool success = true;

            if (e.Cancelled)
                success = false;

            if (e.Error != null)
                success = false;

            PingReply reply = e.Reply;

            if (reply == null)
                success = false;
            else if (reply.Status != IPStatus.Success)
                success = false;

            _onPingComplete?.Invoke(success);
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <returns>Local IPv4 addresses</returns>
        public static IEnumerable<IPAddress> GetLocalIPv4Addresses()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return NetworkInterface.GetAllNetworkInterfaces()
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
            }
            else
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <param name="networkInterfaceType">Filter by NetworkInterfaceType</param>
        /// <returns>Local IPv4 addresses</returns>
        public static IEnumerable<IPAddress> GetLocalIPv4Addresses(NetworkInterfaceType networkInterfaceType)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                   .Where(networkInterface => networkInterface.NetworkInterfaceType == networkInterfaceType)
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <param name="operationalStatus">Filter by OperationalStatus</param>
        /// <returns>Local IPv4 addresses</returns>
        public static IEnumerable<IPAddress> GetLocalIPv4Addresses(OperationalStatus operationalStatus)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                   .Where(networkInterface => networkInterface.OperationalStatus == operationalStatus)
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <param name="networkInterfaceType">Filter by NetworkInterfaceType</param>
        /// <param name="operationalStatus">Filter by OperationalStatus</param>
        /// <returns>Local IPv4 addresses</returns>
        public static IEnumerable<IPAddress> GetLocalIPv4Addresses(NetworkInterfaceType networkInterfaceType, OperationalStatus operationalStatus)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                   .Where(networkInterface => networkInterface.NetworkInterfaceType == networkInterfaceType && networkInterface.OperationalStatus == operationalStatus)
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
        }

        #endregion
    }
}
