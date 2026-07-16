namespace Griddle.Core.History;

public interface IHistoryAction
{
    void Undo();
    void Redo();
}