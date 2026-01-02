using System;

namespace Hardware.Info
{
    /// <inheritdoc />
    public class HardwareInfo : HardwareInfoBase
    {
        /// <inheritdoc />
        public HardwareInfo(TimeSpan? timeoutInWMI = null) : base(timeoutInWMI)
        {
        }
    }
}
