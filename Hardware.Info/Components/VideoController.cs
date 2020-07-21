using System;

namespace Hardware.Info
{
    public class VideoController
    {
        public UInt32 AdapterRAM { get; internal set; }
        public string Caption { get; internal set; } = string.Empty;
        public UInt32 CurrentBitsPerPixel { get; internal set; }
        public UInt32 CurrentHorizontalResolution { get; internal set; }
        public UInt64 CurrentNumberOfColors { get; internal set; }
        public UInt32 CurrentRefreshRate { get; internal set; }
        public UInt32 CurrentVerticalResolution { get; internal set; }
        public string Description { get; internal set; } = string.Empty;
        public string DriverDate { get; internal set; } = string.Empty;
        public string DriverVersion { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public UInt32 MaxRefreshRate { get; internal set; }
        public UInt32 MinRefreshRate { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public string VideoModeDescription { get; internal set; } = string.Empty;
        public string VideoProcessor { get; internal set; } = string.Empty;

        public override string ToString()
        {
            return
                "AdapterRAM: " + AdapterRAM + Environment.NewLine +
                "Caption: " + Caption + Environment.NewLine +
                "CurrentBitsPerPixel: " + CurrentBitsPerPixel + Environment.NewLine +
                "CurrentHorizontalResolution: " + CurrentHorizontalResolution + Environment.NewLine +
                "CurrentNumberOfColors: " + CurrentNumberOfColors + Environment.NewLine +
                "CurrentRefreshRate: " + CurrentRefreshRate + Environment.NewLine +
                "CurrentVerticalResolution: " + CurrentVerticalResolution + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "DriverDate: " + DriverDate + Environment.NewLine +
                "DriverVersion: " + DriverVersion + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "MaxRefreshRate: " + MaxRefreshRate + Environment.NewLine +
                "MinRefreshRate: " + MinRefreshRate + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "VideoModeDescription: " + VideoModeDescription + Environment.NewLine +
                "VideoProcessor: " + VideoProcessor + Environment.NewLine;
        }
    }
}
