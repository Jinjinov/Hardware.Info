using System;

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_PerfFormattedData_PerfOS_Processor
    /// </summary>
    public class CpuCore
    {
        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

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

        public override string ToString()
        {
            return
                "Name: " + Name + Environment.NewLine +
                "PercentProcessorTime: " + PercentProcessorTime + Environment.NewLine;
        }
    }
}
