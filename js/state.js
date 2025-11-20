(function initState(app = window.ChatApp || (window.ChatApp = {})) {
    const API_BASE = "https://appchat-production-fc6d.up.railway.app";
    const HUB_URL = `${API_BASE}/chatHub`;
    const SESSION_KEY = "miniChatSession";

    app.config = { API_BASE, HUB_URL, SESSION_KEY };

    app.state = {
        token: null,
        userId: null,
        fullName: "",
        phoneNumber: "",
        currentChatId: null,
        currentReceiverId: null,
        currentReceiverName: "",
        currentReceiverPresence: null,
        isNewConversation: false,
        chats: [],
        connection: null,
        reconnecting: false,
        filteredChats: [],
        contactSuggestions: [],
        searchKeyword: "",
        searchDebounce: null,
        isContactTab: false,
        renderedMessageIds: new Set()
    };
})(window.ChatApp);
