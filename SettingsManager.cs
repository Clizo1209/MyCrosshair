using System.IO;
using System.Text.Json;

namespace MyCrosshair;

public static class SettingsManager
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MyCrosshair", "settings.json");

    public static CrosshairSettings Current { get; private set; } = new();
    public static event Action? Changed;

    public static void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                Current = JsonSerializer.Deserialize<CrosshairSettings>(json) ?? new CrosshairSettings();
            }
        }
        catch
        {
            Current = new CrosshairSettings();
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch { }
    }

    public static void NotifyChanged() => Changed?.Invoke();
}
