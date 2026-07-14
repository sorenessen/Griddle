# Changelog
# Changelog

## v0.1.0

### Added

- Native macOS proof-of-concept
- Transparent overlay window
- Multi-display overlay support
- Click-through overlay mode
- Initial Griddle architecture

### Changed

- **Migrated the project from Swift/AppKit to Avalonia UI and C#**
- Replaced the Xcode-only architecture with a cross-platform .NET solution
- Introduced a shared Core/App/Platform project structure
- Established Windows and macOS as first-class deployment targets
- Consolidated rendering, models, and services into reusable .NET libraries

### Why

The original Swift/AppKit implementation successfully validated transparent overlays, mouse passthrough, and multi-monitor behavior. After proving the concept, development transitioned to Avalonia to enable a single cross-platform codebase while retaining native platform integrations where necessary.

## v0.1.1

### Added
- Floating toolbar
- Shared PenTool architecture
- Pen presets
- Highlighter preset

### Changed
- Replaced string colors with StrokeColor enum
- Introduced PenSettings model

### Fixed
- Toolbar interaction
- Avalonia constructor warning

## v0.1.2

### Changed

- Replaced segmented line rendering with continuous `StreamGeometry` rendering.

### Improved

- Smoother pen strokes.
- Cleaner highlighter rendering.
- Better stroke joins and visual continuity.

## v0.1.3

### Changed

- Introduced `ActiveToolService` as the single source of truth for the active drawing tool.
- Moved stroke interaction behavior into `ITool` implementations.
- Refactored `DrawingCanvas` to delegate pointer interaction to the active tool.

### Improved

- Simplified drawing pipeline.
- Reduced coupling between the canvas and drawing implementation.
- Established the foundation for future drawing tools.

## v0.1.4

### Added

- Persistent active tool highlighting in the toolbar.

### Changed

- Toolbar styling now reflects application state.
- Added compiled bindings for toolbar selection.

### Improved

- Clear visual indication of the active drawing preset.