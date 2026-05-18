using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man3/sysctlbyname.3.html
// https://wiki.freepascal.org/Accessing_macOS_System_Information
// https://stackoverflow.com/questions/6592578/how-to-to-print-motherboard-and-display-card-info-on-mac
// https://stackoverflow.com/questions/53117107/cocoa-nstask-ouput-extraction
// https://docs.python.org/3/library/plistlib.html
// https://ss64.com/osx/system_profiler.html

namespace Hardware.Info.Mac
{
    internal class PlatformHardwareInfo : PlatformHardwareInfoBase, IPlatformHardwareInfo
    {
        private readonly MemoryStatus _memoryStatus = new MemoryStatus();

        private readonly OS _os = new OS();

        public PlatformHardwareInfo(ILogger? logger = null)
        {
            _logger = logger ?? NullLogger.Instance;
        }

        public OS GetOperatingSystem()
        {
            _os.Name = ReadProcessOutput("sw_vers", "-productName");
            _os.VersionString = ReadProcessOutput("sw_vers", "-productVersion");

            if (Version.TryParse(_os.VersionString, out Version version))
                _os.Version = version;

            return _os;
        }

        [DllImport("libc")]
        static extern int sysctlbyname(string name, out IntPtr oldp, ref IntPtr oldlenp, IntPtr newp, IntPtr newlen);

        /*
        public PlatformHardwareInfo()
        {
            //SystemProfiler();

            //SystemProfilerPList();
        }
        /**/

        /*
        private void SystemProfilerPList()
        {
            try
            {
                using MemoryStream stream = new MemoryStream();
                using StreamWriter output = new StreamWriter(stream);
                StringBuilder error = new StringBuilder();

                if (StartProcess("system_profiler", "-xml", standardOutput => output.Write(standardOutput), standardError => error.AppendLine(standardError)))
                {
                    output.Flush();
                    stream.Position = 0;

                    PNode system_profiler = PList.Load(stream);
                }
            }
            catch (Exception ex)
            {
            }
        }
        /**/

        /*
        private void SystemProfiler()
        {
            string[] dataTypes = {
                "SPParallelATADataType",
                "SPUniversalAccessDataType",
                //"SPApplicationsDataType",
                "SPAudioDataType",
                "SPBluetoothDataType",
                "SPCameraDataType",
                "SPCardReaderDataType",
                //"SPComponentDataType",
                "SPDeveloperToolsDataType",
                "SPDiagnosticsDataType",
                "SPDisabledSoftwareDataType",
                "SPDiscBurningDataType",
                "SPEthernetDataType",
                //"SPExtensionsDataType",
                "SPFibreChannelDataType",
                "SPFireWireDataType",
                "SPFirewallDataType",
                //"SPFontsDataType",
                //"SPFrameworksDataType",
                "SPDisplaysDataType",
                "SPHardwareDataType",
                "SPHardwareRAIDDataType",
                //"SPInstallHistoryDataType",
                "SPNetworkLocationDataType",
                //"SPLogsDataType",
                "SPManagedClientDataType",
                "SPMemoryDataType",
                "SPNVMeDataType",
                "SPNetworkDataType",
                "SPPCIDataType",
                "SPParallelSCSIDataType",
                "SPPowerDataType",
                //"SPPrefPaneDataType",
                //"SPPrintersSoftwareDataType",
                "SPPrintersDataType",
                "SPConfigurationProfileDataType",
                //"SPRawCameraDataType",
                "SPSASDataType",
                "SPSerialATADataType",
                "SPSPIDataType",
                //"SPSmartCardsDataType",
                "SPSoftwareDataType",
                "SPStartupItemDataType",
                "SPStorageDataType",
                //"SPSyncServicesDataType",
                "SPThunderboltDataType",
                "SPUSBDataType",
                "SPNetworkVolumeDataType",
                "SPWWANDataType",
                "SPAirPortDataType",
                "SPiBridgeDataType"
            };

            foreach (string dataType in dataTypes)
            {
                Console.WriteLine(dataType);

                StartProcess("system_profiler", dataType, standardOutput => Console.WriteLine(standardOutput), standardError => Console.WriteLine(standardError));

                Console.WriteLine(dataType + " END");

                //Console.ReadLine();
            }
        }
        /**/

        private bool StartProcess(string fileName, string arguments, Action<string> output, Action<string> error, int millisecondsTimeout = 60 * 1000)
        {
            using Process process = new Process();

            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            using AutoResetEvent outputWaitHandle = new AutoResetEvent(false);
            using AutoResetEvent errorWaitHandle = new AutoResetEvent(false);

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    output.Invoke(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    error.Invoke(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process.WaitForExit(millisecondsTimeout) && outputWaitHandle.WaitOne(millisecondsTimeout) && errorWaitHandle.WaitOne(millisecondsTimeout);
        }

        public MemoryStatus GetMemoryStatus()
        {
            IntPtr SizeOfLineSize = (IntPtr)IntPtr.Size;

            if (sysctlbyname("hw.memsize", out IntPtr lineSize, ref SizeOfLineSize, IntPtr.Zero, IntPtr.Zero) == 0)
            {
                _memoryStatus.TotalPhysical = (ulong)lineSize.ToInt64();
            }

            return _memoryStatus;
        }

        public List<Battery> GetBatteryList()
        {
            List<Battery> batteryList = new List<Battery>();

            Battery battery = new Battery();

            // https://stackoverflow.com/questions/29278961/check-mac-battery-percentage-in-swift

            // https://developer.apple.com/documentation/iokit/iopowersources_h

            /*
            SPPowerDataType
Power:

    System Power Settings:

      AC Power:
          System Sleep Timer (Minutes): 10
          Disk Sleep Timer (Minutes): 10
          Display Sleep Timer (Minutes): 10
          Sleep on Power Button: Yes
          Current Power Source: Yes
          Hibernate Mode: 0
          Standby Delay: 4200
          Standby Enabled: 1

    Hardware Configuration:

      UPS Installed: No

      Model Information:
          Serial Number: W01396THJD3LA
          Manufacturer: SMP
          Device Name: bq20z451
          Pack Lot Code: 0
          PCB Lot Code: 0
          Firmware Version: 201
          Hardware Revision: 000a
          Cell Revision: 165
      Charge Information:
          Charge Remaining (mAh): 5013
          Fully Charged: Yes
          Charging: No
          Full Charge Capacity (mAh): 5086
      Health Information:
          Cycle Count: 72
          Condition: Normal
      Battery Installed: Yes
      Amperage (mA): -300
      Voltage (mV): 12303
            /**/

            // https://www.iphonetricks.org/check-battery-health-on-mac-using-terminal/
            /* 
            Juliens-MacBook-Pro:~ tadel$ ioreg -l -w0 | grep ' \"MaxCapacity\" ' | grep -Eo "\d+"
            5034
            Juliens-MacBook-Pro:~ tadel$ ioreg -l -w0 | grep ' \"CurrentCapacity\" ' | grep -Eo "\d+"
            4860
            Juliens-MacBook-Pro:~ tadel$ ioreg -l -w0 | grep ' \"DesignCapacity\" ' | grep -Eo "\d+"
            5770
            Juliens-MacBook-Pro:~ tadel$ ioreg -l -w0 | grep -E ' \"Voltage\" ' | grep -Eo "\d+"
            12054
            /**/

            /*
            string processOutput = ReadProcessOutput("ioreg", "-l -w0 | grep ' \\\"DesignCapacity\\\" ' | grep -Eo \"\\d+\"");

            if (uint.TryParse(processOutput, out uint designCapacity))
                battery.DesignCapacity = designCapacity;

            processOutput = ReadProcessOutput("ioreg", "-l -w0 | grep ' \\\"MaxCapacity\\\" ' | grep -Eo \"\\d+\"");

            if (uint.TryParse(processOutput, out uint fullChargeCapacity))
                battery.FullChargeCapacity = fullChargeCapacity;
            /**/

            // https://coderwall.com/p/bechiq/macos-get-battery-percentage-from-command-line
            /* 
            Juliens-MacBook-Pro:~ tadel$ pmset -g batt
            Now drawing from 'Battery Power'
             -InternalBattery-0 (id=3539043)	96%; discharging; 5:03 remaining present: true
            Juliens-MacBook-Pro:~ tadel$ pmset -g batt | grep -Eo "\d+%" | cut -d% -f1
            96
            Juliens-MacBook-Pro:~ tadel$ pmset -g batt | grep -Eo "\d+:\d+"
            6:23
            Juliens-MacBook-Pro:~ tadel$ pmset -g batt
            Now drawing from 'AC Power'
             -InternalBattery-0 (id=3539043)	92%; charging; (no estimate) present: true
            /**/

            string processOutput = ReadProcessOutput("pmset", "-g batt");

            if (!processOutput.Contains("InternalBattery"))
                return batteryList;

            Regex estimatedChargeRemainingRegex = new Regex("(\\d+)%");
            Match match = estimatedChargeRemainingRegex.Match(processOutput);

            if (match.Success && match.Groups.Count > 1)
            {
                if (ushort.TryParse(match.Groups[1].Value, out ushort estimatedChargeRemaining))
                {
                    battery.EstimatedChargeRemaining = estimatedChargeRemaining;
                }
            }

            Regex estimatedRunTimeRegex = new Regex("(\\d+:\\d+)");
            match = estimatedRunTimeRegex.Match(processOutput);

            if (match.Success && match.Groups.Count > 1)
            {
                string[] estimatedRunTime = match.Groups[1].Value.Split(':');

                if (estimatedRunTime.Length == 2 && uint.TryParse(estimatedRunTime[0], out uint hours) && uint.TryParse(estimatedRunTime[1], out uint minutes))
                {
                    battery.EstimatedRunTime = hours * 60 + minutes;
                }
            }

            bool fullyCharged = false;
            bool isCharging = false;

            StartProcess("system_profiler", "SPPowerDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (line.StartsWith("Full Charge Capacity (mAh): "))
                    {
                        if (uint.TryParse(line.Replace("Full Charge Capacity (mAh): ", string.Empty), out uint fullChargeCapacity))
                            battery.FullChargeCapacity = fullChargeCapacity;
                    }
                    else if (line.StartsWith("Fully Charged: "))
                    {
                        fullyCharged = line.EndsWith("Yes");
                    }
                    else if (line.StartsWith("Charging: "))
                    {
                        isCharging = line.EndsWith("Yes");
                    }
                },
                standardError => { });

            battery.BatteryStatus = fullyCharged ? (ushort)3 : isCharging ? (ushort)6 : (ushort)1;

            batteryList.Add(battery);

            return batteryList;
        }

        public List<BIOS> GetBiosList()
        {
            List<BIOS> biosList = new List<BIOS>();

            BIOS bios = new BIOS
            {
                Manufacturer = "Apple Inc."
            };

            StartProcess("system_profiler", "SPHardwareDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (line.StartsWith("System Firmware Version: "))
                        bios.Version = line.Replace("System Firmware Version: ", string.Empty);
                },
                standardError => { });

            biosList.Add(bios);

            return biosList;
        }

        /*
        system_profiler SPHardwareDataType
Hardware:

    Hardware Overview:

      Model Name: Mac mini
      Model Identifier: Macmini9,1
      Model Number: MGNR3ZE/A
      Chip: Apple M1
      Total Number of Cores: 8 (4 performance and 4 efficiency)
      Memory: 8 GB
      System Firmware Version: 10151.140.19
      OS Loader Version: 10151.140.19
      Serial Number (system): C07JG1XTQ6NV
      Hardware UUID: A7C8F6C7-C339-5904-B220-CF4C3D22FB1B
      Provisioning UDID: 00008103-001965521E60801E
      Activation Lock Status: Enabled
        */

        public List<ComputerSystem> GetComputerSystemList()
        {
            List<ComputerSystem> computerSystemList = new List<ComputerSystem>();

            ComputerSystem computerSystem = new ComputerSystem
            {
                Vendor = "Apple"
            };

            StartProcess("system_profiler", "SPHardwareDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (line.StartsWith("Model Name: "))
                    {
                        computerSystem.Caption = line.Replace("Model Name: ", string.Empty);
                        computerSystem.Name = line.Replace("Model Name: ", string.Empty);
                    }
                    else if (line.StartsWith("Model Identifier: "))
                    {
                        computerSystem.Description = line.Replace("Model Identifier: ", string.Empty);
                    }
                    else if (line.StartsWith("Serial Number (system): "))
                    {
                        computerSystem.IdentifyingNumber = line.Replace("Serial Number (system): ", string.Empty);
                    }
                    else if (line.StartsWith("Model Number: "))
                    {
                        computerSystem.SKUNumber = line.Replace("Model Number: ", string.Empty);
                    }
                    else if (line.StartsWith("Hardware UUID: "))
                    {
                        computerSystem.UUID = line.Replace("Hardware UUID: ", string.Empty);
                    }
                    else if (line.StartsWith("System Firmware Version: "))
                    {
                        computerSystem.Version = line.Replace("System Firmware Version: ", string.Empty);
                    }
                },
                standardError => { });

            computerSystemList.Add(computerSystem);

            return computerSystemList;
        }

        /*
        SPHardwareDataType
Hardware:

    Hardware Overview:

      Model Name: iMac
      Model Identifier: iMac11,3
      Processor Speed: 3,29 GHz
      Number of Processors: 1
      Total Number of Cores: 4
      L2 Cache (per Core): 256 KB
      L3 Cache: 24 MB
      Memory: 4 GB
      Boot ROM Version: VirtualBox
      SMC Version (system): 2.3f35
      Serial Number (system): 0
      Hardware UUID: F6D9C340-725A-224A-8855-99AB8348F745
        /**/

        public List<CPU> GetCpuList(bool includePercentProcessorTime = true, int millisecondsDelayBetweenTwoMeasurements = 500, bool includePerformanceCounter = true)
        {
            List<CPU> cpuList = new List<CPU>();

            string processOutput = ReadProcessOutput("sysctl", "-n hw.nperflevels");

            if (uint.TryParse(processOutput, out uint levels) && levels > 1)
            {
                for (int i = 0; i < levels; i++)
                {
                    string perflevel = "perflevel" + i;

                    CPU cpu = new CPU();

                    cpu.Caption = i.ToString();
                    cpu.Description = perflevel;

                    processOutput = ReadProcessOutput("sysctl", "-n machdep.cpu.brand_string");

                    // Intel CPUs include the clock speed as part of the name
                    cpu.Name = processOutput.Split('@')[0].Trim();

                    processOutput = ReadProcessOutput("sysctl", "-n hw.cpufrequency_max");

                    if (uint.TryParse(processOutput, out uint maxFrequency))
                        cpu.MaxClockSpeed = maxFrequency / 1_000_000;

                    processOutput = ReadProcessOutput("sysctl", "-n hw.cpufrequency");

                    if (uint.TryParse(processOutput, out uint frequency))
                        cpu.CurrentClockSpeed = frequency / 1_000_000;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.l1icachesize");

                    if (uint.TryParse(processOutput, out uint L1InstructionCacheSize))
                        cpu.L1InstructionCacheSize = L1InstructionCacheSize;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.l1dcachesize");

                    if (uint.TryParse(processOutput, out uint L1DataCacheSize))
                        cpu.L1DataCacheSize = L1DataCacheSize;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.l2cachesize");

                    if (uint.TryParse(processOutput, out uint L2CacheSize))
                        cpu.L2CacheSize = L2CacheSize;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.l3cachesize");

                    if (uint.TryParse(processOutput, out uint L3CacheSize))
                        cpu.L3CacheSize = L3CacheSize;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.physicalcpu");

                    if (uint.TryParse(processOutput, out uint numberOfCores))
                        cpu.NumberOfCores = numberOfCores;

                    processOutput = ReadProcessOutput("sysctl", $"-n hw.{perflevel}.logicalcpu");

                    if (uint.TryParse(processOutput, out uint numberOfLogicalProcessors))
                        cpu.NumberOfLogicalProcessors = numberOfLogicalProcessors;

                    cpuList.Add(cpu);
                }
            }
            else
            {
                CPU cpu = new CPU();

                processOutput = ReadProcessOutput("sysctl", "-n machdep.cpu.brand_string");

                // Intel CPUs include the clock speed as part of the name
                cpu.Name = processOutput.Split('@')[0].Trim();

                processOutput = ReadProcessOutput("sysctl", "-n hw.cpufrequency_max");

                if (uint.TryParse(processOutput, out uint maxFrequency))
                    cpu.MaxClockSpeed = maxFrequency / 1_000_000;

                processOutput = ReadProcessOutput("sysctl", "-n hw.cpufrequency");

                if (uint.TryParse(processOutput, out uint frequency))
                    cpu.CurrentClockSpeed = frequency / 1_000_000;

                processOutput = ReadProcessOutput("sysctl", "-n hw.l1icachesize");

                if (uint.TryParse(processOutput, out uint L1InstructionCacheSize))
                    cpu.L1InstructionCacheSize = L1InstructionCacheSize;

                processOutput = ReadProcessOutput("sysctl", "-n hw.l1dcachesize");

                if (uint.TryParse(processOutput, out uint L1DataCacheSize))
                    cpu.L1DataCacheSize = L1DataCacheSize;

                processOutput = ReadProcessOutput("sysctl", "-n hw.l2cachesize");

                if (uint.TryParse(processOutput, out uint L2CacheSize))
                    cpu.L2CacheSize = L2CacheSize;

                processOutput = ReadProcessOutput("sysctl", "-n hw.l3cachesize");

                if (uint.TryParse(processOutput, out uint L3CacheSize))
                    cpu.L3CacheSize = L3CacheSize;

                processOutput = ReadProcessOutput("sysctl", "-n hw.physicalcpu");

                if (uint.TryParse(processOutput, out uint numberOfCores))
                    cpu.NumberOfCores = numberOfCores;

                processOutput = ReadProcessOutput("sysctl", "-n hw.logicalcpu");

                if (uint.TryParse(processOutput, out uint numberOfLogicalProcessors))
                    cpu.NumberOfLogicalProcessors = numberOfLogicalProcessors;

                cpuList.Add(cpu);
            }

            return cpuList;
        }

        public override List<Drive> GetDriveList()
        {
            /*
            SPSerialATADataType
SATA/SATA Express:

    Intel ICH8-M AHCI:

      Vendor: Intel
      Product: ICH8-M AHCI
      Link Speed: 3 Gigabit
      Negotiated Link Speed: 3 Gigabit
      Physical Interconnect: SATA
      Description: AHCI Version 1.10 Supported

        VBOX HARDDISK:

          Capacity: 536,87 GB (536.870.912.000 bytes)
          Model: VBOX HARDDISK
          Revision: 1,000000
          Serial Number: VBa308df62-62a2d2a0 
          Native Command Queuing: Yes
          Queue Depth: 32
          Removable Media: No
          Detachable Drive: No
          BSD Name: disk0
          Medium Type: Rotational
          Partition Map Type: GPT (GUID Partition Table)
          Volumes:
            EFI:
              Capacity: 209,7 MB (209.715.200 bytes)
              File System: MS-DOS FAT32
              BSD Name: disk0s1
              Content: EFI
              Volume UUID: 0E239BC6-F960-3107-89CF-1C97F78BB46B
            Macintosh HD:
              Capacity: 536,01 GB (536.011.153.408 bytes)
              Available: 513,49 GB (513.488.637.952 bytes)
              Writable: Yes
              File System: Journaled HFS+
              BSD Name: disk0s2
              Mount Point: /
              Content: Apple_HFS
              Volume UUID: 510DC06E-E3D7-36E9-9711-C8A209E8C61E
            Recovery HD:
              Capacity: 650 MB (650.002.432 bytes)
              File System: Journaled HFS+
              BSD Name: disk0s3
              Content: Apple_Boot
              Volume UUID: B822D1A0-3CE3-3BA2-852F-85F818623545

    Intel ICH8-M AHCI:

      Vendor: Intel
      Product: ICH8-M AHCI
      Link Speed: 3 Gigabit
      Negotiated Link Speed: 3 Gigabit
      Physical Interconnect: SATA
      Description: AHCI Version 1.10 Supported

        VBOX CD-ROM:

          Model: VBOX CD-ROM
          Revision: 1,000000
          Serial Number: VB1-1a2b3c4d
          Native Command Queuing: No
          Detachable Drive: No
          Power Off: No
          Async Notification: No
            /**/

            /*
            SPStorageDataType
Storage:

    Macintosh HD:

      Available: 513,49 GB (513.488.637.952 bytes)
      Capacity: 536,01 GB (536.011.153.408 bytes)
      Mount Point: /
      File System: Journaled HFS+
      Writable: Yes
      Ignore Ownership: No
      BSD Name: disk0s2
      Volume UUID: 510DC06E-E3D7-36E9-9711-C8A209E8C61E
      Physical Drive:
          Device Name: VBOX HARDDISK
          Media Name: VBOX HARDDISK Media
          Medium Type: Rotational
          Protocol: SATA
          Internal: Yes
          Partition Map Type: GPT (GUID Partition Table)
            /**/

            static ulong ParseBytesFromParentheses(string value)
            {
                int open = value.LastIndexOf('(');
                int close = value.LastIndexOf(')');

                if (open < 0 || close <= open)
                    return 0;

                string inner = value.Substring(open + 1, close - open - 1);
                string[] parts = inner.Split(' ');

                if (parts.Length < 1)
                    return 0;

                string digits = Regex.Replace(parts[0], @"\D", string.Empty);

                return ulong.TryParse(digits, out ulong result) ? result : 0;
            }

            List<Drive> driveList = new List<Drive>();
            Dictionary<string, Drive> drivesByDeviceName = new Dictionary<string, Drive>();
            Dictionary<string, Partition> partitionsByDeviceName = new Dictionary<string, Partition>();

            Volume? currentVolume = null;
            string currentPhysicalDeviceName = string.Empty;
            bool inPhysicalDriveSection = false;

            void CommitCurrentVolume()
            {
                if (currentVolume == null)
                    return;

                string key = string.IsNullOrEmpty(currentPhysicalDeviceName) ? "__unknown__" : currentPhysicalDeviceName;

                if (!partitionsByDeviceName.TryGetValue(key, out Partition? partition))
                {
                    partition = new Partition();
                    partitionsByDeviceName[key] = partition;

                    if (!drivesByDeviceName.ContainsKey(key))
                        drivesByDeviceName[key] = new Drive { Caption = key, Model = key, Name = key };

                    drivesByDeviceName[key].PartitionList.Add(partition);
                }

                partition.VolumeList.Add(currentVolume);
                currentVolume = null;
                currentPhysicalDeviceName = string.Empty;
                inPhysicalDriveSection = false;
            }

            StartProcess("system_profiler", "SPStorageDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (spaceCount == 4 && line.EndsWith(":") && !line.Contains(": "))
                    {
                        CommitCurrentVolume();
                        currentVolume = new Volume { VolumeName = line.TrimEnd(':') };
                        return;
                    }

                    if (currentVolume == null)
                        return;

                    if (!inPhysicalDriveSection && spaceCount == 6 && line == "Physical Drive:")
                    {
                        inPhysicalDriveSection = true;
                        return;
                    }

                    if (inPhysicalDriveSection)
                    {
                        if (line.StartsWith("Device Name: "))
                        {
                            currentPhysicalDeviceName = line.Replace("Device Name: ", string.Empty);

                            if (!drivesByDeviceName.ContainsKey(currentPhysicalDeviceName))
                                drivesByDeviceName[currentPhysicalDeviceName] = new Drive
                                {
                                    Caption = currentPhysicalDeviceName,
                                    Model = currentPhysicalDeviceName,
                                    Name = currentPhysicalDeviceName
                                };
                        }
                        else if (line.StartsWith("Medium Type: "))
                        {
                            if (drivesByDeviceName.TryGetValue(currentPhysicalDeviceName, out Drive? drive))
                                drive.MediaType = line.Replace("Medium Type: ", string.Empty);
                        }
                        else if (line.StartsWith("Protocol: "))
                        {
                            if (drivesByDeviceName.TryGetValue(currentPhysicalDeviceName, out Drive? drive))
                                drive.Description = line.Replace("Protocol: ", string.Empty);
                        }
                    }
                    else if (spaceCount == 6)
                    {
                        if (line.StartsWith("Available: "))
                            currentVolume.FreeSpace = ParseBytesFromParentheses(line);
                        else if (line.StartsWith("Capacity: "))
                            currentVolume.Size = ParseBytesFromParentheses(line);
                        else if (line.StartsWith("Mount Point: "))
                        {
                            string mountPoint = line.Replace("Mount Point: ", string.Empty);
                            currentVolume.Caption = mountPoint;
                            currentVolume.Name = mountPoint;
                        }
                        else if (line.StartsWith("File System: "))
                            currentVolume.FileSystem = line.Replace("File System: ", string.Empty);
                        else if (line.StartsWith("Volume UUID: "))
                            currentVolume.VolumeSerialNumber = line.Replace("Volume UUID: ", string.Empty);
                    }
                },
                standardError => { });

            CommitCurrentVolume();

            foreach (Drive drive in drivesByDeviceName.Values)
                driveList.Add(drive);

            if (driveList.Count == 0)
                return base.GetDriveList();

            return driveList;
        }

        public List<Keyboard> GetKeyboardList()
        {
            List<Keyboard> keyboardList = new List<Keyboard>();

            StartProcess("system_profiler", "SPUSBDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (line.EndsWith(":") && !line.Contains(": "))
                    {
                        string name = line.TrimEnd(':').Trim();

                        if (name.IndexOf("keyboard", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            keyboardList.Add(new Keyboard
                            {
                                Caption = name,
                                Description = name,
                                Name = name
                            });
                        }
                    }
                },
                standardError => { });

            string btDeviceName = string.Empty;
            bool btIsKeyboard = false;

            void CommitBtKeyboard()
            {
                if (btIsKeyboard && !string.IsNullOrEmpty(btDeviceName))
                {
                    keyboardList.Add(new Keyboard
                    {
                        Caption = btDeviceName,
                        Description = btDeviceName,
                        Name = btDeviceName
                    });
                }

                btDeviceName = string.Empty;
                btIsKeyboard = false;
            }

            StartProcess("system_profiler", "SPBluetoothDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (line.EndsWith(":") && !line.Contains(": "))
                    {
                        CommitBtKeyboard();
                        btDeviceName = line.TrimEnd(':').Trim();
                    }
                    else if (line.StartsWith("Minor Type: ") && line.IndexOf("Keyboard", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        btIsKeyboard = true;
                    }
                },
                standardError => { });

            CommitBtKeyboard();

            return keyboardList;
        }

        public List<Memory> GetMemoryList()
        {
            List<Memory> memoryList = new List<Memory>();

            Memory? memory = null;

            /*
            SPMemoryDataType
Memory:

    Memory Slots:

      ECC: Disabled
      Upgradeable Memory: Yes

        Bank 0/DIMM 0:

          Size: 4 GB
          Type: DRAM
          Speed: 1600 MHz
          Status: OK
          Manufacturer: innotek GmbH
          Part Number: -
          Serial Number: -
            /**/

            StartProcess("system_profiler", "SPMemoryDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    string[] split = line.Split(' ');

                    if (line.StartsWith("Bank"))
                    {
                        if (memory != null)
                            memoryList.Add(memory);

                        memory = new Memory();
                    }

                    if (memory != null)
                    {
                        if (line.StartsWith("Size:"))
                        {
                            if (split.Length == 3)
                            {
                                if (ulong.TryParse(split[1], out ulong size))
                                {
                                    memory.Capacity = split[2] switch
                                    {
                                        "TB" => size * 1024uL * 1024uL * 1024uL * 1024uL,
                                        "GB" => size * 1024uL * 1024uL * 1024uL,
                                        "MB" => size * 1024uL * 1024uL,
                                        "KB" => size * 1024uL,
                                        _ => size,
                                    };
                                }
                            }
                        }

                        if (line.StartsWith("Type:"))
                        {
                            if (split.Length == 2)
                            {
                                if (Enum.TryParse(split[1], out FormFactor formFactor))
                                    memory.FormFactor = formFactor;
                            }
                        }

                        if (line.StartsWith("Speed:"))
                        {
                            if (split.Length == 3)
                            {
                                if (uint.TryParse(split[1], out uint speed))
                                    memory.Speed = speed;
                            }
                        }

                        if (line.StartsWith("Manufacturer: "))
                        {
                            memory.Manufacturer = line.Replace("Manufacturer: ", string.Empty);
                        }

                        if (line.StartsWith("Part Number: "))
                        {
                            memory.PartNumber = line.Replace("Part Number: ", string.Empty);
                        }

                        if (line.StartsWith("Serial Number: "))
                        {
                            memory.SerialNumber = line.Replace("Serial Number: ", string.Empty);
                        }
                    }
                },
                standardError => { });

            if (memory != null)
                memoryList.Add(memory);

            return memoryList;
        }

        public List<Monitor> GetMonitorList()
        {
            List<Monitor> monitorList = new List<Monitor>();

            Monitor? monitor = null;

            // https://developer.apple.com/documentation/appkit/nsscreen

            // https://developer.apple.com/documentation/iokit/iographicslib_h

            // IODisplayConnect

            // IODisplayEDID

            //auto mainDisplayId = CGMainDisplayID();
            //width = CGDisplayPixelsWide(mainDisplayId);
            //height = CGDisplayPixelsHigh(mainDisplayId);

            StartProcess("system_profiler", "SPDisplaysDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();

                    string line = standardOutput.Trim();

                    string[] split = line.Split(':');

                    if (spaceCount == 8)
                    {
                        if (monitor != null)
                            monitorList.Add(monitor);

                        string name = line.TrimEnd(':');

                        monitor = new Monitor
                        {
                            Caption = name,
                            Description = name,
                            Name = name,
                            UserFriendlyName = name
                        };
                    }

                    if (monitor != null)
                    {
                        if (line.StartsWith("Display Type: "))
                        {
                            monitor.MonitorType = line.Replace("Display Type: ", string.Empty);
                        }
                        else if (line.StartsWith("Connection Type: "))
                        {
                            monitor.MonitorType = line.Replace("Connection Type: ", string.Empty);
                        }
                        else if (line.StartsWith("Display Serial Number: "))
                        {
                            monitor.SerialNumberID = line.Replace("Display Serial Number: ", string.Empty);
                        }
                        else if (line == "Main Display: Yes")
                        {
                            monitor.Active = true;
                        }
                    }
                },
                standardError => { });

            /*
            SPDisplaysDataType
Graphics/Displays:

    Display:

      Type: GPU
      Bus: PCI
      VRAM (Total): 8 MB
      Device ID: 0xbeef
      Revision ID: 0x0000
      Kernel Extension Info: No Kext Loaded
      Displays:
        Display:
          Resolution: 1920 x 1200
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Automatically Adjust Brightness: No
      Vendor ID: 0x80ee
            /**/

            /*
Graphics/Displays:

    Intel Iris Pro:

      Chipset Model: Intel Iris Pro
      Type: GPU
      Bus: Built-In
      VRAM (Dynamic, Max): 1536 MB
      Vendor: Intel
      Device ID: 0x0d26
      Revision ID: 0x0008
      Metal: Supported, feature set macOS GPUFamily1 v4
      Displays:
        DELL U2718Q:
          Resolution: 3840 x 2160 (2160p 4K UHD - Ultra High Definition)
          UI Looks like: 1920 x 1080 @ 60 Hz
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Display Serial Number: 5DWRH7AU05LL
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Rotation: Supported
          Automatically Adjust Brightness: No
          Connection Type: DisplayPort
        DELL U2718Q:
          Resolution: 2160 x 3840
          UI Looks like: 1080 x 1920 @ 60 Hz
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Display Serial Number: 5DWRH7AQ12UL
          Mirror: Off
          Online: Yes
          Rotation: 90
          Automatically Adjust Brightness: No
          Connection Type: DisplayPort
            /**/

            /*
Graphics/Displays:

    Intel HD Graphics 4000:

      Chipset Model: Intel HD Graphics 4000
      Type: GPU
      Bus: Built-In
      VRAM (Total): 384 MB
      Vendor: Intel (0x8086)
      Device ID: 0x0166
      Revision ID: 0x0009
      gMux Version: 1.9.23

    NVIDIA GeForce GT 650M:

      Chipset Model: NVIDIA GeForce GT 650M
      Type: GPU
      Bus: PCIe
      PCIe Lane Width: x8
      VRAM (Total): 512 MB
      Vendor: NVIDIA (0x10de)
      Device ID: 0x0fd5
      Revision ID: 0x00a2
      ROM Revision: 3682
      gMux Version: 1.9.23
      Displays:
        Color LCD:
          Display Type: LCD
          Resolution: 1440 x 900
          Pixel Depth: 32-Bit Color (ARGB8888)
          Mirror: Off
          Online: Yes
          Built-In: Yes
        Thunderbolt Display:
          Display Type: LCD
          Resolution: 2560 x 1440
          Pixel Depth: 32-Bit Color (ARGB8888)
          Display Serial Number: C02K80FPF2GC
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Rotation: Supported
          Connection Type: DisplayPort
            /**/

            if (monitor != null)
                monitorList.Add(monitor);

            return monitorList;
        }

        public List<Motherboard> GetMotherboardList()
        {
            List<Motherboard> motherboardList = new List<Motherboard>();

            Motherboard motherboard = new Motherboard
            {
                Manufacturer = "Apple Inc."
            };

            StartProcess("system_profiler", "SPHardwareDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (line.StartsWith("Model Identifier: "))
                        motherboard.Product = line.Replace("Model Identifier: ", string.Empty);
                    else if (line.StartsWith("Serial Number (system): "))
                        motherboard.SerialNumber = line.Replace("Serial Number (system): ", string.Empty);
                },
                standardError => { });

            motherboardList.Add(motherboard);

            return motherboardList;
        }

        public List<Mouse> GetMouseList()
        {
            List<Mouse> mouseList = new List<Mouse>();

            Mouse? currentMouse = null;

            void CommitUsbMouse()
            {
                if (currentMouse != null)
                    mouseList.Add(currentMouse);

                currentMouse = null;
            }

            StartProcess("system_profiler", "SPUSBDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (line.EndsWith(":") && !line.Contains(": "))
                    {
                        CommitUsbMouse();

                        string name = line.TrimEnd(':').Trim();

                        if (name.IndexOf("mouse", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf("trackball", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf("trackpad", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf("pointing", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            currentMouse = new Mouse
                            {
                                Caption = name,
                                Description = name,
                                Name = name
                            };
                        }
                    }
                    else if (currentMouse != null && line.StartsWith("Manufacturer: "))
                    {
                        currentMouse.Manufacturer = line.Replace("Manufacturer: ", string.Empty);
                    }
                },
                standardError => { });

            CommitUsbMouse();

            string btDeviceName = string.Empty;
            bool btIsMouse = false;

            void CommitBtMouse()
            {
                if (btIsMouse && !string.IsNullOrEmpty(btDeviceName))
                {
                    mouseList.Add(new Mouse
                    {
                        Caption = btDeviceName,
                        Description = btDeviceName,
                        Name = btDeviceName
                    });
                }

                btDeviceName = string.Empty;
                btIsMouse = false;
            }

            StartProcess("system_profiler", "SPBluetoothDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (line.EndsWith(":") && !line.Contains(": "))
                    {
                        CommitBtMouse();
                        btDeviceName = line.TrimEnd(':').Trim();
                    }
                    else if (line.StartsWith("Minor Type: ") &&
                             (line.IndexOf("Mouse", StringComparison.OrdinalIgnoreCase) >= 0 ||
                              line.IndexOf("Pointing", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        btIsMouse = true;
                    }
                },
                standardError => { });

            CommitBtMouse();

            return mouseList;
        }

        public override List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000)
        {
            /*
            SPNetworkDataType
Network:

    Ethernet:

      Type: Ethernet
      Hardware: Ethernet
      BSD Device Name: en0
      IPv4 Addresses: 10.0.2.15
      IPv4:
          AdditionalRoutes:
              DestinationAddress: 10.0.2.15
              SubnetMask: 255.255.255.255
              DestinationAddress: 169.254.0.0
              SubnetMask: 255.255.0.0
          Addresses: 10.0.2.15
          ARPResolvedHardwareAddress: 52:54:00:12:35:02
          ARPResolvedIPAddress: 10.0.2.2
          Configuration Method: DHCP
          ConfirmedInterfaceName: en0
          Interface Name: en0
          Network Signature: IPv4.Router=10.0.2.2;IPv4.RouterHardwareAddress=52:54:00:12:35:02
          Router: 10.0.2.2
          Subnet Masks: 255.255.255.0
      IPv6:
          Configuration Method: Automatic
      DNS:
          Server Addresses: 192.168.0.1
      DHCP Server Responses:
          Domain Name Servers: 192.168.0.1
          Lease Duration (seconds): 0
          DHCP Message Type: 0x05
          Routers: 10.0.2.2
          Server Identifier: 10.0.2.2
          Subnet Mask: 255.255.255.0
      Ethernet:
          MAC Address: 08:00:27:5f:7a:7e
          Media Options: Full Duplex
          Media Subtype: 1000baseT
      Proxies:
          Exceptions List: *.local, 169.254/16
          FTP Passive Mode: Yes
      Service Order: 0
            /**/

            List<NetworkAdapter> networkAdapterList = base.GetNetworkAdapterList(false, includeNetworkAdapterConfiguration, millisecondsDelayBetweenTwoMeasurements);

            Dictionary<string, NetworkAdapter> adapterByName = networkAdapterList.ToDictionary(a => a.Name);

            // system_profiler SPNetworkDataType → AdapterType, NetConnectionID, DefaultIPGatewayList, IPSubnetList, DNSServerSearchOrderList, DHCPServer
            string svcName = string.Empty;
            string bsdName = string.Empty;
            string adapterType = string.Empty;
            List<string> routers = new List<string>();
            List<string> subnetMasks = new List<string>();
            List<string> dnsServers = new List<string>();
            string dhcpServer = string.Empty;
            string section = string.Empty;

            void CommitService()
            {
                if (string.IsNullOrEmpty(bsdName) || !adapterByName.TryGetValue(bsdName, out NetworkAdapter? adapter))
                    return;

                if (!string.IsNullOrEmpty(svcName))
                {
                    adapter.NetConnectionID = svcName;
                    adapter.AdapterType = string.IsNullOrEmpty(adapterType) ? svcName : adapterType;
                }

                if (includeNetworkAdapterConfiguration)
                {
                    foreach (string r in routers)
                        if (IPAddress.TryParse(r.Trim(), out IPAddress? ip))
                            adapter.DefaultIPGatewayList.Add(ip);

                    foreach (string m in subnetMasks)
                        if (IPAddress.TryParse(m.Trim(), out IPAddress? ip))
                            adapter.IPSubnetList.Add(ip);

                    foreach (string d in dnsServers)
                        if (IPAddress.TryParse(d.Trim(), out IPAddress? ip))
                            adapter.DNSServerSearchOrderList.Add(ip);

                    if (!string.IsNullOrEmpty(dhcpServer) && IPAddress.TryParse(dhcpServer.Trim(), out IPAddress? dhcp))
                        adapter.DHCPServer = dhcp;
                }
            }

            StartProcess("system_profiler", "SPNetworkDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (spaceCount == 4 && line.EndsWith(":") && !line.Contains(": "))
                    {
                        CommitService();
                        svcName = line.TrimEnd(':');
                        bsdName = string.Empty;
                        adapterType = string.Empty;
                        routers.Clear();
                        subnetMasks.Clear();
                        dnsServers.Clear();
                        dhcpServer = string.Empty;
                        section = string.Empty;
                    }
                    else if (spaceCount == 6)
                    {
                        section = string.Empty;

                        if (line.StartsWith("BSD Device Name: "))
                            bsdName = line.Replace("BSD Device Name: ", string.Empty);
                        else if (line.StartsWith("Type: "))
                            adapterType = line.Replace("Type: ", string.Empty);
                        else if (line == "IPv4:")
                            section = "ipv4";
                        else if (line == "DNS:")
                            section = "dns";
                        else if (line == "DHCP Server Responses:")
                            section = "dhcp";
                    }
                    else if (spaceCount == 10 && !string.IsNullOrEmpty(section))
                    {
                        if (section == "ipv4")
                        {
                            if (line.StartsWith("Router: "))
                                routers.Add(line.Replace("Router: ", string.Empty));
                            else if (line.StartsWith("Subnet Masks: "))
                                subnetMasks.AddRange(line.Replace("Subnet Masks: ", string.Empty).Split(','));
                        }
                        else if (section == "dns" && line.StartsWith("Server Addresses: "))
                        {
                            dnsServers.AddRange(line.Replace("Server Addresses: ", string.Empty).Split(','));
                        }
                        else if (section == "dhcp" && line.StartsWith("Server Identifier: "))
                        {
                            dhcpServer = line.Replace("Server Identifier: ", string.Empty);
                        }
                    }
                },
                standardError => { });

            CommitService();

            // netstat -ib × 2 with delay → BytesSentPersec, BytesReceivedPersec
            if (includeBytesPersec)
            {
                static Dictionary<string, (ulong ibytes, ulong obytes)> ParseNetstatIb(string output)
                {
                    var result = new Dictionary<string, (ulong ibytes, ulong obytes)>();
                    foreach (string rawLine in output.Split('\n'))
                    {
                        string[] cols = rawLine.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (cols.Length < 10 || !cols[2].StartsWith("<Link#"))
                            continue;
                        // Count from the end: Ibytes is 5th from end, Obytes is 2nd from end.
                        // This is robust regardless of whether the Address column is present.
                        int ibytesIdx = cols.Length - 5;
                        int obytesIdx = cols.Length - 2;
                        if (ulong.TryParse(cols[ibytesIdx], out ulong ibytes) && ulong.TryParse(cols[obytesIdx], out ulong obytes))
                            result[cols[0]] = (ibytes, obytes);
                    }
                    return result;
                }

                var snapshot1 = ParseNetstatIb(ReadProcessOutput("netstat", "-ib"));
                Thread.Sleep(millisecondsDelayBetweenTwoMeasurements);
                var snapshot2 = ParseNetstatIb(ReadProcessOutput("netstat", "-ib"));

                double seconds = millisecondsDelayBetweenTwoMeasurements / 1000.0;

                foreach (KeyValuePair<string, (ulong ibytes, ulong obytes)> kv in snapshot2)
                {
                    if (!adapterByName.TryGetValue(kv.Key, out NetworkAdapter? adapter))
                        continue;
                    if (!snapshot1.TryGetValue(kv.Key, out (ulong ibytes, ulong obytes) s1))
                        continue;

                    ulong deltaRx = kv.Value.ibytes >= s1.ibytes ? kv.Value.ibytes - s1.ibytes : 0;
                    ulong deltaTx = kv.Value.obytes >= s1.obytes ? kv.Value.obytes - s1.obytes : 0;

                    adapter.BytesReceivedPersec = (ulong)(deltaRx / seconds);
                    adapter.BytesSentPersec = (ulong)(deltaTx / seconds);
                }
            }

            return networkAdapterList;
        }

        public List<Printer> GetPrinterList()
        {
            List<Printer> printerList = new List<Printer>();

            Printer? printer = null;

            StartProcess("system_profiler", "SPPrintersDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();
                    string line = standardOutput.Trim();

                    if (spaceCount == 4 && line.EndsWith(":") && !line.Contains(": "))
                    {
                        if (printer != null)
                            printerList.Add(printer);

                        string name = line.TrimEnd(':');

                        printer = new Printer
                        {
                            Name = name,
                            Caption = name,
                            Description = name
                        };
                    }
                    else if (printer != null)
                    {
                        if (line == "Default: Yes")
                            printer.Default = true;
                        else if (line == "Print Server: Local")
                            printer.Local = true;
                        else if (line == "Print Server: Network")
                            printer.Network = true;
                        else if (line == "Shared: Yes")
                            printer.Shared = true;
                    }
                },
                standardError => { });

            if (printer != null)
                printerList.Add(printer);

            if (printerList.Count == 0)
            {
                string lpstatOutput = ReadProcessOutput("lpstat", "-p");

                foreach (string line in lpstatOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // format: "printer <name> is idle." or "printer <name> disabled since ..."
                    string[] parts = line.Split(' ');

                    if (parts.Length >= 2 && parts[0] == "printer")
                    {
                        string name = parts[1];

                        printerList.Add(new Printer
                        {
                            Name = name,
                            Caption = name,
                            Description = name,
                            Local = true
                        });
                    }
                }

                string defaultOutput = ReadProcessOutput("lpstat", "-d");

                const string defaultPrefix = "system default destination: ";

                if (defaultOutput.StartsWith(defaultPrefix))
                {
                    string defaultName = defaultOutput.Substring(defaultPrefix.Length).Trim();

                    Printer? defaultPrinter = printerList.FirstOrDefault(p => p.Name == defaultName);

                    if (defaultPrinter != null)
                        defaultPrinter.Default = true;
                }
            }

            return printerList;
        }

        public List<SoundDevice> GetSoundDeviceList()
        {
            List<SoundDevice> soundDeviceList = new List<SoundDevice>();

            bool inDevicesSection = false;
            SoundDevice? soundDevice = null;

            StartProcess("system_profiler", "SPAudioDataType",
                standardOutput =>
                {
                    string line = standardOutput.Trim();

                    if (string.IsNullOrEmpty(line))
                        return;

                    if (line == "Devices:")
                    {
                        inDevicesSection = true;
                        return;
                    }

                    if (!inDevicesSection)
                        return;

                    if (line.EndsWith(":") && !line.Contains(": "))
                    {
                        if (soundDevice != null)
                            soundDeviceList.Add(soundDevice);

                        string name = line.TrimEnd(':').Trim();

                        soundDevice = new SoundDevice
                        {
                            Caption = name,
                            Description = name,
                            Name = name,
                            ProductName = name
                        };
                    }
                    else if (soundDevice != null && line.StartsWith("Manufacturer: "))
                    {
                        soundDevice.Manufacturer = line.Replace("Manufacturer: ", string.Empty);
                    }
                },
                standardError => { });

            if (soundDevice != null)
                soundDeviceList.Add(soundDevice);

            if (soundDeviceList.Count == 0)
            {
                StartProcess("system_profiler", "SPHardwareDataType",
                    standardOutput =>
                    {
                        string line = standardOutput.Trim();

                        if (line.StartsWith("Chip: "))
                        {
                            string name = line.Replace("Chip: ", string.Empty);

                            soundDeviceList.Add(new SoundDevice
                            {
                                Caption = name,
                                Description = name,
                                Name = name,
                                ProductName = name,
                                Manufacturer = "Apple Inc."
                            });
                        }
                    },
                    standardError => { });
            }

            return soundDeviceList;
        }

        public List<VideoController> GetVideoControllerList()
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            VideoController? videoController = null;

            // https://stackoverflow.com/questions/18077639/getting-graphic-card-information-in-objective-c

            // https://developer.apple.com/documentation/iokit/iographicslib_h

            StartProcess("system_profiler", "SPDisplaysDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();

                    string line = standardOutput.Trim();

                    if (spaceCount == 4)
                    {
                        if (videoController != null)
                            videoControllerList.Add(videoController);

                        string name = line.TrimEnd(':');

                        videoController = new VideoController
                        {
                            Caption = name,
                            Description = name,
                            Name = name
                        };
                    }

                    if (videoController != null)
                    {
                        if (line.StartsWith("Chipset Model: "))
                        {
                            videoController.VideoProcessor = line.Replace("Chipset Model: ", string.Empty);
                        }

                        if (line.StartsWith("VRAM "))
                        {
                            string[] split = line.Split(':');

                            if (split.Length == 2)
                            {
                                if (ulong.TryParse(split[1].Replace("MB", string.Empty).Trim(), out ulong ram))
                                    videoController.AdapterRAM = 1024uL * 1024uL * ram;
                            }
                        }

                        if (line.StartsWith("Vendor: "))
                        {
                            videoController.Manufacturer = line.Replace("Vendor: ", string.Empty);
                        }

                        if (line.StartsWith("Resolution: "))
                        {
                            string[] split = line.Replace("Resolution: ", string.Empty).Split(' ');

                            if (split.Length >= 3)
                            {
                                if (uint.TryParse(split[0].Trim(), out uint x))
                                    videoController.CurrentHorizontalResolution = x;

                                if (uint.TryParse(split[2].Trim(), out uint y))
                                    videoController.CurrentVerticalResolution = y;
                            }
                        }

                        if (line.StartsWith("UI Looks like: "))
                        {
                            string[] split = line.Split('@');

                            if (split.Length == 2)
                            {
                                if (uint.TryParse(split[1].Replace("Hz", string.Empty).Trim(), out uint Hz))
                                    videoController.CurrentRefreshRate = Hz;
                            }
                        }

                        if (line.StartsWith("Framebuffer Depth: "))
                        {
                            string[] split = line.Replace("Framebuffer Depth: ", string.Empty).Split('-');

                            if (split.Length == 2)
                            {
                                if (uint.TryParse(split[0].Trim(), out uint depth))
                                    videoController.CurrentBitsPerPixel = depth;
                            }
                        }

                        if (line.StartsWith("Pixel Depth: "))
                        {
                            string[] split = line.Replace("Pixel Depth: ", string.Empty).Split('-');

                            if (split.Length == 2)
                            {
                                if (uint.TryParse(split[0].Trim(), out uint depth))
                                    videoController.CurrentBitsPerPixel = depth;
                            }
                        }
                    }
                },
                standardError => { });

            /*
            SPDisplaysDataType
Graphics/Displays:

    Display:

      Type: GPU
      Bus: PCI
      VRAM (Total): 8 MB
      Device ID: 0xbeef
      Revision ID: 0x0000
      Kernel Extension Info: No Kext Loaded
      Displays:
        Display:
          Resolution: 1920 x 1200
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Automatically Adjust Brightness: No
      Vendor ID: 0x80ee
            /**/

            /*
Graphics/Displays:

    Intel Iris Pro:

      Chipset Model: Intel Iris Pro
      Type: GPU
      Bus: Built-In
      VRAM (Dynamic, Max): 1536 MB
      Vendor: Intel
      Device ID: 0x0d26
      Revision ID: 0x0008
      Metal: Supported, feature set macOS GPUFamily1 v4
      Displays:
        DELL U2718Q:
          Resolution: 3840 x 2160 (2160p 4K UHD - Ultra High Definition)
          UI Looks like: 1920 x 1080 @ 60 Hz
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Display Serial Number: 5DWRH7AU05LL
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Rotation: Supported
          Automatically Adjust Brightness: No
          Connection Type: DisplayPort
        DELL U2718Q:
          Resolution: 2160 x 3840
          UI Looks like: 1080 x 1920 @ 60 Hz
          Framebuffer Depth: 24-Bit Color (ARGB8888)
          Display Serial Number: 5DWRH7AQ12UL
          Mirror: Off
          Online: Yes
          Rotation: 90
          Automatically Adjust Brightness: No
          Connection Type: DisplayPort
            /**/

            /*
Graphics/Displays:

    Intel HD Graphics 4000:

      Chipset Model: Intel HD Graphics 4000
      Type: GPU
      Bus: Built-In
      VRAM (Total): 384 MB
      Vendor: Intel (0x8086)
      Device ID: 0x0166
      Revision ID: 0x0009
      gMux Version: 1.9.23

    NVIDIA GeForce GT 650M:

      Chipset Model: NVIDIA GeForce GT 650M
      Type: GPU
      Bus: PCIe
      PCIe Lane Width: x8
      VRAM (Total): 512 MB
      Vendor: NVIDIA (0x10de)
      Device ID: 0x0fd5
      Revision ID: 0x00a2
      ROM Revision: 3682
      gMux Version: 1.9.23
      Displays:
        Color LCD:
          Display Type: LCD
          Resolution: 1440 x 900
          Pixel Depth: 32-Bit Color (ARGB8888)
          Mirror: Off
          Online: Yes
          Built-In: Yes
        Thunderbolt Display:
          Display Type: LCD
          Resolution: 2560 x 1440
          Pixel Depth: 32-Bit Color (ARGB8888)
          Display Serial Number: C02K80FPF2GC
          Main Display: Yes
          Mirror: Off
          Online: Yes
          Rotation: Supported
          Connection Type: DisplayPort
            /**/

            if (videoController != null)
                videoControllerList.Add(videoController);

            return videoControllerList;
        }
    }
}
