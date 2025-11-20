(function initDomModule(app = window.ChatApp || (window.ChatApp = {})) {
    const { state } = app;
    const dom = {};

    function cacheDom() {
        dom.login = document.getElementById("login");
        dom.loginForm = document.getElementById("loginForm");
        dom.registerForm = document.getElementById("registerForm");
        dom.loginTab = document.getElementById("loginTab");
        dom.registerTab = document.getElementById("registerTab");
        dom.phoneInput = document.getElementById("phoneNumber");
        dom.passwordInput = document.getElementById("password");
        dom.registerPhoneInput = document.getElementById("registerPhone");
        dom.registerPasswordInput = document.getElementById("registerPassword");
        dom.firstNameInput = document.getElementById("firstName");
        dom.lastNameInput = document.getElementById("lastName");
        dom.chatLayout = document.getElementById("chatLayout");
        dom.chatItems = document.getElementById("chatItems");
        dom.contactItems = document.getElementById("contactItems");
        dom.chatTitle = document.getElementById("chatTitle");
        dom.chatSubtitle = document.getElementById("chatSubtitle");
        dom.messages = document.getElementById("messages");
        dom.messageInput = document.getElementById("messageInput");
        dom.fileUpload = document.getElementById("fileUpload");
        dom.sendBtn = document.getElementById("sendBtn");
        dom.searchInput = document.getElementById("searchInput");
        dom.logoutBtn = document.getElementById("logoutBtn");
        dom.userAvatar = document.getElementById("userAvatar");
        dom.userFullName = document.getElementById("userFullName");
        dom.userPhone = document.getElementById("userPhone");
        dom.addFriendBtn = document.getElementById("addFriendBtn");
        dom.addFriendOverlay = document.getElementById("addFriendOverlay");
        dom.addFriendPhoneInput = document.getElementById("addFriendPhone");
        dom.addFriendInfo = document.getElementById("addFriendInfo");
        dom.confirmAddFriendBtn = document.getElementById("confirmAddFriend");
        dom.closeAddFriendBtn = document.getElementById("closeAddFriendBtn");
        dom.tabChats = document.getElementById("tabChats");
        dom.tabContacts = document.getElementById("tabContacts");
    }

    function showAppShell() {
        if (!dom.login || !dom.chatLayout) return;
        dom.login.style.display = "none";
        dom.chatLayout.classList.add("active");
    }

    function hideAppShell() {
        if (!dom.login || !dom.chatLayout) return;
        dom.login.style.display = "block";
        dom.chatLayout.classList.remove("active");
    }

    function toggleAuthView(view = "login") {
        if (!dom.loginForm || !dom.registerForm) return;
        const isLogin = view === "login";
        dom.loginForm.classList.toggle("active", isLogin);
        dom.registerForm.classList.toggle("active", !isLogin);
        dom.loginTab?.classList.toggle("active", isLogin);
        dom.registerTab?.classList.toggle("active", !isLogin);
    }

    function toggleLoginState(isLoading) {
        dom.loginForm?.querySelectorAll("input, button").forEach(el => {
            el.disabled = Boolean(isLoading);
        });
    }

    function toggleRegisterState(isLoading) {
        dom.registerForm?.querySelectorAll("input, button").forEach(el => {
            el.disabled = Boolean(isLoading);
        });
    }

    function updateUserBadge() {
        if (!dom.userAvatar) return;
        const initials = app.utils.buildInitials(state.fullName || state.phoneNumber || "--");
        dom.userAvatar.textContent = initials;
        dom.userFullName.textContent = state.fullName || "Bạn";
        dom.userPhone.textContent = state.phoneNumber || "";
    }

    function updateAddFriendInfo(message, tone = "muted") {
        if (!dom.addFriendInfo) return;
        dom.addFriendInfo.textContent = message;
        dom.addFriendInfo.classList.remove("success", "error");
        if (tone === "success") dom.addFriendInfo.classList.add("success");
        if (tone === "error") dom.addFriendInfo.classList.add("error");
    }

    function showCenteredHint(target, message) {
        if (!target) return;
        target.innerHTML = `<div class="list-empty-hint">${message}</div>`;
    }

    function clearList(target) {
        if (!target) return;
        target.innerHTML = "";
    }

    app.dom = dom;
    app.ui = {
        cacheDom,
        showAppShell,
        hideAppShell,
        toggleAuthView,
        toggleLoginState,
        toggleRegisterState,
        updateUserBadge,
        updateAddFriendInfo,
        showCenteredHint,
        clearList
    };
})(window.ChatApp);
