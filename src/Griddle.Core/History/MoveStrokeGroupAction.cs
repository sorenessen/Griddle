using System.Collections.Generic;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class MoveStrokeGroupAction : IHistoryAction
{
    private readonly List<Stroke> _strokes;
    private readonly double _deltaX;
    private readonly double _deltaY;

    public MoveStrokeGroupAction(
        IEnumerable<Stroke> strokes,
        double deltaX,
        double deltaY)
    {
        _strokes = new List<Stroke>(strokes);
        _deltaX = deltaX;
        _deltaY = deltaY;
    }

    public void Undo()
    {
        foreach (var stroke in _strokes)
        {
            stroke.Translate(
                -_deltaX,
                -_deltaY);
        }
    }

    public void Redo()
    {
        foreach (var stroke in _strokes)
        {
            stroke.Translate(
                _deltaX,
                _deltaY);
        }
    }
}