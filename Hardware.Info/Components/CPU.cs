using System;
using System.Collections.Generic;

namespace Hardware.Info
{
    public class CPU
    {
        public string Caption { get; set; } = string.Empty;
        public UInt32 CurrentClockSpeed { get; set; }
        public string Description { get; set; } = string.Empty;
        public UInt32 L2CacheSize { get; set; }
        public UInt32 L3CacheSize { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public UInt32 MaxClockSpeed { get; set; }
        public string Name { get; set; } = string.Empty;
        public UInt32 NumberOfCores { get; set; }
        public UInt32 NumberOfLogicalProcessors { get; set; }
        public string ProcessorId { get; set; } = string.Empty;
        public Boolean SecondLevelAddressTranslationExtensions { get; set; }
        public string SocketDesignation { get; set; } = string.Empty;
        public Boolean VirtualizationFirmwareEnabled { get; set; }
        public Boolean VMMonitorModeExtensions { get; set; }
        public UInt64 PercentProcessorTime { get; set; }
        public List<CpuCore> CpuCoreList { get; set; } = new List<CpuCore>();

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
                "SLAT: " + SecondLevelAddressTranslationExtensions + Environment.NewLine +
                "SocketDesignation: " + SocketDesignation + Environment.NewLine +
                "VirtualizationFirmwareEnabled: " + VirtualizationFirmwareEnabled + Environment.NewLine +
                "VMMonitorModeExtensions: " + VMMonitorModeExtensions + Environment.NewLine;
        }
    }
}
