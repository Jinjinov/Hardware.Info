using System;

namespace Hardware.Info
{
    public class Printer
    {
        public string Caption { get; internal set; } = string.Empty;
        public Boolean Default { get; internal set; }
        public string Description { get; internal set; } = string.Empty;
        public UInt32 HorizontalResolution { get; internal set; }
        public Boolean Local { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public Boolean Network { get; internal set; }
        public Boolean Shared { get; internal set; }
        public UInt32 VerticalResolution { get; internal set; }

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
