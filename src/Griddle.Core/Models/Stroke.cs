using Griddle.Core.Geometry;

namespace Griddle.Core.Models;

public sealed class Stroke
{
    public Stroke(StrokeColor color, double thickness)
    {
        Color = color;
        Thickness = thickness;
    }

    public StrokeColor Color { get; }

    public double Thickness { get; }

    public List<Point2D> Points { get; } = new();
}