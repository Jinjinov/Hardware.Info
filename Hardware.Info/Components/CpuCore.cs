using System;

namespace Hardware.Info
{
    public class CpuCore
    {
        public string Name { get; internal set; } = string.Empty;
        public UInt64 PercentProcessorTime { get; internal set; }

        public override string ToString()
        {
            return
                "Name: " + Name + Environment.NewLine +
                "PercentProcessorTime: " + PercentProcessorTime + Environment.NewLine;
        }
    }
}
