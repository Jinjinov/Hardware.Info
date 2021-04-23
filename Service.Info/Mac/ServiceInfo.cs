using System.Collections.Generic;

// https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man3/sysctlbyname.3.html
// https://wiki.freepascal.org/Accessing_macOS_System_Information
// https://stackoverflow.com/questions/6592578/how-to-to-print-motherboard-and-display-card-info-on-mac
// https://stackoverflow.com/questions/53117107/cocoa-nstask-ouput-extraction
// https://docs.python.org/3/library/plistlib.html
// https://ss64.com/osx/system_profiler.html

namespace Service.Info.Mac
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
