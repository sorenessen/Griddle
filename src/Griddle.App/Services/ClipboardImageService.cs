using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Input.Platform;

namespace Griddle.App.Services;

public static class ClipboardImageService
{
    public static async Task CopyPngAsync(
        TopLevel topLevel,
        byte[] pngData)
    {
        var clipboard =
            topLevel.Clipboard;

        if (clipboard is null)
        {
            throw new InvalidOperationException(
                "Clipboard is not available.");
        }

        using var stream =
            new MemoryStream(pngData);

        var bitmap =
            new Bitmap(stream);

        await clipboard.SetBitmapAsync(
            bitmap);
    }
}