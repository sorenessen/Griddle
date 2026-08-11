using Griddle.Core.Models;

namespace Griddle.Core.History;

public sealed class SetCalloutLabelPositionAction : IHistoryAction
{
    private readonly Stroke _stroke;
    private readonly CalloutLabelPosition _before;
    private readonly CalloutLabelPosition _after;

    public SetCalloutLabelPositionAction(
        Stroke stroke,
        CalloutLabelPosition before,
        CalloutLabelPosition after)
    {
        _stroke = stroke;
        _before = before;
        _after = after;
    }

    public void Undo()
    {
        _stroke.CalloutLabelPosition = _before;
    }

    public void Redo()
    {
        _stroke.CalloutLabelPosition = _after;
    }
}