(function initApp(app = window.ChatApp || (window.ChatApp = {})) {
    document.addEventListener("DOMContentLoaded", () => {
        app.ui.cacheDom();
        bindEvents();
        app.ui.toggleAuthView("login");
        app.auth.hydrateSession();
    });

    function bindEvents() {
        const { dom } = app;
        dom.loginForm?.addEventListener("submit", app.auth.handleLoginSubmit);
        dom.registerForm?.addEventListener("submit", app.auth.handleRegisterSubmit);
        dom.loginTab?.addEventListener("click", () => app.ui.toggleAuthView("login"));
        dom.registerTab?.addEventListener("click", () => app.ui.toggleAuthView("register"));
        dom.sendBtn?.addEventListener("click", app.messages.sendMessage);
        dom.messageInput?.addEventListener("keydown", app.messages.handleComposerKeydown);
        dom.logoutBtn?.addEventListener("click", app.auth.logout);
        dom.searchInput?.addEventListener("input", app.chat.handleSearchInput);
        dom.addFriendBtn?.addEventListener("click", app.contacts.openAddFriendModal);
        dom.closeAddFriendBtn?.addEventListener("click", app.contacts.closeAddFriendModal);
        dom.addFriendOverlay?.addEventListener("click", app.contacts.handleAddFriendOverlayClick);
        dom.confirmAddFriendBtn?.addEventListener("click", app.contacts.submitAddFriend);
        dom.tabChats?.addEventListener("click", () => app.contacts.switchTab("chats"));
        dom.tabContacts?.addEventListener("click", () => app.contacts.switchTab("contacts"));
        document.addEventListener("keydown", app.contacts.handleGlobalKeydown);
    }
})(window.ChatApp);
