using System;
using System.Collections.Generic;
using System.Management;

namespace Hardware.Info.Windows
{
    internal class ManagementQueryProvider : IWmiQueryProvider
    {
        private readonly EnumerationOptions _enumerationOptions = new EnumerationOptions() { ReturnImmediately = true, Rewindable = false, Timeout = EnumerationOptions.InfiniteTimeout };

        public ManagementQueryProvider(TimeSpan? enumerationOptionsTimeout = null)
        {
            if (enumerationOptionsTimeout.HasValue)
                _enumerationOptions.Timeout = enumerationOptionsTimeout.Value;
        }

        public IEnumerable<IWmiPropertySource> Query(string scope, string query)
        {
            using var mos = new ManagementObjectSearcher(scope, query);

            foreach (ManagementBaseObject mo in mos.Get())
                yield return new ManagementObjectAdapter(mo);
        }

        public IEnumerable<IWmiPropertySource> QueryRelated(string scope, IWmiPropertySource wmiPropertySource, string relatedClass)
        {
            ManagementObjectAdapter? managementObjectAdapter = wmiPropertySource as ManagementObjectAdapter;

            if(managementObjectAdapter is null)
                yield break;

            ManagementBaseObject managementBaseObject = managementObjectAdapter.GetManagementBaseObject();

            ManagementObject? managementObject = managementBaseObject as ManagementObject;

            if (managementObject is null)
                yield break;

            foreach (ManagementBaseObject mo in managementObject.GetRelated(relatedClass))
                yield return new ManagementObjectAdapter(mo);
        }
    }
}
