using System;
using System.IO;

namespace Griddle.App.Services;

public static class DisplayPreferenceStore
{
    private static readonly string SettingsDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Griddle");

    private static readonly string SettingsFile =
        Path.Combine(
            SettingsDirectory,
            "preferred-display.txt");

    public static void Save(string? displayName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return;
            }

            Directory.CreateDirectory(SettingsDirectory);

            File.WriteAllText(
                SettingsFile,
                displayName);
        }
        catch
        {
            // Display preference must never prevent Griddle from running.
        }
    }

    public static string? Load()
    {
        try
        {
            if (!File.Exists(SettingsFile))
            {
                return null;
            }

            var displayName =
                File.ReadAllText(SettingsFile);

            return string.IsNullOrWhiteSpace(displayName)
                ? null
                : displayName;
        }
        catch
        {
            return null;
        }
    }
}