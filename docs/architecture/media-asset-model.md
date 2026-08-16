# Future Media Asset Model

## Purpose

Define how Griddle should represent screenshots, recordings, audio, and other future media without changing the current v1 session document format prematurely.

## Current State

Griddle currently stores session screenshots as `GriddleCapture` objects.

The current capture model contains:

- Id
- Kind
- CreatedAt
- FileName
- Width
- Height
- DisplayName
- IncludesAnnotations

`CaptureKind` currently includes:

- Screenshot
- Recording

Session documents persist captures through `CaptureDocument`.

This model is sufficient for the current v0.9 screenshot functionality but becomes increasingly screenshot-specific as recording and audio capabilities are introduced.

## Future Direction

Future session media should use a broader media asset abstraction rather than continuing to expand `GriddleCapture`.

Conceptually:

`GriddleMediaAsset`

Common properties should include:

- Id
- Kind
- CreatedAt
- FileName
- MimeType

Optional media-specific properties may include:

- Duration
- Width
- Height
- DisplayName
- IncludesAnnotations
- HasSystemAudio
- HasMicrophoneAudio
- Source

## Media Asset Kinds

Future media kinds may include:

- Screenshot
- ScreenRecording
- AudioRecording
- Narration
- ImportedAudio
- ImportedVideo

Additional kinds may be added as presentation and collaboration features evolve.

## Session Storage

Media files should remain external to the `.griddle` document and stored beside the session in its media directory.

Example:

`Demo.griddle`

`Demo.media/`
- screenshot-001.png
- recording-001.mp4
- narration-001.m4a

The `.griddle` document stores metadata and relative asset references rather than embedding media bytes directly.

## Platform Boundary

Capture and recording implementations remain platform-specific.

`Griddle.Core` should contain only platform-neutral media metadata.

macOS, Windows, and future platform implementations are responsible for producing media files and reporting their metadata back through platform-neutral contracts.

No platform-specific capture, codec, audio-device, or native API types should appear in the session model.

## Document Versioning

The current v1 document format should continue using:

- `GriddleCapture`
- `CaptureDocument`
- `Captures`

Do not rename or migrate these structures during v0.9.0-alpha.

A future document version may introduce:

- `GriddleMediaAsset`
- `MediaAssetDocument`
- `MediaAssets`

That migration should occur only when recording or additional media types require it.

## Architectural Decision

For v0.9.0-alpha:

- Keep the existing capture model unchanged.
- Treat `GriddleCapture` as the current screenshot/capture representation.
- Define `GriddleMediaAsset` as the future direction.
- Keep all media files external to the `.griddle` JSON document.
- Store only metadata and relative file references in session documents.
- Defer document migration until a future media feature requires it.