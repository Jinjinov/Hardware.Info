using System;

namespace Hardware.Info
{
    public class Printer
    {
        public string Caption { get; set; } = string.Empty;
        public Boolean Default { get; set; }
        public string Description { get; set; } = string.Empty;
        public UInt32 HorizontalResolution { get; set; }
        public Boolean Local { get; set; }
        public string Name { get; set; } = string.Empty;
        public Boolean Network { get; set; }
        public Boolean Shared { get; set; }
        public UInt32 VerticalResolution { get; set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Default: " + Default + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "HorizontalResolution: " + HorizontalResolution + Environment.NewLine +
                "Local: " + Local + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "Network: " + Network + Environment.NewLine +
                "Shared: " + Shared + Environment.NewLine +
                "VerticalResolution: " + VerticalResolution + Environment.NewLine;
        }
    }
}
