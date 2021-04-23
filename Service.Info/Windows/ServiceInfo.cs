using System;
using System.Collections.Generic;
using System.Management;

namespace Service.Info.Windows
{
    internal class ServiceInfo : ServiceInfoBase, IServiceInfo
    {
        public static T GetPropertyValue<T>(object obj) where T : struct
        {
            return (obj == null) ? default(T) : (T)obj;
        }

        public static T[] GetPropertyArray<T>(object obj)
        {
            return (obj is T[] array) ? array : Array.Empty<T>();
        }

        public static string GetPropertyString(object obj)
        {
            return (obj is string str) ? str : string.Empty;
        }
        
        public List<Service> GetServiceList()
        {
            var services = new List<Service>();
            using var win32Service = new ManagementObjectSearcher("SELECT * FROM Win32_Service");

            foreach (var queryObject in win32Service.Get())
            {
                var service = new Service();
                var processId = GetPropertyValue<uint>(queryObject["ProcessId"]);
                var state = GetPropertyString(queryObject["State"]);
                service.Name = GetPropertyString(queryObject["Name"]);
                service.State = state switch
                {
                    "Stopped" => ServiceState.Stopped,
                    "Start Pending" => ServiceState.StartPending,
                    "Stop Pending" => ServiceState.StopPending,
                    "Running" => ServiceState.Running,
                    "Continue Pending" => ServiceState.ContinuePending,
                    "Pause Pending" => ServiceState.PausePending,
                    "Paused" => ServiceState.Paused,
                    "Unknown" => ServiceState.Unknown,
                    _ => service.State
                };

                if (service.State == ServiceState.Running)
                {
                    using var win32PerfFormattedDataPerfProcProcess = new ManagementObjectSearcher(
                        $"SELECT * FROM Win32_PerfFormattedData_PerfProc_Process WHERE IDProcess = {processId}");

                    foreach (var queryObj in win32PerfFormattedDataPerfProcProcess.Get())
                    {
                        service.CpuUsage = GetPropertyValue<ulong>(queryObj["PercentProcessorTime"]);
                        service.MemoryPrivateBytes = GetPropertyValue<ulong>(queryObj["PrivateBytes"]);
                        service.MemoryWorkingSet = GetPropertyValue<ulong>(queryObj["WorkingSet"]);
                    }
                }

                services.Add(service);
            }
            
            return services;
        }

        public void SetServiceAction(string serviceName, ServiceAction action)
        {
            using var classInstance = new ManagementObject($"Win32_Service.Name = '{serviceName}'", null);
            switch (action)
            {
                case ServiceAction.Stop:
                    classInstance.InvokeMethod("StopService", null, null);
                    break;
                case ServiceAction.Start:
                    classInstance.InvokeMethod("StartService", null, null);
                    break;
                case ServiceAction.Restart:
                    classInstance.InvokeMethod("StopService", null, null);
                    classInstance.InvokeMethod("StartService", null, null);
                    break;
            }
        }
    }
}
