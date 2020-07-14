using System;

namespace Hardware.Info
{
    public class Keyboard
    {
        public string Caption { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public UInt16 NumberOfFunctionKeys { get; internal set; }

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
