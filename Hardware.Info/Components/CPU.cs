using System;

namespace Hardware.Info
{
    public class CPU
    {
        public string Caption;
        public string CurrentClockSpeed;
        public string Description;
        public string L2CacheSize;
        public string L3CacheSize;
        public string Manufacturer;
        public string MaxClockSpeed;
        public string Name;
        public string NumberOfCores;
        public string NumberOfLogicalProcessors;
        public string ProcessorId;
        public string VirtualizationFirmwareEnabled;
        public string VMMonitorModeExtensions;

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "CurrentClockSpeed: " + CurrentClockSpeed + Environment.NewLine +
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
