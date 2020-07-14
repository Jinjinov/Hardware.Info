using System;

namespace Hardware.Info
{
    public class Motherboard
    {
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Product { get; internal set; } = string.Empty;

        public override string ToString()
        {
            return
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Product: " + Product + Environment.NewLine;
        }
    }
}
