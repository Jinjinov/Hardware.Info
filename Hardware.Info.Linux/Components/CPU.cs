using System;
using System.Collections.Generic;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor

namespace Hardware.Info.Linux
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
        /// Size of the Level 1 processor instruction cache. A Level 1 cache is an external memory area that has a faster access time than the main RAM memory.
        /// </summary>
        public UInt32 L1InstructionCacheSize { get; set; }

        /// <summary>
        /// Size of the Level 1 processor data cache. A Level 1 cache is an external memory area that has a faster access time than the main RAM memory.
        /// </summary>
        public UInt32 L1DataCacheSize { get; set; }

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

        /// <summary>
        /// % Processor Time is the percentage of elapsed time that the processor spends to execute a non-Idle thread. 
        /// It is calculated by measuring the percentage of time that the processor spends executing the idle thread and then subtracting that value from 100%. 
        /// (Each processor has an idle thread that consumes cycles when no other threads are ready to run). 
        /// This counter is the primary indicator of processor activity, and displays the average percentage of busy time observed during the sample interval. 
        /// It should be noted that the accounting calculation of whether the processor is idle is performed at an internal sampling interval of the system clock (10ms). 
        /// On todays fast processors, % Processor Time can therefore underestimate the processor utilization as the processor may be spending a lot of time servicing threads between the system clock sampling interval. 
        /// Workload based timer applications are one example of applications which are more likely to be measured inaccurately as timers are signaled just after the sample is taken.
        /// </summary>
        public UInt64 PercentProcessorTime { get; set; }

        public List<CpuCore> CpuCoreList { get; set; } = new List<CpuCore>();

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "CurrentClockSpeed: " + CurrentClockSpeed + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "L1InstructionCacheSize: " + L1InstructionCacheSize + Environment.NewLine +
                "L1DataCacheSize: " + L1DataCacheSize + Environment.NewLine +
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
