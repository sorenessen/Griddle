namespace Griddle.Platform.Recording;

public sealed class ScreenRecordingResult
{
    public required string FilePath { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    public TimeSpan Duration { get; init; }

    public bool IncludesApplicationWindows { get; init; }

    public bool IncludesSystemAudio { get; init; }

    public bool IncludesMicrophone { get; init; }
}