using Griddle.Core.Geometry;
using Griddle.Core.Models;

namespace Griddle.Core.Services;

public sealed class StrokeBuilder
{
    private Stroke? _current;

    public Stroke Begin(Point2D point, string color)
    {
        _current = new Stroke(color);
        _current.Points.Add(point);
        return _current;
    }

    public void Add(Point2D point)
    {
        _current?.Points.Add(point);
    }

    public Stroke End(Point2D point)
    {
        if (_current == null)
            throw new InvalidOperationException("No active stroke.");

        _current.Points.Add(point);

        var completed = _current;
        _current = null;

        return completed;
    }
}