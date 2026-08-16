using Griddle.Platform.Capture;

namespace Griddle.Platform.Recording;

public sealed class ScreenRecordingOptions
{
    public required CaptureRegion Region { get; init; }

    public bool IncludeApplicationWindows { get; init; } = true;

    public bool CaptureSystemAudio { get; init; }

    public bool CaptureMicrophone { get; init; }

    public string? MicrophoneDeviceId { get; init; }

    public int? FramesPerSecond { get; init; }

    public required string OutputFilePath { get; init; }
}