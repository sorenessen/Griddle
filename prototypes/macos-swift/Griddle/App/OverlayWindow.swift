import AppKit

final class OverlayWindow: NSWindow {

    init(screen: NSScreen) {
        super.init(
            contentRect: screen.frame,
            styleMask: [.borderless],
            backing: .buffered,
            defer: false
        )
        configureWindow()
        configureContentView()
    }

    private func configureWindow() {
        // The overlay itself has no visible window background.
        isOpaque = false
        backgroundColor = .clear

        // Keep the overlay above ordinary application windows.
        level = .floating

        // Prevent the overlay from appearing in Mission Control
        // as a normal application window.
        collectionBehavior = [
            .canJoinAllSpaces,
            .fullScreenAuxiliary,
            .stationary
        ]

        // The overlay should not have normal window behavior.
        hasShadow = false
        isMovable = false
        isMovableByWindowBackground = false
        isReleasedWhenClosed = false

        // The overlay starts in pass-through mode.
        ignoresMouseEvents = true

        // Prevent the overlay from stealing keyboard focus.
        hidesOnDeactivate = false
    }

    private func configureContentView() {
        let overlayView = NSView()
        overlayView.wantsLayer = true
        overlayView.autoresizingMask = [.width, .height]

        #if DEBUG
        overlayView.layer?.backgroundColor =
            NSColor.systemOrange.withAlphaComponent(0.08).cgColor
        #else
        overlayView.layer?.backgroundColor = NSColor.clear.cgColor
        #endif

        contentView = overlayView
    }

    func showOverlay() {
        orderFrontRegardless()
    }

    func hideOverlay() {
        orderOut(nil)
    }
}
