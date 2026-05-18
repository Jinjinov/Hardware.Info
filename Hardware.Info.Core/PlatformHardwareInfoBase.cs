using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hardware.Info
{
    internal class PlatformHardwareInfoBase
    {
        protected ILogger _logger = NullLogger.Instance;

        internal static Process StartProcess(string cmd, string args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            return Process.Start(processStartInfo);
        }

        internal string ReadProcessOutput(string cmd, string args)
        {
            try
            {
                using Process process = StartProcess(cmd, args);
                using StreamReader streamReader = process.StandardOutput;
                process.WaitForExit();

                return streamReader.ReadToEnd().Trim();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read process output: {cmd} {args}", cmd, args);
                return string.Empty;
            }
        }

        internal string TryReadTextFromFile(string path)
        {
            try
            {
                return File.ReadAllText(path).Trim();
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogTrace(ex, "File not found: {path}", path);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read file: {path}", path);
                return string.Empty;
            }
        }

        internal byte[] TryReadBytesFromFile(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogTrace(ex, "File not found: {path}", path);
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read file: {path}", path);
                return Array.Empty<byte>();
            }
        }

        internal uint TryReadIntegerFromFile(params string[] possiblePaths)
        {
            foreach (string path in possiblePaths)
            {
                string text = TryReadTextFromFile(path);

                if (uint.TryParse(text, out uint integer))
                {
                    return integer;
                }
            }

            return 0;
        }

        internal string[] TryReadLinesFromFile(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogTrace(ex, "File not found: {path}", path);
                return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read file: {path}", path);
                return Array.Empty<string>();
            }
        }

        public virtual List<Drive> GetDriveList()
        {
            List<Drive> driveList = new List<Drive>();

            Drive drive = new Drive();

            Partition partition = new Partition();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                string driveName = string.Empty;

                try
                {
                    driveName = driveInfo.Name;

                    Volume volume = new Volume
                    {
                        FileSystem = driveInfo.DriveFormat,
                        Description = driveInfo.DriveType.ToString(),
                        Name = driveInfo.Name,
                        Caption = driveInfo.RootDirectory.FullName,
                        FreeSpace = (ulong)driveInfo.TotalFreeSpace,
                        Size = (ulong)driveInfo.TotalSize,
                        VolumeName = driveInfo.VolumeLabel
                    };

                    partition.VolumeList.Add(volume);
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogDebug(ex, "Unauthorized access to drive: {drive}", driveName);
                }
            }

            drive.PartitionList.Add(partition);

            driveList.Add(drive);

            return driveList;
        }

        public virtual List<NetworkAdapter> GetNetworkAdapterList(bool includeBytesPersec = true, bool includeNetworkAdapterConfiguration = true, int millisecondsDelayBetweenTwoMeasurements = 1000)
        {
            List<NetworkAdapter> networkAdapterList = new List<NetworkAdapter>();

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkAdapter networkAdapter = new NetworkAdapter
                {
                    MACAddress = networkInterface.GetPhysicalAddress().ToString().Trim(),
                    Description = networkInterface.Description.Trim(),
                    Name = networkInterface.Name.Trim()
                };

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    networkAdapter.Speed = (ulong)networkInterface.Speed;
                }

                if (includeNetworkAdapterConfiguration)
                {
                    foreach (UnicastIPAddressInformation addressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            networkAdapter.IPAddressList.Add(addressInformation.Address);
                        }
                    }
                }

                networkAdapterList.Add(networkAdapter);
            }

            return networkAdapterList;
        }
    }
}
