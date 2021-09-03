using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;

namespace Hardware.Info.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    internal class HardwareInfoRetrieval : HardwareInfoBase, IHardwareInfoRetrieval
    {
        private readonly MEMORYSTATUSEX _memoryStatusEx = new MEMORYSTATUSEX();

        private readonly MemoryStatus _memoryStatus = new MemoryStatus();

        public bool UseAsteriskInWMI { get; set; }

        private readonly string _managementScope = "root\\cimv2";
        private readonly EnumerationOptions _enumerationOptions = new EnumerationOptions() { ReturnImmediately = true, Rewindable = false, Timeout = EnumerationOptions.InfiniteTimeout };

        public HardwareInfoRetrieval(TimeSpan? enumerationOptionsTimeout = null)
        {
            if (enumerationOptionsTimeout == null)
                enumerationOptionsTimeout = EnumerationOptions.InfiniteTimeout;

            _enumerationOptions = new EnumerationOptions() { ReturnImmediately = true, Rewindable = false, Timeout = enumerationOptionsTimeout.Value };
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        public MemoryStatus GetMemoryStatus()
        {
            if (GlobalMemoryStatusEx(_memoryStatusEx))
            {
                _memoryStatus.TotalPhysical = _memoryStatusEx.ullTotalPhys;
                _memoryStatus.AvailablePhysical = _memoryStatusEx.ullAvailPhys;
                _memoryStatus.TotalPageFile = _memoryStatusEx.ullTotalPageFile;
                _memoryStatus.AvailablePageFile = _memoryStatusEx.ullAvailPageFile;
                _memoryStatus.TotalVirtual = _memoryStatusEx.ullTotalVirtual;
                _memoryStatus.AvailableVirtual = _memoryStatusEx.ullAvailVirtual;
                _memoryStatus.AvailableExtendedVirtual = _memoryStatusEx.ullAvailExtendedVirtual;
            }

            return _memoryStatus;
        }

        public static T GetPropertyValue<T>(object obj) where T : struct
        {
            return (obj == null) ? default(T) : (T)obj;
        }

        public static T[] GetPropertyArray<T>(object obj)
        {
            return (obj is T[] array) ? array : Array.Empty<T>();
        }

        public static string GetPropertyString(object obj)
        {
            return (obj is string str) ? str : string.Empty;
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.management.managementpath.defaultpath?view=netframework-4.8

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Battery" 
                                                  : "SELECT FullChargeCapacity, DesignCapacity, BatteryStatus, EstimatedChargeRemaining, EstimatedRunTime, ExpectedLife, MaxRechargeTime, TimeOnBattery, TimeToFullCharge FROM Win32_Battery";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Battery battery = new Battery
                {
                    FullChargeCapacity = GetPropertyValue<uint>(mo["FullChargeCapacity"]),
                    DesignCapacity = GetPropertyValue<uint>(mo["DesignCapacity"]),
                    BatteryStatus = GetPropertyValue<ushort>(mo["BatteryStatus"]),
                    EstimatedChargeRemaining = GetPropertyValue<ushort>(mo["EstimatedChargeRemaining"]),
                    EstimatedRunTime = GetPropertyValue<uint>(mo["EstimatedRunTime"]),
                    ExpectedLife = GetPropertyValue<uint>(mo["ExpectedLife"]),
                    MaxRechargeTime = GetPropertyValue<uint>(mo["MaxRechargeTime"]),
                    TimeOnBattery = GetPropertyValue<uint>(mo["TimeOnBattery"]),
                    TimeToFullCharge = GetPropertyValue<uint>(mo["TimeToFullCharge"])
                };

                batteryList.Add(battery);
            }

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_BIOS"
                                                  : "SELECT Caption, Description, Manufacturer, Name, ReleaseDate, SerialNumber, SoftwareElementID, Version FROM Win32_BIOS";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                BIOS bios = new BIOS
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Name = GetPropertyString(mo["Name"]),
                    ReleaseDate = GetPropertyString(mo["ReleaseDate"]),
                    SerialNumber = GetPropertyString(mo["SerialNumber"]),
                    SoftwareElementID = GetPropertyString(mo["SoftwareElementID"]),
                    Version = GetPropertyString(mo["Version"])
                };

                biosList.Add(bios);
            }

            return biosList;
        }

        public List<CPU> GetCpuList(bool includePercentProcessorTime = true)
        {
            List<CPU> cpuList = new List<CPU>();

            List<CpuCore> cpuCoreList = new List<CpuCore>();

            ulong percentProcessorTime = 0ul;

            if (includePercentProcessorTime)
            {
                string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name != '_Total'"
                                                      : "SELECT Name, PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name != '_Total'";
                using ManagementObjectSearcher percentProcessorTimeMOS = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

                foreach (ManagementObject mo in percentProcessorTimeMOS.Get())
                {
                    CpuCore core = new CpuCore
                    {
                        Name = GetPropertyString(mo["Name"]),
                        PercentProcessorTime = GetPropertyValue<ulong>(mo["PercentProcessorTime"])
                    };

                    cpuCoreList.Add(core);
                }

                string QueryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'"
                                                      : "SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'";
                using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_managementScope, QueryString, _enumerationOptions);

                foreach (ManagementObject mo in managementObjectSearcher.Get())
                {
                    percentProcessorTime = GetPropertyValue<ulong>(mo["PercentProcessorTime"]);
                }
            }

            string query = UseAsteriskInWMI ? "SELECT * FROM Win32_Processor"
                                            : "SELECT Caption, CurrentClockSpeed, Description, L2CacheSize, L3CacheSize, Manufacturer, MaxClockSpeed, Name, NumberOfCores, NumberOfLogicalProcessors, ProcessorId, VirtualizationFirmwareEnabled, VMMonitorModeExtensions, SecondLevelAddressTranslationExtensions, SocketDesignation FROM Win32_Processor";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, query, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                CPU cpu = new CPU
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    CurrentClockSpeed = GetPropertyValue<uint>(mo["CurrentClockSpeed"]),
                    Description = GetPropertyString(mo["Description"]),
                    L2CacheSize = GetPropertyValue<uint>(mo["L2CacheSize"]),
                    L3CacheSize = GetPropertyValue<uint>(mo["L3CacheSize"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    MaxClockSpeed = GetPropertyValue<uint>(mo["MaxClockSpeed"]),
                    Name = GetPropertyString(mo["Name"]),
                    NumberOfCores = GetPropertyValue<uint>(mo["NumberOfCores"]),
                    NumberOfLogicalProcessors = GetPropertyValue<uint>(mo["NumberOfLogicalProcessors"]),
                    ProcessorId = GetPropertyString(mo["ProcessorId"]),
                    SecondLevelAddressTranslationExtensions = GetPropertyValue<bool>(mo["SecondLevelAddressTranslationExtensions"]),
                    SocketDesignation = GetPropertyString(mo["SocketDesignation"]),
                    VirtualizationFirmwareEnabled = GetPropertyValue<bool>(mo["VirtualizationFirmwareEnabled"]),
                    VMMonitorModeExtensions = GetPropertyValue<bool>(mo["VMMonitorModeExtensions"]),
                    PercentProcessorTime = percentProcessorTime,
                    CpuCoreList = cpuCoreList
                };

                cpuList.Add(cpu);
            }

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_DiskDrive"
                                                  : "SELECT Caption, Description, DeviceID, FirmwareRevision, Index, Manufacturer, Model, Name, Partitions, SerialNumber, Size FROM Win32_DiskDrive";
            using ManagementObjectSearcher Win32_DiskDrive = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);
            foreach (ManagementObject DiskDrive in Win32_DiskDrive.Get())
            {
                Drive drive = new Drive
                {
                    Caption = GetPropertyString(DiskDrive["Caption"]),
                    Description = GetPropertyString(DiskDrive["Description"]),
                    FirmwareRevision = GetPropertyString(DiskDrive["FirmwareRevision"]),
                    Index = GetPropertyValue<uint>(DiskDrive["Index"]),
                    Manufacturer = GetPropertyString(DiskDrive["Manufacturer"]),
                    Model = GetPropertyString(DiskDrive["Model"]),
                    Name = GetPropertyString(DiskDrive["Name"]),
                    Partitions = GetPropertyValue<uint>(DiskDrive["Partitions"]),
                    SerialNumber = GetPropertyString(DiskDrive["SerialNumber"]),
                    Size = GetPropertyValue<ulong>(DiskDrive["Size"])
                };

                string queryString1 = "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + DiskDrive["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                using ManagementObjectSearcher Win32_DiskPartition = new ManagementObjectSearcher(_managementScope, queryString1, _enumerationOptions);
                foreach (ManagementObject DiskPartition in Win32_DiskPartition.Get())
                {
                    Partition partition = new Partition
                    {
                        Bootable = GetPropertyValue<bool>(DiskPartition["Bootable"]),
                        BootPartition = GetPropertyValue<bool>(DiskPartition["BootPartition"]),
                        Caption = GetPropertyString(DiskPartition["Caption"]),
                        Description = GetPropertyString(DiskPartition["Description"]),
                        DiskIndex = GetPropertyValue<uint>(DiskPartition["DiskIndex"]),
                        Index = GetPropertyValue<uint>(DiskPartition["Index"]),
                        Name = GetPropertyString(DiskPartition["Name"]),
                        PrimaryPartition = GetPropertyValue<bool>(DiskPartition["PrimaryPartition"]),
                        Size = GetPropertyValue<ulong>(DiskPartition["Size"]),
                        StartingOffset = GetPropertyValue<ulong>(DiskPartition["StartingOffset"])
                    };

                    string queryString2 = "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + DiskPartition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition";
                    using ManagementObjectSearcher Win32_LogicalDisk = new ManagementObjectSearcher(_managementScope, queryString2, _enumerationOptions);
                    foreach (ManagementObject LogicalDisk in Win32_LogicalDisk.Get())
                    {
                        Volume volume = new Volume
                        {
                            Caption = GetPropertyString(LogicalDisk["Caption"]),
                            Compressed = GetPropertyValue<bool>(LogicalDisk["Compressed"]),
                            Description = GetPropertyString(LogicalDisk["Description"]),
                            FileSystem = GetPropertyString(LogicalDisk["FileSystem"]),
                            FreeSpace = GetPropertyValue<ulong>(LogicalDisk["FreeSpace"]),
                            Name = GetPropertyString(LogicalDisk["Name"]),
                            Size = GetPropertyValue<ulong>(LogicalDisk["Size"]),
                            VolumeName = GetPropertyString(LogicalDisk["VolumeName"]),
                            VolumeSerialNumber = GetPropertyString(LogicalDisk["VolumeSerialNumber"])
                        };

                        partition.VolumeList.Add(volume);
                    }

                    drive.PartitionList.Add(partition);
                }

                driveList.Add(drive);
            }

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Keyboard"
                                                  : "SELECT Caption, Description, Name, NumberOfFunctionKeys FROM Win32_Keyboard";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Keyboard keyboard= new Keyboard
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    Name = GetPropertyString(mo["Name"]),
                    NumberOfFunctionKeys = GetPropertyValue<ushort>(mo["NumberOfFunctionKeys"])
                };

                keyboardList.Add(keyboard);
            }

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PhysicalMemory"
                                                  : "SELECT Capacity, FormFactor, Manufacturer, PartNumber, SerialNumber, Speed, BankLabel, MaxVoltage, MinVoltage FROM Win32_PhysicalMemory";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Memory memory = new Memory
                {
                    Capacity = GetPropertyValue<ulong>(mo["Capacity"]),
                    FormFactor = (FormFactor)GetPropertyValue<ushort>(mo["FormFactor"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    PartNumber = GetPropertyString(mo["PartNumber"]),
                    SerialNumber = GetPropertyString(mo["SerialNumber"]),
                    Speed = GetPropertyValue<uint>(mo["Speed"]),
                    BankLabel = GetPropertyString(mo["BankLabel"]),
                    MaxVoltage = GetPropertyValue<uint>(mo["MaxVoltage"]),
                    MinVoltage = GetPropertyValue<uint>(mo["MinVoltage"])
                };

                memoryList.Add(memory);
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_DesktopMonitor WHERE PNPDeviceID IS NOT NULL"
                                                  : "SELECT Caption, Description, MonitorManufacturer, MonitorType, Name, PixelsPerXLogicalInch, PixelsPerYLogicalInch FROM Win32_DesktopMonitor WHERE PNPDeviceID IS NOT NULL";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Monitor monitor = new Monitor
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    MonitorManufacturer = GetPropertyString(mo["MonitorManufacturer"]),
                    MonitorType = GetPropertyString(mo["MonitorType"]),
                    Name = GetPropertyString(mo["Name"]),
                    PixelsPerXLogicalInch = GetPropertyValue<uint>(mo["PixelsPerXLogicalInch"]),
                    PixelsPerYLogicalInch = GetPropertyValue<uint>(mo["PixelsPerYLogicalInch"])
                };

                monitorList.Add(monitor);
            }

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_BaseBoard"
                                                  : "SELECT Manufacturer, Product, SerialNumber FROM Win32_BaseBoard";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Motherboard motherboard = new Motherboard
                {
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Product = GetPropertyString(mo["Product"]),
                    SerialNumber = GetPropertyString(mo["SerialNumber"])
                };

                motherboardList.Add(motherboard);
            }

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_PointingDevice"
                                                  : "SELECT Caption, Description, Manufacturer, Name, NumberOfButtons FROM Win32_PointingDevice";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Mouse mouse = new Mouse
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Name = GetPropertyString(mo["Name"]),
                    NumberOfButtons = GetPropertyValue<byte>(mo["NumberOfButtons"])
                };

                mouseList.Add(mouse);
            }

            return mouseList;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True AND MACAddress IS NOT NULL"
                                                  : "SELECT AdapterType, Caption, Description, DeviceID, MACAddress, Manufacturer, Name, NetConnectionID, ProductName, Speed FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True AND MACAddress IS NOT NULL";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    AdapterType = GetPropertyString(mo["AdapterType"]),
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    MACAddress = GetPropertyString(mo["MACAddress"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Name = GetPropertyString(mo["Name"]),
                    NetConnectionID = GetPropertyString(mo["NetConnectionID"]),
                    ProductName = GetPropertyString(mo["ProductName"]),
                    Speed = GetPropertyValue<ulong>(mo["Speed"])
                };

                if (includeBytesPersec)
                {
                    string query = UseAsteriskInWMI ? $"SELECT * FROM Win32_PerfFormattedData_Tcpip_NetworkAdapter WHERE Name = '{networkAdapter.Name.Replace("(", "[").Replace(")", "]")}'"
                                                    : $"SELECT BytesSentPersec, BytesReceivedPersec FROM Win32_PerfFormattedData_Tcpip_NetworkAdapter WHERE Name = '{networkAdapter.Name.Replace("(", "[").Replace(")", "]")}'";
                    using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_managementScope, query, _enumerationOptions);
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        networkAdapter.BytesSentPersec = GetPropertyValue<ulong>(managementObject["BytesSentPersec"]);
                        networkAdapter.BytesReceivedPersec = GetPropertyValue<ulong>(managementObject["BytesReceivedPersec"]);
                    }
                }

                if (includeNetworkAdapterConfiguration)
                {
                    IPAddress address;
                    foreach (ManagementObject configuration in mo.GetRelated("Win32_NetworkAdapterConfiguration"))
                    {
                        foreach (string str in GetPropertyArray<string>(configuration["DefaultIPGateway"]))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.DefaultIPGatewayList.Add(address);

                        if (IPAddress.TryParse(GetPropertyString(configuration["DHCPServer"]), out address))
                            networkAdapter.DHCPServer = address;

                        foreach (string str in GetPropertyArray<string>(configuration["DNSServerSearchOrder"]))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.DNSServerSearchOrderList.Add(address);

                        foreach (string str in GetPropertyArray<string>(configuration["IPAddress"]))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.IPAddressList.Add(address);

                        foreach (string str in GetPropertyArray<string>(configuration["IPSubnet"]))
                            if (IPAddress.TryParse(str, out address))
                                networkAdapter.IPSubnetList.Add(address);
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_Printer"
                                                  : "SELECT Caption, Default, Description, HorizontalResolution, Local, Name, Network, Shared, VerticalResolution FROM Win32_Printer";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                Printer printer = new Printer
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Default = GetPropertyValue<bool>(mo["Default"]),
                    Description = GetPropertyString(mo["Description"]),
                    HorizontalResolution = GetPropertyValue<uint>(mo["HorizontalResolution"]),
                    Local = GetPropertyValue<bool>(mo["Local"]),
                    Name = GetPropertyString(mo["Name"]),
                    Network = GetPropertyValue<bool>(mo["Network"]),
                    Shared = GetPropertyValue<bool>(mo["Shared"]),
                    VerticalResolution = GetPropertyValue<uint>(mo["VerticalResolution"])
                };

                printerList.Add(printer);
            }


            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_SoundDevice WHERE NOT Manufacturer='Microsoft'"
                                                  : "SELECT Caption, Description, Manufacturer, Name, ProductName FROM Win32_SoundDevice WHERE NOT Manufacturer='Microsoft'";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                SoundDevice soundDevice = new SoundDevice
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Name = GetPropertyString(mo["Name"]),
                    ProductName = GetPropertyString(mo["ProductName"])
                };

                soundDeviceList.Add(soundDevice);
            }

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            string queryString = UseAsteriskInWMI ? "SELECT * FROM Win32_VideoController"
                                                  : "SELECT AdapterCompatibility, AdapterRAM, Caption, CurrentBitsPerPixel, CurrentHorizontalResolution, CurrentNumberOfColors, CurrentRefreshRate, CurrentVerticalResolution, Description, DriverDate, DriverVersion, MaxRefreshRate, MinRefreshRate, Name, VideoModeDescription, VideoProcessor FROM Win32_VideoController";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                VideoController videoController = new VideoController
                {
                    Manufacturer = GetPropertyString(mo["AdapterCompatibility"]),
                    AdapterRAM = GetPropertyValue<uint>(mo["AdapterRAM"]),
                    Caption = GetPropertyString(mo["Caption"]),
                    CurrentBitsPerPixel = GetPropertyValue<uint>(mo["CurrentBitsPerPixel"]),
                    CurrentHorizontalResolution = GetPropertyValue<uint>(mo["CurrentHorizontalResolution"]),
                    CurrentNumberOfColors = GetPropertyValue<ulong>(mo["CurrentNumberOfColors"]),
                    CurrentRefreshRate = GetPropertyValue<uint>(mo["CurrentRefreshRate"]),
                    CurrentVerticalResolution = GetPropertyValue<uint>(mo["CurrentVerticalResolution"]),
                    Description = GetPropertyString(mo["Description"]),
                    DriverDate = GetPropertyString(mo["DriverDate"]),
                    DriverVersion = GetPropertyString(mo["DriverVersion"]),
                    MaxRefreshRate = GetPropertyValue<uint>(mo["MaxRefreshRate"]),
                    MinRefreshRate = GetPropertyValue<uint>(mo["MinRefreshRate"]),
                    Name = GetPropertyString(mo["Name"]),
                    VideoModeDescription = GetPropertyString(mo["VideoModeDescription"]),
                    VideoProcessor = GetPropertyString(mo["VideoProcessor"])
                };

                videoControllerList.Add(videoController);
            }

            return videoControllerList;
        }
    }
}
