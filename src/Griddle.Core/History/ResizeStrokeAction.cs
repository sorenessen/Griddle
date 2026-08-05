using Griddle.Core.Geometry;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class ResizeStrokeAction : IHistoryAction
{
    private readonly Stroke _stroke;
    private readonly Point2D _beforeStart;
    private readonly Point2D _beforeEnd;
    private readonly Point2D _afterStart;
    private readonly Point2D _afterEnd;

    public ResizeStrokeAction(
        Stroke stroke,
        Point2D beforeStart,
        Point2D beforeEnd,
        Point2D afterStart,
        Point2D afterEnd)
    {
        _stroke = stroke;
        _beforeStart = beforeStart;
        _beforeEnd = beforeEnd;
        _afterStart = afterStart;
        _afterEnd = afterEnd;
    }

    public void Undo()
    {
        _stroke.Points[0] = _beforeStart;
        _stroke.Points[1] = _beforeEnd;
    }

    public void Redo()
    {
        _stroke.Points[0] = _afterStart;
        _stroke.Points[1] = _afterEnd;
    }
}