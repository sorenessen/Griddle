namespace Griddle.Core.Documents;

public sealed class GriddleDocument
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } =
        CurrentVersion;

    public Guid SessionId { get; set; }

    public string Name { get; set; } =
        "Untitled";

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public List<StrokeDocument> Strokes { get; set; } =
        new();
}