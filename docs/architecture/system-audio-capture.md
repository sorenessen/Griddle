# System Audio Capture Investigation

## Purpose

Define the preferred platform APIs and architectural boundary for capturing system audio during future Griddle screen recordings.

This investigation does not implement audio capture.

## macOS

### Primary API

Use ScreenCaptureKit as the system-audio source for macOS recordings.

System audio should be captured through the same `SCStream` used for screen recording.

`SCStreamConfiguration.capturesAudio` enables system audio capture.

Audio samples are delivered through an `SCStreamOutput` using the audio output type.

### Griddle Audio Exclusion

ScreenCaptureKit provides `excludesCurrentProcessAudio`.

This allows future Griddle recording options to determine whether audio produced by Griddle itself should be included in the recording.

The default Griddle recording behavior should normally exclude Griddle-generated application sounds while retaining audio from the captured system content.

### Audio Configuration

ScreenCaptureKit exposes configuration for properties including:

- sample rate
- channel count
- current-process audio exclusion

These settings remain implementation details of the macOS recording backend unless Griddle later exposes them as user-configurable recording options.

## Windows

### Primary API

Use WASAPI loopback capture for Windows system-audio recording.

Loopback capture provides the audio mix being rendered through a Windows audio output endpoint.

This operates separately from the `Windows.Graphics.Capture` video pipeline.

Conceptually:

`Windows.Graphics.Capture`
→ video frames

`WASAPI Loopback`
→ system audio samples

Both streams are synchronized and encoded by the future Windows recording implementation.

### Griddle Audio Exclusion

Windows supports process-aware loopback capture on supported Windows versions.

Process loopback can include or exclude the audio produced by a specified process and its child processes.

A future Griddle Windows backend may use process-exclusion mode to prevent Griddle-generated sounds from appearing in recordings.

Basic endpoint loopback remains available when process-aware filtering is unavailable or unnecessary.

## Shared Architecture

System audio is a recording option, not an independent session asset by default.

A future platform-neutral recording configuration may expose behavior conceptually equivalent to:

`CaptureSystemAudio`

and potentially:

`IncludeApplicationAudio`

The exact interface should be created when recording implementation begins.

## Platform Boundary

All native audio APIs remain inside `Griddle.Platform`.

Platform-specific types must not escape into `Griddle.Core` or `Griddle.App`.

This includes:

- ScreenCaptureKit audio types
- CoreMedia sample buffers
- WASAPI interfaces
- Windows audio endpoint interfaces
- native audio buffers

The platform recording backend is responsible for converting captured audio into the recording pipeline.

## Synchronization

Video and system audio originate from different platform mechanisms on Windows and from separate stream outputs on macOS.

Timestamp synchronization, buffering, drift handling, and media encoding are responsibilities of the future platform recording implementation.

These concerns are deliberately deferred from v0.9.0-alpha.

## Architectural Decision

For future Griddle system-audio capture:

- macOS will use ScreenCaptureKit audio output from the existing recording `SCStream`.
- Windows will use WASAPI loopback capture.
- Griddle-generated audio should be excludable where platform APIs support it.
- System audio will be treated as part of a recording rather than as a standalone media asset by default.
- Native audio types remain entirely inside `Griddle.Platform`.
- Audio/video synchronization and encoding are deferred until recording implementation.
- No system-audio capture implementation will be added during v0.9.0-alpha.