using System;

namespace Hardware.Info.Test;

class TestSuite
{
    internal enum Compiler
    {
        Unknown = 0,
        Jit,
        Aot
    }

    internal enum Architecture
    {
        Unknown = 0,
        x64,
        arm64
    }
    
    
    public static bool HasFlag(string[] args, string flagName)
    {
        return Array.IndexOf(args, flagName) >= 0;
    }
        
    public static string? ReadOption(string[] args, string optionName)
    {
        var index = Array.IndexOf(args, optionName);
        if (index < 0 | index >= args.Length)
        {
            return null;
        }
            
        return args[index+1];
    }
        
    public static TEnum ReadOption<TEnum>(string[] args, string optionName, TEnum defaultValue) where TEnum : struct, System.Enum
    {
        var value = ReadOption(args, optionName);
        if (value == null)
        {
            return defaultValue;
        }
            
        var parsedValue = Enum.Parse<TEnum>(value, true);
        if (!Enum.IsDefined(parsedValue))
        {
            return defaultValue;
        }

        return parsedValue;
    }
}