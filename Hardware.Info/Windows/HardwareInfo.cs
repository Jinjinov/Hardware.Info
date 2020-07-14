using System.Collections.Generic;
using System.Management;

namespace Hardware.Info.Windows
{
    internal class HardwareInfo : HardwareInfoBase, IHardwareInfo
    {
        public static T GetPropertyValue<T>(object obj) where T : struct
        {
            return (obj == null) ? default(T) : (T)obj;
        }

        public static string GetPropertyString(object obj)
        {
            return (obj is string str) ? str : string.Empty;
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.management.managementpath.defaultpath?view=netframework-4.8

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");

            foreach (ManagementObject mo in mos.Get())
            {
                Battery battery = new Battery
                {
                    FullChargeCapacity = GetPropertyValue<uint>(mo["FullChargeCapacity"]),
                    DesignCapacity = GetPropertyValue<uint>(mo["DesignCapacity"]),
                    BatteryStatus = GetPropertyValue<ushort>(mo["BatteryStatus"]),
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

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");

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

        public List<CPU> GetCpuList()
        {
            List<CPU> cpuList = new List<CPU>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

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
                    VirtualizationFirmwareEnabled = GetPropertyValue<bool>(mo["VirtualizationFirmwareEnabled"]),
                    VMMonitorModeExtensions = GetPropertyValue<bool>(mo["VMMonitorModeExtensions"])
                };

                cpuList.Add(cpu);
            }

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject mo in mos.Get())
            {
                Drive drive = new Drive
                {
                    Caption = GetPropertyString(mo["Caption"]),
                    Description = GetPropertyString(mo["Description"]),
                    FirmwareRevision = GetPropertyString(mo["FirmwareRevision"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Model = GetPropertyString(mo["Model"]),
                    Name = GetPropertyString(mo["Name"]),
                    Partitions = GetPropertyValue<uint>(mo["Partitions"]),
                    SerialNumber = GetPropertyString(mo["SerialNumber"]),
                    Size = GetPropertyValue<ulong>(mo["Size"])
                };

                driveList.Add(drive);
            }

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Keyboard");

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

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");

            foreach (ManagementObject mo in mos.Get())
            {
                Memory memory = new Memory
                {
                    Capacity = GetPropertyValue<ulong>(mo["Capacity"]),
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    PartNumber = GetPropertyString(mo["PartNumber"]),
                    SerialNumber = GetPropertyString(mo["SerialNumber"]),
                    Speed = GetPropertyValue<uint>(mo["Speed"])
                };

                memoryList.Add(memory);
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor WHERE PNPDeviceID IS NOT NULL");

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

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject mo in mos.Get())
            {
                Motherboard motherboard = new Motherboard
                {
                    Manufacturer = GetPropertyString(mo["Manufacturer"]),
                    Product = GetPropertyString(mo["Product"])
                };

                //MotherboardSerialNumber = mo["SerialNumber"]?.ToString()?.Trim() ?? string.Empty;

                motherboardList.Add(motherboard);
            }

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice");

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

        public override List<NetworkAdapter> GetNetworkAdapterList()
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True AND MACAddress IS NOT NULL");

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

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

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

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice WHERE NOT Manufacturer='Microsoft'");

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

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            foreach (ManagementObject mo in mos.Get())
            {
                VideoController videoController = new VideoController
                {
                    AdapterCompatibility = GetPropertyString(mo["AdapterCompatibility"]),
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
