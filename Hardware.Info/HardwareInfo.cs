using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Hardware.Info.Linux
{
    public class HardwareInfo : IHardwareInfo
    {
        public OS OperatingSystem { get; private set; } = new OS();
        public MemoryStatus MemoryStatus { get; private set; } = new MemoryStatus();

        public List<Battery> BatteryList { get; private set; } = new List<Battery>();
        public List<BIOS> BiosList { get; private set; } = new List<BIOS>();
        public List<CPU> CpuList { get; private set; } = new List<CPU>();
        public List<Drive> DriveList { get; private set; } = new List<Drive>();
        public List<Keyboard> KeyboardList { get; private set; } = new List<Keyboard>();
        public List<Memory> MemoryList { get; private set; } = new List<Memory>();
        public List<Monitor> MonitorList { get; private set; } = new List<Monitor>();
        public List<Motherboard> MotherboardList { get; private set; } = new List<Motherboard>();
        public List<Mouse> MouseList { get; private set; } = new List<Mouse>();
        public List<NetworkAdapter> NetworkAdapterList { get; private set; } = new List<NetworkAdapter>();
        public List<Printer> PrinterList { get; private set; } = new List<Printer>();
        public List<SoundDevice> SoundDeviceList { get; private set; } = new List<SoundDevice>();
        public List<VideoController> VideoControllerList { get; private set; } = new List<VideoController>();

        private readonly IHardwareInfoRetrieval _hardwareInfoRetrieval = null!;

        public HardwareInfo()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException("Only Linux operating system is supported");
            }

            _hardwareInfoRetrieval = new Hardware.Info.Linux.HardwareInfoRetrieval();

        }

        public void RefreshAll()
        {
            RefreshOperatingSystem();
            RefreshMemoryStatus();

            RefreshBatteryList();
            RefreshBIOSList();
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

        public void RefreshOperatingSystem() => OperatingSystem = _hardwareInfoRetrieval.GetOperatingSystem();
        public void RefreshMemoryStatus() => MemoryStatus = _hardwareInfoRetrieval.GetMemoryStatus();

        public void RefreshBatteryList() => BatteryList = _hardwareInfoRetrieval.GetBatteryList();
        public void RefreshBIOSList() => BiosList = _hardwareInfoRetrieval.GetBiosList();
        public void RefreshCPUList(bool includePercentProcessorTime = true) => CpuList = _hardwareInfoRetrieval.GetCpuList(includePercentProcessorTime);
        public void RefreshDriveList() => DriveList = _hardwareInfoRetrieval.GetDriveList();
        public void RefreshKeyboardList() => KeyboardList = _hardwareInfoRetrieval.GetKeyboardList();
        public void RefreshMemoryList() => MemoryList = _hardwareInfoRetrieval.GetMemoryList();
        public void RefreshMonitorList() => MonitorList = _hardwareInfoRetrieval.GetMonitorList();
        public void RefreshMotherboardList() => MotherboardList = _hardwareInfoRetrieval.GetMotherboardList();
        public void RefreshMouseList() => MouseList = _hardwareInfoRetrieval.GetMouseList();
        public void RefreshNetworkAdapterList(bool includeBytesPerSec = true, bool includeNetworkAdapterConfiguration = true) => NetworkAdapterList = _hardwareInfoRetrieval.GetNetworkAdapterList(includeBytesPerSec, includeNetworkAdapterConfiguration);
        public void RefreshPrinterList() => PrinterList = _hardwareInfoRetrieval.GetPrinterList();
        public void RefreshSoundDeviceList() => SoundDeviceList = _hardwareInfoRetrieval.GetSoundDeviceList();
        public void RefreshVideoControllerList() => VideoControllerList = _hardwareInfoRetrieval.GetVideoControllerList();

        #region Static

        private static bool _pingInProgress;
        private static Action<bool>? _onPingComplete;

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

            PingReply? reply = e.Reply;

            if (reply == null)
                success = false;
            else if (reply.Status != IPStatus.Success)
                success = false;

            _onPingComplete?.Invoke(success);
        }

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

        public static IEnumerable<IPAddress> GetLocalIPv4Addresses(NetworkInterfaceType networkInterfaceType)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                   .Where(networkInterface => networkInterface.NetworkInterfaceType == networkInterfaceType)
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
        }

        public static IEnumerable<IPAddress> GetLocalIPv4Addresses(OperationalStatus operationalStatus)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                   .Where(networkInterface => networkInterface.OperationalStatus == operationalStatus)
                                   .SelectMany(networkInterface => networkInterface.GetIPProperties().UnicastAddresses)
                                   .Where(addressInformation => addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                   .Select(addressInformation => addressInformation.Address);
        }

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
