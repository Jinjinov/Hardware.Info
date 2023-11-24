# Source

This fork of https://github.com/Jinjinov/Hardware.Info/

# Hardware.Info.Linux

Only Linux operating system is supported!

Battery, BIOS, CPU - processor, storage drive, keyboard, RAM - memory, monitor, motherboard, mouse, NIC - network adapter, printer, sound card - audio card, graphics card - video card. Hardware.Info is a .NET Standard 2.0 library and /dev, /proc, /sys on Linux.

## How to use:

1. Include NuGet package from https://www.nuget.org/packages/Hardware.Info.Linux

        <ItemGroup>
            <PackageReference Include="Hardware.Info.Linux" Version="100.0.0.0" />
        </ItemGroup>

2. Call `RefreshAll()` or one of the other `Refresh*()` methods:

        class Program
        {
            static readonly IHardwareInfo hardwareInfo = new HardwareInfo();

            static void Main(string[] _)
            {
                //hardwareInfo.RefreshOperatingSystem();
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

                Console.WriteLine(hardwareInfo.OperatingSystem);

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

-

## Settings

### Refresh methods settings:

In these two methods you can exclude some slow queries by setting the parameters to `false`:

```
RefreshCPUList(bool includePercentProcessorTime = true)

RefreshNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true)
```

Setting `includePercentProcessorTime` and `includeBytesPersec` to `false` will exclude the queries that:

- cause a 1 second delay every time they are called in Linux

Setting `includeNetworkAdapterConfiguration` to `false` has only a small impact on performance.

## Benchmarks

## Version history:

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