namespace Gamma_Switcher.Sonar;

public class SonarObserver : IWindowObserver
{
    private readonly SonarConfiguration? _defaultConfig;
    private readonly SonarRepository? _repository;
    private readonly SonarConfiguration? _targetConfig;

    public SonarObserver(SonarRepository repository)
    {
        if (!repository.IsSonarRunning)
        {
            Enabled = false;
            return;
        }
        _repository = repository;
        var configs = _repository.GetGamingConfigurations().ToArray();
        _defaultConfig = configs.First(x => x.Name == "Flat");
        _targetConfig = configs.First(x => x.Name == "Escape from Tarkov");
    }

    public string TargetWindowTitle => "EscapeFromTarkov";
    public bool Enabled { get; set; }

    public void OnFocus() =>
        this.RunIfEnabled(() => _repository!.ChangeSelectedGamingConfiguration(_targetConfig!, new CancellationToken()));

    public void OnLostFocus() =>
        this.RunIfEnabled(() => _repository!.ChangeSelectedGamingConfiguration(_defaultConfig!, new CancellationToken()));
}
