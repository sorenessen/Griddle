using Griddle.Core.Models;
using Griddle.Core.Tools;

namespace Griddle.App.ViewModels;

public sealed class ToolbarViewModel
{
    public ToolbarViewModel(PenTool pen)
    {
        Pen = pen;
    }

    public PenTool Pen { get; }
}