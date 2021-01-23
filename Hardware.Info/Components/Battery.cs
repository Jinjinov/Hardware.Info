using System;

namespace Hardware.Info
{
    public class Battery
    {
        // Full charge capacity of the battery in milliwatt-hours. 
        // Comparison of the value to the DesignCapacity property determines when the battery requires replacement. 
        // A battery's end of life is typically when the FullChargeCapacity property falls below 80% of the DesignCapacity property.
        public UInt32 FullChargeCapacity { get; internal set; }

        // Design capacity of the battery in milliwatt-hours.
        public UInt32 DesignCapacity { get; internal set; }

        public UInt16 BatteryStatus { get; internal set; }

        // Estimate of the percentage of full charge remaining
        public UInt16 EstimatedChargeRemaining { get; internal set; }

        // Estimate in minutes of the time to battery charge depletion under the present load conditions if the utility power is off, or lost and remains off, or a laptop is disconnected from a power source.
        public UInt32 EstimatedRunTime { get; internal set; }

        // Battery's expected lifetime in minutes, assuming that the battery is fully charged. 
        // The property represents the total expected life of the battery, not its current remaining life, which is indicated by the EstimatedRunTime property.
        public UInt32 ExpectedLife { get; internal set; }

        // Maximum time, in minutes, to fully charge the battery. 
        // The property represents the time to recharge a fully depleted battery, not the current remaining charge time, which is indicated in the TimeToFullCharge property.
        public UInt32 MaxRechargeTime { get; internal set; }

        // Elapsed time in seconds since the computer system's UPS last switched to battery power, or the time since the system or UPS was last restarted, whichever is less. 
        // If the battery is "on line", 0 (zero) is returned.
        public UInt32 TimeOnBattery { get; internal set; }

        // Remaining time to charge the battery fully in minutes at the current charging rate and usage.
        public UInt32 TimeToFullCharge { get; internal set; }

        private string batteryStatusDescription = string.Empty;

        public string BatteryStatusDescription
        {
            get => !string.IsNullOrEmpty(batteryStatusDescription) ? batteryStatusDescription : BatteryStatus switch
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

            internal set => batteryStatusDescription = value;
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
