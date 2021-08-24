using System;

namespace Hardware.Info
{
    public class Keyboard
    {
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UInt16 NumberOfFunctionKeys { get; set; }

        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "NumberOfFunctionKeys: " + NumberOfFunctionKeys + Environment.NewLine;
        }
    }
}
