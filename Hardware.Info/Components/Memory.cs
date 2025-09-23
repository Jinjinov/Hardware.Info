using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-physicalmemory

namespace Hardware.Info
{
    /// <summary>
    /// Implementation form factor for the chip.
    /// </summary>
    public enum FormFactor
    {
        /// <summary>
        /// Represents an unknown or unspecified form factor.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Represents a form factor that is categorized as "OTHER," not falling into specific predefined categories.
        /// </summary>
        OTHER = 1,

        /// <summary>
        /// Represents a Single In-line Package (SIP) form factor.
        /// </summary>
        SIP = 2,

        /// <summary>
        /// Represents a Dual In-line Package (DIP) form factor.
        /// </summary>
        DIP = 3,

        /// <summary>
        /// Represents a Zigzag In-line Package (ZIP) form factor.
        /// </summary>
        ZIP = 4,

        /// <summary>
        /// Represents a Small Outline J-lead (SOJ) form factor.
        /// </summary>
        SOJ = 5,

        /// <summary>
        /// Represents a form factor that is proprietary and not covered by other specific categories.
        /// </summary>
        PROPRIETARY = 6,

        /// <summary>
        /// Represents a Single Inline Memory Module (SIMM) form factor.
        /// </summary>
        SIMM = 7,

        /// <summary>
        /// Represents a Dual Inline Memory Module (DIMM) form factor.
        /// </summary>
        DIMM = 8,

        /// <summary>
        /// Represents a Thin Small Outline Package (TSOP) form factor.
        /// </summary>
        TSOP = 9,

        /// <summary>
        /// Represents a Pin Grid Array (PGA) form factor.
        /// </summary>
        PGA = 10,

        /// <summary>
        /// Represents a Rambus Inline Memory Module (RIMM) form factor.
        /// </summary>
        RIMM = 11,

        /// <summary>
        /// Represents a Small Outline Dual In-line Memory Module (SODIMM) form factor.
        /// </summary>
        SODIMM = 12,

        /// <summary>
        /// Represents a Static Random-Access Memory Inline Memory Module (SRIMM) form factor.
        /// </summary>
        SRIMM = 13,

        /// <summary>
        /// Represents a Surface Mount Device (SMD) form factor.
        /// </summary>
        SMD = 14,

        /// <summary>
        /// Represents a Shrink Small-Outline Package (SSMP) form factor.
        /// </summary>
        SSMP = 15,

        /// <summary>
        /// Represents a Quad Flat Package (QFP) form factor.
        /// </summary>
        QFP = 16,

        /// <summary>
        /// Represents a Thin Quad Flat Package (TQFP) form factor.
        /// </summary>
        TQFP = 17,

        /// <summary>
        /// Represents a Small Outline Integrated Circuit (SOIC) form factor.
        /// </summary>
        SOIC = 18,

        /// <summary>
        /// Represents a Leadless Chip Carrier (LCC) form factor.
        /// </summary>
        LCC = 19,

        /// <summary>
        /// Represents a Plastic Leaded Chip Carrier (PLCC) form factor.
        /// </summary>
        PLCC = 20,

        /// <summary>
        /// Represents a Ball Grid Array (BGA) form factor.
        /// </summary>
        BGA = 21,

        /// <summary>
        /// Represents a Fine-Pitch Ball Grid Array (FPBGA) form factor.
        /// </summary>
        FPBGA = 22,

        /// <summary>
        /// Represents a Land Grid Array (LGA) form factor.
        /// </summary>
        LGA = 23
    }

    public enum MemoryType
    {
        /// <summary>
        /// Represents an unknown or unspecified form factor.
        /// </summary>
        UNKNOWN,
        
        DRAM,
        EDRAM,
        VRAM,
        SRAM,
        RAM,
        SDRAM,
        SGRAM,
        RDRAM,
        FBD2,
        DDR,
        DDR2,
        DDR3,
        DDR4,
        DDR5,
        HBM,
        HBM2,
        LPDDR,
        LPDDR2,
        LPDDR3,
        LPDDR4,
        LPDDR5,
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
        
        /// <summary>
        /// Type of memory
        /// </summary>
        public MemoryType Type { get; set; }
        
        /// <summary>
        /// Data width of the physical memory—in bits
        /// </summary>
        public ushort DataWidth { get; set; }

        /// <summary>
        /// Write all property values to a string
        /// </summary>
        /// <returns>Each property on a new line</returns>
        public override string ToString()
        {
            return
                "BankLabel: " + BankLabel + Environment.NewLine +
                "Type: " + Type + Environment.NewLine +
                "Width: " + DataWidth + Environment.NewLine +
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
