namespace Griddle.Platform.Capture;

public sealed class ScreenCaptureResult
{
    public required byte[] ImageData { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }
}
