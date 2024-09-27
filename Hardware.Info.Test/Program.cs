using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hardware.Info.Test
{
    class Program
    {
        static int Main(string[] args)
        {
            bool testMode = TestSuite.HasFlag(args, "--enable-test-mode");
            bool noReadLine = TestSuite.HasFlag(args, "--no-readline");
            TestSuite.Compiler expectedCompiler = TestSuite.ReadOption(args, "--expected-compiler", TestSuite.Compiler.Unknown);
            TestSuite.Architecture expectedArch = TestSuite.ReadOption(args, "--expected-arch", TestSuite.Architecture.Unknown);
            
            if (testMode)
            {
                var isJit = RuntimeFeature.IsDynamicCodeCompiled;
                Console.WriteLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
                Console.WriteLine($"Operating System Architecture: {RuntimeInformation.OSArchitecture}");
                Console.WriteLine($"Compiler: {(isJit ? "JIT" : "AOT")}");
                Console.WriteLine($"Expected Compiler: {expectedCompiler}");
                Console.WriteLine($"Expected Architecture: {expectedArch}");

                if (expectedCompiler == TestSuite.Compiler.Jit && !isJit)
                {
                    Console.WriteLine($"Expected JIT compiler but was {expectedCompiler}");
                    return 1;
                }

                if (expectedCompiler == TestSuite.Compiler.Aot && isJit)
                {
                    Console.WriteLine($"Expected AOT compiler but was {expectedCompiler}");
                    return 1;
                }

                if (expectedArch == TestSuite.Architecture.x64 && RuntimeInformation.ProcessArchitecture !=
                    System.Runtime.InteropServices.Architecture.X64)
                {
                    Console.WriteLine($"Expected architecture: {expectedArch} but was {RuntimeInformation.ProcessArchitecture}");
                    return 1;
                }
                
                if (expectedArch == TestSuite.Architecture.arm64 && RuntimeInformation.ProcessArchitecture !=
                    System.Runtime.InteropServices.Architecture.Arm64)
                {
                    Console.WriteLine($"Expected architecture: {expectedArch} but was {RuntimeInformation.ProcessArchitecture}");
                    return 1;
                }
            }
            
            Test(true, noReadLine: noReadLine);

            Test(false, noReadLine: noReadLine);

            return 0;
        }
        
        static void Test(bool test, bool noReadLine)
        {
            IHardwareInfo hardwareInfo = new HardwareInfo(useAsteriskInWMI: test);

            hardwareInfo.RefreshOperatingSystem();
            hardwareInfo.RefreshMemoryStatus();
            hardwareInfo.RefreshBatteryList();
            hardwareInfo.RefreshBIOSList();
            hardwareInfo.RefreshComputerSystemList();
            hardwareInfo.RefreshCPUList(includePercentProcessorTime: test);
            hardwareInfo.RefreshDriveList();
            hardwareInfo.RefreshKeyboardList();
            hardwareInfo.RefreshMemoryList();
            hardwareInfo.RefreshMonitorList();
            hardwareInfo.RefreshMotherboardList();
            hardwareInfo.RefreshMouseList();
            hardwareInfo.RefreshNetworkAdapterList(includeBytesPerSec: test, includeNetworkAdapterConfiguration: test);
            hardwareInfo.RefreshPrinterList();
            hardwareInfo.RefreshSoundDeviceList();
            hardwareInfo.RefreshVideoControllerList();

            //hardwareInfo.RefreshAll();

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

            if (!noReadLine)
            {
                return;
            }
            
            Console.ReadLine();
        }
    }
}
