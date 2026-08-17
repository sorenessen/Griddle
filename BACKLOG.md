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
- [x] Callout Label at Target or Anchor
- [x] Spatially Aware Callout Label Placement
- [x] Keep Callout Labels Within Display Bounds
- [x] Callout Label Position Toolbar Control
- [x] Undo/Redo Callout Label Position
- [x] Callout Label Hit Testing at Either Endpoint
- [x] Reliable Canvas Keyboard Focus During Callout Text Entry

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
- [x] General Multi-select

### Planned

- [ ] Select Freehand Strokes
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
- [X] Remember Preferred Display
- [x] Handle Display Disconnect Gracefully
- [x] Handle Display Connect While Running

### Planned

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

- [x] Red, Orange, Yellow, Green, Blue, Purple, White, Black
- [x] Color Palette Popup

### Planned

- [ ] Toolbar Skin Selector
- [ ] Industrial Red Skin
- [ ] Electric Blue Skin
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
- [x] Remember Toolbar Position

### Planned

- [ ] Improve Callout Sequence Control Icons
- [ ] Reorganize / Group Toolbar Controls
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

## Capture & Export

### Screenshot Capture

- [x] Capture Active Griddle Display
- [x] Capture Display With Annotations
- [x] Capture Display Without Annotations
- [x] Save Screenshot to Session
- [x] Screenshot Metadata
- [x] Copy Screenshot to Clipboard
- [x] Save Screenshot as PNG

### Screen Recording

- [ ] Screen Recording Architecture
- [ ] Record Active Griddle Display
- [ ] Record Annotations During Presentation
- [ ] Recording Start / Stop Controls
- [ ] Recording Status Indicator
- [ ] Save Recording to Session
- [ ] Recording Metadata
- [ ] System Audio Capture
- [ ] Microphone Capture
- [ ] Combined System + Microphone Audio
- [ ] Audio Input Selection

### Export

- [ ] Save PNG
- [ ] Save SVG
- [ ] Save PDF
- [ ] Export Recording
- [ ] Export Presentation Video

---

## Presentations

### Foundations

- [ ] Presentation Model
- [ ] Create Presentation From Session
- [ ] Presentation Scenes
- [ ] Add Captures to Presentation
- [ ] Reorder Presentation Scenes
- [ ] Presentation Playback
- [ ] Full-Screen Presentation Mode

### Audio

- [ ] Presentation Audio Tracks
- [ ] Add Narration
- [ ] Record Narration
- [ ] Add Music / Audio Files
- [ ] Audio Playback
- [ ] Audio Volume Controls
- [ ] Mute / Unmute Tracks

### Future Editing

- [ ] Presentation Timeline
- [ ] Scene Duration
- [ ] Recording Trimming
- [ ] Audio Trimming
- [ ] Audio Fades
- [ ] Scene Transitions
- [ ] Annotation Animation
- [ ] Presentation Export

## Sessions & Documents

- [x] Save Annotation Session
- [x] Load Annotation Session
- [x] Auto Recovery
- [x] Griddle Session Model
- [x] Versioned Griddle Document Format
- [x] New Session
- [x] Save Session
- [x] Save Session As
- [x] Load Session
- [x] Restore Annotations
- [x] Restore Callout Sequences
- [x] Restore Annotation Styles
- [x] Restore Session Metadata
- [x] Dirty / Unsaved State Tracking
- [x] Recent Sessions
- [ ] Document Version Migration
- [ ] Graceful Handling of Unsupported Document Versions

---

## Collaboration

### Future

- [ ] Shared Griddle Sessions
- [ ] Collaborative Annotation
- [ ] Participant Identity
- [ ] Annotation Ownership
- [ ] Live Session Synchronization
- [ ] Shared Callout Sequences
- [ ] Shared Presentation Control
- [ ] Session Invitations
- [ ] Collaboration Permissions
- [ ] Conflict Resolution

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
- [x] Toolbar Control Organization
- [x] Color Palette Popup
- [x] General Multi-select

---

# Current Milestone

## v0.9.0-alpha — Sessions & Capture Foundations

### Goal

Transform Griddle from an ephemeral annotation overlay into a persistent workspace that can save, restore, and capture annotated work. Establish the document and capture architecture that future presentations, recordings, and collaboration features can build upon.

### Completed

### Phase 1 — Session Foundation

- [x] Define Griddle Session Model
- [x] Define Versioned Griddle Document Format
- [x] Serialize Annotation Data
- [x] Serialize Callout Sequence Data
- [x] Serialize Annotation Styles
- [x] Save Session
- [x] Load Session
- [x] Restore Session to Canvas
- [x] New Session
- [x] Dirty / Unsaved State Tracking

### Phase 2 — Screenshot Foundation

- [x] Define Capture Model
- [x] Capture Active Display
- [x] Capture Display With Griddle Annotations
- [x] Save Screenshot
- [x] Associate Screenshot With Current Session
- [x] Store Screenshot Metadata
- [x] Copy Screenshot to Clipboard

### Phase 3 — Session UX

- [x] Save / Save As Controls
- [x] Open Session
- [x] Recent Sessions
- [x] Unsaved Changes Prompt
- [x] Session Name / Metadata
- [x] Auto Recovery Foundation

### Phase 4 — Architecture & Hardening

- [x] Capture Display Without Annotations
- [x] Define Windows Screen Capture Implementation Boundary
- [x] Define Future Media Asset Model
- [x] Investigate Screen Recording APIs
- [x] Investigate System Audio Capture
- [x] Investigate Microphone Capture
- [x] Document Capture / Recording Architecture Decisions
- [x] Fix macOS Native File Menu Startup Synchronization

### Architecture / Research

- [x] Keep Session Model Platform-Agnostic
- [x] Keep Capture Abstractions Platform-Agnostic
- [x] Define macOS Screen Capture Implementation Boundary
- [x] Define Windows Screen Capture Implementation Boundary
- [x] Investigate Screen Recording APIs
- [x] Investigate System Audio Capture
- [x] Investigate Microphone Capture
- [x] Define Future Media Asset Model

### Exit Criteria - DONE

v0.9.0-alpha is complete when a user can:

1. Create annotations and callout sequences.
2. Save the current Griddle session.
3. Quit Griddle.
4. Reopen the saved session and recover the annotations accurately.
5. Capture the active display with Griddle annotations visible.
6. Save that capture as part of the session.

---

## v0.10.0-alpha — Screen Recording Foundations

### Goal

Extend Griddle's capture system from static screenshots to recorded demonstrations, allowing users to record the active Griddle display and preserve recordings as part of a session.

### Phase 1 — Recording Foundation

- [x] Define Screen Recording Service Contract
- [ ] Implement macOS Screen Recording Service
- [ ] Record Active Griddle Display
- [ ] Record Griddle Annotations
- [ ] Start Recording
- [ ] Stop Recording
- [ ] Recording Status Indicator

### Phase 2 — Session Integration

- [ ] Save Recording to Session Media
- [ ] Define Recording Metadata
- [ ] Associate Recording With Current Session
- [ ] Restore Recording Metadata When Session Loads
- [ ] Handle Missing Recording Media Gracefully

### Phase 3 — Audio

- [ ] Capture System Audio
- [ ] Capture Microphone Audio
- [ ] Record System Audio With Video
- [ ] Record Microphone With Video
- [ ] Record Combined System + Microphone Audio
- [ ] Enable / Disable System Audio
- [ ] Enable / Disable Microphone
- [ ] Select Microphone Input

### Phase 4 — Recording UX & Hardening

- [ ] Recording Start / Stop Toolbar Control
- [ ] Persistent Recording Indicator
- [ ] Prevent Accidental Session Close While Recording
- [ ] Handle Recording Permission Failures
- [ ] Handle Recording Failure Gracefully
- [ ] Verify Multi-Display Recording
- [ ] Verify Recording With Griddle Excluded
- [ ] Verify Recording With Griddle Included

### Discovered Issues

- [ ] Global system hotkeys for capture / recording controls
  - [ ] Screenshot with annotations
  - [ ] Screenshot without annotations
  - [ ] Start / stop recording

### Exit Criteria

v0.10.0-alpha is complete when a user can:

1. Start recording the active Griddle display.
2. Continue annotating normally while recording.
3. Stop the recording from Griddle.
4. Record system audio and/or microphone audio when enabled.
5. Save the resulting recording as part of the current session.
6. Reopen the session with the recording metadata intact.
7. Clearly see when Griddle is actively recording.

# Icebox

Interesting ideas that should not distract current development.

- [ ] Cloud Sync
- [ ] Plugin System
- [ ] Animation Support
- [ ] Whiteboard Mode