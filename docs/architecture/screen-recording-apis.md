# Screen Recording API Investigation

## Purpose

Define the preferred native screen-recording APIs for macOS and Windows and establish the platform boundary for future Griddle recording functionality.

This investigation does not implement screen recording.

## macOS

### Primary API

Use Apple's ScreenCaptureKit with `SCStream` as the primary macOS screen-recording API.

This is a natural extension of Griddle's existing ScreenCaptureKit screenshot implementation.

An `SCStream` is created using:

- `SCContentFilter`
- `SCStreamConfiguration`
- `SCStreamDelegate`

Video frames are delivered through an `SCStreamOutput`.

### Griddle Window Exclusion

Griddle already uses `SCContentFilter` to exclude the current Griddle process when capturing screenshots without annotations.

The same filtering model should be used for recording.

This allows future recording modes to support:

- recording with Griddle annotations visible
- recording without Griddle UI or annotations
- excluding Griddle controls without hiding or flickering windows

### Recording Configuration

`SCStreamConfiguration` should remain a macOS platform implementation detail.

Future configuration may include:

- output dimensions
- frame rate
- source region
- pixel format
- queue depth
- system audio capture
- microphone capture

These settings should be represented by platform-neutral Griddle options before reaching the native implementation.

### Output

The macOS implementation will be responsible for receiving stream frames and producing the final recording media file.

Codec, container, CoreMedia, AVFoundation, and ScreenCaptureKit types must not escape `Griddle.Platform`.

## Windows

### Primary API

Use `Windows.Graphics.Capture` as the primary Windows recording API.

A recording session should use:

- `GraphicsCaptureItem`
- `GraphicsCaptureSession`
- `Direct3D11CaptureFramePool`

The frame pool supplies continuous `Direct3D11CaptureFrame` instances for the selected display.

This extends the Windows screenshot architecture already defined for Griddle.

### Griddle Window Exclusion

The Windows implementation should continue using Windows capture-exclusion mechanisms for Griddle-owned windows when recording without Griddle UI.

The application should not need to visually hide the overlay or toolbar during recording.

### Frame Processing

Windows capture frames and Direct3D resources remain implementation details of `Griddle.Platform`.

Future encoding may use an appropriate Windows media encoding pipeline, but the specific codec and encoding implementation are deferred until recording is implemented.

### DXGI Desktop Duplication

DXGI Desktop Duplication remains a possible fallback or specialized backend.

It is not the preferred initial Griddle recording API.

It may be reconsidered if future performance, compatibility, or advanced capture requirements cannot be satisfied by `Windows.Graphics.Capture`.

## Shared Architecture

Screen recording should eventually be exposed through a platform-neutral recording abstraction separate from the existing single-frame screenshot service.

Conceptually:

`IScreenRecordingService`

Future shared recording options may include:

- capture region
- include Griddle windows / annotations
- frame rate
- output dimensions
- system audio enabled
- microphone enabled
- output file destination

The exact interface should not be created during v0.9.0-alpha.

It should be introduced when recording implementation begins and the required behavior is known.

## Platform Boundary

Platform-specific recording APIs remain entirely within `Griddle.Platform`.

`Griddle.Core` should contain only platform-neutral session and media metadata.

`Griddle.App` should request recording behavior without depending directly on:

- ScreenCaptureKit
- CoreMedia
- AVFoundation
- Windows.Graphics.Capture
- Direct3D
- DXGI
- WinRT capture objects

## Architectural Decision

For future Griddle screen recording:

- macOS will use ScreenCaptureKit `SCStream`.
- Windows will use `Windows.Graphics.Capture`.
- Existing platform-specific window-exclusion strategies should be reused.
- Recording will use a dedicated platform-neutral service rather than expanding `IScreenCaptureService` into a streaming interface.
- DXGI Desktop Duplication remains a fallback candidate, not the primary Windows implementation.
- Codec and container decisions are deferred until recording implementation.
- No recording implementation or recording interface will be added during v0.9.0-alpha.