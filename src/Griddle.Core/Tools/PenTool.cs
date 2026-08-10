using Griddle.Core.Geometry;
using Griddle.Core.Models;
using Griddle.Core.Services;

namespace Griddle.Core.Tools;

public sealed class PenTool : ITool
{
    private readonly StrokeBuilder _strokeBuilder = new();

    public PenTool(AnnotationStyle settings)
    {
        Settings = settings;
    }

    public string Name => "Pen";

    public AnnotationStyle Settings { get; }

    public PenPreset Preset { get; set; } =
        PenPreset.Pen;

    public Stroke Begin(Point2D point)
    {
        if (Preset == PenPreset.Highlighter)
        {
            return _strokeBuilder.Begin(
                point,
                StrokeColor.Yellow,
                thickness: 16,
                opacity: 0.18);
        }

        return _strokeBuilder.Begin(
            point,
            Settings.Color,
            Settings.Thickness,
            Settings.Opacity);
    }

    public void Continue(Point2D point)
    {
        _strokeBuilder.Add(point);
    }

    public Stroke End(Point2D point)
    {
        return _strokeBuilder.End(point);
    }
}