using System;
using System.Collections.Generic;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class AddStrokeAction : IHistoryAction
{
    private readonly IList<Stroke> _strokes;
    private readonly Stroke _stroke;
    private readonly int _index;

    public AddStrokeAction(
        IList<Stroke> strokes,
        Stroke stroke,
        int index)
    {
        _strokes = strokes;
        _stroke = stroke;
        _index = index;
    }

    public void Undo()
    {
        _strokes.Remove(_stroke);
    }

    public void Redo()
    {
        var index = Math.Min(
            _index,
            _strokes.Count);

        _strokes.Insert(
            index,
            _stroke);
    }
}