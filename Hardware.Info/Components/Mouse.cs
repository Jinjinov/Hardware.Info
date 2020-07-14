using System;

namespace Hardware.Info
{
    public class Mouse
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public byte NumberOfButtons { get; internal set; }

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
