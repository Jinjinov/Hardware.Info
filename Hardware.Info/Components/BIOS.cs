using System;

namespace Hardware.Info
{
    public class BIOS
    {
        public string Caption;
        public string Description;
        public string Manufacturer;
        public string Name;
        public string ReleaseDate;
        public string SerialNumber;
        public string SoftwareElementID;
        public string Version;

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
