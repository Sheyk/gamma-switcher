using Gamma_Switcher.Gammas;
using Gamma_Switcher.Peace;
using Gamma_Switcher.Sonar;
namespace Gamma_Switcher;

internal static class Program
{
    [STAThread]
    private static void Main()
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

        if (!createdNew) return; // App already running

        var windowTracker = new WindowChangeTracker([
            new GammaObserver(configFile),
            new SonarObserver(new SonarRepository()),
            new PeaceObserver()
        ]);

        var app = new App(windowTracker);

        Application.Run(app);
    }
}
