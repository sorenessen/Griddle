using Griddle.Core.Models;

namespace Griddle.Core.Services;

public sealed class SelectionService
{
    public Stroke? SelectedStroke { get; private set; }

    public bool HasSelection =>
        SelectedStroke is not null;

    public void Select(Stroke stroke)
    {
        SelectedStroke = stroke;
    }

    public void Clear()
    {
        SelectedStroke = null;
    }
}