(function initContactsModule(app = window.ChatApp || (window.ChatApp = {})) {
    const { state, config } = app;

    async function loadContactList() {
        if (!state.token) return;
        try {
            const res = await fetch(`${config.API_BASE}/contact`, {
                headers: { Authorization: `Bearer ${state.token}` }
            });
            const payload = await res.json();
            const list = payload?.contacts || [];
            renderContactList(list);
        } catch (error) {
            console.error("Failed to load contacts", error);
            renderContactList([]);
        }
    }

    function renderContactList(list) {
        const container = app.dom.contactItems;
        if (!container) return;
        app.ui.clearList(container);
        if (!Array.isArray(list) || list.length === 0) {
            app.ui.showCenteredHint(container, "Danh bạ trống");
            return;
        }
        list.forEach(ct => {
            const user = ct.user;
            if (!user) return;
            const receiverId = user.id;
            const fullName = user.fullName || "Không tên";
            const initials = app.utils.buildInitials(fullName);
            const presenceData = app.utils.resolvePresence(user);
            const avatarClasses = ["avatar-sm"];
            if (presenceData.isOnline) avatarClasses.push("is-online");
            const phoneNumber = user.phoneNumber || "Chưa cập nhật";
            const presenceHtml = app.utils.buildPresenceRowHtml(user);
            const item = document.createElement("div");
            item.className = "chat-item";
            item.innerHTML = `
                <div class="${avatarClasses.join(" ")}">${initials}<span class="avatar-status-dot"></span></div>
                <div class="chat-item-content">
                    <div class="chat-name-row">
                        <b>${fullName}</b>
                    </div>
                    <div class="preview">SĐT: ${app.utils.sanitize(phoneNumber)}</div>
                    ${presenceHtml}
                </div>
            `;
            item.addEventListener("click", () => openContactChatFromContactList(user));
            container.appendChild(item);
        });
    }

    function openContactChatFromContactList(user) {
        const receiverId = user.id;
        const fullName = user.fullName || "Không tên";
        state.currentReceiverId = receiverId;
        state.currentReceiverName = fullName;
        const existing = state.chats.find(x => x?.info?.id === receiverId);
        if (existing && existing.chat && existing.chat.id) {
            app.messages.openChat(existing.chat.id, fullName, user);
        } else {
            app.utils.logData("contact:newConversation", {
                receiverId,
                phoneNumber: user.phoneNumber || null,
                note: "Chưa có chat, chuyển sang chế độ ad-hoc"
            });
            app.messages.startAdHocConversation({
                id: receiverId,
                fullName,
                phoneNumber: user.phoneNumber || ""
            });
        }
        switchTab("chats");
    }

    function switchTab(tab) {
        state.isContactTab = tab === "contacts";
        if (tab === "chats") {
            app.dom.tabChats.classList.add("active");
            app.dom.tabContacts.classList.remove("active");
            app.dom.chatItems.style.display = "flex";
            app.dom.contactItems.style.display = "none";
            app.dom.searchInput.placeholder = "Tìm tên hoặc số điện thoại";
            app.chat.applyChatFilter(app.dom.searchInput.value || "");
        } else {
            app.dom.tabChats.classList.remove("active");
            app.dom.tabContacts.classList.add("active");
            app.dom.chatItems.style.display = "none";
            app.dom.contactItems.style.display = "flex";
            app.dom.searchInput.placeholder = "Tìm bạn trong danh bạ";
            loadContactList();
        }
    }

    function openAddFriendModal() {
        if (!app.dom.addFriendOverlay) return;
        app.dom.addFriendOverlay.classList.add("active");
        app.dom.addFriendOverlay.setAttribute("aria-hidden", "false");
        if (app.dom.addFriendPhoneInput) {
            app.dom.addFriendPhoneInput.value = "";
            app.dom.addFriendPhoneInput.focus();
        }
        if (app.dom.confirmAddFriendBtn) {
            app.dom.confirmAddFriendBtn.disabled = false;
        }
        app.ui.updateAddFriendInfo("Nhập số điện thoại và bấm Tìm để thêm bạn.");
    }

    function closeAddFriendModal() {
        if (!app.dom.addFriendOverlay) return;
        app.dom.addFriendOverlay.classList.remove("active");
        app.dom.addFriendOverlay.setAttribute("aria-hidden", "true");
    }

    function handleAddFriendOverlayClick(event) {
        if (event.target === app.dom.addFriendOverlay) {
            closeAddFriendModal();
        }
    }

    function handleGlobalKeydown(event) {
        if (event.key === "Escape" && app.dom.addFriendOverlay?.classList.contains("active")) {
            closeAddFriendModal();
        }
    }

    async function submitAddFriend() {
        if (!state.token) {
            app.ui.updateAddFriendInfo("Vui lòng đăng nhập trước.", "error");
            return;
        }
        const phoneNumber = app.dom.addFriendPhoneInput.value.trim();
        if (!phoneNumber) {
            app.ui.updateAddFriendInfo("Hãy nhập số điện thoại.", "error");
            return;
        }
        if (app.dom.confirmAddFriendBtn) {
            app.dom.confirmAddFriendBtn.disabled = true;
        }
        app.ui.updateAddFriendInfo("Đang kiểm tra thông tin...");
        try {
            const res = await fetch(`${config.API_BASE}/contact/add/${encodeURIComponent(phoneNumber)}`, {
                method: "POST",
                headers: { Authorization: `Bearer ${state.token}` }
            });
            const data = await res.json().catch(() => ({}));
            const message = data?.message || "Không thể thêm bạn.";
            if (!res.ok) {
                throw new Error(message);
            }
            const normalized = message.toLowerCase();
            let tone = "muted";
            if (normalized.includes("success")) tone = "success";
            else if (normalized.includes("not") || normalized.includes("exist")) tone = "error";
            app.ui.updateAddFriendInfo(message, tone);
            if (tone === "success") {
                setTimeout(() => {
                    closeAddFriendModal();
                    loadContactList();
                }, 800);
            }
        } catch (error) {
            console.error("Add friend error", error);
            app.ui.updateAddFriendInfo("Không thể thêm bạn lúc này.", "error");
        } finally {
            if (app.dom.confirmAddFriendBtn) {
                app.dom.confirmAddFriendBtn.disabled = false;
            }
        }
    }

    app.contacts = {
        loadContactList,
        renderContactList,
        openContactChatFromContactList,
        switchTab,
        openAddFriendModal,
        closeAddFriendModal,
        handleAddFriendOverlayClick,
        handleGlobalKeydown,
        submitAddFriend
    };
})(window.ChatApp);
