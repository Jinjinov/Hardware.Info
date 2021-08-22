using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    public class HardwareInfo
    {
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

        private readonly IHardwareInfo hardwareInfo = null!;

        public HardwareInfo(bool useAsteriskInWMI = true, TimeSpan? timeoutInWMI = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                hardwareInfo = new Hardware.Info.Windows.HardwareInfo(timeoutInWMI) { UseAsteriskInWMI = useAsteriskInWMI };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                hardwareInfo = new Hardware.Info.Mac.HardwareInfo();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Environment.OSVersion.Platform == PlatformID.Unix)
            {
                hardwareInfo = new Hardware.Info.Linux.HardwareInfo();
            }
        }

        public void RefreshAll()
        {
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

        public void RefreshMemoryStatus() => MemoryStatus = hardwareInfo.GetMemoryStatus();

        public void RefreshBatteryList() => BatteryList = hardwareInfo.GetBatteryList();
        public void RefreshBIOSList() => BiosList = hardwareInfo.GetBiosList();
        public void RefreshCPUList(bool includePercentProcessorTime = true) => CpuList = hardwareInfo.GetCpuList(includePercentProcessorTime);
        public void RefreshDriveList() => DriveList = hardwareInfo.GetDriveList();
        public void RefreshKeyboardList() => KeyboardList = hardwareInfo.GetKeyboardList();
        public void RefreshMemoryList() => MemoryList = hardwareInfo.GetMemoryList();
        public void RefreshMonitorList() => MonitorList = hardwareInfo.GetMonitorList();
        public void RefreshMotherboardList() => MotherboardList = hardwareInfo.GetMotherboardList();
        public void RefreshMouseList() => MouseList = hardwareInfo.GetMouseList();
        public void RefreshNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true) => NetworkAdapterList = hardwareInfo.GetNetworkAdapterList(includeBytesPersec, includeNetworkAdapterConfiguration);
        public void RefreshPrinterList() => PrinterList = hardwareInfo.GetPrinterList();
        public void RefreshSoundDeviceList() => SoundDeviceList = hardwareInfo.GetSoundDeviceList();
        public void RefreshVideoControllerList() => VideoControllerList = hardwareInfo.GetVideoControllerList();

        #region Static

        private static bool pingInProgress;
        private static Action<bool>? OnPingComplete;

        public static void Ping(string hostNameOrAddress, Action<bool> onPingComplete)
        {
            if (pingInProgress)
                return;

            pingInProgress = true;

            OnPingComplete = onPingComplete;

            using Ping pingSender = new Ping();
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompleted);

            byte[] buffer = Enumerable.Repeat<byte>(97, 32).ToArray();

            int timeout = 12000;

            PingOptions options = new PingOptions(64, true);

            pingSender.SendAsync(hostNameOrAddress, timeout, buffer, options, null);
        }

        private static void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            pingInProgress = false;

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

            OnPingComplete?.Invoke(success);
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
