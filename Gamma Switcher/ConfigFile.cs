using Gamma_Switcher.Gammas;
using System.Text.Json;
namespace Gamma_Switcher;

public static class ConfigFile
{
    private const string Path = "./colors.json";

    public static ColorConfigs Read()
    {
        if (!File.Exists(Path))
        {
            var newConfig = new ColorConfigs(null, null);
            var newConfigJson = JsonSerializer.Serialize(newConfig);
            File.WriteAllText(Path, newConfigJson);
            return newConfig;
        }

        var json = File.ReadAllText(Path);
        return JsonSerializer.Deserialize<ColorConfigs>(json)!;
    }

    public static void Write(ColorConfigs config)
    {
        var json = JsonSerializer.Serialize(config);
        File.WriteAllText(Path, json);
    }
}

public record ColorConfigs(ColorConfig? Default, ColorConfig? Target)
{
    public ColorConfig? Default { get; set; } = Default;
    public ColorConfig? Target { get; set; } = Target;
}

public record ColorConfig(int Vibrance, GammaRamp.RAMP GammaRamp);
