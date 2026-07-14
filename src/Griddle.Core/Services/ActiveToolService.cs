using Griddle.Core.Tools;

namespace Griddle.Core.Services;

public sealed class ActiveToolService
{
    public ActiveToolService(ITool initialTool)
    {
        Current = initialTool;
    }

    public ITool Current { get; set; }
}