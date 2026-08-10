using System;
using System.IO;
using System.Text.Json;
using Avalonia;

namespace Griddle.App.Services;

public static class ToolbarPositionStore
{
    private static readonly string SettingsDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Griddle");

    private static readonly string SettingsFile =
        Path.Combine(
            SettingsDirectory,
            "toolbar-position.json");

    public static void Save(PixelPoint position)
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);

            var settings = new ToolbarPositionSettings
            {
                X = position.X,
                Y = position.Y
            };

            var json = JsonSerializer.Serialize(settings);

            File.WriteAllText(
                SettingsFile,
                json);
        }
        catch
        {
            // Position persistence should never prevent Griddle from running.
        }
    }

    public static PixelPoint? Load()
    {
        try
        {
            if (!File.Exists(SettingsFile))
            {
                return null;
            }

            var json =
                File.ReadAllText(SettingsFile);

            var settings =
                JsonSerializer.Deserialize<ToolbarPositionSettings>(
                    json);

            if (settings is null)
            {
                return null;
            }

            return new PixelPoint(
                settings.X,
                settings.Y);
        }
        catch
        {
            return null;
        }
    }

    private sealed class ToolbarPositionSettings
    {
        public int X { get; set; }

        public int Y { get; set; }
    }
}