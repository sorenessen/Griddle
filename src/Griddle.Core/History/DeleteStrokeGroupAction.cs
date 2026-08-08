using System.Collections.Generic;
using System.Linq;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class DeleteStrokeGroupAction : IHistoryAction
{
    private readonly List<Stroke> _strokes;
    private readonly List<Entry> _entries;

    public DeleteStrokeGroupAction(
        List<Stroke> strokes,
        IEnumerable<(Stroke Stroke, int Index)> entries)
    {
        _strokes = strokes;

        _entries = entries
            .Select(entry =>
                new Entry(
                    entry.Stroke,
                    entry.Index))
            .OrderBy(entry => entry.Index)
            .ToList();
    }

    public void Undo()
    {
        foreach (var entry in _entries)
        {
            _strokes.Insert(
                entry.Index,
                entry.Stroke);
        }
    }

    public void Redo()
    {
        foreach (var entry in _entries
                     .OrderByDescending(entry => entry.Index))
        {
            _strokes.RemoveAt(
                entry.Index);
        }
    }

    private sealed record Entry(
        Stroke Stroke,
        int Index);
}