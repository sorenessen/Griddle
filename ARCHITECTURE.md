> "Build the simplest architecture that enables the next feature.
> Avoid solving problems before they exist."

# Why Avalonia?

Griddle began as a native Swift/AppKit application to validate transparent overlays and click-through windows on macOS.

Once those platform risks were eliminated, the project migrated to Avalonia UI and .NET to achieve:

- Shared business logic
- Cross-platform desktop support
- Unified rendering pipeline
- Faster feature development
- Reduced platform-specific maintenance

Native platform code remains isolated to the Platform project for operating-system-specific integrations.

# Griddle Architecture

## Vision

Griddle is a cross-platform desktop annotation application developed by Calypso Labs.

The goal is to provide a fast, lightweight overlay that allows users to draw, annotate, highlight, and present on top of any desktop application with minimal interruption to their workflow.

---

# History

## Phase 1 — Native macOS Prototype

The original implementation was built using Swift and AppKit.

This phase focused exclusively on answering platform-risk questions:

- Can a transparent overlay be created?
- Can mouse click-through be toggled?
- Can multiple displays be supported?
- Can an always-on-top overlay coexist with normal desktop applications?

Once these questions were successfully answered, the prototype was considered complete.

---

## Phase 2 — Cross-Platform Migration

After validating the concept, Griddle migrated to Avalonia UI and .NET.

Reasons for the migration included:

- Shared code between macOS and Windows
- Faster feature development
- Single rendering architecture
- Easier maintenance
- Modern C# tooling
- Reduced platform-specific code

Native integrations remain isolated inside the Platform project.

---

# Solution Structure

src/

    Griddle.App
        Avalonia UI
        Windows
        Views
        ViewModels

    Griddle.Core
        Models
        Services
        Geometry
        Tools

    Griddle.Platform
        macOS
        Windows (future)

---

# Core Design Principles

## Models own state

Examples:

- Stroke
- PenSettings

Models should never know about UI.

---

## Services perform work

Examples:

- StrokeBuilder

Services should contain behavior without owning presentation.

---

## Views render

Avalonia windows and controls should contain minimal business logic.

---

## Tools encapsulate interaction

Examples:

- PenTool

Future:

- ArrowTool
- RectangleTool
- TextTool

---

# Current Rendering Pipeline

Toolbar

↓

PenTool

↓

PenSettings

↓

StrokeBuilder

↓

Stroke

↓

DrawingCanvas

↓

Avalonia Renderer

---

# Branch Strategy

Main

Always stable.

Every commit should:

- Build
- Run
- Be demoable

Feature branches:

- feature/highlighter
- feature/stroke-rendering
- feature/toolbar-drag

Each branch should represent one feature with a clear demonstration of completion.

---

# Long-Term Vision

Griddle is intended to become a professional presentation and annotation platform.

Planned capabilities include:

- Advanced pen presets
- Shape tools
- Text tools
- Screenshot capture
- Session persistence
- Export
- Plugin architecture
- Cross-platform deployment