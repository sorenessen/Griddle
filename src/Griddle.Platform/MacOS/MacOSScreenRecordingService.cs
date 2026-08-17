using System.Runtime.InteropServices;
using Griddle.Platform.Recording;

namespace Griddle.Platform.MacOS;

public sealed class MacOSScreenRecordingService
    : IScreenRecordingService
{
    private static readonly
        MacOSRecordingNative.RecordingCallback
        RecordingCallback =
            OnRecordingCompleted;

    public bool IsRecording =>
        OperatingSystem.IsMacOS() &&
        MacOSRecordingNative
            .griddle_recording_is_active() != 0;

    public Task StartAsync(
        ScreenRecordingOptions options)
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(
                "macOS screen recording is only available on macOS.");
        }

        var completionSource =
            new TaskCompletionSource<bool>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);

        var handle =
            GCHandle.Alloc(
                completionSource);

        var framesPerSecond =
            options.FramesPerSecond ?? 30;

        try
        {
            MacOSRecordingNative
                .griddle_recording_start(
                    options.Region.X,
                    options.Region.Y,
                    options.Region.Width,
                    options.Region.Height,
                    options.IncludeApplicationWindows
                        ? 1
                        : 0,
                    options.CaptureSystemAudio
                        ? 1
                        : 0,
                    options.CaptureMicrophone
                        ? 1
                        : 0,
                    framesPerSecond,
                    options.OutputFilePath,
                    RecordingCallback,
                    GCHandle.ToIntPtr(
                        handle));
        }
        catch
        {
            handle.Free();
            throw;
        }

        return completionSource.Task;
    }

    public async Task<ScreenRecordingResult>
        StopAsync()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(
                "macOS screen recording is only available on macOS.");
        }

        var completionSource =
            new TaskCompletionSource<bool>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);

        var handle =
            GCHandle.Alloc(
                completionSource);

        try
        {
            MacOSRecordingNative
                .griddle_recording_stop(
                    RecordingCallback,
                    GCHandle.ToIntPtr(
                        handle));
        }
        catch
        {
            handle.Free();
            throw;
        }

        await completionSource.Task;

        return new ScreenRecordingResult
        {
            FilePath = string.Empty,
            Width = 0,
            Height = 0,
            Duration = TimeSpan.Zero,
            IncludesApplicationWindows = false,
            IncludesSystemAudio = false,
            IncludesMicrophone = false
        };
    }

    private static void OnRecordingCompleted(
        IntPtr errorMessage,
        IntPtr context)
    {
        var handle =
            GCHandle.FromIntPtr(
                context);

        try
        {
            var completionSource =
                (TaskCompletionSource<bool>)
                    handle.Target!;

            if (errorMessage != IntPtr.Zero)
            {
                var message =
                    Marshal.PtrToStringUTF8(
                        errorMessage)
                    ?? "Screen recording failed.";

                completionSource.SetException(
                    new InvalidOperationException(
                        message));

                return;
            }

            completionSource.SetResult(
                true);
        }
        finally
        {
            handle.Free();
        }
    }
}