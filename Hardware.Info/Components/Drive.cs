using System;
using System.Collections.Generic;
using System.Text;

namespace Hardware.Info
{
    public class Drive
    {
        public string Model { get; internal set; } = string.Empty;
        public string SerialNumber { get; internal set; } = string.Empty;
        public string Temperature { get; internal set; } = string.Empty;
        public string Size { get; internal set; } = string.Empty;
        public string FreeSpace { get; internal set; } = string.Empty;

        public long AvailableFreeSpace { get; internal set; }
        public string DriveFormat { get; internal set; } = string.Empty;
        public string DriveType { get; internal set; } = string.Empty;
        public bool IsReady { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public string RootDirectory { get; internal set; } = string.Empty;
        public long TotalFreeSpace { get; internal set; }
        public long TotalSize { get; internal set; }
        public string VolumeLabel { get; internal set; } = string.Empty;
    }
}
