using System;

namespace Hardware.Info
{
    public class Memory
    {
        public UInt64 Capacity { get; internal set; }
        public string Manufacturer { get; internal set; } = string.Empty;
        public string PartNumber { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public UInt32 Speed { get; internal set; }

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
