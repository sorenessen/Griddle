# griddle
# Griddle

> A lightweight desktop overlay engine for presentations, annotations, and productivity.

Griddle is a native macOS application that lives above the desktop, providing lightweight tools that appear instantly, help the user accomplish a task, and disappear just as quickly.

It is designed to augment the operating system—not replace it.

Griddle is part of the **Calypso Toolbox**, alongside projects like **Corvus** and **Sparrow**, and serves as the foundation for desktop interaction, live presentations, and productivity utilities.

---

# Vision

Traditional annotation tools are built around drawing.

Griddle is built around **interaction**.

Drawing is only one capability.

The long-term goal is to create a lightweight desktop layer that can provide visual annotations, screen utilities, AI-powered tools, and presentation enhancements without interrupting the user's workflow.

The overlay should always feel:

- Instant
- Beautiful
- Native
- Unobtrusive

---

# Design Philosophy

Griddle should never feel like "another application."

It should feel like a natural extension of macOS.

## Core Principles

- Fast
- Zero friction
- Always available
- Never intrusive
- Keyboard-first
- Mouse-friendly
- Beautiful animations
- Plugin architecture
- Native macOS

Whenever a new feature is proposed, ask:

> **Does this make Griddle faster, simpler, or more useful?**

If not, it probably doesn't belong.

---

# Minimum Viable Product (MVP)

The first milestone is intentionally small.

## Features

- [ ] Transparent desktop overlay
- [ ] Global keyboard shortcut
- [ ] Freehand drawing
- [ ] Undo
- [ ] Clear annotations
- [ ] Escape to dismiss overlay
- [ ] Mouse pass-through when inactive

Nothing more.

The goal is simply to prove that Griddle can become a seamless desktop overlay.

---

# Future Roadmap

This list is intentionally aspirational.

## Presentation Tools

- Cursor spotlight
- Click animations
- Highlighter
- Arrow tool
- Circle tool
- Rectangle tool
- Magnifier
- Laser pointer
- Countdown timer

## Productivity Tools

- OCR
- Clipboard history
- Sticky notes
- Screenshot capture
- Screen recorder
- Color picker
- Pixel ruler
- Window inspector

## AI Features

- Corvus integration
- UI element detection
- Intelligent highlighting
- Voice commands
- AI-assisted presentation mode

## Workflow

- Custom tool layouts
- Presentation profiles
- Plugin marketplace
- User-created tools

---

# Architecture

Griddle is not a drawing application.

It is an **overlay engine**.

Drawing is simply the first tool.

```text
Griddle
│
├── App
│
├── Core
│
├── Overlay
│
├── HotBar
│
├── Tool Manager
│
├── Tools
│     ├── Pen
│     ├── Highlighter
│     ├── Arrow
│     ├── Circle
│     ├── Spotlight
│     ├── OCR
│     └── ...
│
├── Plugins
│
├── Animations
│
├── Preferences
│
└── Utilities
```

Each tool should be independent and easily added or removed without affecting the rest of the application.

---

# Development Milestones

## Phase 1

- [ ] Create transparent overlay
- [ ] Draw with mouse
- [ ] Global hotkey
- [ ] ESC dismisses overlay

## Phase 2

- [ ] Toolbar
- [ ] Shapes
- [ ] Undo
- [ ] Clear

## Phase 3

- [ ] Cursor pass-through
- [ ] Spotlight
- [ ] Click animations

## Phase 4

- [ ] Radial Hot Bar

## Phase 5

- [ ] AI-assisted tools

---

# Repository Structure

```text
Sources/
│
├── App/
├── Core/
├── Overlay/
├── HotBar/
├── ToolManager/
├── Tools/
├── Plugins/
├── Animations/
├── Preferences/
└── Utilities/
```

Avoid generic folders such as:

- Helpers
- Misc
- Stuff
- New Folder

Organize code by responsibility, not convenience.

---

# Commit Philosophy

Every commit should tell part of the project's story.

## Good Examples

- Initialize native macOS application structure
- Implement transparent overlay window
- Add global hotkey registration
- Introduce drawing canvas abstraction
- Implement stroke rendering pipeline
- Add undo/redo infrastructure
- Create plugin architecture
- Implement radial tool selector

## Avoid

- Update
- Fixed stuff
- Changes
- WIP

The Git history should read like a development journal.

---

# Long-Term Goal

Griddle should become the universal desktop interaction layer for macOS.

Imagine pressing a single keyboard shortcut and instantly accessing:

- Drawing
- Spotlight
- OCR
- AI Assistant
- Clipboard History
- Screenshot
- Screen Recording
- Color Picker
- Measurement Tools
- Corvus

...all without leaving the application you're currently using.

The best tools are the ones you forget are there.

Griddle should always feel one keystroke away, ready to help, and gone the moment you're finished.

---

## Motto

> **Let's start cooking. 🍳**
