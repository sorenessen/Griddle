using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class MoveStrokeAction : IHistoryAction
{
    private readonly Stroke _stroke;
    private readonly double _deltaX;
    private readonly double _deltaY;

    public MoveStrokeAction(
        Stroke stroke,
        double deltaX,
        double deltaY)
    {
        _stroke = stroke;
        _deltaX = deltaX;
        _deltaY = deltaY;
    }

    public void Undo()
    {
        _stroke.Translate(
            -_deltaX,
            -_deltaY);
    }

    public void Redo()
    {
        _stroke.Translate(
            _deltaX,
            _deltaY);
    }
}