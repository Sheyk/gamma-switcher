namespace Gamma_Switcher.Peace;

public class PeaceObserver : IWindowObserver
{
    private const string EqualizerApoPath = "C:\\Program Files\\EqualizerAPO\\config\\";
    private const string TarkovConfig = EqualizerApoPath + "tarkov.txt";
    private const string DefaultConfig = EqualizerApoPath + "config.txt";
    private const string BackupDefaultConfig = EqualizerApoPath + "config_backup.txt";

    public PeaceObserver()
    {
        if (!File.Exists(TarkovConfig))
            return;

        if (!File.Exists(BackupDefaultConfig))
            File.Copy(DefaultConfig, BackupDefaultConfig, true);
    }

    public string TargetWindowTitle => "EscapeFromTarkov";
    public bool Enabled { get; } = File.Exists(TarkovConfig);

    public void OnFocus() =>
        this.RunIfEnabled(() => File.Copy(TarkovConfig, DefaultConfig, true));

    public void OnLostFocus() =>
        this.RunIfEnabled(() => File.Copy(BackupDefaultConfig, DefaultConfig, true));
}
