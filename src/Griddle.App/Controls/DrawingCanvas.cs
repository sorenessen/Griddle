using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Griddle.App.Controls;

public sealed class DrawingCanvas : Control
{
    private readonly List<Point> _points = new();

    public void BeginStroke(Point point)
    {
        _points.Clear();
        _points.Add(point);
        InvalidateVisual();
    }

    public void ContinueStroke(Point point)
    {
        _points.Add(point);
        InvalidateVisual();
    }

    public void EndStroke(Point point)
    {
        _points.Add(point);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_points.Count < 2)
        {
            return;
        }

        var pen = new Pen(Brushes.Red, 4);

        for (var index = 1; index < _points.Count; index++)
        {
            context.DrawLine(
                pen,
                _points[index - 1],
                _points[index]);
        }
    }
}