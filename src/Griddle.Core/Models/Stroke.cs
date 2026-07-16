using System;
using Griddle.Core.Geometry;

namespace Griddle.Core.Models;

public sealed class Stroke
{
    public Stroke(
        StrokeColor color,
        double thickness,
        double opacity,
        StrokeKind kind = StrokeKind.Freehand,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Color = color;
        Thickness = thickness;
        Opacity = opacity;
        Kind = kind;
    }

    public Guid Id { get; }
    public StrokeColor Color { get; }
    public double Thickness { get; }
    public double Opacity { get; }
    public StrokeKind Kind { get; }
    public List<Point2D> Points { get; } = new();

    public void Translate(
        double deltaX,
        double deltaY)
    {
        for (var index = 0; index < Points.Count; index++)
        {
            var point = Points[index];

            Points[index] = new Point2D(
                point.X + deltaX,
                point.Y + deltaY);
        }
    }

}