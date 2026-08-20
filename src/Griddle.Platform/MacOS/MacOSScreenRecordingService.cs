using System.Runtime.InteropServices;
using Griddle.Platform.Recording;
using System.Text.Json;

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

    private static readonly
        MacOSRecordingNative.MicrophonePermissionCallback
        MicrophonePermissionCallback =
            OnMicrophonePermissionCompleted;

    private static readonly
        MacOSRecordingNative.ScreenPermissionCallback
        ScreenPermissionCallback =
            OnScreenPermissionCompleted;

    private static readonly
        MacOSRecordingNative.MicrophoneDevicesCallback
        MicrophoneDevicesCallback =
            OnMicrophoneDevicesReceived;

    private static readonly
        MacOSRecordingNative.MicrophoneDisconnectedCallback
        MicrophoneDisconnectedCallback =
            OnMicrophoneDisconnected;

    private ScreenRecordingOptions?
        _activeOptions;

    private GCHandle?
        _microphoneDisconnectHandle;

    public event Action<string?, string?>?
        MicrophoneDisconnected;

    public bool IsRecording =>
        OperatingSystem.IsMacOS() &&
        MacOSRecordingNative
            .griddle_recording_is_active() != 0;

    public Task<IReadOnlyList<MicrophoneDevice>>
        GetMicrophoneDevicesAsync()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(
                "Microphone enumeration is only available on macOS.");
        }

        var completionSource =
            new TaskCompletionSource<IReadOnlyList<MicrophoneDevice>>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);

        var handle =
            GCHandle.Alloc(
                completionSource);

        try
        {
            MacOSRecordingNative
                .griddle_get_microphone_devices(
                    MicrophoneDevicesCallback,
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

    private static void OnMicrophoneDevicesReceived(
        IntPtr devicesJson,
        IntPtr context)
    {
        var handle =
            GCHandle.FromIntPtr(
                context);

        try
        {
            var completionSource =
                (TaskCompletionSource<IReadOnlyList<MicrophoneDevice>>)
                    handle.Target!;

            var json =
                devicesJson != IntPtr.Zero
                    ? Marshal.PtrToStringUTF8(
                        devicesJson)
                    : null;

            var devices =
                string.IsNullOrWhiteSpace(
                    json)
                    ? []
                    : JsonSerializer.Deserialize<
                        List<MicrophoneDevice>>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive =
                                    true
                            })
                        ?? [];

            completionSource.SetResult(
                devices);
        }
        catch (Exception ex)
        {
            var completionSource =
                (TaskCompletionSource<IReadOnlyList<MicrophoneDevice>>)
                    handle.Target!;

            completionSource.SetException(
                ex);
        }
        finally
        {
            handle.Free();
        }
    }

    private static void OnMicrophoneDisconnected(
        IntPtr deviceId,
        IntPtr deviceName,
        IntPtr context)
    {
        if (context == IntPtr.Zero)
        {
            return;
        }

        var handle =
            GCHandle.FromIntPtr(
                context);

        if (handle.Target
            is not MacOSScreenRecordingService service)
        {
            return;
        }

        var id =
            deviceId != IntPtr.Zero
                ? Marshal.PtrToStringUTF8(
                    deviceId)
                : null;

        var name =
            deviceName != IntPtr.Zero
                ? Marshal.PtrToStringUTF8(
                    deviceName)
                : null;

        service.MicrophoneDisconnected?.Invoke(
            id,
            name);
    }

    public async Task StartAsync(
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

        await RequestScreenAccessAsync();

        if (options.CaptureMicrophone)
        {
            await RequestMicrophoneAccessAsync();
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

        var microphoneDisconnectContext =
            IntPtr.Zero;

        if (options.CaptureMicrophone)
        {
            _microphoneDisconnectHandle =
                GCHandle.Alloc(
                    this);

            microphoneDisconnectContext =
                GCHandle.ToIntPtr(
                    _microphoneDisconnectHandle.Value);
        }

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
                    options.MicrophoneDeviceId,
                    framesPerSecond,
                    options.OutputFilePath,
                    StartCallback,
                    GCHandle.ToIntPtr(
                        handle),
                    MicrophoneDisconnectedCallback,
                    microphoneDisconnectContext);
        }
        catch
        {
            handle.Free();

            if (_microphoneDisconnectHandle is { } disconnectHandle)
            {
                disconnectHandle.Free();
                _microphoneDisconnectHandle =
                    null;
            }

            throw;
        }

        await CompleteStartAsync(
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

        if (_microphoneDisconnectHandle is { } disconnectHandle)
        {
            disconnectHandle.Free();

            _microphoneDisconnectHandle =
                null;
        }

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

    private static Task RequestMicrophoneAccessAsync()
    {
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
                .griddle_request_microphone_access(
                    MicrophonePermissionCallback,
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

    private static Task RequestScreenAccessAsync()
    {
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
                .griddle_request_screen_access(
                    ScreenPermissionCallback,
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

    private static void OnScreenPermissionCompleted(
        int granted,
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

            if (granted != 0)
            {
                completionSource.SetResult(
                    true);

                return;
            }

            completionSource.SetException(
                new InvalidOperationException(
                    "Screen recording access was not granted."));
        }
        finally
        {
            handle.Free();
        }
    }

    private static void OnMicrophonePermissionCompleted(
        int granted,
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

            if (granted != 0)
            {
                completionSource.SetResult(
                    true);

                return;
            }

            var message =
                errorMessage != IntPtr.Zero
                    ? Marshal.PtrToStringUTF8(
                        errorMessage)
                    : null;

            completionSource.SetException(
                new InvalidOperationException(
                    message
                    ?? "Microphone access was not granted."));
        }
        finally
        {
            handle.Free();
        }
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