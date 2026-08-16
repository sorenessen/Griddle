# Windows Screen Capture Architecture

## Purpose

Define the Windows implementation boundary for Griddle screen capture without introducing Windows-specific types into Griddle.Core or Griddle.App.

## Platform Boundary

Windows screen capture will be implemented in:

`src/Griddle.Platform/Windows/`

The implementation will satisfy the existing platform-neutral contract:

- `IScreenCaptureService`
- `CaptureRegion`
- `ScreenCaptureOptions`
- `ScreenCaptureResult`

Windows-specific types such as `HWND`, `HMONITOR`, COM interfaces, WinRT capture objects, Direct3D surfaces, and DXGI resources must remain inside `Griddle.Platform`.

## Primary Capture API

Use `Windows.Graphics.Capture` as the primary Windows screenshot backend.

For display capture, create a `GraphicsCaptureItem` programmatically from the target monitor using:

`IGraphicsCaptureItemInterop::CreateForMonitor`

The capture implementation should:

1. Resolve the target monitor from `CaptureRegion`.
2. Create a capture item for that monitor.
3. Acquire a frame using `GraphicsCaptureSession`.
4. Crop to the requested `CaptureRegion` if necessary.
5. Encode the result as PNG.
6. Return the encoded bytes and pixel dimensions through `ScreenCaptureResult`.

## Griddle Window Exclusion

`ScreenCaptureOptions.IncludeApplicationWindows` controls whether Griddle itself appears in the capture.

When:

`IncludeApplicationWindows = true`

capture normally.

When:

`IncludeApplicationWindows = false`

Griddle top-level windows should be excluded from capture using Windows capture-exclusion support such as:

`SetWindowDisplayAffinity(..., WDA_EXCLUDEFROMCAPTURE)`

This should apply to Griddle windows such as:

- annotation overlay
- toolbar
- capture controls introduced later

The application UI should not need to hide or flicker during capture.

## Coordinate Handling

`CaptureRegion` remains platform-neutral and uses desktop coordinates:

- X
- Y
- Width
- Height

The Windows implementation is responsible for mapping those coordinates to the correct monitor and capture surface.

High-DPI scaling and physical pixel conversion remain platform implementation concerns.

## Fallback / Future Recording Backend

DXGI Desktop Duplication may be evaluated later as:

- a fallback capture mechanism
- a higher-performance recording backend
- a source for continuous frame acquisition

It is not required for the initial Windows screenshot implementation.

## Architectural Constraints

The Windows implementation must not require changes to:

`Griddle.Core`

or expose Windows-specific types to:

`Griddle.App`

The existing capture abstraction should remain usable by both macOS and Windows.

## Current Decision

No Windows capture implementation is required for v0.9.0-alpha.

This document defines the boundary so a future `WindowsScreenCaptureService` can be implemented without reshaping the current capture architecture.