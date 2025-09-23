﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Hardware.Info
{
    public class HardwareInfoBase
    {
        protected internal static Process StartProcess(string cmd, string args)
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

        protected internal static string ReadProcessOutput(string cmd, string args)
        {
            try
            {
                using Process process = StartProcess(cmd, args);
                using StreamReader streamReader = process.StandardOutput;
                process.WaitForExit();

                return streamReader.ReadToEnd().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        protected internal static string TryReadTextFromFile(string path)
        {
            try
            {
                return File.ReadAllText(path).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        protected internal static uint TryReadIntegerFromFile(params string[] possiblePaths)
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

        protected internal static string[] TryReadLinesFromFile(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch
            {
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
                try
                {
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
                catch (UnauthorizedAccessException)
                {
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
