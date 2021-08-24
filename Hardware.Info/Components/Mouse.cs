using System;

namespace Hardware.Info
{
    public class Mouse
    {
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
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
