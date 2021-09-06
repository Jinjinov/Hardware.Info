using System;

// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery

namespace Hardware.Info
{
    /// <summary>
    /// WMI class: Win32_Battery
    /// </summary>
    public class Battery
    {
        /// <summary>
        /// Full charge capacity of the battery in milliwatt-hours. 
        /// Comparison of the value to the DesignCapacity property determines when the battery requires replacement. 
        /// A battery's end of life is typically when the FullChargeCapacity property falls below 80% of the DesignCapacity property.
        /// </summary>
        public UInt32 FullChargeCapacity { get; set; }

        /// <summary>
        /// Design capacity of the battery in milliwatt-hours.
        /// </summary>
        public UInt32 DesignCapacity { get; set; }

        /// <summary>
        /// Status of the battery.
        /// </summary>
        public UInt16 BatteryStatus { get; set; }

        /// <summary>
        /// Estimate of the percentage of full charge remaining
        /// </summary>
        public UInt16 EstimatedChargeRemaining { get; set; }

        /// <summary>
        /// Estimate in minutes of the time to battery charge depletion under the present load conditions if the utility power is off, or lost and remains off, or a laptop is disconnected from a power source.
        /// </summary>
        public UInt32 EstimatedRunTime { get; set; }

        /// <summary>
        /// Battery's expected lifetime in minutes, assuming that the battery is fully charged. 
        /// The property represents the total expected life of the battery, not its current remaining life, which is indicated by the EstimatedRunTime property.
        /// </summary>
        public UInt32 ExpectedLife { get; set; }

        /// <summary>
        /// Maximum time, in minutes, to fully charge the battery. 
        /// The property represents the time to recharge a fully depleted battery, not the current remaining charge time, which is indicated in the TimeToFullCharge property.
        /// </summary>
        public UInt32 MaxRechargeTime { get; set; }

        /// <summary>
        /// Elapsed time in seconds since the computer system's UPS last switched to battery power, or the time since the system or UPS was last restarted, whichever is less. 
        /// If the battery is "on line", 0 (zero) is returned.
        /// </summary>
        public UInt32 TimeOnBattery { get; set; }

        /// <summary>
        /// Remaining time to charge the battery fully in minutes at the current charging rate and usage.
        /// </summary>
        public UInt32 TimeToFullCharge { get; set; }

        private string _batteryStatusDescription = string.Empty;

        public string BatteryStatusDescription
        {
            get => !string.IsNullOrEmpty(_batteryStatusDescription) ? _batteryStatusDescription : BatteryStatus switch
            {
                1 => "The battery is discharging",
                2 => "The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging.",
                3 => "Fully Charged",
                4 => "Low",
                5 => "Critical",
                6 => "Charging",
                7 => "Charging and High",
                8 => "Charging and Low",
                9 => "Charging and Critical",
                10 => "No battery is installed",
                11 => "Partially Charged",
                _ => string.Empty
            };

            set => _batteryStatusDescription = value;
        }

        public override string ToString()
        {
            return
                "FullChargeCapacity: " + FullChargeCapacity + Environment.NewLine +
                "DesignCapacity: " + DesignCapacity + Environment.NewLine +
                "BatteryStatusDescription: " + BatteryStatusDescription + Environment.NewLine +
                "EstimatedChargeRemaining: " + EstimatedChargeRemaining + Environment.NewLine +
                "EstimatedRunTime: " + EstimatedRunTime + Environment.NewLine +
                "ExpectedLife: " + ExpectedLife + Environment.NewLine +
                "MaxRechargeTime: " + MaxRechargeTime + Environment.NewLine +
                "TimeOnBattery: " + TimeOnBattery + Environment.NewLine +
                "TimeToFullCharge: " + TimeToFullCharge + Environment.NewLine;
        }
    }
}
