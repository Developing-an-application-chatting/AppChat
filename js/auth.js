(function initAuthModule(app = window.ChatApp || (window.ChatApp = {})) {
    const { state, config } = app;
    const SESSION_KEY = config.SESSION_KEY;

    function handleLoginSubmit(event) {
        event.preventDefault();
        login();
    }

    function handleRegisterSubmit(event) {
        event.preventDefault();
        registerUser();
    }

    function persistSession() {
        const payload = {
            token: state.token,
            userId: state.userId,
            fullName: state.fullName,
            phoneNumber: state.phoneNumber
        };
        localStorage.setItem(SESSION_KEY, JSON.stringify(payload));
    }

    function clearSession() {
        localStorage.removeItem(SESSION_KEY);
    }

    async function hydrateSession() {
        try {
            const raw = localStorage.getItem(SESSION_KEY);
            if (!raw) return;
            const parsed = JSON.parse(raw);
            if (!parsed?.token) return;
            state.token = parsed.token;
            state.userId = parsed.userId;
            state.fullName = parsed.fullName || "";
            state.phoneNumber = parsed.phoneNumber || "";
            app.ui.showAppShell();
            app.ui.updateUserBadge();
            await app.messages.startSignalR();
            await app.chat.loadChatList();
        } catch (error) {
            console.error("Failed to hydrate session", error);
        }
    }

    async function login() {
        const phoneNumber = app.dom.phoneInput.value.trim();
        const password = app.dom.passwordInput.value.trim();
        if (!phoneNumber || !password) return;
        app.ui.toggleLoginState(true);
        try {
            const res = await fetch(`${config.API_BASE}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ phoneNumber, password })
            });
            const data = await res.json();
            if (!data?.accessToken) {
                alert("Đăng nhập thất bại, vui lòng kiểm tra lại thông tin.");
                return;
            }
            state.token = data.accessToken;
            state.userId = data.id;
            state.fullName = data.fullName || data.name || "Bạn";
            state.phoneNumber = data.phoneNumber || phoneNumber;
            persistSession();
            app.ui.showAppShell();
            app.ui.updateUserBadge();
            await app.messages.startSignalR();
            await app.chat.loadChatList();
        } catch (error) {
            console.error("Login error", error);
            alert("Không thể đăng nhập. Vui lòng thử lại sau.");
        } finally {
            app.ui.toggleLoginState(false);
        }
    }

    async function registerUser() {
        const phoneNumber = app.dom.registerPhoneInput.value.trim();
        const password = app.dom.registerPasswordInput.value.trim();
        const firstName = app.dom.firstNameInput.value.trim();
        const lastName = app.dom.lastNameInput.value.trim();
        if (!phoneNumber || !password || !firstName || !lastName) {
            alert("Vui lòng nhập đầy đủ thông tin đăng ký.");
            return;
        }
        app.ui.toggleRegisterState(true);
        try {
            const res = await fetch(`${config.API_BASE}/auth/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ phoneNumber, password, firstName, lastName })
            });
            const data = await res.json();
            if (!res.ok) {
                throw new Error(data?.message || "Đăng ký thất bại");
            }
            alert("Tạo tài khoản thành công. Vui lòng đăng nhập.");
            app.dom.phoneInput.value = phoneNumber;
            app.dom.passwordInput.focus();
            app.ui.toggleAuthView("login");
        } catch (error) {
            console.error("Register error", error);
            alert(error.message || "Không thể đăng ký.");
        } finally {
            app.ui.toggleRegisterState(false);
        }
    }

    function logout() {
        if (state.connection) {
            state.connection.stop().catch(console.error);
            state.connection = null;
        }
        Object.assign(state, {
            token: null,
            userId: null,
            fullName: "",
            phoneNumber: "",
            chats: [],
            currentChatId: null,
            currentReceiverId: null,
            currentReceiverName: "",
            currentReceiverPresence: null,
            isNewConversation: false,
            contactSuggestions: [],
            filteredChats: [],
            renderedMessageIds: new Set()
        });
        if (state.searchDebounce) {
            clearTimeout(state.searchDebounce);
            state.searchDebounce = null;
        }
        app.dom.messages.innerHTML = "";
        app.dom.chatItems.innerHTML = "";
        app.dom.contactItems.innerHTML = "";
        app.dom.searchInput.value = "";
        app.dom.chatTitle.textContent = "Chọn cuộc trò chuyện";
        app.dom.chatSubtitle.textContent = "Tin nhắn sẽ hiển thị tại đây";
        clearSession();
        app.ui.hideAppShell();
        app.contacts?.closeAddFriendModal?.();
    }

    app.auth = {
        handleLoginSubmit,
        handleRegisterSubmit,
        hydrateSession,
        login,
        registerUser,
        logout,
        persistSession,
        clearSession
    };
})(window.ChatApp);
