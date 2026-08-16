using System;
using System.IO;
using System.Text.Json;

namespace Griddle.App.Services;

public static class AutoRecoveryStore
{
    private static readonly string RecoveryDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Griddle",
            "recovery");

    private static readonly string RecoveryFile =
        Path.Combine(
            RecoveryDirectory,
            "recovery.griddle");

    private static readonly string StateFile =
        Path.Combine(
            RecoveryDirectory,
            "recovery-state.json");

    public static void Save(
        string documentJson,
        string? originalFilePath)
    {
        try
        {
            Directory.CreateDirectory(
                RecoveryDirectory);

            File.WriteAllText(
                RecoveryFile,
                documentJson);

            var state =
                new AutoRecoveryState
                {
                    OriginalFilePath =
                        originalFilePath,

                    SavedAtUtc =
                        DateTime.UtcNow
                };

            var stateJson =
                JsonSerializer.Serialize(
                    state);

            File.WriteAllText(
                StateFile,
                stateJson);
        }
        catch
        {
            // Recovery failure must never prevent
            // Griddle from continuing to run.
        }
    }

    public static bool Exists()
    {
        try
        {
            return
                File.Exists(RecoveryFile) &&
                File.Exists(StateFile);
        }
        catch
        {
            return false;
        }
    }

    public static string? LoadDocument()
    {
        try
        {
            if (!File.Exists(
                    RecoveryFile))
            {
                return null;
            }

            return File.ReadAllText(
                RecoveryFile);
        }
        catch
        {
            return null;
        }
    }

    public static AutoRecoveryState? LoadState()
    {
        try
        {
            if (!File.Exists(
                    StateFile))
            {
                return null;
            }

            var json =
                File.ReadAllText(
                    StateFile);

            return JsonSerializer
                .Deserialize<AutoRecoveryState>(
                    json);
        }
        catch
        {
            return null;
        }
    }

    public static void Clear()
    {
        try
        {
            if (Directory.Exists(
                    RecoveryDirectory))
            {
                Directory.Delete(
                    RecoveryDirectory,
                    recursive: true);
            }
        }
        catch
        {
            // Recovery cleanup is non-critical.
        }
    }
}

public sealed class AutoRecoveryState
{
    public string? OriginalFilePath { get; set; }

    public DateTime SavedAtUtc { get; set; }
}