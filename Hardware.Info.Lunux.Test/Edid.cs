using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hardware.Info.Test;

internal static class Edid
{
    static void EDID()
    {
        //string[] files = Directory.GetFiles("/sys/class/drm/", "edid", SearchOption.AllDirectories);

        //foreach (string file in files)
        string[] folders = Directory.GetDirectories("/sys/class/drm/", "card*");
        foreach (string folder in folders)
            foreach (string folder2 in Directory.GetDirectories(folder, "card*"))
                foreach (string file in Directory.GetFiles(folder2, "edid"))
                {
                    Console.WriteLine($"Reading EDID information from file: {file}");
                    byte[] data = File.ReadAllBytes(file);

                    if (data.Length == 0)
                        continue;

                    // Extract the manufacturer name and serial number
                    string manufacturerName = Encoding.ASCII.GetString(data, 0x08, 0x0F - 0x08 + 1);
                    string serialNumber = Encoding.ASCII.GetString(data, 0x0E, 0x17 - 0x0E + 1);

                    Console.WriteLine($"Manufacturer name: {manufacturerName}");
                    Console.WriteLine($"Serial number: {serialNumber}");

                    // Extract the monitor name
                    string monitorName = Encoding.ASCII.GetString(data, 0x36, 0x7F - 0x36 + 1);
                    Console.WriteLine($"Monitor name: {monitorName}");

                    // Extract the EDID version and revision numbers
                    byte versionNumber = data[0x12];
                    byte revisionNumber = data[0x13];
                    Console.WriteLine($"EDID version: {versionNumber}.{revisionNumber}");

                    // Extract the preferred timing mode
                    byte preferredTimingIndex = data[0x36];
                    ushort horizontalResolution = (ushort)((data[0x38] << 8) | data[0x39]);
                    ushort verticalResolution = (ushort)((data[0x3A] << 8) | data[0x3B]);
                    byte refreshRate = data[0x3E];
                    Console.WriteLine($"Preferred timing mode: {horizontalResolution}x{verticalResolution}@{refreshRate}Hz (index {preferredTimingIndex})");

                    // Extract the supported video timings
                    Console.WriteLine("Supported video timings:");
                    for (int i = 0x38; i <= 0x4D; i += 2)
                    {
                        byte timingByte1 = data[i];
                        byte timingByte2 = data[i + 1];

                        if (timingByte1 == 0x01 && timingByte2 == 0x01)
                        {
                            break;
                        }

                        byte hActive = (byte)(((timingByte1 >> 4) & 0x0F) + 31);
                        byte hBlank = (byte)(((timingByte1 & 0x0F) << 4) | ((timingByte2 >> 4) & 0x0F) + 40);
                        byte vActive = (byte)(((timingByte2 & 0x0F) << 4) | ((timingByte1 >> 2) & 0x0F) + 2);
                        byte vBlank = (byte)(((timingByte1 & 0x03) << 6) | ((timingByte2 >> 2) & 0x3F) + 3);
                        byte hSyncOffset = (byte)(((timingByte1 >> 2) & 0x03) | ((timingByte2 >> 6) & 0x03) << 2);
                        byte hSyncPulse = (byte)(((timingByte1 >> 6) & 0x03) | ((timingByte2 >> 4) & 0x03) << 2);
                        byte vSyncOffset = (byte)((timingByte2 >> 2) & 0x0F);
                        byte vSyncPulse = (byte)((timingByte2 & 0x03) | ((timingByte1 >> 4) << 2));

                        Console.WriteLine($"- {hActive}x{vActive}@{hBlank - hActive}x{vBlank - vActive}@{refreshRate}Hz");
                    }

                    // Extract the supported video input types
                    Console.WriteLine("Supported video input types:");
                    byte videoInputFlags = data[0x18];
                    if ((videoInputFlags & 0x80) != 0) Console.WriteLine("- Digital input");
                    if ((videoInputFlags & 0x40) != 0) Console.WriteLine("- Analog input");
                    if ((videoInputFlags & 0x20) != 0) Console.WriteLine("- Separate syncs");
                    if ((videoInputFlags & 0x10) != 0) Console.WriteLine("- Composite syncs");
                    if ((videoInputFlags & 0x08) != 0) Console.WriteLine("- Sync on green");
                    if ((videoInputFlags & 0x04) != 0) Console.WriteLine("- Serration on vertical sync");

                    // Extract the supported display sizes and aspect ratios
                    Console.WriteLine("Supported display sizes and aspect ratios:");
                    byte displaySizeFlags = data[0x15];
                    if ((displaySizeFlags & 0x80) != 0) Console.WriteLine("- Display size is aspect ratio");
                    if ((displaySizeFlags & 0x40) != 0) Console.WriteLine("- Display size is in centimeters");
                    byte displayWidthCm = data[0x1C];
                    byte displayHeightCm = data[0x1D];
                    byte displayWidthInches = (byte)(displayWidthCm / 2.54);
                    byte displayHeightInches = (byte)(displayHeightCm / 2.54);
                    Console.WriteLine($"- Display size: {displayWidthInches}\"x{displayHeightInches}\"");

                    // Extract the supported audio formats
                    Console.WriteLine("Supported audio formats:");
                    byte audioFlags = data[0x18];
                    if ((audioFlags & 0x08) != 0) Console.WriteLine("- LPCM");
                    if ((audioFlags & 0x04) != 0) Console.WriteLine("- AC-3");
                    if ((audioFlags & 0x02) != 0) Console.WriteLine("- MPEG1");
                    if ((audioFlags & 0x01) != 0) Console.WriteLine("- MP3");
                }
    }

    static void Monitor()
    {
        string resolution = null;
        string refreshRate = null;
        string edidFile = null;

        // Get the display resolution and refresh rate using xrandr
        var xrandrProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "xrandr",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        while (!xrandrProcess.StandardOutput.EndOfStream)
        {
            string line = xrandrProcess.StandardOutput.ReadLine();

            if (line.Contains("*"))
            {
                resolution = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
                refreshRate = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
                break;
            }
        }

        Console.WriteLine($"Monitor resolution: {resolution}");
        Console.WriteLine($"Monitor refreshRate: {refreshRate}");

        // Look up the EDID file based on the display configuration
        var drmDirectory = new DirectoryInfo("/sys/class/drm");
        foreach (var cardDirectory in drmDirectory.GetDirectories("card*"))
        {
            foreach (var connectorDirectory in cardDirectory.GetDirectories("card*"))
            {
                var edidPath = Path.Combine(connectorDirectory.FullName, "edid");

                if (File.Exists(edidPath))
                {
                    edidFile = edidPath;
                    break;
                }
            }

            if (edidFile != null)
            {
                break;
            }
        }

        // Read the EDID file and extract the manufacturer and model information
        if (edidFile != null)
        {
            var edidBytes = File.ReadAllBytes(edidFile);
            if (edidBytes.Length > 0)
            {
                var manufacturerCode = BitConverter.ToUInt16(edidBytes, 0x08);
                var modelCode = BitConverter.ToUInt16(edidBytes, 0x0a);
            }
        }
    }
}