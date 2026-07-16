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
    private readonly Stack<Stroke> _redoStack = new();
    private readonly PenTool _pen;
    private readonly ActiveToolService _activeTool;
    private readonly SelectionService _selection;

    private bool _isToolInteractionActive;

    private Stroke? _activeStroke;

    public PenTool Pen => _pen;
    public ActiveToolService ActiveTool => _activeTool;
    public SelectionService Selection => _selection;

    public DrawingCanvas()
        : this(
            new PenTool(new PenSettings()),
            null,
            null)
    {
    }

    public DrawingCanvas(
        PenTool pen,
        ActiveToolService? activeTool,
        SelectionService? selection)
    {
        _pen = pen;
        _activeTool = activeTool ?? new ActiveToolService(pen);
        _selection = selection ?? new SelectionService();
    }

    public void SetColor(StrokeColor color)
    {
        Pen.Settings.Color = color;
    }

    public void SetThickness(double thickness)
    {
        Pen.Settings.Thickness = thickness;
    }

    public void BeginInteraction(Point point)
    {
        if (_activeTool.Current is SelectionTool)
        {
            var hit = HitTest(point);

            if (hit is null)
            {
                _selection.Clear();
            }
            else
            {
                _selection.Select(hit);
            }

            InvalidateVisual();
            return;
        }

        _isToolInteractionActive = true;

        _activeStroke = _activeTool.Current.Begin(
            ToPoint2D(point));

        InvalidateVisual();
    }

    public void ContinueInteraction(Point point)
    {
        if (!_isToolInteractionActive)
        {
            return;
        }

        _activeTool.Current.Continue(
            ToPoint2D(point));

        InvalidateVisual();
    }

    public void EndInteraction(Point point)
    {
        if (!_isToolInteractionActive)
        {
            return;
        }

        var completedStroke = _activeTool.Current.End(
            ToPoint2D(point));

        if (completedStroke is not null)
        {
            _redoStack.Clear();
            _strokes.Add(completedStroke);
        }

        _activeStroke = null;
        _isToolInteractionActive = false;

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

        if (_selection.SelectedStroke is not null)
        {
            DrawSelectionOutline(
                context,
                _selection.SelectedStroke);
        }
    }

    private static void DrawStroke(
        DrawingContext context,
        Stroke stroke)
    {
        switch (stroke.Kind)
        {
            case StrokeKind.Freehand:
                DrawFreehand(context, stroke);
                break;

            case StrokeKind.Arrow:
                DrawArrow(context, stroke);
                break;

            case StrokeKind.Rectangle:
                DrawRectangle(context, stroke);
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported stroke kind: {stroke.Kind}");
        }
    }

    private static void DrawFreehand(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var pen = CreatePen(stroke);
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
        var pen = CreatePen(stroke);

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

    private static void DrawRectangle(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[1]);

        var x = Math.Min(start.X, end.X);
        var y = Math.Min(start.Y, end.Y);
        var width = Math.Abs(end.X - start.X);
        var height = Math.Abs(end.Y - start.Y);

        var rectangle = new Rect(
            x,
            y,
            width,
            height);

        context.DrawRectangle(
            brush: null,
            CreatePen(stroke),
            rectangle);
    }

    private static void DrawSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        switch (stroke.Kind)
        {
            case StrokeKind.Rectangle:
                DrawRectangleSelectionOutline(context, stroke);
                break;

            case StrokeKind.Arrow:
                DrawArrowSelectionOutline(context, stroke);
                break;
        }
    }

    private static void DrawRectangleSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[1]);

        var left = Math.Min(start.X, end.X);
        var right = Math.Max(start.X, end.X);
        var top = Math.Min(start.Y, end.Y);
        var bottom = Math.Max(start.Y, end.Y);

        var bounds = new Rect(
            left - 4,
            top - 4,
            (right - left) + 8,
            (bottom - top) + 8);

        var pen = new Pen(
            Brushes.White,
            1.5,
            dashStyle: new DashStyle(
                new[] { 4.0, 4.0 },
                0));

        context.DrawRectangle(null, pen, bounds);
    }

    private static void DrawArrowSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[^1]);

        var left = Math.Min(start.X, end.X);
        var right = Math.Max(start.X, end.X);
        var top = Math.Min(start.Y, end.Y);
        var bottom = Math.Max(start.Y, end.Y);

        var bounds = new Rect(
            left - 8,
            top - 8,
            Math.Max((right - left) + 16, 16),
            Math.Max((bottom - top) + 16, 16));

        var pen = new Pen(
            Brushes.White,
            1.5,
            dashStyle: new DashStyle(
                new[] { 4.0, 4.0 },
                0));

        context.DrawRectangle(null, pen, bounds);
    }

    private Stroke? HitTest(Point point)
    {
        const double tolerance = 8.0;

        for (var i = _strokes.Count - 1; i >= 0; i--)
        {
            var stroke = _strokes[i];

            if (IsHit(stroke, point, tolerance))
            {
                return stroke;
            }
        }

        return null;
    }

    private static bool IsHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        return stroke.Kind switch
    {
        StrokeKind.Rectangle =>
            IsRectangleHit(stroke, point, tolerance),

        StrokeKind.Arrow =>
            IsLineHit(stroke, point, tolerance),

        _ => false
    };
    }

    private static bool IsRectangleHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count < 2)
        {
            return false;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[1]);

        var left = Math.Min(start.X, end.X);
        var right = Math.Max(start.X, end.X);
        var top = Math.Min(start.Y, end.Y);
        var bottom = Math.Max(start.Y, end.Y);

        var expanded = new Rect(
            left - tolerance,
            top - tolerance,
            (right - left) + tolerance * 2,
            (bottom - top) + tolerance * 2);

        return expanded.Contains(point);
    }

    private static bool IsLineHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count < 2)
        {
            return false;
        }

        var start = ToAvaloniaPoint(stroke.Points[0]);
        var end = ToAvaloniaPoint(stroke.Points[^1]);

        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var lengthSquared = dx * dx + dy * dy;

        if (lengthSquared == 0)
        {
        var startDistance = Math.Sqrt(
            Math.Pow(point.X - start.X, 2) +
            Math.Pow(point.Y - start.Y, 2));

        return startDistance <= tolerance;
        }

        var t =
            ((point.X - start.X) * dx +
             (point.Y - start.Y) * dy) /
            lengthSquared;

        t = Math.Clamp(t, 0, 1);

        var nearest = new Point(
            start.X + t * dx,
            start.Y + t * dy);

        var distance = Math.Sqrt(
            Math.Pow(point.X - nearest.X, 2) +
            Math.Pow(point.Y - nearest.Y, 2));

        return distance <= tolerance;
    }

    private static Pen CreatePen(Stroke stroke)
    {
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

        return new Pen
        {
            Brush = brush,
            Thickness = stroke.Thickness,
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };
    }

    private static Point2D ToPoint2D(Point point)
    {
        return new Point2D(
            point.X,
            point.Y);
    }

    private static Point ToAvaloniaPoint(Point2D point)
    {
        return new Point(
            point.X,
            point.Y);
    }

    public void Clear()
    {
        _strokes.Clear();
        _redoStack.Clear();
        _activeStroke = null;
        _selection.Clear();

        InvalidateVisual();
    }

    public void Undo()
    {
        if (_strokes.Count == 0)
        {
            return;
        }

        var stroke = _strokes[^1];

        _strokes.RemoveAt(
            _strokes.Count - 1);

        if (_selection.SelectedStroke?.Id == stroke.Id)
        {
            _selection.Clear();
        }

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