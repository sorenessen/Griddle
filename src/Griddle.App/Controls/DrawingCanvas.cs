using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Griddle.App.Controls;

public sealed class DrawingCanvas : Control
{
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var pen = new Pen(Brushes.Red, 4);

        context.DrawLine(
            pen,
            new Point(100, 100),
            new Point(400, 300));
    }
}