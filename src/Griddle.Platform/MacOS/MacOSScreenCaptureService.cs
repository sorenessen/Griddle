using System.Runtime.InteropServices;
using Griddle.Platform.Capture;

namespace Griddle.Platform.MacOS;

public sealed class MacOSScreenCaptureService
    : IScreenCaptureService
{
    private static readonly
        MacOSCaptureNative.CaptureCallback
        CaptureCallback = OnCaptureCompleted;

    public Task<ScreenCaptureResult> CaptureAsync(
        CaptureRegion region)
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(
                "macOS screen capture is only available on macOS.");
        }

        var completionSource =
            new TaskCompletionSource<ScreenCaptureResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        var handle =
            GCHandle.Alloc(completionSource);

        try
        {
            MacOSCaptureNative.griddle_capture_region(
                region.X,
                region.Y,
                region.Width,
                region.Height,
                CaptureCallback,
                GCHandle.ToIntPtr(handle));
        }
        catch
        {
            handle.Free();
            throw;
        }

        return completionSource.Task;
    }

    private static void OnCaptureCompleted(
        IntPtr data,
        int dataLength,
        int width,
        int height,
        IntPtr errorMessage,
        IntPtr context)
    {
        var handle =
            GCHandle.FromIntPtr(context);

        try
        {
            var completionSource =
                (TaskCompletionSource<ScreenCaptureResult>)
                    handle.Target!;

            if (errorMessage != IntPtr.Zero)
            {
                var message =
                    Marshal.PtrToStringUTF8(
                        errorMessage)
                    ?? "Screen capture failed.";

                completionSource.SetException(
                    new InvalidOperationException(
                        message));

                return;
            }

            if (data == IntPtr.Zero ||
                dataLength <= 0)
            {
                completionSource.SetException(
                    new InvalidOperationException(
                        "Screen capture returned no image data."));

                return;
            }

            var imageData =
                new byte[dataLength];

            Marshal.Copy(
                data,
                imageData,
                0,
                dataLength);

            completionSource.SetResult(
                new ScreenCaptureResult
                {
                    ImageData = imageData,
                    Width = width,
                    Height = height
                });
        }
        finally
        {
            handle.Free();
        }
    }
}