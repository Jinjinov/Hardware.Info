using System;

namespace Hardware.Info
{
    public class MemoryStatus
    {
        public ulong TotalPhysical { get; set; }
        public ulong AvailablePhysical { get; set; }
        public ulong TotalPageFile { get; set; }
        public ulong AvailablePageFile { get; set; }
        public ulong TotalVirtual { get; set; }
        public ulong AvailableVirtual { get; set; }
        public ulong AvailableExtendedVirtual { get; set; }

        public override string ToString()
        {
            return
                "TotalPhysical: " + TotalPhysical + Environment.NewLine +
                "AvailablePhysical: " + AvailablePhysical + Environment.NewLine +
                "TotalPageFile: " + TotalPageFile + Environment.NewLine +
                "AvailablePageFile: " + AvailablePageFile + Environment.NewLine +
                "TotalVirtual: " + TotalVirtual + Environment.NewLine +
                "AvailableVirtual: " + AvailableVirtual + Environment.NewLine +
                "AvailableExtendedVirtual: " + AvailableExtendedVirtual + Environment.NewLine;
        }
    }
}
