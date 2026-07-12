using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Griddle.Core.Geometry;
using Griddle.Core.Models;
using Griddle.Core.Services;

namespace Griddle.App.Controls;

public sealed class DrawingCanvas : Control
{
    private readonly List<Stroke> _strokes = new();
    private readonly StrokeBuilder _strokeBuilder = new();

    private Stroke? _activeStroke;

    public void BeginStroke(Point point)
    {
        _activeStroke = _strokeBuilder.Begin(ToPoint2D(point));
        InvalidateVisual();
    }

    public void ContinueStroke(Point point)
    {
        if (_activeStroke is null)
        {
            return;
        }

        _strokeBuilder.Add(ToPoint2D(point));
        InvalidateVisual();
    }

    public void EndStroke(Point point)
    {
        if (_activeStroke is null)
        {
            return;
        }

        var completedStroke = _strokeBuilder.End(ToPoint2D(point));

        _strokes.Add(completedStroke);
        _activeStroke = null;

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var pen = new Pen(Brushes.Red, 4);

        foreach (var stroke in _strokes)
        {
            DrawStroke(context, pen, stroke);
        }

        if (_activeStroke is not null)
        {
            DrawStroke(context, pen, _activeStroke);
        }
    }

    private static void DrawStroke(
        DrawingContext context,
        Pen pen,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        for (var index = 1; index < stroke.Points.Count; index++)
        {
            context.DrawLine(
                pen,
                ToAvaloniaPoint(stroke.Points[index - 1]),
                ToAvaloniaPoint(stroke.Points[index]));
        }
    }

    private static Point2D ToPoint2D(Point point)
    {
        return new Point2D(point.X, point.Y);
    }

    private static Point ToAvaloniaPoint(Point2D point)
    {
        return new Point(point.X, point.Y);
    }
}