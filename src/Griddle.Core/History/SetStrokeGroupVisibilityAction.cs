using System.Collections.Generic;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class SetStrokeGroupVisibilityAction : IHistoryAction
{
    private readonly List<Stroke> _strokes;
    private readonly bool _before;
    private readonly bool _after;

    public SetStrokeGroupVisibilityAction(
        IEnumerable<Stroke> strokes,
        bool before,
        bool after)
    {
        _strokes = new List<Stroke>(strokes);
        _before = before;
        _after = after;
    }

    public void Undo()
    {
        foreach (var stroke in _strokes)
        {
            stroke.IsVisible = _before;
        }
    }

    public void Redo()
    {
        foreach (var stroke in _strokes)
        {
            stroke.IsVisible = _after;
        }
    }
}