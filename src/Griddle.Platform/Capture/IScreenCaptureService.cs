namespace Griddle.Platform.Capture;

public interface IScreenCaptureService
{
    Task<ScreenCaptureResult> CaptureAsync(
        CaptureRegion region,
        ScreenCaptureOptions? options = null);
}
