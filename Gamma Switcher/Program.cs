using EftSettings;
namespace Gamma_Switcher;

static class Program
{
    [STAThread]
    static void Main()
    {
        var configFile = ConfigFile.Read();

        if (configFile.Default == null)
        {
            var vibrance = NvidiaSettings.GetDigitalVibrance();
            var gammaRamp = WindowsSettings.GetGammaRamp();
            configFile.Default = new ColorConfig(vibrance, gammaRamp);
            ConfigFile.Write(configFile);
            return;
        }

        if (configFile.Target == null)
        {
            var vibrance = NvidiaSettings.GetDigitalVibrance();
            var gammaRamp = WindowsSettings.GetGammaRamp();
            configFile.Target = new ColorConfig(vibrance, gammaRamp);
            ConfigFile.Write(configFile);
            return;
        }
        
        using var mutex = new Mutex(true, "Gamma Switcher", out var createdNew);

        if (!createdNew)  return; // App already running

        Application.Run(new App(configFile));

    }
}
