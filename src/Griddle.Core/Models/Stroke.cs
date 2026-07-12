using Griddle.Core.Geometry;

namespace Griddle.Core.Models;

public sealed class Stroke
{
    public Stroke(string color)
    {
        Color = color;
    }

    public string Color { get; }

    public List<Point2D> Points { get; } = new();
}