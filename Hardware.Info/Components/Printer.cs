using System;

namespace Hardware.Info
{
    public class Printer
    {
        public string Caption;
        public string Default;
        public string Description;
        public string HorizontalResolution;
        public string Local;
        public string Name;
        public string Network;
        public string Shared;
        public string VerticalResolution;

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
