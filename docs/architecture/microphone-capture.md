# Microphone Capture Investigation

## Purpose

Define the preferred platform APIs and architectural boundary for microphone capture during future Griddle screen recordings.

This investigation does not implement microphone capture.

## macOS

### Primary API

Use ScreenCaptureKit microphone capture as part of the same `SCStream` used for screen recording.

Microphone samples are exposed through the microphone stream output type.

On supported macOS versions, `SCStreamConfiguration` also allows a specific microphone device to be selected.

### Device Selection

Future Griddle recording controls may allow the user to:

- disable microphone capture
- use the system default microphone
- select a specific microphone input device

Microphone-device identifiers remain platform implementation details.

### Stream Handling

Microphone samples should be handled separately from:

- screen frames
- system audio samples

The future macOS recording backend is responsible for timestamping, buffering, mixing, and encoding these streams into the final recording.

## Windows

### Primary API

Use WASAPI capture for Windows microphone input.

The Windows implementation should open a capture stream for the selected recording endpoint and obtain audio packets through `IAudioCaptureClient`.

### Device Selection

Future Griddle recording controls may support:

- microphone disabled
- system default recording device
- explicit microphone selection

Windows audio endpoint identifiers and COM interfaces remain implementation details of `Griddle.Platform`.

## Shared Architecture

Microphone capture is a recording option.

A future platform-neutral recording configuration may expose concepts such as:

- CaptureMicrophone
- MicrophoneDevice
- MicrophoneVolume
- MuteMicrophone

The exact interface should not be created during v0.9.0-alpha.

## Audio Mixing

When both system audio and microphone audio are enabled, the recording backend must combine or otherwise encode both audio sources into the resulting recording.

Possible future approaches include:

- mixing both sources into a single audio track
- preserving separate tracks for editing or presentation workflows

That decision is deferred until recording implementation.

## Permissions

Microphone capture requires operating-system permission.

The platform implementation is responsible for:

- detecting microphone permission state
- requesting permission where appropriate
- reporting permission failures through platform-neutral errors

Permission APIs and native device objects must remain inside `Griddle.Platform`.

## Platform Boundary

All native microphone APIs remain inside `Griddle.Platform`.

Platform-specific types must not escape into `Griddle.Core` or `Griddle.App`.

This includes:

- ScreenCaptureKit microphone output types
- CoreMedia audio sample buffers
- CoreAudio device identifiers
- WASAPI interfaces
- Windows audio endpoint objects
- native audio buffers

## Architectural Decision

For future Griddle microphone capture:

- macOS will use ScreenCaptureKit microphone output from the recording `SCStream`.
- Windows will use WASAPI capture from a recording endpoint.
- Microphone capture remains independently configurable from system audio capture.
- Device selection will be exposed through a future platform-neutral recording configuration.
- Native device and audio types remain entirely inside `Griddle.Platform`.
- Mixing, track layout, synchronization, and encoding are deferred until recording implementation.
- No microphone-capture implementation will be added during v0.9.0-alpha.