using System;

namespace Hardware.Info
{
    public class Monitor
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string MonitorManufacturer { get; internal set; } = string.Empty;
        public string MonitorType { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public UInt32 PixelsPerXLogicalInch { get; internal set; }
        public UInt32 PixelsPerYLogicalInch { get; internal set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "MonitorManufacturer: " + MonitorManufacturer + Environment.NewLine +
                "MonitorType: " + MonitorType + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "PixelsPerXLogicalInch: " + PixelsPerXLogicalInch + Environment.NewLine +
                "PixelsPerYLogicalInch: " + PixelsPerYLogicalInch + Environment.NewLine;
        }
    }
}
