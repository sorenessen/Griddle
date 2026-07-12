using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace Griddle.Platform.MacOS;

public static class MacOSWindowInterop
{
    private const string ObjectiveCLibrary = "/usr/lib/libobjc.A.dylib";

    [DllImport(ObjectiveCLibrary)]
    private static extern IntPtr sel_registerName(string selectorName);

    [DllImport(ObjectiveCLibrary)]
    private static extern void objc_msgSend(
        IntPtr receiver,
        IntPtr selector,
        bool value);

    public static void SetIgnoresMouseEvents(
        Window window,
        bool ignoresMouseEvents)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        var platformHandle = window.TryGetPlatformHandle();

        if (platformHandle is null || platformHandle.Handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Could not obtain the native macOS window handle.");
        }

        var selector = sel_registerName("setIgnoresMouseEvents:");

        objc_msgSend(
            platformHandle.Handle,
            selector,
            ignoresMouseEvents);
    }
}
