using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-bios

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_BIOS
    /// </summary>
    public class BIOS
    {
        /// <summary>
        /// Short description of the object a one-line string.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer of this software element.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Name used to identify this software element.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Release date of the Windows BIOS in the Coordinated Universal Time (UTC) format of YYYYMMDDHHMMSS.MMMMMM(+-)OOO.
        /// </summary>
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Assigned serial number of the software element.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Identifier for this software element; designed to be used in conjunction with other keys to create a unique representation of this instance.
        /// </summary>
        public string SoftwareElementID { get; set; } = string.Empty;

        /// <summary>
        /// Version of the BIOS. This string is created by the BIOS manufacturer.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "ReleaseDate: " + ReleaseDate + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "SoftwareElementID: " + SoftwareElementID + Environment.NewLine +
                "Version: " + Version + Environment.NewLine;
        }
    }
}
