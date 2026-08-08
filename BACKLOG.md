# Griddle Product Backlog

## Vision

Griddle is a lightweight cross-platform screen annotation tool designed for technical demonstrations, presentations, pair programming, and collaborative meetings.

---

# Epics

## Drawing Tools

### Completed

- [x] Pen
- [x] Highlighter
- [x] Arrow
- [x] Rectangle
- [x] Text
- [x] Callout

### Planned

- [ ] Ellipse
- [ ] Line
- [ ] Freeform Polygon

---

## Callouts & Sequences

### Completed

- [x] Numbered Callouts
- [x] Automatic Sequential Numbering
- [x] Neutral Placeholder While Drawing
- [x] Callout Text Labels
- [x] Select Callouts
- [x] Move Callouts
- [x] Resize Callout Endpoints
- [x] Callout Resize Cursors
- [x] Undo/Redo Callout Creation
- [x] Independent Callout Sequences
- [x] Start New Callout Sequence
- [x] Continue Existing Callout Sequence
- [x] Automatically Fill Missing Sequence Numbers
- [x] Compact/Renumber Callout Sequence
- [x] Undo/Redo Sequence Renumbering
- [x] Select Entire Callout Sequence
- [x] Move Entire Callout Sequence
- [x] Undo/Redo Sequence Movement

### Planned

- [ ] Delete Entire Sequence
- [ ] Change Sequence Color
- [ ] Hide/Show Sequence
- [ ] Lock Sequence
- [ ] Reorder Callouts Manually
- [ ] Duplicate Callout
- [ ] Sequence Labels / Names
- [ ] Select Active Sequence
- [ ] Callout Alignment / Snapping

### Presentation

- [ ] Present Callout Sequence
- [ ] Progressive Callout Reveal
- [ ] Advance to Next Callout
- [ ] Previous Callout
- [ ] Reset Sequence Presentation
- [ ] Focus Sequence / Dim Other Annotations

---

## Text

### Completed

- [x] Text Tool
- [x] Text Entry
- [x] Blinking Caret
- [x] Select Text
- [x] Edit Existing Text
- [x] Move Text
- [x] Undo/Redo Text Creation

### Planned

- [ ] Font Size
- [ ] Font Family
- [ ] Text Color
- [ ] Text Alignment
- [ ] Text Background
- [ ] Rich Text Styling

---

## Editing

### Completed

- [x] Selection Tool
- [x] Select Rectangles
- [x] Select Arrows
- [x] Select Text
- [x] Select Callouts
- [x] Multi-select Callout Sequences
- [x] Move Shapes
- [x] Move Callout Sequences
- [x] Keyboard Nudge for Selected Shapes
- [x] Rectangle Resize Handles
- [x] Bottom-right Rectangle Resize
- [x] Top-left Rectangle Resize
- [x] Top-right Rectangle Resize
- [x] Bottom-left Rectangle Resize
- [x] Resize Cursors
- [x] Arrow Endpoint Editing
- [x] Callout Endpoint Editing
- [x] Delete Selected Shape
- [x] Undo/Redo Rectangle Resize
- [x] Undo/Redo Group Movement

### Planned

- [ ] Select Freehand Strokes
- [ ] General Multi-select
- [ ] Group/Ungroup Arbitrary Shapes
- [ ] Duplicate Shape
- [ ] Layer Ordering
- [ ] Minimum Rectangle Size

---

## Appearance

### Colors

- [x] Red
- [x] Blue
- [x] Black

### Planned

- [ ] Color Palette Popup
- [ ] Custom Colors
- [ ] Recent Colors

### Stroke

- [ ] Thickness Picker
- [ ] Dashed Lines
- [ ] Dotted Lines
- [ ] Opacity Slider

### Fill

- [ ] Filled Rectangles
- [ ] Filled Ellipses
- [ ] Transparency

---

## User Experience

### Toolbar

- [x] Draggable
- [x] Active Tool Highlighting
- [x] Callout Sequence Controls

### Planned

- [ ] Improve Callout Sequence Control Icons
- [ ] Remember Toolbar Position
- [ ] Dock to Screen Edge
- [ ] Compact Mode
- [ ] Horizontal / Vertical Layout

---

## Productivity

### Completed

- [x] Undo
- [x] Redo
- [x] Keyboard Shortcuts
- [x] History Tracking for Text Creation
- [x] History Tracking for Callout Creation
- [x] Atomic History for Sequence Renumbering
- [x] Atomic History for Sequence Movement

### Planned

- [ ] Command Palette
- [ ] Configurable Shortcuts
- [ ] Preferences
- [ ] Recently Used Tools

---

## Export

- [ ] Copy to Clipboard
- [ ] Save PNG
- [ ] Save SVG
- [ ] Save PDF

---

## Sessions

- [ ] Save Annotation Session
- [ ] Load Annotation Session
- [ ] Auto Recovery

---

## Platform

### macOS

- [x] Click-through Overlay
- [x] Always-on-top Toolbar

### Windows

- [ ] Native Overlay
- [ ] Click-through Support

### Linux

- [ ] Overlay Investigation

---

## Performance

- [ ] Shape Caching
- [ ] Renderer Optimization
- [ ] High DPI Improvements
- [ ] Large Canvas Performance

---

# Future Ideas

These are intentionally not prioritized.

- [ ] Spotlight Tool
- [ ] Magnifier
- [ ] Blur Tool
- [ ] Laser Pointer
- [ ] Countdown Timer
- [ ] Screen Freeze
- [ ] Zoom Window
- [ ] Presenter Notes
- [ ] OCR Integration
- [ ] AI-assisted Shape Recognition
- [ ] Alternate Callout Styles
- [ ] Collapsible Callout Labels

---

# Current Milestone

## v0.7.0-alpha — Callout Sequences & Presentation Foundations

### Goal

Evolve Griddle's annotations from independent shapes into structured presentation objects that can be manipulated and presented as groups.

### Completed

- [x] Numbered Callouts
- [x] Editable Callout Endpoints
- [x] Independent Callout Sequences
- [x] Gap-aware Number Assignment
- [x] Continue Existing Sequence
- [x] Sequence Renumbering
- [x] Sequence Selection
- [x] Sequence Movement
- [x] Undo/Redo Group Operations

### Next

- [ ] Delete Entire Sequence
- [ ] Sequence Visibility
- [ ] Present Sequence
- [ ] Progressive Callout Reveal

---

# Icebox

Interesting ideas that should not distract current development.

- [ ] Collaborative Annotation
- [ ] Cloud Sync
- [ ] Plugin System
- [ ] Animation Support
- [ ] Whiteboard Mode