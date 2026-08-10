using Griddle.Core.Models;

namespace Griddle.Core.Services;

public sealed class SelectionService
{
    private readonly List<Stroke> _selectedStrokes = new();

    public Stroke? SelectedStroke { get; private set; }

    public IReadOnlyList<Stroke> SelectedStrokes =>
        _selectedStrokes;

    public bool HasSelection =>
        SelectedStroke is not null;

    public void Select(Stroke stroke)
    {
        _selectedStrokes.Clear();
        _selectedStrokes.Add(stroke);

        SelectedStroke = stroke;
    }

    public void SelectMany(
        IEnumerable<Stroke> strokes,
        Stroke primary)
    {
        _selectedStrokes.Clear();
        _selectedStrokes.AddRange(strokes);

        SelectedStroke = primary;
    }

    public bool IsSelected(Stroke stroke)
    {
        return _selectedStrokes.Contains(stroke);
    }

    public void Toggle(Stroke stroke)
    {
        if (_selectedStrokes.Contains(stroke))
        {
            _selectedStrokes.Remove(stroke);

            if (ReferenceEquals(
                    SelectedStroke,
                    stroke))
            {
                SelectedStroke =
                    _selectedStrokes.Count > 0
                        ? _selectedStrokes[^1]
                        : null;
            }

            return;
        }

        _selectedStrokes.Add(stroke);
        SelectedStroke = stroke;
    }

    public void Clear()
    {
        _selectedStrokes.Clear();
        SelectedStroke = null;
    }
}