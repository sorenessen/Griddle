using System;
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
    private readonly PenTool _pen;

    private Stroke? _activeStroke;
    private readonly ActiveToolService _activeTool;

    public PenTool Pen => _pen;
    public ActiveToolService ActiveTool => _activeTool;

    public DrawingCanvas()
        : this(
            new PenTool(new PenSettings()),
            null)
    {
    }

    public DrawingCanvas(
        PenTool pen,
        ActiveToolService? activeTool)
    {
        _pen = pen;
        _activeTool = activeTool ?? new ActiveToolService(pen);
    }

    public void SetColor(StrokeColor color)
    {
        Pen.Settings.Color = color;
    }

    public void SetThickness(double thickness)
    {
        Pen.Settings.Thickness = thickness;
    }

    public void BeginStroke(Point point)
    {
        _activeStroke = _activeTool.Current.Begin(
            ToPoint2D(point));

        InvalidateVisual();
    }

    public void ContinueStroke(Point point)
    {
        if (_activeStroke is null)
        {
            return;
        }

        _activeTool.Current.Continue(ToPoint2D(point));
        InvalidateVisual();
    }

    public void EndStroke(Point point)
    {
        if (_activeStroke is null)
        {
            return;
        }

        var completedStroke = _activeTool.Current.End(
            ToPoint2D(point));

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
        if (stroke.Kind == StrokeKind.Arrow)
        {
            DrawArrow(context, stroke);
            return;
        }

        if (stroke.Points.Count < 2)
        {
            return;
        }

        var baseColor = stroke.Color switch
        {
            StrokeColor.Blue => Colors.DodgerBlue,
            StrokeColor.Black => Colors.Black,
            StrokeColor.Yellow => Colors.Yellow,
            _ => Colors.Red
        };

        var brush = new SolidColorBrush(
            baseColor,
            stroke.Opacity);

        var pen = new Pen
        {
            Brush = brush,
            Thickness = stroke.Thickness,
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        var geometry = new StreamGeometry();

        using (var geometryContext = geometry.Open())
        {
            geometryContext.BeginFigure(
                ToAvaloniaPoint(stroke.Points[0]),
                isFilled: false);

            for (var index = 1; index < stroke.Points.Count; index++)
            {
                geometryContext.LineTo(
                    ToAvaloniaPoint(stroke.Points[index]));
            }

            geometryContext.EndFigure(isClosed: false);
        }

        context.DrawGeometry(
            brush: null,
            pen,
            geometry);
    }

    private static void DrawArrow(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[1]);

        var baseColor = stroke.Color switch
        {
            StrokeColor.Blue => Colors.DodgerBlue,
            StrokeColor.Black => Colors.Black,
            StrokeColor.Yellow => Colors.Yellow,
            _ => Colors.Red
        };

        var brush = new SolidColorBrush(
            baseColor,
            stroke.Opacity);

        var pen = new Pen
        {
            Brush = brush,
            Thickness = stroke.Thickness,
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        context.DrawLine(pen, start, end);

        var direction = start - end;
        var length = Math.Sqrt(
            direction.X * direction.X +
            direction.Y * direction.Y);

        if (length < 1)
        {
            return;
        }

        var unitX = direction.X / length;
        var unitY = direction.Y / length;

        const double arrowHeadLength = 18;
        const double arrowHeadWidth = 8;

        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var left = new Point(
            end.X + unitX * arrowHeadLength +
            perpendicularX * arrowHeadWidth,
            end.Y + unitY * arrowHeadLength +
            perpendicularY * arrowHeadWidth);

        var right = new Point(
            end.X + unitX * arrowHeadLength -
            perpendicularX * arrowHeadWidth,
            end.Y + unitY * arrowHeadLength -
            perpendicularY * arrowHeadWidth);

        context.DrawLine(pen, end, left);
        context.DrawLine(pen, end, right);
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