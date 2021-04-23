using System;
using System.Collections.Generic;
using System.Linq;

// https://www.binarytides.com/linux-commands-hardware-info/

namespace Service.Info.Linux
{
    internal class ServiceInfo : ServiceInfoBase, IServiceInfo
    {
        public List<Service> GetServiceList()
        {
            var serviceList = new List<Service>();

            var processOutput = ReadProcessOutput("service", "--status-all");
            var lines = processOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var service = new Service();
                if (line.Contains("[ + ]"))
                {
                    service.State = ServiceState.Running;
                }
                else if (line.Contains("[ - ]"))
                {
                    service.State = ServiceState.Stopped;
                }
                else
                {
                    service.State = ServiceState.Unknown;
                }
                    
                var split = line.Split(" ");

                service.Name = split.Last();

                var systemctlOutput = ReadProcessOutput("systemctl", $"status {service.Name}");
                var systemctlOutputLines = systemctlOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                var pid = systemctlOutputLines.FirstOrDefault(l => l.Contains("PID:"))?.Split(":").Last().Trim().Split(" ").First();
                if (!string.IsNullOrWhiteSpace(pid))
                {
                    var psOutput = ReadProcessOutput("ps", $"-p {pid} -o %cpu,vsz,rss");
                    var psOutputLines = psOutput.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                    // Output of above ps command
                    //%CPU   VSZ   RSS
                    // 0.0  1463  3242 

                    if (psOutputLines.Length == 2)
                    {
                        var values = psOutputLines.Last().Trim().Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        
                        service.CpuUsage = Convert.ToUInt64(Math.Round(Convert.ToDecimal(values[0].Trim()), MidpointRounding.AwayFromZero));
                        service.MemoryPrivateBytes = Convert.ToUInt64(values[1].Trim());
                        service.MemoryWorkingSet = Convert.ToUInt64(values[2].Trim());
                    }
                }

                serviceList.Add(service);
            }
            
            return serviceList;
        }

        public void SetServiceAction(string serviceName, ServiceAction action)
        {
            var serviceAction = string.Empty;
            switch (action)
            {
                case ServiceAction.Stop:
                    serviceAction = "stop";
                    break;
                case ServiceAction.Start:
                    serviceAction = "start";
                    break;
                case ServiceAction.Restart:
                    serviceAction = "restart";
                    break;
            }

            StartProcess("service", $"{serviceName} {serviceAction}");
        }
    }
}
