using System;

// https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystemproduct

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_ComputerSystemProduct
    /// </summary>
    public class ComputerSystem
    {
        /// <summary>
        /// Short textual description for the product.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Textual description of the product.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Product identification, such as a serial number on software, a die number on a hardware chip, or (for noncommercial products) a project number.
        /// </summary>
        public string IdentifyingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Commonly used product name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product's stock-keeping unit (SKU) information.
        /// </summary>
        public string SKUNumber { get; set; } = string.Empty;

        /// <summary>
        /// Universally unique identifier (UUID) for this product. A UUID is a 128-bit identifier that is guaranteed to be different from other generated UUIDs. If a UUID is not available, a UUID of all zeros is used.
        /// </summary>
        public string UUID { get; set; } = string.Empty;

        /// <summary>
        /// Name of the product's supplier, or the entity selling the product (the manufacturer, reseller, OEM, and so on).
        /// </summary>
        public string Vendor { get; set; } = string.Empty;

        /// <summary>
        /// Product version information.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Write all property values to a string
        /// </summary>
        /// <returns>Each property on a new line</returns>
        public override string ToString()
        {
            return
                "Caption: " + Caption + Environment.NewLine +
                "Description: " + Description + Environment.NewLine +
                "IdentifyingNumber: " + IdentifyingNumber + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "SKUNumber: " + SKUNumber + Environment.NewLine +
                "UUID: " + UUID + Environment.NewLine +
                "Vendor: " + Vendor + Environment.NewLine +
                "Version: " + Version + Environment.NewLine;
        }
    }
}