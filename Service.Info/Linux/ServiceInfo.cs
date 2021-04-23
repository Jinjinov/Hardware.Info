using System.Collections.Generic;

// https://www.binarytides.com/linux-commands-hardware-info/

namespace Service.Info.Linux
{
    internal class ServiceInfo : ServiceInfoBase, IServiceInfo
    {
        public List<Service> GetServiceList()
        {
            return new List<Service>();
        }

        public void SetServiceAction(string serviceName, ServiceAction action)
        {
            
        }
    }
}
