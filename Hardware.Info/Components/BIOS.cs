using System;

namespace Hardware.Info
{
    public class BIOS
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string ReleaseDate { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public string SoftwareElementID { get; internal set; } = string.Empty;
        public string Version { get; internal set; } = string.Empty;

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
