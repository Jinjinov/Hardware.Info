using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-physicalmemory

namespace Hardware.Info.Linux
{
    /// <summary>
    /// Implementation form factor for the chip.
    /// </summary>
    public enum FormFactor
    {
        UNKNOWN = 0,
        OTHER = 1,
        SIP = 2,
        DIP = 3,
        ZIP = 4,
        SOJ = 5,
        PROPRIETARY = 6,
        SIMM = 7,
        DIMM = 8,
        TSOP = 9,
        PGA = 10,
        RIMM = 11,
        SODIMM = 12,
        SRIMM = 13,
        SMD = 14,
        SSMP = 15,
        QFP = 16,
        TQFP = 17,
        SOIC = 18,
        LCC = 19,
        PLCC = 20,
        BGA = 21,
        FPBGA = 22,
        LGA = 23
    }

    /// <summary>
    /// WMI class: Win32_PhysicalMemory
    /// </summary>
    public class Memory
    {
        /// <summary>
        /// Physically labeled bank where the memory is located.
        /// </summary>
        public string BankLabel { get; set; } = string.Empty;

        /// <summary>
        /// Total capacity of the physical memory in bytes.
        /// </summary>
        public UInt64 Capacity { get; set; }

        /// <summary>
        /// Implementation form factor for the chip.
        /// </summary>
        public FormFactor FormFactor { get; set; }

        /// <summary>
        /// Name of the organization responsible for producing the physical element.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// The maximum operating voltage for this device, in millivolts, or 0, if the voltage is unknown.
        /// </summary>
        public UInt32 MaxVoltage { get; set; }

        /// <summary>
        /// The minimum operating voltage for this device, in millivolts, or 0, if the voltage is unknown.
        /// </summary>
        public UInt32 MinVoltage { get; set; }

        /// <summary>
        /// Part number assigned by the organization responsible for producing or manufacturing the physical element.
        /// </summary>
        public string PartNumber { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer-allocated number to identify the physical element.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Speed of the physical memory in nanoseconds.
        /// </summary>
        public UInt32 Speed { get; set; }

        public override string ToString()
        {
            return
                "BankLabel: " + BankLabel + Environment.NewLine +
                "Capacity: " + Capacity + Environment.NewLine +
                "FormFactor: " + FormFactor + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "MaxVoltage: " + MaxVoltage + Environment.NewLine +
                "MinVoltage: " + MinVoltage + Environment.NewLine +
                "PartNumber: " + PartNumber + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "Speed: " + Speed + Environment.NewLine;
        }
    }
}
