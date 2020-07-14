using System;

namespace Hardware.Info
{
    public class Keyboard
    {
        public string Caption;
        public string Description;
        public string Name;
        public string NumberOfFunctionKeys;

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
