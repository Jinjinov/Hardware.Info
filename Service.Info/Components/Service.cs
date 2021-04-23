using System;

namespace Service.Info
{
    public class Service
    {
        public string Name { get; internal set; } = string.Empty;
        public ServiceState State { get; internal set; } = ServiceState.Unknown;
        public UInt64 CpuUsage { get; internal set; }
        public UInt64 MemoryWorkingSet { get; internal set; }
        public UInt64 MemoryPrivateBytes { get; internal set; }

        public override string ToString()
        {
            return
                "Name: " + Name + Environment.NewLine +
                "State: " + State + Environment.NewLine +
                "CpuUsage: " + CpuUsage + Environment.NewLine +
                "MemoryWorkingSet: " + MemoryWorkingSet + Environment.NewLine +
                "MemoryPrivateBytes: " + MemoryPrivateBytes + Environment.NewLine;
        }
    }
}