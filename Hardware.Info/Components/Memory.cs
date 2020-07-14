using System;
using System.Collections.Generic;
using System.Text;

namespace Hardware.Info
{
    public class Memory
    {
        public string Model { get; private set; } = string.Empty;
        public string SerialNumber { get; private set; } = string.Empty;
        public string Size { get; private set; } = string.Empty;
    }
}
