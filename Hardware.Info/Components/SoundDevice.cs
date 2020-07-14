using System;

namespace Hardware.Info
{
    public class SoundDevice
    {
        public string Caption;
        public string Description;
        public string Manufacturer;
        public string Name;
        public string ProductName;

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
