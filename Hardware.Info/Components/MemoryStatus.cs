using System;

namespace Hardware.Info
{
    public class MemoryStatus
    {
        public ulong TotalPhysical { get; internal set; }
        public ulong AvailablePhysical { get; internal set; }
        public ulong TotalPageFile { get; internal set; }
        public ulong AvailablePageFile { get; internal set; }
        public ulong TotalVirtual { get; internal set; }
        public ulong AvailableVirtual { get; internal set; }
        public ulong AvailableExtendedVirtual { get; internal set; }

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
