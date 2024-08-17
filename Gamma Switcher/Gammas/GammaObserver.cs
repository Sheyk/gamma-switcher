namespace Gamma_Switcher.Gammas;

public class GammaObserver(ColorConfigs configFile) : IWindowObserver
{
    public string TargetWindowTitle => "EscapeFromTarkov";
    public bool Enabled => true;

    public void OnFocus()
    {
        NvidiaSettings.SetDigitalVibrance(configFile.Target!.Vibrance);
        WindowsSettings.SetGammaRamp(configFile.Target.GammaRamp);
    }

    public void OnLostFocus()
    {
        NvidiaSettings.SetDigitalVibrance(configFile.Default!.Vibrance);
        WindowsSettings.SetGammaRamp(configFile.Default.GammaRamp);
    }
}
