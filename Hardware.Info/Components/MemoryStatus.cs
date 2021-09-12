using System;

// https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/ns-sysinfoapi-memorystatusex

namespace Hardware.Info
{
    public class MemoryStatus
    {
        /// <summary>
        /// The amount of actual physical memory, in bytes.
        /// </summary>
        public ulong TotalPhysical { get; set; }

        /// <summary>
        /// The amount of physical memory currently available, in bytes. 
        /// This is the amount of physical memory that can be immediately reused without having to write its contents to disk first. 
        /// It is the sum of the size of the standby, free, and zero lists.
        /// </summary>
        public ulong AvailablePhysical { get; set; }

        /// <summary>
        /// The current committed memory limit for the system or the current process, whichever is smaller, in bytes.
        /// </summary>
        public ulong TotalPageFile { get; set; }

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes. 
        /// This value is equal to or smaller than the system-wide available commit value.
        /// </summary>
        public ulong AvailablePageFile { get; set; }

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process, in bytes. 
        /// This value depends on the type of process, the type of processor, and the configuration of the operating system.
        /// </summary>
        public ulong TotalVirtual { get; set; }

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual address space of the calling process, in bytes.
        /// </summary>
        public ulong AvailableVirtual { get; set; }

        /// <summary>
        /// Reserved. This value is always 0.
        /// </summary>
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
