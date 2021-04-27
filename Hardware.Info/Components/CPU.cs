using System;
using System.Collections.Generic;

namespace Hardware.Info
{
    public class CPU
    {
        public string Caption { get; internal set; } = string.Empty;
        public UInt32 CurrentClockSpeed { get; internal set; }
        public string Description { get; internal set; } = string.Empty;
        public UInt32 L2CacheSize { get; internal set; }
        public UInt32 L3CacheSize { get; internal set; }
        public string Manufacturer { get; internal set; } = string.Empty;
        public UInt32 MaxClockSpeed { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public UInt32 NumberOfCores { get; internal set; }
        public UInt32 NumberOfLogicalProcessors { get; internal set; }
        public string ProcessorId { get; internal set; } = string.Empty;
        public Boolean VirtualizationFirmwareEnabled { get; internal set; }
        public Boolean VMMonitorModeExtensions { get; internal set; }
        public UInt64 PercentProcessorTime { get; internal set; }
        public List<CpuCore> CpuCoreList { get; internal set; } = new List<CpuCore>();

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "CurrentClockSpeed: " + CurrentClockSpeed + Environment.NewLine +
                "PercentProcessorTime: " + PercentProcessorTime + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "L2CacheSize: " + L2CacheSize + Environment.NewLine +
                "L3CacheSize: " + L3CacheSize + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "MaxClockSpeed: " + MaxClockSpeed + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "NumberOfCores: " + NumberOfCores + Environment.NewLine +
                "NumberOfLogicalProcessors: " + NumberOfLogicalProcessors + Environment.NewLine +
                "ProcessorId: " + ProcessorId + Environment.NewLine +
                "VirtualizationFirmwareEnabled: " + VirtualizationFirmwareEnabled + Environment.NewLine +
                "VMMonitorModeExtensions: " + VMMonitorModeExtensions + Environment.NewLine;
        }
    }
}
