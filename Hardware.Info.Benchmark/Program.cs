using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Hardware.Info.Benchmark
{
    public class Benchmarks
    {
        readonly HardwareInfo hardwareInfo = new HardwareInfo();

        [Benchmark]
        public void RefreshMemoryStatus() => hardwareInfo.RefreshMemoryStatus();
        [Benchmark]
        public void RefreshBatteryList() => hardwareInfo.RefreshBatteryList();
        [Benchmark]
        public void RefreshBIOSList() => hardwareInfo.RefreshBIOSList();
        [Benchmark]
        public void RefreshCPUList() => hardwareInfo.RefreshCPUList();
        [Benchmark]
        public void RefreshDriveList() => hardwareInfo.RefreshDriveList();
        [Benchmark]
        public void RefreshKeyboardList() => hardwareInfo.RefreshKeyboardList();
        [Benchmark]
        public void RefreshMemoryList() => hardwareInfo.RefreshMemoryList();
        [Benchmark]
        public void RefreshMonitorList() => hardwareInfo.RefreshMonitorList();
        [Benchmark]
        public void RefreshMotherboardList() => hardwareInfo.RefreshMotherboardList();
        [Benchmark]
        public void RefreshMouseList() => hardwareInfo.RefreshMouseList();
        [Benchmark]
        public void RefreshNetworkAdapterList() => hardwareInfo.RefreshNetworkAdapterList();
        [Benchmark]
        public void RefreshPrinterList() => hardwareInfo.RefreshPrinterList();
        [Benchmark]
        public void RefreshSoundDeviceList() => hardwareInfo.RefreshSoundDeviceList();
        [Benchmark]
        public void RefreshVideoControllerList() => hardwareInfo.RefreshVideoControllerList();
    }

    class Program
    {
        static void Main(string[] _)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
