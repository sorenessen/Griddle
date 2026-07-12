using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Griddle.Core.Geometry;
using Griddle.Core.Models;
using Griddle.Core.Services;
using Griddle.Core.Tools;


namespace Griddle.App.Controls;

public sealed class DrawingCanvas : Control
{
    private readonly List<Stroke> _strokes = new();
    private readonly StrokeBuilder _strokeBuilder = new();
    private readonly Stack<Stroke> _redoStack = new();

    private Stroke? _activeStroke;
    private readonly PenTool _pen = new(new PenSettings());

    public void SetColor(StrokeColor color)
    {
        _pen.Settings.Color = color;
    }

    public void SetThickness(double thickness)
    {
        _pen.Settings.Thickness = thickness;
    }

    public void BeginStroke(Point point)
    {
        _activeStroke = _strokeBuilder.Begin(
            ToPoint2D(point),
            _pen.Settings.Color,
            _pen.Settings.Thickness);
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

        _redoStack.Clear();
        _strokes.Add(completedStroke);
        _activeStroke = null;

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var stroke in _strokes)
        {
            DrawStroke(context, stroke);
        }

        if (_activeStroke is not null)
        {
            DrawStroke(context, _activeStroke);
        }
    }

    private static void DrawStroke(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var brush = stroke.Color switch
        {
            StrokeColor.Blue => Brushes.DodgerBlue,
            _ => Brushes.Red
        };

        var pen = new Pen
        {
            Brush = brush,
            Thickness = stroke.Thickness,
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

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

    public void Clear()
    {
        _strokes.Clear();
        _redoStack.Clear();
        _activeStroke = null;

        InvalidateVisual();
    }

    public void Undo()
    {
        if (_strokes.Count == 0)
        {
            return;
        }

        var stroke = _strokes[^1];
        _strokes.RemoveAt(_strokes.Count - 1);
        _redoStack.Push(stroke);

        InvalidateVisual();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        var stroke = _redoStack.Pop();
        _strokes.Add(stroke);

        InvalidateVisual();
    }
}