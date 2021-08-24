using System;
using System.Collections.Generic;

namespace Hardware.Info
{
    public class Volume
    {
        public string Caption { get; set; } = string.Empty;
        public Boolean Compressed { get; set; }
        public string Description { get; set; } = string.Empty;
        public string FileSystem { get; set; } = string.Empty;
        public UInt64 FreeSpace { get; set; }
        public string Name { get; set; } = string.Empty;
        public UInt64 Size { get; set; }
        public string VolumeName { get; set; } = string.Empty;
        public string VolumeSerialNumber { get; set; } = string.Empty;

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Compressed: " + Compressed + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "FileSystem: " + FileSystem + Environment.NewLine +
                "FreeSpace: " + FreeSpace + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "Size: " + Size + Environment.NewLine +
                "VolumeName: " + VolumeName + Environment.NewLine +
                "VolumeSerialNumber: " + VolumeSerialNumber + Environment.NewLine;
        }
    }

    public class Partition
    {
        public List<Volume> VolumeList { get; set; } = new List<Volume>();

        public Boolean Bootable { get; set; }
        public Boolean BootPartition { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UInt32 DiskIndex { get; set; }
        public UInt32 Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public Boolean PrimaryPartition { get; set; }
        public UInt64 Size { get; set; }
        public UInt64 StartingOffset { get; set; }

        public override string ToString()
        {
            return
                "Bootable: " + Bootable + Environment.NewLine +
                "BootPartition: " + BootPartition + Environment.NewLine +
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "DiskIndex: " + DiskIndex + Environment.NewLine +
                "Index: " + Index + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "PrimaryPartition: " + PrimaryPartition + Environment.NewLine +
                "Size: " + Size + Environment.NewLine +
                "StartingOffset: " + StartingOffset + Environment.NewLine;
        }
    }

    public class Drive
    {
        public List<Partition> PartitionList { get; set; } = new List<Partition>();

        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FirmwareRevision { get; set; } = string.Empty;
        public UInt32 Index { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UInt32 Partitions { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public UInt64 Size { get; set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "FirmwareRevision: " + FirmwareRevision + Environment.NewLine +
                "Index: " + Index + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "Model: " + Model + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "Partitions: " + Partitions + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "Size: " + Size + Environment.NewLine;
        }
    }
}
