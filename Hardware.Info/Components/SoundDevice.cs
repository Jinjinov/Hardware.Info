using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-sounddevice

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_SoundDevice
    /// </summary>
    public class SoundDevice
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
        /// Manufacturer of the sound device.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product name of the sound device.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "ProductName: " + ProductName + Environment.NewLine;
        }
    }
}
