using Hardware.Info.Windows;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    /// <inheritdoc />
    public class HardwareInfo : HardwareInfoBase
    {
        /// <inheritdoc />
        public HardwareInfo(TimeSpan? timeoutInWMI = null, ILogger<HardwareInfo>? logger = null) : base(timeoutInWMI, logger)
        {
        }

        protected override IPlatformHardwareInfo CreatePlatformHardwareInfo(TimeSpan? timeoutInWMI)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return new Hardware.Info.Windows.PlatformHardwareInfo(new ManagementQueryProvider(timeoutInWMI), Logger);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return new Hardware.Info.Mac.PlatformHardwareInfo(Logger);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return new Hardware.Info.Linux.PlatformHardwareInfo(Logger);
            }

            throw new PlatformNotSupportedException();
        }
    }
}
