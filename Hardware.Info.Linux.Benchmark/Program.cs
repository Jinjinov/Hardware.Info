using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Hardware.Info.Benchmark
{
    public class Benchmarks
    {
        readonly IHardwareInfo _hardwareInfo = new HardwareInfo();

        [Benchmark]
        public void RefreshOperatingSystem() => _hardwareInfo.RefreshOperatingSystem();
        [Benchmark]
        public void RefreshMemoryStatus() => _hardwareInfo.RefreshMemoryStatus();
        [Benchmark]
        public void RefreshBatteryList() => _hardwareInfo.RefreshBatteryList();
        [Benchmark]
        public void RefreshBIOSList() => _hardwareInfo.RefreshBIOSList();
        [Benchmark]
        public void RefreshCPUList() => _hardwareInfo.RefreshCPUList();
        [Benchmark]
        public void RefreshDriveList() => _hardwareInfo.RefreshDriveList();
        [Benchmark]
        public void RefreshKeyboardList() => _hardwareInfo.RefreshKeyboardList();
        [Benchmark]
        public void RefreshMemoryList() => _hardwareInfo.RefreshMemoryList();
        [Benchmark]
        public void RefreshMonitorList() => _hardwareInfo.RefreshMonitorList();
        [Benchmark]
        public void RefreshMotherboardList() => _hardwareInfo.RefreshMotherboardList();
        [Benchmark]
        public void RefreshMouseList() => _hardwareInfo.RefreshMouseList();
        [Benchmark]
        public void RefreshNetworkAdapterList() => _hardwareInfo.RefreshNetworkAdapterList();
        [Benchmark]
        public void RefreshPrinterList() => _hardwareInfo.RefreshPrinterList();
        [Benchmark]
        public void RefreshSoundDeviceList() => _hardwareInfo.RefreshSoundDeviceList();
        [Benchmark]
        public void RefreshVideoControllerList() => _hardwareInfo.RefreshVideoControllerList();
    }

    class Program
    {
        static void Main(string[] _)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
