(function initMessagesModule(app = window.ChatApp || (window.ChatApp = {})) {
    const { state, config } = app;
    const MESSAGE_CACHE_LIMIT = 400;

    async function startSignalR() {
        if (state.connection || !state.token) return state.connection;
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(config.HUB_URL, { accessTokenFactory: () => state.token })
            .withAutomaticReconnect()
            .build();
        connection.on("ReceiveMessage", handleIncomingMessage);
        connection.on("MessagesRead", handleMessagesRead);
        connection.on("UpdateMessageStatus", handleMessageStatusUpdate);
        connection.onreconnecting(() => {
            state.reconnecting = true;
            app.dom.chatSubtitle.textContent = "Đang khôi phục kết nối...";
        });
        connection.onreconnected(() => {
            state.reconnecting = false;
            setSubtitleToPresence(state.currentReceiverPresence);
            if (state.currentChatId) {
                connection.invoke("JoinGroup", state.currentChatId.toString()).catch(console.error);
            }
        });
        await connection.start();
        state.connection = connection;
        if (state.currentChatId) {
            setSubtitleToPresence(state.currentReceiverPresence);
        } else {
            app.dom.chatSubtitle.textContent = "Đang trò chuyện";
            app.dom.chatSubtitle.removeAttribute("title");
        }
        return connection;
    }

    async function openChat(chatId, name, receiverInfo = {}) {
        if (!chatId) return;
        state.currentChatId = chatId;
        state.isNewConversation = false;
        state.currentReceiverName = name;
        if (receiverInfo === null) {
            state.currentReceiverPresence = null;
        } else if (typeof receiverInfo !== "undefined") {
            const resolvedPresence = app.utils.describePresence(receiverInfo);
            state.currentReceiverPresence = resolvedPresence || null;
        }
        app.dom.chatTitle.textContent = name;
        if (state.reconnecting) {
            app.dom.chatSubtitle.textContent = "Đang khôi phục kết nối...";
        } else {
            setSubtitleToPresence(state.currentReceiverPresence);
        }
        app.dom.messages.innerHTML = "";
        resetRenderedMessageCache();
        app.chat.highlightActiveChat(chatId);
        try {
            const res = await fetch(`${config.API_BASE}/message/${chatId}`, {
                headers: { Authorization: `Bearer ${state.token}` }
            });
            const messages = await res.json();
            (Array.isArray(messages) ? messages : []).forEach(msg => appendMessage(msg));
            scrollMessagesToBottom();
            if (state.connection && state.connection.state === signalR.HubConnectionState.Connected) {
                await state.connection.invoke("JoinGroup", chatId.toString()).catch(console.error);
                await state.connection.invoke("MarkMessagesAsRead", Number(chatId)).catch(console.error);
            }
            app.chat.clearUnreadBadge(chatId);
        } catch (error) {
            console.error("Failed to open chat", error);
        }
    }

    function startAdHocConversation(contact = {}, element) {
        if (!contact.id) return;
        state.currentChatId = null;
        state.currentReceiverId = contact.id;
        state.currentReceiverName = contact.fullName || contact.phoneNumber || "Liên hệ mới";
        state.currentReceiverPresence = null;
        state.isNewConversation = true;
        app.dom.chatTitle.textContent = state.currentReceiverName;
        app.dom.chatSubtitle.textContent = "Chưa có tin nhắn - bắt đầu trò chuyện";
        app.dom.messages.innerHTML = "";
        resetRenderedMessageCache();
        scrollMessagesToBottom();
        app.dom.chatItems?.querySelectorAll(".chat-item").forEach(el => el.classList.remove("active"));
        if (element) element.classList.add("active");
        resetComposer();
    }

    function appendMessage(msg) {
        if (!msg) return false;
        const messageKey = buildMessageCacheKey(msg);
        if (messageKey && hasRenderedMessage(messageKey)) {
            console.error("Duplicate message skipped", { messageKey, msg });
            return false;
        }
        const bubble = document.createElement("div");
        const isMine = msg.senderId === state.userId || msg.SenderId === state.userId;
        bubble.className = isMine ? "message-right" : "message-left";
        const messageId = msg.id ?? msg.Id ?? null;
        if (messageId) bubble.dataset.messageId = String(messageId);
        const chunks = [];
        if (!isMine) {
            const senderLabel = msg.senderName || msg.SenderName || "";
            if (senderLabel) {
                chunks.push(`<div style="font-size:12px;color:#4a4a4a;margin-bottom:4px;">${app.utils.sanitize(senderLabel)}</div>`);
            }
        }
        const textContent = msg.content || msg.Content;
        if (textContent) {
            chunks.push(`<div>${app.utils.sanitize(textContent)}</div>`);
        }
        const fileUrl = msg.fileUrl || msg.FileUrl;
        if (fileUrl) {
            const safeUrl = app.utils.sanitize(fileUrl);
            if (/\.(jpeg|jpg|gif|png)$/i.test(fileUrl)) {
                chunks.push(`<div class="message-attachment"><img src="${safeUrl}" alt="image" loading="lazy"></div>`);
            } else if (/\.(mp4|webm|ogg)$/i.test(fileUrl)) {
                chunks.push(`<div class="message-attachment"><video controls src="${safeUrl}"></video></div>`);
            } else {
                chunks.push(`<div class="message-attachment"><a href="${safeUrl}" target="_blank" rel="noopener noreferrer">Tải tệp</a></div>`);
            }
        }
        const stamp = app.utils.formatTimestamp(app.utils.resolveMessageTimestamp(msg));
        const absoluteLabel = stamp?.absolute || stamp?.absoluteFull || "";
        const relativeLabel = stamp?.relative || "";
        const statusDescriptor = isMine
            ? app.utils.describeMessageStatus(msg, { defaultCode: "sent" })
            : null;
        if (statusDescriptor?.code) {
            bubble.dataset.messageStatus = statusDescriptor.code;
        }
        const metaSegments = [];
        if (stamp && absoluteLabel) {
            metaSegments.push(`<span class="msg-meta-sent" title="${stamp.absoluteFull}">Gửi lúc ${absoluteLabel}</span>`);
        }
        if (stamp && relativeLabel) {
            metaSegments.push(`<span class="msg-meta-ago">${relativeLabel}</span>`);
        }
        if (statusDescriptor) {
            const statusTitle = app.utils.sanitize(statusDescriptor.tooltip || statusDescriptor.label);
            const statusLabel = app.utils.sanitize(statusDescriptor.label);
            metaSegments.push(`<span class="msg-status ${statusDescriptor.code}" title="${statusTitle}">${statusLabel}</span>`);
        }
        const meta = metaSegments.length
            ? `<div class="msg-meta">${metaSegments.join('<span class="meta-divider">•</span>')}</div>`
            : "";
        bubble.innerHTML = `${chunks.join("") || "(Tin nhắn trống)"}${meta}`;
        app.dom.messages.appendChild(bubble);
        rememberRenderedMessage(messageKey);
        return true;
    }

    function handleIncomingMessage(msg) {
        const matchesActiveChat = Boolean(state.currentChatId) && msg.chatId === state.currentChatId;
        const matchesAdHoc = !state.currentChatId && state.isNewConversation &&
            (msg.senderId === state.currentReceiverId || msg.receiverId === state.currentReceiverId);
        if (matchesAdHoc && msg.chatId) {
            state.currentChatId = msg.chatId;
            state.isNewConversation = false;
        }
        if (matchesActiveChat || matchesAdHoc) {
            const appended = appendMessage(msg);
            if (appended) scrollMessagesToBottom();
        } else {
            app.chat.updateChatListItem(msg);
        }
        app.chat.loadChatList().catch(() => {});
    }

    function handleMessagesRead(chatId) {
        app.chat.clearUnreadBadge(chatId);
    }

    function handleMessageStatusUpdate(...payload) {
        const normalized = normalizeMessageStatusPayload(payload);
        if (!normalized?.messageId) return;
        const descriptor = app.utils.describeMessageStatus({
            status: normalized.status,
            readAt: normalized.readAt,
            isRead: normalized.isRead,
            isSeen: normalized.isSeen
        });
        updateMessageStatusBubble(normalized.messageId, descriptor);
        app.chat.loadChatList().catch(() => {});
    }

    async function sendMessage() {
        if (!state.userId || !state.currentReceiverId) {
            alert("Hãy chọn cuộc trò chuyện trước khi gửi");
            return;
        }
        const content = app.dom.messageInput.value.trim();
        const file = app.dom.fileUpload.files[0];
        if (!content && !file) return;
        app.dom.sendBtn.disabled = true;
        const form = new FormData();
        const existingChat = Boolean(state.currentChatId);
        if (existingChat) form.append("ChatId", state.currentChatId);
        form.append("SenderId", state.userId);
        form.append("ReceiverId", state.currentReceiverId);
        form.append("Content", content);
        if (file) {
            let type = "file";
            if (file.type.startsWith("image/")) type = "image";
            else if (file.type.startsWith("video/")) type = "video";
            form.append("FileType", type);
            form.append("File", file);
        } else {
            form.append("FileType", "text");
        }
        try {
            const endpoint = `${config.API_BASE}/message/send`;
            const res = await fetch(endpoint, {
                method: "POST",
                headers: { Authorization: `Bearer ${state.token}` },
                body: form
            });
            const data = await res.json();
            if (!res.ok) {
                throw new Error(data?.message || "Không thể gửi tin nhắn");
            }
            if (!existingChat && data?.chatId) {
                state.currentChatId = data.chatId;
                state.isNewConversation = false;
                if (state.connection && state.connection.state === signalR.HubConnectionState.Connected) {
                    state.connection.invoke("JoinGroup", data.chatId.toString()).catch(console.error);
                }
                await app.chat.loadChatList();
            }
            if (data?.message) {
                const appended = appendMessage(data.message);
                if (appended) scrollMessagesToBottom();
            } else if (state.currentChatId) {
                openChat(state.currentChatId, state.currentReceiverName);
            }
            resetComposer();
            setSubtitleToPresence(state.currentReceiverPresence);
        } catch (error) {
            console.error("Send message error", error);
            alert("Gửi tin nhắn thất bại");
        } finally {
            app.dom.sendBtn.disabled = false;
        }
    }

    function setSubtitleToPresence(presence) {
        if (!app.dom.chatSubtitle) return;
        if (state.reconnecting) {
            app.dom.chatSubtitle.textContent = "Đang khôi phục kết nối...";
            app.dom.chatSubtitle.removeAttribute("title");
            return;
        }
        if (presence) {
            const inlineDotClass = presence.isOnline ? "presence-inline-dot online" : "presence-inline-dot";
            app.dom.chatSubtitle.innerHTML = `<span class="${inlineDotClass}"></span>${app.utils.sanitize(presence.text)}`;
            if (presence.tooltip) app.dom.chatSubtitle.title = presence.tooltip;
            else app.dom.chatSubtitle.removeAttribute("title");
            return;
        }
        app.dom.chatSubtitle.textContent = "Đang trò chuyện";
        app.dom.chatSubtitle.removeAttribute("title");
    }

    function resetComposer() {
        app.dom.messageInput.value = "";
        app.dom.fileUpload.value = "";
    }

    function handleComposerKeydown(event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            sendMessage();
        }
    }

    function scrollMessagesToBottom() {
        app.dom.messages.scrollTop = app.dom.messages.scrollHeight;
    }

    function ensureMessageCache() {
        if (!(state.renderedMessageIds instanceof Set)) {
            state.renderedMessageIds = new Set();
        }
        return state.renderedMessageIds;
    }

    function resetRenderedMessageCache() {
        ensureMessageCache().clear();
    }

    function normalizeMessageStatusPayload(args) {
        if (!args || args.length === 0) return null;
        if (args.length === 1) {
            const entry = args[0];
            if (Array.isArray(entry)) {
                return normalizeMessageStatusPayload(entry);
            }
            if (typeof entry === "object" && entry !== null) {
                return {
                    messageId:
                        entry.messageId ||
                        entry.MessageId ||
                        entry.id ||
                        entry.Id ||
                        entry.message?.id ||
                        null,
                    status: entry.status || entry.Status || entry.messageStatus || entry.MessageStatus || null,
                    readAt: entry.readAt || entry.ReadAt || entry.seenAt || entry.SeenAt || null,
                    isRead: entry.isRead ?? entry.IsRead ?? null,
                    isSeen: entry.isSeen ?? entry.IsSeen ?? null
                };
            }
            return null;
        }
        if (args.length >= 2) {
            const [messageId, status, rest] = args;
            let readAt = null;
            if (typeof rest === "object" && rest !== null) {
                readAt = rest.readAt || rest.ReadAt || rest.seenAt || rest.SeenAt || null;
            }
            return {
                messageId,
                status: typeof status === "string" ? status : null,
                readAt
            };
        }
        return null;
    }

    function updateMessageStatusBubble(messageId, descriptor) {
        if (!messageId) return;
        const bubble = app.dom.messages?.querySelector(`[data-message-id="${messageId}"]`);
        if (!bubble) return;
        if (descriptor) {
            bubble.dataset.messageStatus = descriptor.code;
        } else {
            delete bubble.dataset.messageStatus;
        }
        const statusNode = bubble.querySelector(".msg-status");
        if (!statusNode) return;
        if (!descriptor) {
            statusNode.textContent = "";
            statusNode.removeAttribute("data-status");
            statusNode.removeAttribute("title");
            statusNode.style.display = "none";
            return;
        }
        statusNode.style.display = "inline-flex";
        statusNode.textContent = descriptor.label;
        statusNode.title = descriptor.tooltip || descriptor.label;
        statusNode.dataset.status = descriptor.code;
    }

    function buildMessageCacheKey(msg = {}) {
        if (!msg) return null;
        const directId = msg.id ?? msg.Id;
        if (directId) return `id-${directId}`;
        const chatKey = msg.chatId ?? msg.ChatId ?? state.currentChatId ?? "unknown";
        const senderKey = msg.senderId ?? msg.SenderId ?? "unknown";
        const stampKey = msg.sentTime || msg.SentTime || msg.timestamp || msg.Timestamp || msg.createdAt || msg.CreatedAt || Date.now();
        const contentKey = (msg.content || msg.Content || msg.fileUrl || msg.FileUrl || "").slice(-24);
        return `ck-${chatKey}-${senderKey}-${stampKey}-${contentKey}`;
    }

    function rememberRenderedMessage(messageKey) {
        if (!messageKey) return;
        const cache = ensureMessageCache();
        if (cache.has(messageKey)) return;
        cache.add(messageKey);
        if (cache.size > MESSAGE_CACHE_LIMIT) {
            const firstKey = cache.values().next().value;
            cache.delete(firstKey);
        }
    }

    function hasRenderedMessage(messageKey) {
        if (!messageKey) return false;
        return ensureMessageCache().has(messageKey);
    }

    app.messages = {
        startSignalR,
        openChat,
        startAdHocConversation,
        appendMessage,
        handleIncomingMessage,
        handleMessagesRead,
        handleMessageStatusUpdate,
        sendMessage,
        resetComposer,
        handleComposerKeydown,
        scrollMessagesToBottom
    };
})(window.ChatApp);
