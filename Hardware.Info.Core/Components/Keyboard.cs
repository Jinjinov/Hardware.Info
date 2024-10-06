﻿using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-keyboard

namespace Hardware.Info.Core
{
    /// <summary>
    /// WMI class: Win32_Keyboard
    /// </summary>
    public class Keyboard
    {
        /// <summary>
        /// Short description of the object a one-line string.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Number of function keys on the keyboard.
        /// </summary>
        public UInt16 NumberOfFunctionKeys { get; set; }

        /// <summary>
        /// Write all property values to a string
        /// </summary>
        /// <returns>Each property on a new line</returns>
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
