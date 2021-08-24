using System;

namespace Hardware.Info
{
    public class BIOS
    {
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string SoftwareElementID { get; set; } = string.Empty;
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
