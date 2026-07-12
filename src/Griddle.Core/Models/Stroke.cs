using Griddle.Core.Geometry;

namespace Griddle.Core.Models;

public sealed class Stroke
{
    public Stroke(
        StrokeColor color,
        double thickness,
        double opacity)
    {
        Color = color;
        Thickness = thickness;
        Opacity = opacity;
    }

    public StrokeColor Color { get; }

    public double Thickness { get; }

    public double Opacity { get; }

    public List<Point2D> Points { get; } = new();
}