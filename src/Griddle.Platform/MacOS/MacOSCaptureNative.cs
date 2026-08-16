using System.Runtime.InteropServices;

namespace Griddle.Platform.MacOS;

internal static class MacOSCaptureNative
{
    private const string LibraryName =
        "GriddleCaptureBridge";

    [UnmanagedFunctionPointer(
        CallingConvention.Cdecl)]
    internal delegate void CaptureCallback(
        IntPtr data,
        int dataLength,
        int width,
        int height,
        IntPtr errorMessage,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern void griddle_capture_region(
        int x,
        int y,
        int width,
        int height,
        int includeApplicationWindows,
        CaptureCallback callback,
        IntPtr context);
}
