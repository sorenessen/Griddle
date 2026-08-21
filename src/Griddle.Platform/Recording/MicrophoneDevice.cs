namespace Griddle.Platform.Recording;

public sealed class MicrophoneDevice
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public override string ToString()
    {
        return Name;
    }
}