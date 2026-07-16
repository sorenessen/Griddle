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

## v0.1.5

### Added

- Draggable floating toolbar.
- Cross-display toolbar movement.

### Improved

- Toolbar can be repositioned to avoid obstructing content.
- Toolbar buttons remain clickable while background areas support window dragging.

## v0.1.6

### Added

- Blue and Black pen colors.
- Persistent active color selection.

### Changed

- Refactored pen selection into a single parameterized API.
- Simplified color selection logic for future palette expansion.

### Improved

- Color selection now scales cleanly as additional colors are added.

## v0.1.7

### Added

- Native keyboard shortcuts.
- ⌘Z / Ctrl+Z for Undo.
- ⌘⇧Z / Ctrl+Shift+Z and ⌘Y / Ctrl+Y for Redo.
- P to select the Pen.
- H to select the Highlighter.

### Changed

- Keyboard tool selection now routes through the ToolbarViewModel to keep toolbar state synchronized.

## v0.1.8

### Added

- Arrow annotation tool.
- Arrow toolbar button with active selection state.
- Arrow rendering with configurable stroke styling.

### Changed

- Introduced StrokeKind to support multiple annotation types.
- DrawingCanvas now renders based on stroke type.
- Toolbar and DrawingCanvas now share a single ActiveToolService instance for synchronized tool selection.

### Added

- Added stable GUID identifiers to strokes.
- Added a selection tool and selection service.
- Added rectangle and arrow hit testing.
- Added visible dashed selection outlines for selected rectangles and arrows.

### Changed

- Renamed drawing interaction methods from `BeginStroke`, `ContinueStroke`, and `EndStroke` to `BeginInteraction`, `ContinueInteraction`, and `EndInteraction`.
- Updated the tool interface to support non-drawing tools that do not create strokes.

### Fixed

- Clear and Undo now remove stale selection state when the selected stroke is removed.

### Added

- Added deletion of selected annotations.
- Added command-based history actions for drawing and deletion.
- Added undo/redo support for deleted annotations.

### Changed

- Replaced stroke-only redo stack with a generalized history action model.
- Refactored undo/redo to support multiple action types.

### Added

- Added drag-to-move support for selected rectangles and arrows.
- Added undo and redo support for move operations.
- Added command-based history actions for add, delete, and move operations.

### Changed

- Replaced enum-based history handling with polymorphic `IHistoryAction` commands.
- Simplified undo and redo execution through action-specific behavior.