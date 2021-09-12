using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-printer

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_Printer
    /// </summary>
    public class Printer
    {
        /// <summary>
        /// Short description of an object (a one-line string).
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// If TRUE, the printer is the default printer.
        /// </summary>
        public Boolean Default { get; set; }

        /// <summary>
        /// Description of an object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Horizontal resolution of the printer in pixels per inch.
        /// </summary>
        public UInt32 HorizontalResolution { get; set; }

        /// <summary>
        /// If TRUE, the printer is not attached to a network. 
        /// If both the Local and Network properties are set to TRUE, then the printer is a network printer.
        /// </summary>
        public Boolean Local { get; set; }

        /// <summary>
        /// Name of the printer.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// If TRUE, the printer is a network printer. 
        /// If both the Local and Network properties are set to TRUE, then the printer is a network printer.
        /// </summary>
        public Boolean Network { get; set; }

        /// <summary>
        /// If TRUE, the printer is available as a shared network resource.
        /// </summary>
        public Boolean Shared { get; set; }

        /// <summary>
        /// Vertical resolution, in pixels-per-inch, of the printer.
        /// </summary>
        public UInt32 VerticalResolution { get; set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Default: " + Default + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "HorizontalResolution: " + HorizontalResolution + Environment.NewLine +
                "Local: " + Local + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "Network: " + Network + Environment.NewLine +
                "Shared: " + Shared + Environment.NewLine +
                "VerticalResolution: " + VerticalResolution + Environment.NewLine;
        }
    }
}
