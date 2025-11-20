(function initChatListModule(app = window.ChatApp || (window.ChatApp = {})) {
    const { state, config } = app;
    const STATUS_CLASSNAMES = ["pending", "sent", "delivered", "seen", "unread", "failed"];

    async function loadChatList() {
        if (!state.token) return;
        try {
            const res = await fetch(`${config.API_BASE}/chat`, {
                headers: { Authorization: `Bearer ${state.token}` }
            });
            const payload = await res.json();
            state.chats = Array.isArray(payload) ? payload : [];
            const keyword = state.searchKeyword || app.dom.searchInput.value || "";
            applyChatFilter(keyword);
        } catch (error) {
            console.error("Failed to load chats", error);
        }
    }

    function handleSearchInput() {
        const keyword = app.dom.searchInput.value;
        applyChatFilter(keyword);
        if (state.searchDebounce) clearTimeout(state.searchDebounce);
        state.searchDebounce = setTimeout(() => fetchContactSuggestions(keyword), 400);
    }

    async function fetchContactSuggestions(keyword) {
        const trimmed = keyword.trim();
        if (!state.token || trimmed.length < 3) {
            state.contactSuggestions = [];
            renderChatList(state.filteredChats);
            return;
        }
        try {
            const res = await fetch(`${config.API_BASE}/users/search?keyword=${encodeURIComponent(trimmed)}`, {
                headers: { Authorization: `Bearer ${state.token}` }
            });
            const payload = await res.json();
            const existingIds = new Set(
                state.chats
                    .map(c => c?.info?.id)
                    .filter(Boolean)
                    .map(id => id.toString())
            );
            const normalized = Array.isArray(payload) ? payload : [];
            state.contactSuggestions = normalized
                .filter(u => u?.id && u.id !== state.userId && !existingIds.has(u.id.toString()))
                .map(u => ({
                    id: u.id,
                    fullName: `${u?.firstName || ""} ${u?.lastName || ""}`.trim() || u?.fullName || "Không tên",
                    phoneNumber: u?.phoneNumber || "",
                    lastSeen: u?.lastSeenAt || u?.updatedAt || u?.createdAt,
                    isOnline: Boolean(u?.isOnline ?? u?.online ?? false)
                }))
                .slice(0, 5);
        } catch (error) {
            console.error("Suggestion search error", error);
            state.contactSuggestions = [];
        }
        renderChatList(state.filteredChats);
    }

    function applyChatFilter(keyword = "") {
        state.searchKeyword = keyword;
        const query = keyword.trim().toLowerCase();
        if (!query) state.contactSuggestions = [];
        const filtered = !query
            ? state.chats
            : state.chats.filter(item => {
                  const name = (item?.info?.fullName || "").toLowerCase();
                  const phone = (item?.info?.phoneNumber || "").toLowerCase();
                  return name.includes(query) || phone.includes(query);
              });
        state.filteredChats = filtered;
        renderChatList(filtered);
    }

    function renderChatList(items) {
        const container = app.dom.chatItems;
        if (!container) return;
        app.ui.clearList(container);
        if ((!items || !items.length) && !state.contactSuggestions.length) {
            app.ui.showCenteredHint(container, "Chưa có cuộc trò chuyện nào");
            return;
        }
        items.forEach(chat => {
            const chatInfo = chat.info || {};
            const chatMeta = chat.chat || {};
            const element = document.createElement("div");
            element.className = "chat-item";
            element.dataset.chatId = chatMeta.id;
            element.dataset.receiverId = chatInfo.id;
            if (chatMeta.id === state.currentChatId) {
                element.classList.add("active");
            }
            const initials = app.utils.buildInitials(chatInfo.fullName || chatInfo.phoneNumber || "?");
            const presenceData = app.utils.resolvePresence(chatInfo);
            const avatarClasses = ["avatar-sm"];
            if (presenceData.isOnline) avatarClasses.push("is-online");
            const previewText = app.utils.buildPreviewText(chatMeta);
            const unreadCount = chatMeta.unreadCount || 0;
            const stamp = app.utils.formatTimestamp(app.utils.resolveChatTimestamp(chatMeta));
            const absoluteTime = stamp?.absolute || "";
            const relativeTime = stamp?.relative || "";
            const presenceHtml = app.utils.buildPresenceRowHtml(chatInfo);
            const statusInfo = app.utils.describeMessageStatus(chatMeta, {
                defaultCode: unreadCount > 0 ? "unread" : null
            });
            const statusChipHtml = buildStatusChip(statusInfo);
            element.innerHTML = `
                <div class="${avatarClasses.join(" ")}">${initials}<span class="avatar-status-dot"></span></div>
                <div class="chat-item-content">
                    <div class="chat-name-row">
                        <b>${chatInfo.fullName || "Chưa xác định"}</b>
                        <div class="meta-right">
                            ${absoluteTime ? `<span class="time-chip" title="${stamp.absoluteFull}">${absoluteTime}</span>` : ""}
                            <span class="badge">${unreadCount > 0 ? unreadCount : ""}</span>
                        </div>
                    </div>
                    <div class="preview-row">
                        ${statusChipHtml}
                        <div class="preview">${previewText}</div>
                        ${relativeTime ? `<span class="preview-time-inline" title="${stamp ? stamp.absoluteFull : ""}">${relativeTime}</span>` : ""}
                    </div>
                    ${presenceHtml}
                </div>
            `;
            const badge = element.querySelector(".badge");
            if (unreadCount > 0) badge.style.display = "inline-block";
            element.addEventListener("click", () => {
                state.currentReceiverId = chatInfo.id;
                state.currentReceiverName = chatInfo.fullName || chatInfo.phoneNumber || "Không tên";
                app.messages.openChat(chatMeta.id, state.currentReceiverName, chatInfo);
            });
            container.appendChild(element);
        });
        if (state.contactSuggestions.length) {
            const label = document.createElement("div");
            label.className = "chat-section-label";
            label.textContent = "Liên hệ chưa trò chuyện";
            container.appendChild(label);
            state.contactSuggestions.forEach(contact => {
                const item = document.createElement("div");
                item.className = "chat-item suggestion";
                const initials = app.utils.buildInitials(contact.fullName || contact.phoneNumber || "?");
                const presenceData = app.utils.resolvePresence({
                    isOnline: contact.isOnline,
                    lastSeenAt: contact.lastSeen
                });
                const avatarClasses = ["avatar-sm"];
                if (presenceData.isOnline) avatarClasses.push("is-online");
                const stamp = app.utils.formatTimestamp(contact.lastSeen);
                const absoluteTime = stamp?.absolute || "";
                const relativeTime = stamp?.relative || "";
                const presenceHtml = app.utils.buildPresenceRowHtml({
                    isOnline: contact.isOnline,
                    lastSeenAt: contact.lastSeen
                });
                item.innerHTML = `
                    <div class="${avatarClasses.join(" ")}">${initials}<span class="avatar-status-dot"></span></div>
                    <div class="chat-item-content">
                        <div class="chat-name-row">
                            <b>${contact.fullName}</b>
                            <div class="meta-right">
                                ${absoluteTime ? `<span class="time-chip" title="${stamp.absoluteFull}">${absoluteTime}</span>` : ""}
                            </div>
                        </div>
                        <div class="preview-row">
                            <div class="preview">SĐT: ${contact.phoneNumber || "Không rõ"}</div>
                            ${relativeTime ? `<span class="preview-time-inline" title="${stamp.absoluteFull}">${relativeTime}</span>` : ""}
                        </div>
                        ${presenceHtml}
                    </div>
                `;
                item.addEventListener("click", () => app.messages.startAdHocConversation(contact, item));
                container.appendChild(item);
            });
        }
        if (state.currentChatId) {
            highlightActiveChat(state.currentChatId);
        }
    }

    function highlightActiveChat(chatId) {
        app.dom.chatItems?.querySelectorAll(".chat-item").forEach(el => {
            el.classList.toggle("active", el.dataset.chatId === String(chatId));
        });
    }

    function updateChatListItem(msg) {
        const item = app.dom.chatItems?.querySelector(`[data-chat-id="${msg.chatId}"]`);
        if (!item) {
            loadChatList();
            return;
        }
        const preview = item.querySelector(".preview");
        preview.textContent = msg.content || msg.Content || "[Tệp]";
        const timeChip = item.querySelector(".time-chip");
        const inlineTime = item.querySelector(".preview-time-inline");
        const stamp = app.utils.formatTimestamp(app.utils.resolveMessageTimestamp(msg));
        if (timeChip && stamp) {
            timeChip.textContent = stamp.absolute || stamp.relative;
            timeChip.title = stamp.absoluteFull;
        }
        if (inlineTime && stamp) {
            inlineTime.textContent = stamp.relative || stamp.absolute;
            inlineTime.title = stamp.absoluteFull;
        }
        const statusInfo = app.utils.describeMessageStatus(msg);
        const statusChip = item.querySelector(".status-chip");
        if (statusChip) {
            if (statusInfo) {
                statusChip.textContent = statusInfo.shortLabel;
                statusChip.title = statusInfo.tooltip || statusInfo.shortLabel;
                statusChip.dataset.status = statusInfo.code;
                statusChip.style.display = "inline-flex";
                statusChip.classList.remove(...STATUS_CLASSNAMES);
                if (statusInfo.code) statusChip.classList.add(statusInfo.code);
                statusChip.removeAttribute("aria-hidden");
            } else {
                statusChip.textContent = "";
                statusChip.removeAttribute("data-status");
                statusChip.removeAttribute("title");
                statusChip.style.display = "none";
                statusChip.classList.remove(...STATUS_CLASSNAMES);
                statusChip.setAttribute("aria-hidden", "true");
            }
        }
        const badge = item.querySelector(".badge");
        if (badge) {
            const current = Number(badge.textContent || 0) + 1;
            badge.textContent = current;
            badge.style.display = "inline-block";
            item.classList.add("bold");
        }
        app.dom.chatItems.prepend(item);
    }

    function buildStatusChip(info) {
        if (!info) {
            return '<span class="status-chip" data-status="" style="display:none" aria-hidden="true"></span>';
        }
        const label = app.utils.sanitize(info.shortLabel);
        const title = app.utils.sanitize(info.tooltip || info.label || info.shortLabel);
        return `<span class="status-chip ${info.code}" data-status="${info.code}" title="${title}">${label}</span>`;
    }

    function clearUnreadBadge(chatId) {
        const item = app.dom.chatItems?.querySelector(`[data-chat-id="${chatId}"]`);
        if (!item) return;
        const badge = item.querySelector(".badge");
        if (!badge) return;
        badge.textContent = "";
        badge.style.display = "none";
        item.classList.remove("bold");
    }

    app.chat = {
        loadChatList,
        handleSearchInput,
        applyChatFilter,
        renderChatList,
        highlightActiveChat,
        updateChatListItem,
        clearUnreadBadge
    };
})(window.ChatApp);
