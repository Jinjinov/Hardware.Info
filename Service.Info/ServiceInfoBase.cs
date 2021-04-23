using System;
using System.Diagnostics;
using System.IO;

namespace Service.Info
{
    internal class ServiceInfoBase
    {
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

        internal static string ReadProcessOutput(string cmd, string args)
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

        internal static string TryReadFileText(string path)
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

        internal static string[] TryReadFileLines(string path)
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
    }
}
