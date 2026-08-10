using Griddle.Core.Geometry;
using Griddle.Core.Models;

namespace Griddle.Core.Tools;

public sealed class TextTool : ITool
{
    private readonly AnnotationStyle _style;

    public TextTool(
        AnnotationStyle style)
    {
        _style = style;
    }
    
    public string Name => "Text";

    public Stroke? Begin(Point2D point)
    {
        return new Stroke(
            _style.Color,
            thickness: 1,
            opacity: 1,
            StrokeKind.Text)
        {
            Points =
            {
                point
            }
        };
    }

    public void Continue(Point2D point)
    {
    }

    public Stroke? End(Point2D point)
    {
        return null;
    }
}