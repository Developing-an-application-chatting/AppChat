(function initUtils(app = window.ChatApp || (window.ChatApp = {})) {
    function buildInitials(name) {
        return (name || "")
            .split(" ")
            .filter(Boolean)
            .slice(0, 2)
            .map(part => part[0])
            .join("")
            .toUpperCase() || "--";
    }

    function buildPreviewText(chatMeta = {}) {
        const text = chatMeta.lastMessage;
        if (text && text.trim()) return text;
        if (chatMeta.fileUrl) {
            const url = chatMeta.fileUrl.toLowerCase();
            if (/(jpeg|jpg|gif|png)$/i.test(url)) return "[Ảnh]";
            if (/(mp4|webm|ogg)$/i.test(url)) return "[Video]";
            return "[Tệp đính kèm]";
        }
        return "Chưa có tin nhắn";
    }

    function resolveChatTimestamp(chatMeta = {}) {
        return (
            chatMeta.lastMessageTime ||
            chatMeta.LastMessageTime ||
            chatMeta.lastMessageAt ||
            chatMeta.LastMessageAt ||
            chatMeta.lastMessageCreatedAt ||
            chatMeta.LastMessageCreatedAt ||
            chatMeta.updatedAt ||
            chatMeta.UpdatedAt ||
            chatMeta.createdAt ||
            chatMeta.CreatedAt ||
            null
        );
    }

    function resolveMessageTimestamp(msg = {}) {
        return (
            msg.sentTime ||
            msg.SentTime ||
            msg.sentAt ||
            msg.SentAt ||
            msg.timestamp ||
            msg.Timestamp ||
            msg.createdAt ||
            msg.CreatedAt ||
            msg.createdDate ||
            msg.CreatedDate ||
            msg.updatedAt ||
            msg.UpdatedAt ||
            null
        );
    }

    function formatTimestamp(value) {
        const date = normalizeToDate(value);
        if (!date || Number.isNaN(date.getTime())) return null;
        const options = { hour: "2-digit", minute: "2-digit" };
        return {
            date,
            absolute: date.toLocaleTimeString("vi-VN", options),
            absoluteFull: date.toLocaleString("vi-VN"),
            relative: relativeTimeFromNow(date)
        };
    }

    function normalizeToDate(value) {
        if (!value && value !== 0) return null;
        if (value instanceof Date) return value;
        if (typeof value === "number" && !Number.isNaN(value)) {
            return new Date(value);
        }
        if (typeof value === "string") {
            const parsed = Date.parse(value.trim());
            if (!Number.isNaN(parsed)) return new Date(parsed);
        }
        return null;
    }

    function relativeTimeFromNow(date) {
        const diffMs = Date.now() - date.getTime();
        const absDiff = Math.abs(diffMs);
        if (absDiff < 60 * 1000) return "Vừa xong";
        if (absDiff < 60 * 60 * 1000) {
            const mins = Math.round(absDiff / (60 * 1000));
            return `${mins} phút trước`;
        }
        if (absDiff < 24 * 60 * 60 * 1000) {
            const hours = Math.round(absDiff / (60 * 60 * 1000));
            return `${hours} giờ trước`;
        }
        if (absDiff < 7 * 24 * 60 * 60 * 1000) {
            const days = Math.round(absDiff / (24 * 60 * 60 * 1000));
            return `${days} ngày trước`;
        }
        return date.toLocaleDateString("vi-VN");
    }

    function sanitize(text) {
        const div = document.createElement("div");
        div.textContent = text ?? "";
        return div.innerHTML;
    }

    function logData(label, payload) {
        console.log(label, payload);
    }

    function resolvePresence(user = {}) {
        if (!user || typeof user !== "object") {
            return { isOnline: false, lastSeen: null };
        }
        const normalizedStatus = typeof user.status === "string" ? user.status.toLowerCase() : "";
        const onlineFlag =
            user.isOnline ??
            user.online ??
            (normalizedStatus ? ["online", "active", "on"].includes(normalizedStatus) : false);
        const lastSeen =
            user.lastSeenAt ||
            user.lastSeen ||
            user.lastOnlineAt ||
            user.lastActiveAt ||
            user.updatedAt ||
            user.UpdatedAt ||
            user.createdAt ||
            user.CreatedAt ||
            null;
        return {
            isOnline: Boolean(onlineFlag),
            lastSeen
        };
    }

    function describePresence(user = {}) {
        const presence = resolvePresence(user);
        if (!presence.isOnline && !presence.lastSeen) return null;
        if (presence.isOnline) {
            return {
                isOnline: true,
                text: "Đang trực tuyến",
                tooltip: "Đang trực tuyến"
            };
        }
        const stamp = presence.lastSeen ? formatTimestamp(presence.lastSeen) : null;
        if (stamp) {
            return {
                isOnline: false,
                text: `Hoạt động ${stamp.relative}`,
                tooltip: `Hoạt động lần cuối: ${stamp.absoluteFull}`
            };
        }
        return {
            isOnline: false,
            text: "Ngoại tuyến",
            tooltip: "Ngoại tuyến"
        };
    }

    function buildPresenceRowHtml(user = {}) {
        const presence = describePresence(user);
        if (!presence) return "";
        const label = sanitize(presence.text);
        const tooltip = sanitize(presence.tooltip || "");
        const statusClass = presence.isOnline ? "online" : "offline";
        return `
            <div class="presence-row ${statusClass}" title="${tooltip}">
                <span class="presence-dot"></span>
                <span>${label}</span>
            </div>
        `.trim();
    }

    function resolveMessageStatus(message = {}) {
        if (!message || typeof message !== "object") {
            return { code: null, readAt: null, raw: null };
        }
        const statusCandidates = [
            message.status,
            message.Status,
            message.messageStatus,
            message.MessageStatus,
            message.deliveryStatus,
            message.DeliveryStatus
        ];
        const rawStatus = statusCandidates
            .map(value => (typeof value === "string" ? value.trim() : ""))
            .find(Boolean);
        const normalizedRaw = rawStatus?.toLowerCase?.() || "";
        const seenFlags = [message.isSeen, message.IsSeen, message.isRead, message.IsRead];
        const deliveredFlags = [message.isDelivered, message.IsDelivered];
        const readAt =
            message.readAt ||
            message.ReadAt ||
            message.readTime ||
            message.ReadTime ||
            message.seenAt ||
            message.SeenAt ||
            message.readDate ||
            message.ReadDate ||
            null;
        const translateRawToCode = value => {
            if (!value) return null;
            if (value.includes("seen") || value.includes("read")) return "seen";
            if (value.includes("deliver")) return "delivered";
            if (value.includes("send")) return "sent";
            if (value.includes("pending") || value.includes("queue")) return "pending";
            if (value.includes("fail") || value.includes("error")) return "failed";
            if (value.includes("unread") || value.includes("unseen")) return "unread";
            return value;
        };
        const codeFromRaw = translateRawToCode(normalizedRaw);
        let code = codeFromRaw;
        if (!code) {
            if (seenFlags.some(flag => flag === true)) code = "seen";
            else if (deliveredFlags.some(flag => flag === true)) code = "delivered";
            else if (typeof readAt !== "undefined" && readAt !== null) code = "seen";
            else if (seenFlags.some(flag => flag === false)) code = "unread";
        }
        return {
            code,
            readAt,
            raw: rawStatus || null
        };
    }

    function describeMessageStatus(message = {}, options = {}) {
        const { defaultCode = null } = options;
        const resolved = resolveMessageStatus(message);
        const code = resolved.code || defaultCode;
        if (!code) return null;
        const readStamp = resolved.readAt ? formatTimestamp(resolved.readAt) : null;
        const tooltipSuffix = readStamp ? ` • ${readStamp.absoluteFull}` : "";
        const dictionary = {
            seen: { label: "Đã xem", short: "Đã xem" },
            read: { label: "Đã xem", short: "Đã xem" },
            delivered: { label: "Đã nhận", short: "Đã nhận" },
            sent: { label: "Đã gửi", short: "Đã gửi" },
            pending: { label: "Đang gửi", short: "Đang gửi" },
            unread: { label: "Chưa xem", short: "Chưa xem" },
            unseen: { label: "Chưa xem", short: "Chưa xem" },
            failed: { label: "Lỗi gửi", short: "Lỗi" }
        };
        const fallback = { label: code, short: code };
        const data = dictionary[code] || fallback;
        return {
            code,
            label: data.label,
            shortLabel: data.short,
            tooltip: `${data.label}${tooltipSuffix}`.trim()
        };
    }

    app.utils = {
        buildInitials,
        buildPreviewText,
        resolveChatTimestamp,
        resolveMessageTimestamp,
        formatTimestamp,
        sanitize,
        logData,
        resolvePresence,
        describePresence,
        buildPresenceRowHtml,
        resolveMessageStatus,
        describeMessageStatus
    };
})(window.ChatApp);
