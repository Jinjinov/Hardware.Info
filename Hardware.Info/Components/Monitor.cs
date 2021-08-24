using System;

namespace Hardware.Info
{
    public class Monitor
    {
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MonitorManufacturer { get; set; } = string.Empty;
        public string MonitorType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UInt32 PixelsPerXLogicalInch { get; set; }
        public UInt32 PixelsPerYLogicalInch { get; set; }

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
