using System;
using System.Collections.Generic;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class DeleteStrokeAction : IHistoryAction
{
    private readonly IList<Stroke> _strokes;
    private readonly Stroke _stroke;
    private readonly int _index;

    public DeleteStrokeAction(
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
        var index = Math.Min(
            _index,
            _strokes.Count);

        _strokes.Insert(
            index,
            _stroke);
    }

    public void Redo()
    {
        _strokes.Remove(_stroke);
    }
}