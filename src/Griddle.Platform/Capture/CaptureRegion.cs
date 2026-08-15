namespace Griddle.Platform.Capture;

public readonly record struct CaptureRegion(
    int X,
    int Y,
    int Width,
    int Height);
