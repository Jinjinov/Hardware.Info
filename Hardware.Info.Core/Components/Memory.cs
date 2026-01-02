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
    
    /// <summary>
    /// Represents the type of memory device as defined by the SMBIOS specification.
    /// Corresponds to the 'Memory Type' field in the Memory Device (Type 17) structure.
    /// </summary>
    public enum MemoryType
    {
        /// <summary>
        /// Memory type is not specified or known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Other memory type not listed here.
        /// </summary>
        Other = 1,

        /// <summary>
        /// Dynamic Random Access Memory (DRAM).
        /// </summary>
        DRAM = 2,

        /// <summary>
        /// Synchronous DRAM (SDRAM).
        /// </summary>
        SynchronousDRAM = 3,

        /// <summary>
        /// Cache DRAM.
        /// </summary>
        CacheDRAM = 4,

        /// <summary>
        /// Extended Data Out RAM (EDO).
        /// </summary>
        EDO = 5,

        /// <summary>
        /// Extended Data Out RAM (EDRAM).
        /// </summary>
        EDRAM = 6,

        /// <summary>
        /// Video RAM (VRAM).
        /// </summary>
        VRAM = 7,

        /// <summary>
        /// Static RAM (SRAM).
        /// </summary>
        SRAM = 8,

        /// <summary>
        /// Random Access Memory (RAM).
        /// </summary>
        RAM = 9,

        /// <summary>
        /// Read Only Memory (ROM).
        /// </summary>
        ROM = 10,

        /// <summary>
        /// Flash memory.
        /// </summary>
        Flash = 11,

        /// <summary>
        /// Electrically Erasable Programmable Read-Only Memory (EEPROM).
        /// </summary>
        EEPROM = 12,

        /// <summary>
        /// Ferroelectric Random Access Memory (FEPROM).
        /// </summary>
        FEPROM = 13,

        /// <summary>
        /// Erasable Programmable Read-Only Memory (EPROM).
        /// </summary>
        EPROM = 14,

        /// <summary>
        /// Burst Extended Data Out DRAM (CDRAM).
        /// </summary>
        CDRAM = 15,

        /// <summary>
        /// 3D RAM (Three-dimensional RAM).
        /// </summary>
        _3DRAM = 16,

        /// <summary>
        /// Synchronous DRAM (SDRAM).
        /// </summary>
        SDRAM = 17,

        /// <summary>
        /// Synchronous Graphics RAM (SGRAM).
        /// </summary>
        SGRAM = 18,

        /// <summary>
        /// Rambus Dynamic RAM (RDRAM).
        /// </summary>
        RDRAM = 19,

        /// <summary>
        /// Double Data Rate Synchronous DRAM (DDR or DDR1).
        /// </summary>
        DDR = 20,

        /// <summary>
        /// Double Data Rate Two Synchronous DRAM (DDR2).
        /// </summary>
        DDR2 = 21,

        /// <summary>
        /// DDR2 Fully Buffered DIMM.
        /// </summary>
        DDR2_FB_DIMM = 22,

        /// <summary>
        /// Double Data Rate Three Synchronous DRAM (DDR3).
        /// </summary>
        DDR3 = 24,

        /// <summary>
        /// Fully Buffered DIMM Generation 2.
        /// </summary>
        FBD2 = 25,

        /// <summary>
        /// Double Data Rate Four Synchronous DRAM (DDR4).
        /// </summary>
        DDR4 = 26,

        /// <summary>
        /// Low Power Double Data Rate SDRAM (LPDDR).
        /// </summary>
        LPDDR = 27,

        /// <summary>
        /// Low Power Double Data Rate Two Synchronous DRAM (LPDDR2).
        /// </summary>
        LPDDR2 = 28,

        /// <summary>
        /// Low Power Double Data Rate Three Synchronous DRAM (LPDDR3).
        /// </summary>
        LPDDR3 = 29,

        /// <summary>
        /// Low Power Double Data Rate Four Synchronous DRAM (LPDDR4).
        /// </summary>
        LPDDR4 = 30,

        /// <summary>
        /// Logical Non-Volatile Device.
        /// </summary>
        LogicalNonVolatileDevice = 31,

        /// <summary>
        /// High Bandwidth Memory (HBM).
        /// </summary>
        HBM = 32,

        /// <summary>
        /// High Bandwidth Memory 2 (HBM2).
        /// </summary>
        HBM2 = 33,

        /// <summary>
        /// Double Data Rate Five Synchronous DRAM (DDR5).
        /// </summary>
        DDR5 = 34,

        /// <summary>
        /// Low Power Double Data Rate Five Synchronous DRAM (LPDDR5).
        /// </summary>
        LPDDR5 = 35,

        /// <summary>
        /// High Bandwidth Memory 3 (HBM3).
        /// </summary>
        HBM3 = 36,
        
        // Values 23, and 37 through 0x7E are reserved for future assignment by DMTF.
        // Values 0x7F to 0xFF are reserved for OEM-specific values.
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
        /// Data width of the physical memory in bits.
        /// </summary>
        public UInt16 DataWidth { get; set; }

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
        /// Type of physical memory.
        /// </summary>
        public MemoryType MemoryType { get; set; }

        /// <summary>
        /// Speed of the physical memory in nanoseconds.
        /// </summary>
        public UInt32 Speed { get; set; }

        /// <summary>
        /// Write all property values to a string
        /// </summary>
        /// <returns>Each property on a new line</returns>
        public override string ToString()
        {
            return
                "BankLabel: " + BankLabel + Environment.NewLine +
                "Capacity: " + Capacity + Environment.NewLine +
                "DataWidth: " + DataWidth + Environment.NewLine +
                "FormFactor: " + FormFactor + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "MaxVoltage: " + MaxVoltage + Environment.NewLine +
                "MinVoltage: " + MinVoltage + Environment.NewLine +
                "PartNumber: " + PartNumber + Environment.NewLine +
                "SerialNumber: " + SerialNumber + Environment.NewLine +
                "MemoryType: " + MemoryType + Environment.NewLine +
                "Speed: " + Speed + Environment.NewLine;
        }
    }
}
