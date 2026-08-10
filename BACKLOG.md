# Griddle Product Backlog

## Vision

Griddle is a lightweight cross-platform screen annotation and presentation tool designed for technical demonstrations, presentations, pair programming, and collaborative meetings.

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
- [x] Optional Callout Text Labels
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
- [x] Delete Entire Sequence
- [x] Undo/Redo Sequence Deletion
- [x] Hide/Show Sequence
- [x] Undo/Redo Sequence Visibility
- [x] Empty Callouts Remain Valid Sequence Members
- [x] Sequence Selection Restores Canvas Keyboard Focus

### Planned

- [ ] Change Sequence Color
- [ ] Lock Sequence
- [ ] Reorder Callouts Manually
- [ ] Duplicate Callout
- [ ] Sequence Labels / Names
- [ ] Select Active Sequence
- [ ] Callout Alignment / Snapping

### Presentation

- [x] Present Callout Sequence
- [x] Progressive Callout Reveal
- [x] Advance to Next Callout
- [x] Previous Callout
- [x] Reset / Exit Sequence Presentation
- [x] Focus Sequence / Dim Other Annotations
- [x] Presentation Progress Indicator
- [x] Presentation Active-State Indicator
- [x] Safely Switch Between Presented Sequences

### Planned

- [ ] Presentation Keyboard Shortcut Configuration
- [ ] Presentation Sequence Picker
- [ ] Auto-advance Timing
- [ ] Presentation Notes

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
- [x] Multi-selection State Cleanup Across Undo/Redo

### Planned

- [ ] Select Freehand Strokes
- [ ] General Multi-select
- [ ] Group/Ungroup Arbitrary Shapes
- [ ] Duplicate Shape
- [ ] Layer Ordering
- [ ] Minimum Rectangle Size

---

## Overlay Controls

### Completed

- [x] macOS Click-through Overlay
- [x] Explicit Overlay Interaction State
- [x] Toggle Click-through from Toolbar
- [x] Griddle G Master Engagement Control
- [x] Visual ON/OFF G Indicator
- [x] Toolbar Remains Interactive During Click-through
- [x] Local Keyboard Shortcut for Click-through
- [x] ⌘⇧G / Ctrl+Shift+G Toggle
- [x] Optional Overlay Tint
- [x] Toggle Tint Independently of Click-through
- [x] Tint OFF by Default
- [x] Tint Active-State Indicator
- [x] Clear Presentation Overlay
- [x] Remove Default Button Flash from G Control

### Planned

- [ ] Global System Hotkey for Click-through
- [ ] Configurable Tint Color
- [ ] Configurable Tint Opacity
- [ ] Persist Tint Preference
- [ ] Persist Click-through Startup Preference

---

### Multi-Display

#### Completed

- [x] Detect Connected Displays
- [x] Display Selection Menu
- [x] Identify Active Display
- [x] Move Annotation Overlay Between Displays
- [x] Move Toolbar With Selected Display
- [x] Preserve Overlay Interaction State When Switching Displays
- [x] Preserve Existing Annotations When Switching Displays
- [x] Live Display Selection Checkmark Update
- [x] Correct Display Selection After Menu Reopen
- [x] Multi-Display Overlay Bounds Management

### Planned

- [ ] Remember Preferred Display
- [ ] Handle Display Disconnect Gracefully
- [ ] Handle Display Connect While Running
- [ ] Identify Displays by Friendly Name
- [ ] Annotate Multiple Displays Simultaneously

---

## Appearance

### Branding & Toolbar

- [x] Griddle G Industrial Control Logo
- [x] Distressed Industrial Toolbar Skin
- [x] Dark Mounted Control Styling
- [x] Metallic Toolbar Border
- [x] Active Tool Highlighting
- [x] Griddle Engagement Indicator
- [x] Expandable Texture-Based Toolbar Background

### Colors

- [x] Red
- [x] Blue
- [x] Black

### Planned

- [ ] Toolbar Skin Selector
- [ ] Industrial Red Skin
- [ ] Electric Blue Skin
- [ ] Color Palette Popup
- [ ] Custom Colors
- [ ] Recent Colors

### Stroke

- [x] Thickness Picker
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
- [x] Presentation Controls
- [x] Presentation Progress Display
- [x] Master Overlay Engagement Control
- [x] Tint Toggle
- [x] Industrial Griddle Branding

### Planned

- [ ] Improve Callout Sequence Control Icons
- [ ] Reorganize / Group Toolbar Controls
- [ ] Remember Toolbar Position
- [ ] Dock to Screen Edge
- [ ] Compact Mode
- [ ] Horizontal / Vertical Layout
- [ ] Overflow Menu for Smaller Displays
- [ ] Tool Groups / Separators

---

## Productivity

### Completed

- [x] Undo
- [x] Redo
- [x] Keyboard Shortcuts
- [x] Click-through Keyboard Shortcut
- [x] Keyboard Nudge
- [x] History Tracking for Text Creation
- [x] History Tracking for Callout Creation
- [x] Atomic History for Sequence Renumbering
- [x] Atomic History for Sequence Movement
- [x] Atomic History for Sequence Deletion
- [x] Atomic History for Sequence Visibility

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

- [x] Transparent Overlay
- [x] Click-through Support
- [x] Always-on-top Toolbar
- [x] Runtime Click-through Toggle
- [x] Multi-Display Detection
- [x] Runtime Display Switching
- [x] Overlay Repositioning Across Displays
- [x] Toolbar Repositioning Across Displays

### Windows

- [ ] Native Overlay
- [ ] Click-through Support
- [ ] Runtime Click-through Toggle

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
- [ ] Animated Callout Reveal
- [ ] Toolbar Theme Packs

---

# Milestones

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
- [x] Sequence Deletion
- [x] Sequence Visibility
- [x] Progressive Presentation
- [x] Presentation Focus / Dimming
- [x] Presentation Progress
- [x] Undo/Redo Group Operations

---

# Current Milestone

## v0.8.0-alpha — Overlay & Multi-Display UX

### Goal

Make Griddle practical during live demonstrations by allowing presenters to move seamlessly between annotation and normal application interaction, including across multiple displays, without interrupting their workflow.

### Completed

- [x] Master G Engagement Control
- [x] Runtime Click-through Toggle
- [x] ON/OFF Engagement Indicator
- [x] ⌘⇧G / Ctrl+Shift+G Shortcut
- [x] Optional Overlay Tint
- [x] Tint OFF by Default
- [x] Independent Tint / Click-through State
- [x] Industrial Toolbar Branding
- [x] Distressed Metal Toolbar Skin
- [x] Callout Interaction Regression Fixes
- [x] Thickness Picker
- [x] Connected Display Detection
- [x] Display Selection Menu
- [x] Runtime Display Switching
- [x] Overlay Movement Between Displays
- [x] Toolbar Movement Between Displays
- [x] Active Display Indicator
- [x] Live Display Selection State Updates
- [X] Toolbar Position Persistence
	- [x] Validate saved toolbar position against currently connected displays
	- [x] Fallback to active display if saved position is off-screen

### Next

- [ ] Toolbar Control Organization
- [ ] Color Palette Popup
- [ ] General Multi-select

---

# Icebox

Interesting ideas that should not distract current development.

- [ ] Collaborative Annotation
- [ ] Cloud Sync
- [ ] Plugin System
- [ ] Animation Support
- [ ] Whiteboard Mode