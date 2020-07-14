using System;
using System.Collections.Generic;
using System.Text;

namespace Hardware.Info
{
    public class CPU
    {
        public string Model { get; private set; } = string.Empty;
        public string SerialNumber { get; private set; } = string.Empty;
        public string Temperature { get; private set; } = string.Empty;
        public string Speed { get; private set; } = string.Empty;
    }
}
