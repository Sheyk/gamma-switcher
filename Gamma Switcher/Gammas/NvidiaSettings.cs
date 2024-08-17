using NvAPIWrapper;
using NvAPIWrapper.Display;
namespace Gamma_Switcher.Gammas;

public static class NvidiaSettings
{
    static NvidiaSettings()
    {
        NVIDIA.Initialize();
    }

    public static int GetDigitalVibrance()
    {
        var display = DisplayDevice.GetGDIPrimaryDisplayDevice();
        if (display == null)
            throw new Exception("No display found");

        return display.Output.DigitalVibranceControl.CurrentLevel;
    }

    public static void SetDigitalVibrance(int level)
    {
        var display = DisplayDevice.GetGDIPrimaryDisplayDevice();
        if (display == null)
            throw new Exception("No display found");

        display.Output.DigitalVibranceControl.CurrentLevel = level;
    }
}
