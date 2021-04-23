using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Service.Info
{
    public class ServiceInfo
    {
        public List<Service> Services { get; private set; } = new List<Service>();

        private readonly IServiceInfo serviceInfo = null!;

        public ServiceInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                serviceInfo = new Windows.ServiceInfo();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                serviceInfo = new Mac.ServiceInfo();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Environment.OSVersion.Platform == PlatformID.Unix)
            {
                serviceInfo = new Linux.ServiceInfo();
            }
        }

        public void RefreshAll()
        {
            RefreshServiceList();
        }

        public void RefreshServiceList() => Services = serviceInfo.GetServiceList();

        public void SetServiceAction(string serviceName, ServiceAction action) =>
            serviceInfo.SetServiceAction(serviceName, action);
    }
}
