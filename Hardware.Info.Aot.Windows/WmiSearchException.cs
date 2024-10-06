using Windows.Win32.Foundation;

namespace Hardware.Info.Aot.Windows;

public class WmiSearchException: Exception
{
    internal WmiSearchException(HRESULT hResult, string message) : base($"{message} ({hResult.Value:X})")
    {
        this.HResult = hResult;
    }
}