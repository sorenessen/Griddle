using Griddle.Core.Geometry;
using Griddle.Core.Models;

namespace Griddle.Core.Tools;

public sealed class RectangleTool : ITool
{
    private Point2D? _startPoint;
    private Stroke? _current;

    public string Name => "Rectangle";

    public Stroke Begin(Point2D point)
    {
        _startPoint = point;

        _current = new Stroke(
            StrokeColor.Red,
            thickness: 4,
            opacity: 1.0,
            kind: StrokeKind.Rectangle);

        _current.Points.Add(point);
        _current.Points.Add(point);

        return _current;
    }

    public void Continue(Point2D point)
    {
        if (_current is null || _startPoint is null)
        {
            return;
        }

        _current.Points[1] = point;
    }

    public Stroke End(Point2D point)
    {
        if (_current is null || _startPoint is null)
        {
            throw new InvalidOperationException("No active rectangle.");
        }

        _current.Points[1] = point;

        var completed = _current;

        _current = null;
        _startPoint = null;

        return completed;
    }
}