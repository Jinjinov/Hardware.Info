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
                };

                //CPUSpeed = mo["MaxClockSpeed"]?.ToString()?.Trim() ?? string.Empty;
                //CPUSerialNumber = mo["ProcessorID"]?.ToString()?.Trim() ?? string.Empty;

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
                };

                //HarddriveSerialNumber = mo["SerialNumber"]?.ToString()?.Trim() ?? string.Empty;

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
                };

                //RAMSize = mo["Capacity"]?.ToString()?.Trim() ?? string.Empty;
                //RAMSerialNumber = mo["SerialNumber"]?.ToString()?.Trim() ?? string.Empty;

                memoryList.Add(memory);
            }

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            using ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");

            foreach (ManagementObject mo in mos.Get())
            {
                Monitor monitor = new Monitor
                {
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
                };

                videoControllerList.Add(videoController);
            }

            return videoControllerList;
        }
    }
}
