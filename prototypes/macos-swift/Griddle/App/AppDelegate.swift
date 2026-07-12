import AppKit

@MainActor
final class AppDelegate: NSObject, NSApplicationDelegate {

    private var statusItem: NSStatusItem?
    private var overlayWindows: [OverlayWindow] = []
    private var isOverlayVisible = false

    func applicationDidFinishLaunching(
        _ notification: Notification
    ) {
        configureApplication()
        createOverlayWindows()
        createStatusItem()
    }

    private func configureApplication() {
        // Makes Griddle behave like a menu-bar utility rather
        // than a conventional Dock application.
        NSApp.setActivationPolicy(.accessory)
    }

    private func createOverlayWindows() {
        overlayWindows = NSScreen.screens.map { screen in
            OverlayWindow(screen: screen)
        }
    }

    private func createStatusItem() {
        let statusItem = NSStatusBar.system.statusItem(
            withLength: NSStatusItem.squareLength
        )

        if let button = statusItem.button {
            button.image = NSImage(
                systemSymbolName: "flame.fill",
                accessibilityDescription: "Griddle"
            )
        }

        let menu = NSMenu()

        let toggleItem = NSMenuItem(
            title: "Show Overlay",
            action: #selector(toggleOverlay),
            keyEquivalent: ""
        )
        toggleItem.target = self
        menu.addItem(toggleItem)

        menu.addItem(.separator())

        let quitItem = NSMenuItem(
            title: "Quit Griddle",
            action: #selector(quitApplication),
            keyEquivalent: "q"
        )
        quitItem.target = self
        menu.addItem(quitItem)

        statusItem.menu = menu
        self.statusItem = statusItem
    }

    @objc
    private func toggleOverlay(_ sender: NSMenuItem) {
        isOverlayVisible.toggle()

        if isOverlayVisible {
            overlayWindows.forEach { $0.showOverlay() }
            sender.title = "Hide Overlay"
        } else {
            overlayWindows.forEach { $0.hideOverlay() }
            sender.title = "Show Overlay"
        }
    }

    @objc
    private func quitApplication() {
        NSApp.terminate(nil)
    }
}
