using Hardware.Info.Windows;
using System;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    /// <inheritdoc />
    public class HardwareInfo : HardwareInfoBase
    {
        /// <inheritdoc />
        public HardwareInfo(TimeSpan? timeoutInWMI = null) : base(timeoutInWMI)
        {
        }

        protected override IPlatformHardwareInfo CreatePlatformHardwareInfo(TimeSpan? timeoutInWMI)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return new Hardware.Info.Windows.PlatformHardwareInfo(new ManagementQueryProvider(timeoutInWMI));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return new Hardware.Info.Mac.PlatformHardwareInfo();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return new Hardware.Info.Linux.PlatformHardwareInfo();
            }

            throw new PlatformNotSupportedException();
        }
    }
}
