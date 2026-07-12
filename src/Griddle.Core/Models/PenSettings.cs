namespace Griddle.Core.Models;

public sealed class PenSettings
{
    public StrokeColor Color { get; set; } = StrokeColor.Red;

    public double Thickness { get; set; } = 4;

    public double Opacity { get; set; } = 1.0;
}