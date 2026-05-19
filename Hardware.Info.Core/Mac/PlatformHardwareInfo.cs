using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
            string processOutput = ReadProcessOutput("ioreg", "-l -w0 | grep ' \\\"DesignCapacity\\\" ' | grep -Eo \"\\d+\"");

            if (uint.TryParse(processOutput, out uint designCapacity))
                battery.DesignCapacity = designCapacity;

            processOutput = ReadProcessOutput("ioreg", "-l -w0 | grep ' \\\"MaxCapacity\\\" ' | grep -Eo \"\\d+\"");

            if (uint.TryParse(processOutput, out uint fullChargeCapacity))
                battery.FullChargeCapacity = fullChargeCapacity;
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
                        if (line.StartsWith("Resolution: "))
                        {
                            string[] resSplit = line.Replace("Resolution: ", string.Empty).Split(' ');

                            if (resSplit.Length >= 3)
                            {
                                if (uint.TryParse(resSplit[0].Trim(), out uint x))
                                    monitor.CurrentHorizontalResolution = x;

                                if (uint.TryParse(resSplit[2].Trim(), out uint y))
                                    monitor.CurrentVerticalResolution = y;
                            }
                        }
                        else if (line.StartsWith("UI Looks like: "))
                        {
                            string[] hzSplit = line.Split('@');

                            if (hzSplit.Length == 2)
                            {
                                if (double.TryParse(hzSplit[1].Replace("Hz", string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double hz))
                                    monitor.CurrentRefreshRate = (uint)Math.Round(hz);
                            }
                        }
                        else if (line.StartsWith("Display Type: "))
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

        public List<VideoController> GetVideoControllerList(bool refreshMonitorList = true)
        {
            List<VideoController> videoControllerList = new List<VideoController>();

            VideoController? videoController = null;
            Monitor? monitor = null;
            bool inDisplaysSection = false;

            // https://stackoverflow.com/questions/18077639/getting-graphic-card-information-in-objective-c

            // https://developer.apple.com/documentation/iokit/iographicslib_h

            StartProcess("system_profiler", "SPDisplaysDataType",
                standardOutput =>
                {
                    int spaceCount = standardOutput.TakeWhile(c => c == ' ').Count();

                    string line = standardOutput.Trim();

                    if (spaceCount == 4)
                    {
                        if (refreshMonitorList && monitor != null && videoController != null)
                        {
                            videoController.MonitorList.Add(monitor);
                            monitor = null;
                        }
                        inDisplaysSection = false;

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

                    if (refreshMonitorList && videoController != null)
                    {
                        if (spaceCount == 6 && line == "Displays:")
                        {
                            inDisplaysSection = true;
                        }
                        else if (spaceCount == 6 && inDisplaysSection)
                        {
                            if (monitor != null)
                                videoController.MonitorList.Add(monitor);
                            monitor = null;
                            inDisplaysSection = false;
                        }
                        else if (spaceCount == 8 && inDisplaysSection)
                        {
                            if (monitor != null)
                                videoController.MonitorList.Add(monitor);
                            string monitorName = line.TrimEnd(':');
                            monitor = new Monitor
                            {
                                Caption = monitorName,
                                Description = monitorName,
                                Name = monitorName,
                                UserFriendlyName = monitorName
                            };
                        }
                        else if (spaceCount == 10 && monitor != null)
                        {
                            if (line.StartsWith("Resolution: "))
                            {
                                string[] split = line.Replace("Resolution: ", string.Empty).Split(' ');

                                if (split.Length >= 3)
                                {
                                    if (uint.TryParse(split[0].Trim(), out uint x))
                                        monitor.CurrentHorizontalResolution = x;

                                    if (uint.TryParse(split[2].Trim(), out uint y))
                                        monitor.CurrentVerticalResolution = y;
                                }
                            }
                            else if (line.StartsWith("UI Looks like: "))
                            {
                                string[] split = line.Split('@');

                                if (split.Length == 2)
                                {
                                    if (double.TryParse(split[1].Replace("Hz", string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double hz))
                                        monitor.CurrentRefreshRate = (uint)Math.Round(hz);
                                }
                            }
                            else if (line.StartsWith("Display Type: "))
                                monitor.MonitorType = line.Replace("Display Type: ", string.Empty);
                            else if (line.StartsWith("Connection Type: "))
                                monitor.MonitorType = line.Replace("Connection Type: ", string.Empty);
                            else if (line.StartsWith("Display Serial Number: "))
                                monitor.SerialNumberID = line.Replace("Display Serial Number: ", string.Empty);
                            else if (line == "Main Display: Yes")
                                monitor.Active = true;
                        }
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
                                if (double.TryParse(split[1].Replace("Hz", string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double Hz))
                                    videoController.CurrentRefreshRate = (uint)Math.Round(Hz);
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

            if (refreshMonitorList && monitor != null && videoController != null)
                videoController.MonitorList.Add(monitor);

            if (videoController != null)
                videoControllerList.Add(videoController);

            return videoControllerList;
        }
    }
}
