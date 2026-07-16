using Griddle.Core.Geometry;
using Griddle.Core.Models;
using Griddle.Core.Services;

namespace Griddle.Core.Tools;

public sealed class SelectionTool : ITool
{
    private readonly SelectionService _selection;

    public SelectionTool(
        SelectionService selection)
    {
        _selection = selection;
    }

    public string Name => "Selection";

    public Stroke? Begin(Point2D point)
    {
        return null;
    }

    public void Continue(Point2D point)
    {
    }

    public Stroke? End(Point2D point)
    {
        return null;
    }
}