using Griddle.Core.Models;

namespace Griddle.Core.Tools;

public sealed class PenTool : ITool
{
    public PenTool(PenSettings settings)
    {
        Settings = settings;
    }

    public string Name => "Pen";

    public PenSettings Settings { get; }
}