using System;
using System.Collections.Generic;

namespace Hardware.Info
{
    public class Volume
    {
        public string Caption { get; internal set; } = string.Empty;
        public Boolean Compressed { get; internal set; }
        public string Description { get; internal set; } = string.Empty;
        public string FileSystem { get; internal set; } = string.Empty;
        public UInt64 FreeSpace { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public UInt64 Size { get; internal set; }
        public string VolumeName { get; internal set; } = string.Empty;
        public string VolumeSerialNumber { get; internal set; } = string.Empty;

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
        public List<Volume> VolumeList { get; internal set; } = new List<Volume>();

        public Boolean Bootable { get; internal set; }
        public Boolean BootPartition { get; internal set; }
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public UInt32 DiskIndex { get; internal set; }
        public UInt32 Index { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public Boolean PrimaryPartition { get; internal set; }
        public UInt64 Size { get; internal set; }
        public UInt64 StartingOffset { get; internal set; }

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
        public List<Partition> PartitionList { get; internal set; } = new List<Partition>();

        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string FirmwareRevision { get; internal set; } = string.Empty;
        public UInt32 Index { get; internal set; }
        public string Manufacturer { get; internal set; } = string.Empty;
        public string Model { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public UInt32 Partitions { get; internal set; }
        public string SerialNumber { get; internal set; } = string.Empty;
        public UInt64 Size { get; internal set; }

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
