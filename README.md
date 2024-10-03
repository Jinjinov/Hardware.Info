# Hardware.Info

Battery, BIOS, CPU - processor, storage drive, keyboard, RAM - memory, monitor, motherboard, mouse, NIC - network adapter, printer, sound card - audio card, graphics card - video card. Hardware.Info is a .NET Standard 2.0 library and uses WMI on Windows, /dev, /proc, /sys on Linux and sysctl, system_profiler on macOS.

## How to use:

1. Include NuGet package from https://www.nuget.org/packages/Hardware.Info

        <ItemGroup>
            <PackageReference Include="Hardware.Info" Version="101.0.0.0" />
        </ItemGroup>

2. Call `RefreshAll()` or one of the other `Refresh*()` methods:

        class Program
        {
            static IHardwareInfo hardwareInfo;

            static void Main(string[] _)
            {
                try
                {
                    hardwareInfo = new HardwareInfo();

                    //hardwareInfo.RefreshOperatingSystem();
                    //hardwareInfo.RefreshMemoryStatus();
                    //hardwareInfo.RefreshBatteryList();
                    //hardwareInfo.RefreshBIOSList();
                    //hardwareInfo.RefreshComputerSystemList();
                    //hardwareInfo.RefreshCPUList();
                    //hardwareInfo.RefreshDriveList();
                    //hardwareInfo.RefreshKeyboardList();
                    //hardwareInfo.RefreshMemoryList();
                    //hardwareInfo.RefreshMonitorList();
                    //hardwareInfo.RefreshMotherboardList();
                    //hardwareInfo.RefreshMouseList();
                    //hardwareInfo.RefreshNetworkAdapterList();
                    //hardwareInfo.RefreshPrinterList();
                    //hardwareInfo.RefreshSoundDeviceList();
                    //hardwareInfo.RefreshVideoControllerList();

                    hardwareInfo.RefreshAll();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine(hardwareInfo.OperatingSystem);

                Console.WriteLine(hardwareInfo.MemoryStatus);

                foreach (var hardware in hardwareInfo.BatteryList)
                    Console.WriteLine(hardware);

                foreach (var hardware in hardwareInfo.BiosList)
                    Console.WriteLine(hardware);

                foreach (var hardware in hardwareInfo.ComputerSystemList)
                    Console.WriteLine(hardware);

                foreach (var cpu in hardwareInfo.CpuList)
                {
                    Console.WriteLine(cpu);

                    foreach (var cpuCore in cpu.CpuCoreList)
                        Console.WriteLine(cpuCore);
                }

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

                foreach (var address in HardwareInfo.GetLocalIPv4Addresses(NetworkInterfaceType.Ethernet, OperationalStatus.Up))
                    Console.WriteLine(address);

                Console.WriteLine();

                foreach (var address in HardwareInfo.GetLocalIPv4Addresses(NetworkInterfaceType.Wireless80211))
                    Console.WriteLine(address);

                Console.WriteLine();

                foreach (var address in HardwareInfo.GetLocalIPv4Addresses(OperationalStatus.Up))
                    Console.WriteLine(address);

                Console.WriteLine();

                foreach (var address in HardwareInfo.GetLocalIPv4Addresses())
                    Console.WriteLine(address);

                Console.ReadLine();
            }
        }

## Known issues

### 21 second delay on first use in Windows

Hardware.Info uses WMI (Windows Management Instrumentation) on Windows OS. For certain queries WMI takes 21 seconds to initialize the first time you use it, after that all subsequent queries will execute immediately. If WMI isn't used for 15 minutes it will have to be initialized again the next time you use it.

The 21 second initialization delay is caused by RPC that WMI uses internally. In RPC [documentation](https://docs.microsoft.com/en-us/windows/win32/services/services-and-rpc-tcp) it says that the RPC/TCP time-out interval is defined with a `SCMApiConnectionParam` registry value located at `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control` and that the default value is set to 21,000 (21 seconds).

You can avoid the 21 second delay by excluding the queries that cause it (see Settings).

### Invalid `NetworkAdapter.Speed` in Windows

Sometimes `NetworkAdapter.Speed` in `Win32_NetworkAdapter` can be `0` or `long.MaxValue`. The correct value can be retrived from `CurrentBandwidth` in `Win32_PerfFormattedData_Tcpip_NetworkAdapter` but unfortunately reading from `Win32_PerfFormattedData_Tcpip_NetworkAdapter` causes a 21 second delay on the first read, like mentioned in the previous paragraph. Calling `RefreshNetworkAdapterList` with `includeBytesPersec = true` will also read the `CurrentBandwidth`.

### `WmiNetUtilsHelper` will throw an exception in Windows if publish settings use `<PublishTrimmed>true</PublishTrimmed>`

This is a known error: https://github.com/dotnet/core/issues/7051#issuecomment-1071484354

## Settings

### Constructor settings:

```
HardwareInfo(bool useAsteriskInWMI = true, TimeSpan? timeoutInWMI = null)
```

The construcotr accepts two settings for WMI:
- `useAsteriskInWMI` causes WMI queries to use `SELECT * FROM` instead of `SELECT` with a list of property names. This is slower, but safer, more compatible with older Windows (XP, Vista, 7, 8) where a certain WMI property might be missing and throw an exception when queried by name. The default value is `true`.
- `timeoutInWMI` sets the `Timeout` property of the `EnumerationOptions` in the `ManagementObjectSearcher` that executes each query. The default value is `EnumerationOptions.InfiniteTimeout`. There are one or more queries for each hardware component, so there are more than 16 queries executed on `RefreshAll()`. If a query reaches the timeout it will throw a `System.Management.ManagementException` exception where `ErrorCode` will be `System.Management.ManagementStatus.Timedout`. If you set the `timeoutInWMI` then use a `try-catch` block like this:

        IHardwareInfo hardwareInfo;

        try
        {
            hardwareInfo = new HardwareInfo(timeoutInWMI: TimeSpan.FromMilliseconds(100));

            hardwareInfo.RefreshAll();
        }
        catch (ManagementException ex) when (ex.ErrorCode == ManagementStatus.Timedout)
        {
            Console.WriteLine(ex);
        }

### Refresh methods settings:

```
RefreshCPUList(
    bool includePercentProcessorTime = true, 
    int millisecondsDelayBetweenTwoMeasurements = 500)

RefreshNetworkAdapterList(
    bool includeBytesPersec = true, 
    bool includeNetworkAdapterConfiguration = true, 
    int millisecondsDelayBetweenTwoMeasurements = 1000)
```

Setting `includePercentProcessorTime` and `includeBytesPersec` to `false` will exclude the queries that:
- cause a 21 second delay the first time they are called in Windows
- cause a 1 second delay every time they are called in Linux

Setting `includeNetworkAdapterConfiguration` to `false` has only a small impact on performance.

Delay in milliseconds between two measurements in Linux:

For `PercentProcessorTime` in Linux:
```
string[] cpuUsageLineLast = TryReadLinesFromFile("/proc/stat");
Task.Delay(millisecondsDelayBetweenTwoMeasurements).Wait();
string[] cpuUsageLineNow = TryReadLinesFromFile("/proc/stat");
```
If `includePercentProcessorTime` is false, `millisecondsDelayBetweenTwoMeasurements` has no effect.

For `BytesSentPersec` and `BytesReceivedPersec` in Linux:
```
string[] procNetDevLast = TryReadLinesFromFile("/proc/net/dev");
Task.Delay(millisecondsDelayBetweenTwoMeasurements).Wait();
string[] procNetDevNow = TryReadLinesFromFile("/proc/net/dev");
```
If `includeBytesPersec` is false, `millisecondsDelayBetweenTwoMeasurements` has no effect.

## Benchmarks

### Windows 8.1 (Intel i5-2500, 8 GB RAM):

|                     Method |               Mean |            Error |           StdDev |
|--------------------------- |-------------------:|-----------------:|-----------------:|
|        RefreshMemoryStatus |           947.8 ns |          3.77 ns |          3.53 ns |
|         RefreshBatteryList |     1,811,885.7 ns |     12,921.05 ns |     11,454.17 ns |
|            RefreshBIOSList |     2,086,001.0 ns |     23,896.69 ns |     22,352.98 ns |
|             RefreshCPUList | 1,543,579,005.2 ns |  2,405,376.47 ns |  2,132,303.59 ns |
|           RefreshDriveList |   409,137,516.3 ns |  8,612,410.99 ns | 25,258,710.57 ns |
|        RefreshKeyboardList |     5,568,039.5 ns |     44,228.57 ns |     41,371.43 ns |
|          RefreshMemoryList |     2,120,024.5 ns |     26,103.39 ns |     24,417.13 ns |
|         RefreshMonitorList |     5,669,237.8 ns |     50,801.76 ns |     45,034.44 ns |
|     RefreshMotherboardList |     1,965,222.9 ns |     14,387.30 ns |     13,457.89 ns |
|           RefreshMouseList |     6,003,924.9 ns |     60,725.05 ns |     50,708.17 ns |
|  RefreshNetworkAdapterList | 1,412,244,738.6 ns | 14,681,615.28 ns | 12,259,813.69 ns |
|         RefreshPrinterList |    28,244,822.2 ns |    143,359.60 ns |    134,098.66 ns |
|     RefreshSoundDeviceList |     3,608,577.5 ns |     68,688.62 ns |     73,496.06 ns |
| RefreshVideoControllerList |    11,568,549.2 ns |     54,666.07 ns |     48,460.05 ns |

### Windows 10 (AMD Ryzen 5 5600G, 32 GB RAM):

|                     Method |                 Mean |               Error |              StdDev |
|--------------------------- |---------------------:|--------------------:|--------------------:|
|     RefreshOperatingSystem |             2.946 ns |           0.0052 ns |           0.0047 ns |
|        RefreshMemoryStatus |           460.552 ns |           4.4810 ns |           3.9723 ns |
|         RefreshBatteryList |     1,624,392.057 ns |      22,526.9314 ns |      21,071.7057 ns |
|            RefreshBIOSList |     1,785,673.828 ns |       8,812.8115 ns |       8,243.5094 ns |
|             RefreshCPUList | 1,964,995,539.000 ns | 171,465,934.5051 ns | 505,571,176.5574 ns |
|           RefreshDriveList |    62,452,668.148 ns |     342,662.0413 ns |     320,526.2860 ns |
|        RefreshKeyboardList |     4,303,528.516 ns |      47,355.1733 ns |      41,979.1277 ns |
|          RefreshMemoryList |     1,926,931.367 ns |      19,754.4179 ns |      18,478.2948 ns |
|         RefreshMonitorList |     3,884,362.370 ns |      29,422.1438 ns |      27,521.4916 ns |
|     RefreshMotherboardList |     1,782,235.664 ns |      12,974.2296 ns |      12,136.1024 ns |
|           RefreshMouseList |     4,700,086.615 ns |      44,435.0631 ns |      41,564.5856 ns |
|  RefreshNetworkAdapterList |   945,004,493.333 ns |   8,568,978.4607 ns |   8,015,427.7687 ns |
|         RefreshPrinterList |    48,126,103.030 ns |     729,958.0933 ns |     682,803.2534 ns |
|     RefreshSoundDeviceList |     4,154,082.924 ns |      46,922.5501 ns |      41,595.6184 ns |
| RefreshVideoControllerList |     8,784,372.500 ns |     125,080.5212 ns |     117,000.3971 ns |

## Version history:

- 101.0.0.0
    - Fixed `GetCpuList` in Linux - thanks to [@inelisoni](https://github.com/inelisoni)
    - Added `int millisecondsDelayBetweenTwoMeasurements` to `GetCpuList`
    - Added `int millisecondsDelayBetweenTwoMeasurements` to `GetNetworkAdapterList`
- 100.1.1.1
    - Fixed `GetNetworkAdapterList` in Linux - thanks to [@Pregath0r](https://github.com/Pregath0r)
- 100.1.1.0
    - Added `ComputerSystem` info in Windows, macOS, Linux - thanks to [@Zagrthos](https://github.com/Zagrthos)
- 100.1.0.1
    - Fixed `GetVideoControllerList` in Linux - thanks to [@NogginBops](https://github.com/NogginBops)
- 100.1.0.0
    - Fixed `GetDriveList` in Linux - thanks to [@GusanoGris](https://github.com/GusanoGris)
    - Added `Microsoft.SourceLink.GitHub` - by [@andreas-eriksson](https://github.com/andreas-eriksson)
- 100.0.1.1
    - Added XML documentation - thanks to [@andreas-eriksson](https://github.com/andreas-eriksson)
- 100.0.1.0
    - Added `Disk.Description` in Linux
    - Added `Disk.FirmwareRevision` in Linux
    - Added `Disk.Name` in Linux
    - Added `Disk.SerialNumber` in Linux
    - Added `Disk.Size` in Linux
- 100.0.0.1
    - Added `HardwareInfo.snk` to sign the assembly with a strong name key
- 100.0.0.0
    - Fixed `GetCpuList` in Linux - thanks to [@inelisoni](https://github.com/inelisoni)
- 11.1.1.1
    - Fixed `GetMonitorList` in Windows - by [@Geevo](https://github.com/Geevo)
- 11.1.1.0
    - Fixed `GetMonitorList` in Windows - by [@Geevo](https://github.com/Geevo)
- 11.1.0.1
    - Fixed `GetNetworkAdapterList` in Linux - thanks to [@Nihlus](https://github.com/Nihlus)
    - Fixed `GetCpuList` in Windows - by [@Frooxius](https://github.com/Frooxius)
- 11.1.0.0
    - Fixed `NetworkAdapter.Speed` in Windows - by [@isenmann](https://github.com/isenmann)
- 11.0.1.1
    - Added `Keyboard` info in Linux
    - Added `Mouse` info in Linux
    - Added `SoundDevice` info in Linux
    - Added `VideoController.CurrentHorizontalResolution` in Linux
    - Added `VideoController.CurrentVerticalResolution` in Linux
    - Added `VideoController.CurrentRefreshRate` in Linux
- 11.0.1.0
    - Fixed `VideoController.AdapterRAM` in Windows - by [@jesperll](https://github.com/jesperll)
- 11.0.0.1
    - Added `L1DataCacheSize` and `L1InstructionCacheSize` in Windows, macOS, Linux
    - Fixed `L2CacheSize` and `L3CacheSize` in Windows, Linux
- 11.0.0.0
    - Fixed `GetNetworkAdapterList` in Windows - thanks to [@isenmann](https://github.com/isenmann)
- 10.1.1.1:
    - Fixed `CurrentClockSpeed` in Windows - thanks to [@jason-c-daniels](https://github.com/jason-c-daniels)
- 10.1.1.0:
    - Fixed `GetCpuUsage` in Linux - thanks to [@glebov21](https://github.com/glebov21)
- 10.1.0.1:
    - Fixed `CPU.Name` and `CPU.CurrentClockSpeed` in macOS - by [@davidaramant](https://github.com/davidaramant)
    - Added `CPU.MaxClockSpeed` in macOS - by [@davidaramant](https://github.com/davidaramant)
- 10.1.0.0:
    - Fixed `PercentProcessorTime` in Windows - thanks to [@C6OI](https://github.com/C6OI)
- 10.0.1.1:
    - Added `GetOperatingSystem()` in Windows, macOS, Linux - thanks to [@adhip94](https://github.com/adhip94)
- 10.0.1.0:
    - Added `GetBatteryList()` in macOS - by [@Tadelsucht](https://github.com/Tadelsucht)
- 10.0.0.1:
    - Fixed `GetBatteryList()` in Linux - by [@Tadelsucht](https://github.com/Tadelsucht)
- 10.0.0.0:
    - Fixed `GetDriveList()` and `GetMemoryList()` in Linux - thanks to [@misaka00251](https://github.com/misaka00251)
- 1.1.1.1:
    - Added `Memory.BankLabel`, `Memory.MinVoltage`, `Memory.MaxVoltage` in Windows - by [@AathifMahir](https://github.com/AathifMahir)
    - Added `CPU.SocketDesignation`, `CPU.SecondLevelAddressTranslationExtensions` in Windows - by [@AathifMahir](https://github.com/AathifMahir)
    - Added Windows version check for WMI properties that require at least Windows 8
    - Added Windows version check for WMI properties that require at least Windows 10
    - Added XML summary for public properties in every class.
- 1.1.1.0:
    - Added `IHardwareInfo` so that `HardwareInfo` can be mocked - by [@240026763](https://github.com/240026763)
- 1.1.0.1:
    - Added two settings for WMI queries in Windows
    - Added three settings to exclude slow queries in Windows, macOS, Linux
- 1.1.0.0:
    - Fixed reading `MemAvailable` instead of `MemFree` in Linux - by [@schotime](https://github.com/schotime)
- 1.0.1.1:
    - Added `CpuCore` info in Linux - by [@isenmann](https://github.com/isenmann)
    - Added `CPU.PercentProcessorTime`, `CPU.CpuCoreList` in Linux - by [@isenmann](https://github.com/isenmann)
- 1.0.1.0:
    - Added `CpuCore` info in Windows - by [@isenmann](https://github.com/isenmann)
    - Added `CPU.PercentProcessorTime`, `CPU.CpuCoreList` in Windows - by [@isenmann](https://github.com/isenmann)
    - Added `NetworkAdapter.BytesSentPersec`, `NetworkAdapter.BytesReceivedPersec` in Windows - by [@isenmann](https://github.com/isenmann)
- 1.0.0.1:
    - Added `Battery.EstimatedChargeRemaining` in Windows, Linux - by [@reptail](https://github.com/reptail)
- 1.0.0.0:
    - Added `Battery.ExpectedLife` in Linux
    - Added `Battery.EstimatedRunTime` in Linux
    - Added `Battery.MaxRechargeTime` in Linux
    - Added `Battery.TimeToFullCharge` in Linux
- 0.1.1.1:
    - Added `Battery.DesignCapacity` in Linux
    - Added `Battery.FullChargeCapacity` in Linux
- 0.1.1.0:
    - Added `Battery.BatteryStatusDescription` in Linux
- 0.1.0.1:
    - Added `Monitor` info in macOS
    - Added `VideoController` info in macOS
- 0.1.0.0:
    - Added `CPU.L2CacheSize` in macOS
    - Added `CPU.L3CacheSize` in macOS
    - Added `Memory` info in macOS
- 0.0.1.1:
    - Added `BIOS.ReleaseDate` in Linux
    - Added `CPU.Manufacturer` in Linux
    - Added `CPU.L3CacheSize` in Linux
    - Added `Motherboard.SerialNumber` in Linux
    - Fixed `NetworkAdapter` info in Linux
    - Fixed `GetLocalIPv4Addresses()` in macOS
- 0.0.1.0:
    - Added `GetLocalIPv4Addresses()` in Windows, macOS, Linux
    - Added `Motherboard.SerialNumber` in Windows
    - Added `Drive`, `NetworkAdapter` info in macOS, Linux
- 0.0.0.1:
    - All hardware info in Windows
    - CPU, RAM info in macOS, Linux