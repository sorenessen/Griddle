namespace Griddle.Core.Models;

public sealed class ColorPreset
{
    public ColorPreset(
        string name,
        StrokeColor color)
    {
        Name = name;
        Color = color;
    }

    public string Name { get; }

    public StrokeColor Color { get; }
}