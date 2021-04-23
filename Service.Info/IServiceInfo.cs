using System.Collections.Generic;

namespace Service.Info
{
    internal interface IServiceInfo
    {
        List<Service> GetServiceList();
        void SetServiceAction(string serviceName, ServiceAction action);
    }
}
