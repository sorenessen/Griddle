namespace Griddle.Core.Captures;

public sealed class GriddleCapture
{
    public Guid Id { get; init; } =
        Guid.NewGuid();

    public CaptureKind Kind { get; init; }

    public DateTime CreatedAt { get; init; } =
        DateTime.UtcNow;

    public string FileName { get; set; } =
        string.Empty;

    public int Width { get; set; }

    public int Height { get; set; }

    public string? DisplayName { get; set; }

    public bool IncludesAnnotations { get; set; }
}