namespace Griddle.Platform.Recording;

public interface IScreenRecordingService
{
    bool IsRecording { get; }

    Task StartAsync(
        ScreenRecordingOptions options);

    Task<ScreenRecordingResult> StopAsync();
}