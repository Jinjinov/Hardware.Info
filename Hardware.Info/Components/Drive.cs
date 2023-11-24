using System;
using System.Collections.Generic;

namespace Hardware.Info.Linux
{
    // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-logicaldisk

    /// <summary>
    /// WMI class: Win32_LogicalDisk
    /// </summary>
    public class Volume
    {
        /// <summary>
        /// Short description of the object (a one-line string).
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// If True, the logical volume exists as a single compressed entity, such as a DoubleSpace volume. 
        /// If file based compression is supported, such as on NTFS, this property is False.
        /// </summary>
        public Boolean Compressed { get; set; }

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// File system on the logical disk.
        /// </summary>
        public string FileSystem { get; set; } = string.Empty;

        /// <summary>
        /// Space, in bytes, available on the logical disk.
        /// </summary>
        public UInt64 FreeSpace { get; set; }

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Size of the disk drive.
        /// </summary>
        public UInt64 Size { get; set; }

        /// <summary>
        /// Volume name of the logical disk.
        /// </summary>
        public string VolumeName { get; set; } = string.Empty;

        /// <summary>
        /// Volume serial number of the logical disk.
        /// </summary>
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

    // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-diskpartition

    /// <summary>
    /// WMI class: Win32_DiskPartition
    /// </summary>
    public class Partition
    {
        public List<Volume> VolumeList { get; set; } = new List<Volume>();

        /// <summary>
        /// Indicates whether the computer can be booted from this partition.
        /// </summary>
        public Boolean Bootable { get; set; }

        /// <summary>
        /// Partition is the active partition. 
        /// The operating system uses the active partition when booting from a hard disk.
        /// </summary>
        public Boolean BootPartition { get; set; }

        /// <summary>
        /// Short description of the object.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Index number of the disk containing this partition.
        /// </summary>
        public UInt32 DiskIndex { get; set; }

        /// <summary>
        /// Index number of the partition.
        /// </summary>
        public UInt32 Index { get; set; }

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// If True, this is the primary partition.
        /// </summary>
        public Boolean PrimaryPartition { get; set; }

        /// <summary>
        /// Total size of the partition.
        /// </summary>
        public UInt64 Size { get; set; }

        /// <summary>
        /// Starting offset (in bytes) of the partition.
        /// </summary>
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

    // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-diskdrive

    /// <summary>
    /// WMI class: Win32_DiskDrive
    /// </summary>
    public class Drive
    {
        public List<Partition> PartitionList { get; set; } = new List<Partition>();

        /// <summary>
        /// Short description of the object.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Revision for the disk drive firmware that is assigned by the manufacturer.
        /// </summary>
        public string FirmwareRevision { get; set; } = string.Empty;

        /// <summary>
        /// Physical drive number of the given drive.
        /// </summary>
        public UInt32 Index { get; set; }

        /// <summary>
        /// Name of the disk drive manufacturer.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer's model number of the disk drive.
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Number of partitions on this physical disk drive that are recognized by the operating system.
        /// </summary>
        public UInt32 Partitions { get; set; }

        /// <summary>
        /// Number allocated by the manufacturer to identify the physical media.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Size of the disk drive.
        /// </summary>
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
