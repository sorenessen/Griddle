using System.Runtime.InteropServices;

namespace Griddle.Platform.MacOS;

internal static class MacOSRecordingNative
{
    private const string LibraryName =
        "GriddleRecordingBridge";

    [UnmanagedFunctionPointer(
        CallingConvention.Cdecl)]
    internal delegate void RecordingCallback(
        IntPtr errorMessage,
        IntPtr context);

    [UnmanagedFunctionPointer(
        CallingConvention.Cdecl)]
    internal delegate void RecordingStopCallback(
        double durationSeconds,
        IntPtr errorMessage,
        IntPtr context);

    [UnmanagedFunctionPointer(
        CallingConvention.Cdecl)]
    internal delegate void MicrophonePermissionCallback(
        int granted,
        IntPtr errorMessage,
        IntPtr context);

    [UnmanagedFunctionPointer(
        CallingConvention.Cdecl)]
    internal delegate void ScreenPermissionCallback(
        int granted,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern void griddle_request_screen_access(
        ScreenPermissionCallback callback,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern void griddle_request_microphone_access(
        MicrophonePermissionCallback callback,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern void griddle_recording_start(
        int x,
        int y,
        int width,
        int height,
        int includeApplicationWindows,
        int captureSystemAudio,
        int captureMicrophone,
        int framesPerSecond,
        string outputFilePath,
        RecordingCallback callback,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern void griddle_recording_stop(
        RecordingStopCallback callback,
        IntPtr context);

    [DllImport(
        LibraryName,
        CallingConvention = CallingConvention.Cdecl)]
    internal static extern int griddle_recording_is_active();
}