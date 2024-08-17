using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Net;
namespace Gamma_Switcher.Sonar;

public class SonarRepository
{
    private readonly string _connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            @"SteelSeries\GG\apps\sonar\db\database.db")
    }.ToString();

    private int? _lastWorkingPort;

    public IEnumerable<SonarConfiguration> AvailableGamingConfigurations =>
        GetGamingConfigurations().OrderBy(s => s.Name);

    public bool IsSonarRunning => Process.GetProcessesByName("SteelSeriesSonar").Length > 0;

    public IEnumerable<SonarConfiguration> GetGamingConfigurations()
    {
        // Get all the available profiles from SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();

        using var sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "select id, name, vad from configs where vad == 1";
        using var sqliteDataReader = sqliteCommand.ExecuteReader();

        while (sqliteDataReader.Read())
        {
            var id = sqliteDataReader.GetString(0);
            var name = sqliteDataReader.GetString(1);
            yield return new SonarConfiguration(id, name);
        }
    }

    public async Task ChangeSelectedGamingConfiguration(
        SonarConfiguration config,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(config.Id))
            return;

        var processesByName = Process.GetProcessesByName("SteelSeriesSonar");
        if (processesByName.Length <= 0 || cancellationToken.IsCancellationRequested)
            return;

        var potentialPorts = processesByName.SelectMany(p => NetworkHelper.GetPortById(p.Id, false));

        potentialPorts = _lastWorkingPort != null ? potentialPorts.Prepend(_lastWorkingPort.Value) : potentialPorts;
        _lastWorkingPort = null;

        using var httpClient = new HttpClient();

        foreach (var potentialPort in potentialPorts.Distinct())
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            var httpResponseMessage = await httpClient.PutAsync(
                $"http://localhost:{potentialPort}/configs/{config.Id}/select",
                new StringContent(""),
                cancellationToken).ContinueWith(t => t.IsCompletedSuccessfully ? t.Result : null);

            if (httpResponseMessage?.StatusCode == HttpStatusCode.OK)
            {
                _lastWorkingPort = potentialPort;
                break;
            }
        }
    }

    public string GetSelectedGamingConfiguration()
    {
        // Get all the available profiles from SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();

        using var sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "select config_id, vad from selected_config where vad == 1";
        using var sqliteDataReader = sqliteCommand.ExecuteReader();
        if (!sqliteDataReader.Read())
            throw new InvalidOperationException("Unable to check for selected gaming profile");
        return sqliteDataReader.GetString(0);
    }
}

public record SonarConfiguration(string? Id, string Name);
