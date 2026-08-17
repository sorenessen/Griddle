using System.Runtime.InteropServices;
using Griddle.Platform.Recording;

namespace Griddle.Platform.MacOS;

public sealed class MacOSScreenRecordingService
    : IScreenRecordingService
{
    private static readonly
        MacOSRecordingNative.RecordingCallback
        StartCallback =
            OnRecordingStarted;

    private static readonly
        MacOSRecordingNative.RecordingStopCallback
        StopCallback =
            OnRecordingStopped;

    private ScreenRecordingOptions?
        _activeOptions;

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

        if (_activeOptions is not null)
        {
            throw new InvalidOperationException(
                "A screen recording is already active.");
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
                    StartCallback,
                    GCHandle.ToIntPtr(
                        handle));
        }
        catch
        {
            handle.Free();
            throw;
        }

        return CompleteStartAsync(
            completionSource.Task,
            options);
    }

    private async Task CompleteStartAsync(
        Task completionTask,
        ScreenRecordingOptions options)
    {
        await completionTask;

        _activeOptions =
            options;
    }

    public async Task<ScreenRecordingResult>
        StopAsync()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(
                "macOS screen recording is only available on macOS.");
        }

        var activeOptions =
            _activeOptions
            ?? throw new InvalidOperationException(
                "No screen recording is active.");

        var completionSource =
            new TaskCompletionSource<double>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);

        var handle =
            GCHandle.Alloc(
                completionSource);

        try
        {
            MacOSRecordingNative
                .griddle_recording_stop(
                    StopCallback,
                    GCHandle.ToIntPtr(
                        handle));
        }
        catch
        {
            handle.Free();
            throw;
        }

        var durationSeconds =
            await completionSource.Task;

        var result =
            new ScreenRecordingResult
            {
                FilePath =
                    activeOptions.OutputFilePath,

                Width =
                    activeOptions.Region.Width,

                Height =
                    activeOptions.Region.Height,

                Duration =
                    TimeSpan.FromSeconds(
                        durationSeconds),

                IncludesApplicationWindows =
                    activeOptions.IncludeApplicationWindows,

                IncludesSystemAudio =
                    activeOptions.CaptureSystemAudio,

                IncludesMicrophone =
                    activeOptions.CaptureMicrophone
            };

        _activeOptions =
            null;

        return result;
    }

    private static void OnRecordingStarted(
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
                    ?? "Screen recording failed to start.";

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

    private static void OnRecordingStopped(
        double durationSeconds,
        IntPtr errorMessage,
        IntPtr context)
    {
        var handle =
            GCHandle.FromIntPtr(
                context);

        try
        {
            var completionSource =
                (TaskCompletionSource<double>)
                    handle.Target!;

            if (errorMessage != IntPtr.Zero)
            {
                var message =
                    Marshal.PtrToStringUTF8(
                        errorMessage)
                    ?? "Screen recording failed to stop.";

                completionSource.SetException(
                    new InvalidOperationException(
                        message));

                return;
            }

            completionSource.SetResult(
                durationSeconds);
        }
        finally
        {
            handle.Free();
        }
    }
}