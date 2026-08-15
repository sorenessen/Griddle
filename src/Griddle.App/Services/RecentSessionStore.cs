using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Griddle.App.Services;

public static class RecentSessionStore
{
    private const int MaxRecentSessions = 10;

    private static readonly string SettingsDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Griddle");

    private static readonly string SettingsFile =
        Path.Combine(
            SettingsDirectory,
            "recent-sessions.json");

    public static IReadOnlyList<string> Load()
    {
        try
        {
            if (!File.Exists(SettingsFile))
            {
                return Array.Empty<string>();
            }

            var json =
                File.ReadAllText(SettingsFile);

            var paths =
                JsonSerializer.Deserialize<List<string>>(
                    json);

            if (paths is null)
            {
                return Array.Empty<string>();
            }

            var validPaths =
                paths
                    .Where(path =>
                        !string.IsNullOrWhiteSpace(path) &&
                        File.Exists(path))
                    .Distinct()
                    .Take(MaxRecentSessions)
                    .ToList();

            if (validPaths.Count != paths.Count)
            {
                Save(validPaths);
            }

            return validPaths;
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public static void Add(string? sessionFilePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(
                    sessionFilePath))
            {
                return;
            }

            var paths =
                Load()
                    .Where(path =>
                        !string.Equals(
                            path,
                            sessionFilePath,
                            StringComparison.Ordinal))
                    .ToList();

            paths.Insert(
                0,
                sessionFilePath);

            Save(
                paths.Take(
                    MaxRecentSessions));
        }
        catch
        {
            // Recent sessions should never prevent
            // Griddle from running.
        }
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
            }
        }
        catch
        {
            // Recent-session cleanup is non-critical.
        }
    }

    private static void Save(
        IEnumerable<string> paths)
    {
        try
        {
            Directory.CreateDirectory(
                SettingsDirectory);

            var json =
                JsonSerializer.Serialize(
                    paths.ToList());

            File.WriteAllText(
                SettingsFile,
                json);
        }
        catch
        {
            // Recent-session persistence is non-critical.
        }
    }
}