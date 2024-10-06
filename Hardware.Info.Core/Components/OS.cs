﻿using System;

namespace Hardware.Info.Core
{
    /// <summary>
    /// Operating System Info
    /// </summary>
    public class OS
    {
        /// <summary>
        /// OS name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// OS version string
        /// </summary>
        public string VersionString { get; set; } = string.Empty;

        /// <summary>
        /// OS version
        /// </summary>
        public Version Version { get; set; } = new Version();

        /// <summary>
        /// Write all property values to a string
        /// </summary>
        /// <returns>Each property on a new line</returns>
        public override string ToString()
        {
            return
                "Name: " + Name + Environment.NewLine +
                "Version: " + VersionString + Environment.NewLine;
        }
    }
}
