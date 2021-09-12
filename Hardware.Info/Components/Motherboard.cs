using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-baseboard

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_BaseBoard
    /// </summary>
    public class Motherboard
    {
        /// <summary>
        /// Name of the organization responsible for producing the physical element.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Baseboard part number defined by the manufacturer.
        /// </summary>
        public string Product { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer-allocated number used to identify the physical element.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        public override string ToString()
        {
            return
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Product: " + Product + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine;
        }
    }
}
