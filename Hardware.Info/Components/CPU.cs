using System;
using System.Collections.Generic;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_Processor
    /// </summary>
    public class CPU
    {
        /// <summary>
        /// Short description of an object (a one-line string).
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Current speed of the processor, in MHz.
        /// </summary>
        public UInt32 CurrentClockSpeed { get; set; }

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Size of the Level 2 processor cache. A Level 2 cache is an external memory area that has a faster access time than the main RAM memory.
        /// </summary>
        public UInt32 L2CacheSize { get; set; }

        /// <summary>
        /// Size of the Level 3 processor cache. A Level 3 cache is an external memory area that has a faster access time than the main RAM memory.
        /// </summary>
        public UInt32 L3CacheSize { get; set; }

        /// <summary>
        /// Name of the processor manufacturer.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Maximum speed of the processor, in MHz.
        /// </summary>
        public UInt32 MaxClockSpeed { get; set; }

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Number of cores for the current instance of the processor. A core is a physical processor on the integrated circuit. For example, in a dual-core processor this property has a value of 2.
        /// </summary>
        public UInt32 NumberOfCores { get; set; }

        /// <summary>
        /// Number of logical processors for the current instance of the processor. For processors capable of hyperthreading, this value includes only the processors which have hyperthreading enabled.
        /// </summary>
        public UInt32 NumberOfLogicalProcessors { get; set; }

        /// <summary>
        /// Processor information that describes the processor features. 
        /// For an x86 class CPU, the field format depends on the processor support of the CPUID instruction. 
        /// If the instruction is supported, the property contains 2 (two) DWORD formatted values. 
        /// The first is an offset of 08h-0Bh, which is the EAX value that a CPUID instruction returns with input EAX set to 1. 
        /// The second is an offset of 0Ch-0Fh, which is the EDX value that the instruction returns. 
        /// Only the first two bytes of the property are significant and contain the contents of the DX register at CPU reset—all others are set to 0 (zero), and the contents are in DWORD format.
        /// </summary>
        public string ProcessorId { get; set; } = string.Empty;

        /// <summary>
        /// If True, the processor supports address translation extensions used for virtualization.
        /// </summary>
        public Boolean SecondLevelAddressTranslationExtensions { get; set; }

        /// <summary>
        /// Type of chip socket used on the circuit.
        /// </summary>
        public string SocketDesignation { get; set; } = string.Empty;

        /// <summary>
        /// If True, the Firmware has enabled virtualization extensions.
        /// </summary>
        public Boolean VirtualizationFirmwareEnabled { get; set; }

        /// <summary>
        /// If True, the processor supports Intel or AMD Virtual Machine Monitor extensions.
        /// </summary>
        public Boolean VMMonitorModeExtensions { get; set; }

        public UInt64 PercentProcessorTime { get; set; }

        public List<CpuCore> CpuCoreList { get; set; } = new List<CpuCore>();

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "CurrentClockSpeed: " + CurrentClockSpeed + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "L2CacheSize: " + L2CacheSize + Environment.NewLine +
                "L3CacheSize: " + L3CacheSize + Environment.NewLine +
                "Manufacturer: " + Manufacturer + Environment.NewLine +
                "MaxClockSpeed: " + MaxClockSpeed + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "NumberOfCores: " + NumberOfCores + Environment.NewLine +
                "NumberOfLogicalProcessors: " + NumberOfLogicalProcessors + Environment.NewLine +
                "PercentProcessorTime: " + PercentProcessorTime + Environment.NewLine +
                "ProcessorId: " + ProcessorId + Environment.NewLine +
                "SecondLevelAddressTranslationExtensions: " + SecondLevelAddressTranslationExtensions + Environment.NewLine +
                "SocketDesignation: " + SocketDesignation + Environment.NewLine +
                "VirtualizationFirmwareEnabled: " + VirtualizationFirmwareEnabled + Environment.NewLine +
                "VMMonitorModeExtensions: " + VMMonitorModeExtensions + Environment.NewLine;
        }
    }
}
