using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Hardware.Info.Test
{
    class Program
    {
        static readonly HardwareInfo hardwareInfo = new HardwareInfo();

        static void Main(string[] _)
        {
            foreach (var address in HardwareInfo.GetLocalIPv4Address())
                Console.WriteLine(address);

            Console.WriteLine();

            hardwareInfo.RefreshAll();

            foreach (var hardware in hardwareInfo.BatteryList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.BiosList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.CpuList)
                Console.WriteLine(hardware);

            foreach (var drive in hardwareInfo.DriveList)
            {
                Console.WriteLine(drive);

                foreach (var partition in drive.PartitionList)
                {
                    Console.WriteLine(partition);

                    foreach (var volume in partition.VolumeList)
                        Console.WriteLine(volume);
                }
            }

            foreach (var hardware in hardwareInfo.KeyboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MemoryList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MonitorList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MotherboardList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.MouseList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.NetworkAdapterList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.PrinterList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.SoundDeviceList)
                Console.WriteLine(hardware);

            foreach (var hardware in hardwareInfo.VideoControllerList)
                Console.WriteLine(hardware);

            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);

            string[] Names = { "Win32_Fan", "Win32_HeatPipe", "Win32_Refrigeration", "Win32_TemperatureProbe", "Win32_Keyboard", "Win32_PointingDevice",
                             "Win32_AutochkSetting", "Win32_CDROMDrive" , "Win32_DiskDrive" ,"Win32_FloppyDrive", "Win32_PhysicalMedia","Win32_TapeDrive" ,
                             "Win32_1394Controller","Win32_1394ControllerDevice", "Win32_AllocatedResource", "Win32_AssociatedProcessorMemory","Win32_BaseBoard",
                             "Win32_BIOS", "Win32_Bus","Win32_CacheMemory","Win32_ControllerHasHub","Win32_DeviceBus","Win32_DeviceMemoryAddress", "Win32_DeviceSettings",
                             "Win32_DMAChannel","Win32_FloppyController","Win32_IDEController","Win32_IDEControllerDevice","Win32_InfraredDevice","Win32_IRQResource","Win32_MemoryArray",
                             "Win32_MemoryArrayLocation","Win32_MemoryDevice","Win32_MemoryDeviceArray","Win32_MemoryDeviceLocation","Win32_MotherboardDevice","Win32_OnBoardDevice",
                             "Win32_ParallelPort","Win32_PCMCIAController","Win32_PhysicalMemory","Win32_PhysicalMemoryArray","Win32_PhysicalMemoryLocation","Win32_PNPAllocatedResource",
                             "Win32_PNPDevice","Win32_PNPEntity","Win32_PortConnector","Win32_PortResource","Win32_Processor","Win32_SCSIController", "Win32_SCSIControllerDevice","Win32_SerialPort",
                             "Win32_SerialPortConfiguration","Win32_SerialPortSetting","Win32_SMBIOSMemory","Win32_SoundDevice","Win32_SystemBIOS","Win32_SystemDriverPNPEntity","Win32_SystemEnclosure",
                             "Win32_SystemMemoryResource","Win32_SystemSlot","Win32_USBController","Win32_USBControllerDevice","Win32_USBHub","Win32_NetworkAdapter",
                             "Win32_NetworkAdapterConfiguration","Win32_NetworkAdapterSetting","Win32_Battery","Win32_CurrentProbe","Win32_PortableBattery",
                             "Win32_PowerManagementEvent","Win32_VoltageProbe","Win32_DriverForDevice","Win32_Printer","Win32_PrinterConfiguration","Win32_PrinterController",
                             "Win32_PrinterDriver","Win32_PrinterDriverDll","Win32_PrinterSetting","Win32_PrintJob","Win32_TCPIPPrinterPort","Win32_POTSModem","Win32_POTSModemToSerialPort",
                             "Win32_DesktopMonitor","Win32_DisplayConfiguration","Win32_DisplayControllerConfiguration","Win32_VideoConfiguration","Win32_VideoController","Win32_VideoSettings"};

            Dictionary<string, int> tooLong = new Dictionary<string, int>();

            foreach (string name in Names)
            {
                sw.WriteLine();
                sw.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                sw.WriteLine(name);
                sw.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + name);
                ManagementObjectCollection information = searcher.Get();

                int count = 0;

                if (information.Count < 20)
                {
                    foreach (ManagementObject obj in information)
                    {
                        sw.WriteLine();
                        sw.WriteLine("count: " + ++count);
                        sw.WriteLine();

                        int a = obj.Properties.Count;
                        int b = obj.Properties.Cast<PropertyData>().Count();

                        foreach (PropertyData data in obj.Properties)
                        {
                            sw.WriteLine("{0} = {1}", data.Name, data.Value);
                        }
                    }
                }

                {
                    tooLong.Add(name, information.Count);
                }

            }

            sw.Close();
        }
    }
}
