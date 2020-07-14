using System;

namespace Hardware.Info
{
    public class Motherboard
    {
        public string Manufacturer;
        public string Product;

        public override string ToString()
        {
            return
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Product: " + Product + Environment.NewLine;
        }
    }
}
