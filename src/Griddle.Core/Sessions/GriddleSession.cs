using Griddle.Core.Models;

namespace Griddle.Core.Sessions;

public sealed class GriddleSession
{
    public const int CurrentDocumentVersion = 1;

    public int DocumentVersion { get; init; } =
        CurrentDocumentVersion;

    public Guid Id { get; init; } =
        Guid.NewGuid();

    public string Name { get; set; } =
        "Untitled";

    public DateTime CreatedAt { get; init; } =
        DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } =
        DateTime.UtcNow;

    public List<Stroke> Strokes { get; set; } =
        new();
}