using Griddle.Core.Geometry;

namespace Griddle.Core.Models;

public sealed class Stroke
{
    public List<Point2D> Points { get; } = new();
}