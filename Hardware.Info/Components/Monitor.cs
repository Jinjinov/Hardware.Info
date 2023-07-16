using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-desktopmonitor
// https://learn.microsoft.com/en-us/windows/win32/wmicoreprov/wmimonitorid

namespace Hardware.Info
{
    /// <summary>
    /// WMI classes: Win32_DesktopMonitor And WmiMonitorID
    /// </summary>
    public class Monitor
    {
        /// <summary>
        /// Short description of the object.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Description of the object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Name of the monitor manufacturer.
        /// </summary>
        public string MonitorManufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Type of monitor.
        /// </summary>
        public string MonitorType { get; set; } = string.Empty;

        /// <summary>
        /// Label by which the object is known.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Resolution along the x-axis (horizontal direction) of the monitor.
        /// </summary>
        public UInt32 PixelsPerXLogicalInch { get; set; }

        /// <summary>
        /// Resolution along the y-axis (vertical direction) of the monitor.
        /// </summary>
        public UInt32 PixelsPerYLogicalInch { get; set; }

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
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "MonitorManufacturer: " + MonitorManufacturer + Environment.NewLine +
                "MonitorType: " + MonitorType + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "PixelsPerXLogicalInch: " + PixelsPerXLogicalInch + Environment.NewLine +
                "PixelsPerYLogicalInch: " + PixelsPerYLogicalInch + Environment.NewLine +
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
