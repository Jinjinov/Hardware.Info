using System;

namespace Hardware.Info
{
    public class SoundDevice
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string ProductName { get; internal set; } = string.Empty;

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
