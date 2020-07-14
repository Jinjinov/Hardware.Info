using System;

namespace Hardware.Info
{
    public class Drive
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string FirmwareRevision { get; internal set; } = string.Empty;
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Model { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public UInt32 Partitions { get; internal set; }
        public string SerialNumber { get; internal set; } = string.Empty;
        public UInt64 Size { get; internal set; }

        public long AvailableFreeSpace { get; internal set; }
        public string DriveFormat { get; internal set; } = string.Empty;
        public string DriveType { get; internal set; } = string.Empty;
        public bool IsReady { get; internal set; }
        //public string Name { get; internal set; } = string.Empty;
        public string RootDirectory { get; internal set; } = string.Empty;
        public long TotalFreeSpace { get; internal set; }
        public long TotalSize { get; internal set; }
        public string VolumeLabel { get; internal set; } = string.Empty;

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "FirmwareRevision: " + FirmwareRevision + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Model: " + Model + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "Partitions: " + Partitions + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "Size: " + Size + Environment.NewLine;
        }
    }
}
