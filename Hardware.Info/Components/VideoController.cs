using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-videocontroller

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_VideoController
    /// </summary>
    public class VideoController
    {
        /// <summary>
        /// Memory size of the video adapter.
        /// </summary>
        public UInt64 AdapterRAM { get; set; }

        /// <summary>
        /// Short description of the object.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Number of bits used to display each pixel.
        /// </summary>
        public UInt32 CurrentBitsPerPixel { get; set; }

        /// <summary>
        /// Current number of horizontal pixels.
        /// </summary>
        public UInt32 CurrentHorizontalResolution { get; set; }

        /// <summary>
        /// Number of colors supported at the current resolution.
        /// </summary>
        public UInt64 CurrentNumberOfColors { get; set; }

        /// <summary>
        /// Frequency at which the video controller refreshes the image for the monitor. 
        /// A value of 0 (zero) indicates the default rate is being used, while 0xFFFFFFFF indicates the optimal rate is being used.
        /// </summary>
        public UInt32 CurrentRefreshRate { get; set; }

        /// <summary>
        /// Current number of vertical pixels.
        /// </summary>
        public UInt32 CurrentVerticalResolution { get; set; }

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Last modification date and time of the currently installed video driver.
        /// </summary>
        public string DriverDate { get; set; } = string.Empty;

        /// <summary>
        /// Version number of the video driver.
        /// </summary>
        public string DriverVersion { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer of the video controller.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Maximum refresh rate of the video controller in hertz.
        /// </summary>
        public UInt32 MaxRefreshRate { get; set; }

        /// <summary>
        /// Minimum refresh rate of the video controller in hertz.
        /// </summary>
        public UInt32 MinRefreshRate { get; set; }

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Current resolution, color, and scan mode settings of the video controller.
        /// </summary>
        public string VideoModeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Free-form string describing the video processor.
        /// </summary>
        public string VideoProcessor { get; set; } = string.Empty;

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
