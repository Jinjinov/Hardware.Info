using System;

namespace Hardware.Info
{
    public class VideoController
    {
        public string AdapterCompatibility;
        public string AdapterRAM;
        public string Caption;
        public string CurrentBitsPerPixel;
        public string CurrentHorizontalResolution;
        public string CurrentNumberOfColors;
        public string CurrentRefreshRate;
        public string CurrentVerticalResolution;
        public string Description;
        public string DriverDate;
        public string DriverVersion;
        public string MaxRefreshRate;
        public string MinRefreshRate;
        public string Name;
        public string VideoModeDescription;
        public string VideoProcessor;

        public override string ToString()
        {
            return
                "AdapterCompatibility: " + AdapterCompatibility + Environment.NewLine +
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
                "MaxRefreshRate: " + MaxRefreshRate + Environment.NewLine +
                "MinRefreshRate: " + MinRefreshRate + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "VideoModeDescription: " + VideoModeDescription + Environment.NewLine +
                "VideoProcessor: " + VideoProcessor + Environment.NewLine;
        }
    }
}
