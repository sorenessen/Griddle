using Griddle.Core.Captures;

namespace Griddle.Core.Documents;

public sealed class CaptureDocument
{
    public Guid Id { get; set; }

    public CaptureKind Kind { get; set; }

    public DateTime CreatedAt { get; set; }

    public string FileName { get; set; } =
        string.Empty;

    public int Width { get; set; }

    public int Height { get; set; }

    public string? DisplayName { get; set; }

    public bool IncludesAnnotations { get; set; }

    public TimeSpan? Duration { get; set; }
}