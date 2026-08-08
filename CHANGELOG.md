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

### Added

- Added keyboard arrow controls for nudging selected shapes.
- Added Shift + Arrow shortcuts for larger 10-pixel movements.
- Added undo and redo support for keyboard nudge operations.

### Added

- Added corner resize handles to selected rectangles.
- Resize handles remain aligned while selected rectangles are moved.

### Added

- Added bottom-right rectangle resize handle.
- Added stable anchor-based rectangle resizing.
- Added undo and redo support for rectangle resize operations.

## v0.6.0-alpha

### Added

- Added Text annotation tool.
- Added toolbar control and active selection state for Text.
- Added direct keyboard text entry on the annotation canvas.
- Added blinking caret while editing text.
- Added selection support for Text annotations.
- Added editing of existing Text annotations.
- Added movement of Text annotations.

### Changed

- Extended annotation rendering to support persistent text content.
- Extended selection behavior to support text-based annotations.

### Fixed

- Added history tracking for Text creation so newly created Text annotations can be removed correctly with Undo.
- Improved text editing and selection behavior.

---

## v0.7.0-alpha

### Added

- Added numbered Callout annotation tool.
- Added Callout toolbar control and active selection state.
- Added text labels to Callouts.
- Added neutral placeholder indicator while a Callout is being created.
- Added automatic sequential Callout numbering.
- Added selection outlines and endpoint handles for Callouts.
- Added independent movement of Callout endpoints.
- Added resize cursor feedback for Callout endpoints.
- Added movement of completed Callouts.

### Callout Sequences

- Added independent Callout sequences using persistent group identifiers.
- Added support for starting a new Callout sequence beginning at 1.
- Added support for continuing an existing Callout sequence.
- Added automatic reuse of missing numbers within a sequence.
- Added automatic switch to the Callout tool when continuing an existing sequence.
- Added explicit sequence compaction and renumbering.
- Added selection of an entire Callout sequence.
- Added movement of an entire selected Callout sequence.

### History

- Added history tracking for Callout creation.
- Added undo and redo support for Callout endpoint movement.
- Added atomic undo and redo for Callout sequence renumbering.
- Added atomic undo and redo for Callout sequence movement.

### Changed

- Extended `Stroke` with Callout numbering and sequence identity.
- Extended `SelectionService` to support multiple selected strokes while preserving a primary selection.
- Extended selection rendering to display selection state for multiple annotations.
- Extended movement behavior to operate on selected Callout sequences.
- Callout numbering now finds the lowest available number within the active sequence instead of relying on a global incrementing counter.

### Fixed

- Fixed Callouts being omitted from creation history and becoming impossible to remove through Undo.
- Fixed Text annotations being omitted from creation history.
- Fixed deleted Callout numbers being permanently skipped when replacement Callouts are created.
- Fixed older Callout sequences being unable to resume numbering after a newer sequence was created.
- Fixed group dragging causing the grabbed Callout to move twice as far as other members of the selected sequence.

