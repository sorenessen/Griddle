# Griddle

> A lightweight cross-platform screen annotation and presentation layer for live demos, technical walkthroughs, meetings, and collaborative work.

Griddle is a desktop overlay application that lets users annotate, explain, and present directly on top of the applications they are already using.

Rather than pulling users into a separate whiteboard or presentation environment, Griddle stays out of the way until it is needed. Its transparent overlay can capture input for annotation, switch into click-through mode for normal application interaction, and preserve annotations while the presenter continues working underneath them.

Griddle is part of the **Calypso Toolbox**, alongside projects such as **Sparrow** and **Corvus**.

---

# What Griddle Is

Griddle is not simply a drawing application.

It is an **interactive presentation overlay**.

Drawing is one part of the experience, but the larger goal is to provide a lightweight layer for:

- live screen annotation
- technical demonstrations
- structured callout sequences
- presentation walkthroughs
- visual emphasis
- collaborative explanation
- desktop productivity tools

The underlying application should remain the center of attention.

Griddle exists to enhance it.

---

# Product Philosophy

Griddle should feel immediate.

A presenter should be able to move from using an application, to annotating it, to presenting a sequence, and back to normal interaction without breaking the flow of the demonstration.

The core principles are:

- **Fast**
- **Low friction**
- **Unobtrusive**
- **Keyboard-friendly**
- **Mouse-friendly**
- **Visually distinctive**
- **Presentation-first**
- **Cross-platform**
- **Extensible**

A useful question for every feature is:

> **Does this help someone explain, demonstrate, or interact with what is already on their screen more effectively?**

If not, it probably does not belong in Griddle.

---

# Current Capabilities

## Drawing & Annotation

Griddle currently supports:

- [x] Pen
- [x] Highlighter
- [x] Arrow
- [x] Rectangle
- [x] Text
- [x] Numbered Callouts
- [x] Multiple annotation colors
- [x] Selection
- [x] Move
- [x] Keyboard nudge
- [x] Rectangle resizing
- [x] Arrow endpoint editing
- [x] Callout endpoint editing
- [x] Delete
- [x] Undo
- [x] Redo

---

## Callout Sequences

Callouts can be organized into independent numbered sequences.

This allows several unrelated walkthroughs to coexist on the same screen without forcing all callouts into one global numbering system.

Current sequence capabilities include:

- [x] Automatic numbering
- [x] Independent sequence groups
- [x] Start a new sequence
- [x] Continue an existing sequence
- [x] Reuse missing sequence numbers
- [x] Compact / renumber a sequence
- [x] Select an entire sequence
- [x] Move an entire sequence
- [x] Delete an entire sequence
- [x] Hide and show a sequence
- [x] Undo / redo group operations

Example:

```text
Navigation
1 → Search
2 → Filters
3 → Settings

Main Panel
1 → Create
2 → Edit
3 → Save
```

Both groups can exist independently on the same screen.

---

# Presentation Mode

Griddle includes a presentation workflow built around callout sequences.

A presenter can select a sequence and enter presentation mode. The sequence is temporarily hidden while unrelated annotations dim into the background.

The presenter can then reveal the sequence one step at a time.

```text
0 / 3
    ↓
1 / 3
    ↓
2 / 3
    ↓
3 / 3
```

Current presentation features include:

- [x] Present selected sequence
- [x] Progressive callout reveal
- [x] Advance with Right Arrow
- [x] Step backward with Left Arrow
- [x] Exit with Escape
- [x] Restore the full sequence on exit
- [x] Dim unrelated annotations
- [x] Presentation progress indicator
- [x] Presentation active-state indicator
- [x] Switch between presentation sequences safely

Presentation visibility is maintained separately from normal annotation visibility, so presenting a sequence does not overwrite the user's hide/show choices.

---

# Overlay Interaction

Griddle's overlay can operate in two distinct modes.

## Engaged

The overlay captures pointer input.

This mode is used for:

- drawing
- selecting
- moving
- resizing
- editing

## Click-through

The overlay ignores pointer input and allows the user to interact normally with the application underneath it.

Annotations remain visible.

The Griddle toolbar remains available while the overlay is click-through.

The large **Griddle G control** at the left side of the toolbar acts as the master engagement control.

```text
G illuminated
    → overlay engaged

G inactive
    → click-through
```

The current local shortcut is:

```text
macOS:      ⌘ ⇧ G
Windows:    Ctrl + Shift + G
```

A future platform-level implementation may support the shortcut globally even when another application owns keyboard focus.

---

# Overlay Tint

The screen tint that originally helped indicate overlay activity is now optional.

Tint and click-through are independent states.

This allows combinations such as:

```text
Engaged + Tint
Engaged + Clear
Click-through + Tint
Click-through + Clear
```

Griddle now starts with the tint **off by default**, keeping the underlying application visually unchanged during demonstrations.

---

# Toolbar

The floating toolbar is:

- [x] Always on top
- [x] Draggable
- [x] State-aware
- [x] Presentation-aware
- [x] Available during click-through
- [x] Visually branded

The current design uses a distressed industrial-metal shell with dark mounted controls and a mechanical Griddle G engagement indicator.

The toolbar is intentionally built from normal controls over a scalable background texture rather than from a fixed toolbar image. This allows new controls to be added without rebuilding the entire toolbar skin.

Future toolbar work includes:

- control grouping
- overflow handling
- compact mode
- toolbar position persistence
- improved callout sequence icons
- horizontal / vertical layouts
- optional toolbar skins

---

# Architecture

Griddle is implemented as a cross-platform .NET application using **Avalonia UI**.

The current solution is divided into three projects:

```text
Griddle.slnx

src/
├── Griddle.App
├── Griddle.Core
└── Griddle.Platform
```

## Griddle.App

Application and UI responsibilities.

Examples include:

- overlay window
- floating toolbar
- drawing canvas
- rendering
- pointer interaction
- keyboard interaction
- presentation state
- toolbar bindings

## Griddle.Core

Platform-independent application logic.

Examples include:

- models
- tools
- services
- selection
- history actions
- annotation behavior

## Griddle.Platform

Native platform integration.

Examples include:

- macOS window interoperability
- click-through support
- future Windows overlay behavior

---

# Tool Architecture

Drawing and interaction tools implement shared tool behavior rather than placing every interaction directly inside the canvas.

Current tools include:

```text
Tools
├── SelectionTool
├── PenTool
├── Highlighter
├── ArrowTool
├── RectangleTool
├── TextTool
└── CalloutTool
```

`ActiveToolService` provides the shared active-tool state used by both the toolbar and drawing surface.

This keeps the toolbar, canvas, and tool implementations synchronized.

---

# History Architecture

Undo and redo are implemented through command-style history actions using `IHistoryAction`.

Current history operations include:

- Add stroke
- Delete stroke
- Move stroke
- Resize stroke
- Move arrow endpoint
- Move stroke group
- Delete stroke group
- Renumber callout group
- Set group visibility

This allows complex operations such as moving or renumbering an entire callout sequence to remain a single atomic undo action.

---

# Platform Support

## macOS

Currently implemented:

- [x] Transparent overlay
- [x] Always-on-top toolbar
- [x] Runtime click-through
- [x] Overlay interaction toggle
- [x] Keyboard click-through shortcut

## Windows

Planned:

- [ ] Native transparent overlay
- [ ] Click-through support
- [ ] Runtime interaction toggle
- [ ] Platform shortcut support

## Linux

Linux is not currently a primary development target.

---

# Current Development

## v0.8.0-alpha — Overlay Controls & Presentation UX

The current development cycle focuses on making Griddle practical during real live demonstrations.

Recently completed work includes:

- Griddle G master engagement control
- illuminated overlay state indicator
- runtime click-through switching
- click-through keyboard shortcut
- optional overlay tint
- clear overlay by default
- presentation sequence controls
- progressive callout presentation
- group visibility
- group movement and deletion
- sequence history operations
- industrial toolbar branding

Current priorities include:

- toolbar organization
- thickness controls
- color palette
- general multi-select
- toolbar position persistence
- Windows overlay support

See the project backlog for the full roadmap.

---

# Roadmap

## Drawing

- Ellipse
- Line
- Freeform polygon
- Thickness controls
- Custom colors
- Fill and transparency

## Editing

- General multi-select
- Arbitrary grouping
- Duplicate shape
- Layer ordering
- Alignment and snapping

## Callouts

- Sequence colors
- Sequence labels
- Manual reordering
- Lock sequence
- Alternate callout styles

## Presentation

- Sequence picker
- Auto-advance
- Presenter notes
- Additional focus effects
- Configurable shortcuts

## Export

- Copy annotated screen to clipboard
- PNG export
- SVG export
- PDF export

## Sessions

- Save annotation session
- Load session
- Auto recovery

## Future Tools

- Spotlight
- Magnifier
- Blur
- Laser pointer
- Countdown timer
- Screen freeze
- Zoom window
- OCR
- AI-assisted shape recognition

---

# Development Principles

## Keep Features Independent

New tools should be added without requiring unrelated systems to be rewritten.

## Preserve Interaction Flow

Griddle should never force the presenter to stop what they are doing merely to operate Griddle.

## Keep Presentation State Temporary

Presentation effects should not silently mutate the underlying annotation state.

## Treat History as a First-Class Feature

Any meaningful canvas mutation should have predictable Undo/Redo behavior.

## Prefer Explicit State

Click-through, tint, presentation visibility, and annotation visibility are intentionally modeled as separate concepts.

---

# Commit Philosophy

Every commit should describe a meaningful change to the product.

Good examples:

```text
Add callout sequence selection and group movement
Add independent numbering sequences for callouts
Add overlay engagement controls
Add progressive callout presentation
Add tint controls and fix callout interaction bugs
```

Avoid vague messages such as:

```text
Update
Changes
Fixed stuff
WIP
```

The Git history should read like a development journal.

---

# Calypso Toolbox

Griddle is part of a growing family of Calypso tools.

Current projects include:

- **Griddle** — screen annotation and presentation overlay
- **Sparrow** — developer workspace and environment tooling
- **Corvus** — AI initiative

Each product is independent, but they share the same goal:

> Build focused tools that remove friction without getting in the user's way.

---

# Long-Term Goal

Griddle should become a lightweight interaction layer that can sit between the user and the desktop without taking over the desktop.

Imagine being able to:

- annotate an application
- walk through numbered steps
- highlight an interface
- interact with the underlying application
- reveal presentation annotations progressively
- capture the result
- invoke visual utilities
- use AI-assisted tools

...without ever leaving the application being demonstrated.

The best version of Griddle should feel less like opening another program and more like gaining another layer of control over the screen already in front of you.

---

## Motto

> ** Motto y'self, I'm cookin'. 🍳 **
