using System.Collections.Generic;
using System.Linq;
using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class RenumberCalloutGroupAction : IHistoryAction
{
    private readonly List<Entry> _entries;

    public RenumberCalloutGroupAction(
        IEnumerable<(Stroke Stroke, int Before, int After)> changes)
    {
        _entries = changes
            .Select(change =>
                new Entry(
                    change.Stroke,
                    change.Before,
                    change.After))
            .ToList();
    }

    public void Undo()
    {
        foreach (var entry in _entries)
        {
            entry.Stroke.CalloutNumber =
                entry.Before;
        }
    }

    public void Redo()
    {
        foreach (var entry in _entries)
        {
            entry.Stroke.CalloutNumber =
                entry.After;
        }
    }

    private sealed record Entry(
        Stroke Stroke,
        int Before,
        int After);
}