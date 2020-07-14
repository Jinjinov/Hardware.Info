using System;

namespace Hardware.Info
{
    public class Monitor
    {
        public string Caption;
        public string Description;
        public string MonitorManufacturer;
        public string MonitorType;
        public string Name;
        public string PixelsPerXLogicalInch;
        public string PixelsPerYLogicalInch;

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
