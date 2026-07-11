# ADR-0001: Project Foundation

## Status

Accepted

## Decision

Griddle will begin as a native macOS menu bar application using AppKit for the overlay system.

The initial architecture consists of:

- Menu bar application
- Transparent overlay window
- Tool-based architecture
- Plugin-ready design

## Notes

Observed development-time warnings while running under Xcode:

- Unable to obtain a task name port right...
- layoutSubtreeIfNeeded warning

Current assessment:

- No functional impact.
- Overlay behaves correctly.
- Continue development and revisit if warnings persist after the drawing engine is implemented.
