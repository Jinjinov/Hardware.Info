using System;

// https://learn.microsoft.com/en-us/windows/win32/wmicoreprov/wmimonitorid

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_DesktopMonitor
    /// </summary>
    public class MonitorExtended
    {
        /// <summary>
        /// Indicates the active monitor.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Name of manufacturer.
        /// </summary>
        public string ManufacturerName { get; set; } = string.Empty;

        /// <summary>
        /// Vendor assigned product code ID.
        /// </summary>
        public string ProductCodeID { get; set; } = string.Empty;

        /// <summary>
        /// Serial number.
        /// </summary>
        public string SerialNumberID { get; set; } = string.Empty;

        /// <summary>
        /// The friendly name of the monitor.
        /// </summary>
        public string UserFriendlyName { get; set; } = string.Empty;

        /// <summary>
        /// Week of manufacture by week number. The range is from 1 through 53. Zero (0) is undefined.
        /// </summary>
        public UInt16 WeekOfManufacture { get; set; }

        /// <summary>
        /// Year of manufacture.
        /// </summary>
        public UInt16 YearOfManufacture { get; set; }

        public override string ToString()
        {
            return
                "Active: " + Active + Environment.NewLine +
                "ManufacturerName: " + ManufacturerName + Environment.NewLine +
                "ProductCodeID: " + ProductCodeID + Environment.NewLine +
                "SerialNumberID: " + SerialNumberID + Environment.NewLine +
                "UserFriendlyName: " + UserFriendlyName + Environment.NewLine +
                "WeekOfManufacture: " + WeekOfManufacture + Environment.NewLine +
                "YearOfManufacture: " + YearOfManufacture + Environment.NewLine;
        }
    }
}
