using System;

namespace Hardware.Info
{
    public class Memory
    {
        public string Capacity;
        public string Manufacturer;
        public string PartNumber;
        public string SerialNumber;
        public string Speed;

        public override string ToString()
        {
            return
                "Capacity: " + Capacity + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "PartNumber: " + PartNumber + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "Speed: " + Speed + Environment.NewLine;
        }
    }
}
