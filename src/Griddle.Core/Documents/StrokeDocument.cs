using Griddle.Core.Models;

namespace Griddle.Core.Documents;

public sealed class StrokeDocument
{
    public Guid Id { get; set; }

    public StrokeColor Color { get; set; }

    public double Thickness { get; set; }

    public double Opacity { get; set; }

    public StrokeKind Kind { get; set; }

    public int? CalloutNumber { get; set; }

    public Guid? CalloutGroupId { get; set; }

    public CalloutLabelPosition CalloutLabelPosition { get; set; } =
        CalloutLabelPosition.Target;

    public string Text { get; set; } =
        string.Empty;

    public List<PointDocument> Points { get; set; } =
        new();

    public bool IsVisible { get; set; } =
        true;
}