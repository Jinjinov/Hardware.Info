# Hardware.Info

Battery, BIOS, CPU - processor, storage drive, keyboard, RAM - memory, monitor, motherboard, mouse, NIC - network adapter, printer, sound card - audio card, graphics card - video card. Hardware.Info is a .NET Standard 2.0 library and uses WMI on Windows, /dev, /proc, /sys on Linux and sysctl, system_profiler on macOS.

## How to use:

1. Include NuGet package from https://www.nuget.org/packages/Hardware.Info

        <ItemGroup>
            <PackageReference Include="Hardware.Info" Version="1.1.1.0" />
        </ItemGroup>

2. Call `RefreshAll()` or one of the other `Refresh*()` methods:

        class Program
        {
            static readonly IHardwareInfo hardwareInfo = new HardwareInfo();

            static void Main(string[] _)
            {
                //hardwareInfo.RefreshMemoryStatus();
                //hardwareInfo.RefreshBatteryList();
                //hardwareInfo.RefreshBIOSList();
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

                Console.WriteLine(hardwareInfo.MemoryStatus);

                foreach (var hardware in hardwareInfo.BatteryList)
                    Console.WriteLine(hardware);

                foreach (var hardware in hardwareInfo.BiosList)
                    Console.WriteLine(hardware);

                foreach (var cpu in hardwareInfo.CpuList)
                {
                    Console.WriteLine(cpu);

                    foreach (var cpuCore in cpu.CpuCoreList)
                        Console.WriteLine(cpuCore);
                }

                Console.ReadLine();

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

                Console.ReadLine();

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

                Console.ReadLine();

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

## Settings

### Constructor settings:

```
HardwareInfo(bool useAsteriskInWMI = true, TimeSpan? timeoutInWMI = null)
```

The construcotr accepts two settings for WMI:
- `useAsteriskInWMI` causes WMI queries to use `SELECT * FROM` instead of `SELECT` with a list of property names. This is slower, but safer, more compatible with older Windows (XP, Vista, 7, 8) where a certain WMI property might be missing and throw an exception when queried by name. The default value is `true`.
- `timeoutInWMI` sets the `Timeout` property of the `EnumerationOptions` in the `ManagementObjectSearcher` that executes the query. The default value is `EnumerationOptions.InfiniteTimeout`. Changing this could cause the query to return empty results in certain cases.

### Refresh methods settings:

In these two methods you can exclude some slow queries by setting the parameters to `false`:

```
RefreshCPUList(bool includePercentProcessorTime = true)

RefreshNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
```

Setting `includePercentProcessorTime` and `includeBytesPersec` to `false` will exclude the queries that:
- cause a 21 second delay the first time they are called in Windows
- cause a 1 second delay every time they are called in Linux

Setting `includeNetworkAdapterConfiguration` to `false` has only a small impact on performance.

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

## Version history:

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