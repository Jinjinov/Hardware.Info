using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-pointingdevice

namespace Hardware.Info.Linux
{
    /// <summary>
    /// WMI class: Win32_PointingDevice
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Short description of the object.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Name of the processor's manufacturer.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Number of buttons on the pointing device.
        /// </summary>
        public byte NumberOfButtons { get; set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "NumberOfButtons: " + NumberOfButtons + Environment.NewLine;
        }
    }
}
