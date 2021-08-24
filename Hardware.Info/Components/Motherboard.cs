using System;

namespace Hardware.Info
{
    public class Motherboard
    {
        public string Manufacturer { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;

        public override string ToString()
        {
            return
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Product: " + Product + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine;
        }
    }
}
