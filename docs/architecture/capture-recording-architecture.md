# Capture and Recording Architecture

## Purpose

Define the overall architecture for screenshot capture, screen recording, system audio, microphone capture, and resulting media assets in Griddle.

This document consolidates the capture and recording decisions made during Phase 4.

It defines architectural direction only. Screen recording and audio capture are not implemented in v0.9.0-alpha.

## Design Principles

Griddle capture and recording features follow these principles:

1. Application and session code remain platform-neutral.
2. Native capture APIs remain inside `Griddle.Platform`.
3. Screenshots and recordings use the operating system's modern native capture APIs.
4. Griddle UI should be excludable from captured output without visually hiding or flickering windows.
5. Media files remain external to the `.griddle` document.
6. Session documents store metadata and relative references to media assets.
7. Recording abstractions should be introduced only when recording implementation begins.

## Architecture Overview

Conceptually:

Griddle.App
    |
    | platform-neutral requests
    v
Griddle.Platform
    |
    +-- macOS
    |     +-- ScreenCaptureKit
    |
    +-- Windows
          +-- Windows.Graphics.Capture
          +-- WASAPI

Griddle.Core contains platform-neutral session and media metadata only.

Native operating-system types must not cross the `Griddle.Platform` boundary.

## Screenshot Capture

### macOS

Use ScreenCaptureKit.

Single-frame screenshots use:

- `SCScreenshotManager`
- `SCContentFilter`
- `SCStreamConfiguration`

Griddle currently supports:

- capture with annotations
- capture without annotations
- active-display capture
- PNG output
- clipboard copy
- session media storage

When Griddle should not appear in a screenshot, the ScreenCaptureKit content filter excludes the Griddle application rather than hiding Griddle windows.

### Windows

Use `Windows.Graphics.Capture`.

The future Windows implementation should:

1. Resolve the target monitor from `CaptureRegion`.
2. Create a `GraphicsCaptureItem` for the monitor.
3. Acquire a frame through a capture session and frame pool.
4. Crop the result to the requested capture region when necessary.
5. Encode the result as PNG.
6. Return the image through the platform-neutral capture contract.

Griddle-owned Windows should use capture-exclusion support when screenshots should omit Griddle UI.

## Screenshot Contract

Screenshot capture currently uses:

- `IScreenCaptureService`
- `CaptureRegion`
- `ScreenCaptureOptions`
- `ScreenCaptureResult`

This contract remains platform-neutral.

Platform-specific concepts such as:

- `SCDisplay`
- `SCContentFilter`
- `HWND`
- `HMONITOR`
- `GraphicsCaptureItem`
- Direct3D surfaces

must not become part of the shared contract.

## Screen Recording

Screen recording should use a separate service from screenshot capture.

Conceptually:

`IScreenRecordingService`

The exact interface should not be defined until recording implementation begins.

### macOS

Use ScreenCaptureKit `SCStream`.

The stream uses:

- `SCContentFilter`
- `SCStreamConfiguration`
- `SCStreamOutput`

Screen, system-audio, and microphone output may originate from the same ScreenCaptureKit stream.

The existing Griddle application-exclusion strategy should also be usable during recording.

### Windows

Use `Windows.Graphics.Capture`.

Continuous video frames should be acquired through:

- `GraphicsCaptureItem`
- `GraphicsCaptureSession`
- `Direct3D11CaptureFramePool`

DXGI Desktop Duplication remains a fallback or specialized future backend rather than the preferred initial implementation.

## System Audio

### macOS

Use ScreenCaptureKit system-audio output from the recording `SCStream`.

System audio can be enabled through the stream configuration.

Griddle-generated application audio should be excludable when supported.

### Windows

Use WASAPI loopback capture.

The Windows recording implementation therefore combines:

Windows.Graphics.Capture
    -> video frames

WASAPI loopback
    -> system audio

Synchronization and encoding remain responsibilities of the Windows recording backend.

Process-aware loopback may be used where supported to exclude Griddle-generated audio.

## Microphone Capture

Microphone capture remains independently configurable from system audio.

### macOS

Use ScreenCaptureKit microphone output from the recording stream on supported macOS versions.

Future recording controls may allow:

- microphone disabled
- default microphone
- explicit microphone selection

### Windows

Use WASAPI capture from the selected recording endpoint.

Future controls may likewise support:

- microphone disabled
- default recording device
- explicit microphone selection

Native device identifiers remain inside `Griddle.Platform`.

## Audio and Video Synchronization

Recording may contain several independently produced streams:

- video
- system audio
- microphone audio

The platform recording implementation is responsible for:

- timestamps
- buffering
- synchronization
- drift handling
- encoding
- final media-file creation

These concerns must not be handled by `Griddle.Core`.

## Recording Output

The final codec and container strategy is deliberately deferred until recording implementation.

On macOS, ScreenCaptureKit recording output APIs should be evaluated as a direct file-recording option before implementing a custom sample-buffer encoding pipeline.

A custom pipeline remains an option if Griddle later requires greater control over:

- codecs
- audio tracks
- editing
- post-processing
- annotations
- export formats

Windows encoding technology should likewise be selected when the recording implementation requirements are known.

## Recording Options

A future platform-neutral recording configuration may eventually expose concepts such as:

- capture region
- include Griddle windows / annotations
- frame rate
- output dimensions
- capture system audio
- capture microphone
- microphone device
- output destination

The configuration should describe desired behavior rather than expose native API settings.

## Media Asset Model

The current v1 session format continues to use:

- `GriddleCapture`
- `CaptureDocument`
- `Captures`

No migration is required for v0.9.0-alpha.

Future recording and media functionality should evolve toward a broader:

`GriddleMediaAsset`

Possible media kinds include:

- Screenshot
- ScreenRecording
- AudioRecording
- Narration
- ImportedAudio
- ImportedVideo

Common metadata may include:

- Id
- Kind
- CreatedAt
- FileName
- MimeType

Optional metadata may include:

- Duration
- Width
- Height
- DisplayName
- IncludesAnnotations
- HasSystemAudio
- HasMicrophoneAudio
- Source

## Media Storage

Media remains external to the `.griddle` document.

Example:

Demo.griddle

Demo.media/
    screenshot-001.png
    recording-001.mp4
    narration-001.m4a

The `.griddle` document stores metadata and relative asset references rather than embedding large binary media.

## Permissions

Capture and recording implementations are responsible for operating-system permissions associated with:

- screen capture
- system audio where applicable
- microphone access

Native permission APIs remain platform-specific.

Permission failures should eventually be translated into platform-neutral errors that `Griddle.App` can present to the user.

## Platform Responsibilities

### Griddle.App

Responsible for:

- user commands
- recording controls
- capture options
- status and feedback
- presentation workflow

It should not know which native API performs the capture.

### Griddle.Core

Responsible for:

- session metadata
- media metadata
- persistent platform-neutral models

It must not depend on native capture or audio APIs.

### Griddle.Platform

Responsible for:

- native screenshot capture
- native screen recording
- native system-audio capture
- native microphone capture
- device interaction
- native permissions
- synchronization
- encoding integration

## Deferred Decisions

The following decisions intentionally remain open until recording implementation:

- final recording codec
- final media container
- audio mixing versus separate tracks
- recording quality presets
- frame-rate presets
- microphone gain controls
- recording pause/resume semantics
- recording editing
- media export workflows
- Windows encoding implementation
- migration from `GriddleCapture` to `GriddleMediaAsset`

## Phase 4 Decision Summary

Griddle will use:

### macOS

- Screenshots: ScreenCaptureKit / `SCScreenshotManager`
- Recording: ScreenCaptureKit / `SCStream`
- System audio: ScreenCaptureKit
- Microphone: ScreenCaptureKit

### Windows

- Screenshots: `Windows.Graphics.Capture`
- Recording: `Windows.Graphics.Capture`
- System audio: WASAPI loopback
- Microphone: WASAPI capture

Across both platforms:

- native APIs remain inside `Griddle.Platform`
- screenshot and recording services remain separate abstractions
- Griddle UI should be excludable without visually hiding windows
- media files remain external to session JSON
- the existing v1 capture model remains unchanged for v0.9.0-alpha
- recording interfaces and encoding details are deferred until recording implementation begins