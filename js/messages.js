(function initMessagesModule(app = window.ChatApp || (window.ChatApp = {})) {
  const { state, config } = app;
  const MESSAGE_CACHE_LIMIT = 400;

  // ---------- New: chunk upload helpers ----------
  async function uploadFileInChunks(file, onProgress = null) {
    const CHUNK_SIZE = 5 * 1024 * 1024;
    const totalChunks = Math.ceil(file.size / CHUNK_SIZE);
    const MAX_RETRY = 3;

    console.log(
      "[CHUNK] Init:",
      file.name,
      "size:",
      file.size,
      "chunks:",
      totalChunks
    );

    // Step 1 - init upload
    const initRes = await fetch(`${config.API_BASE}/message/upload/init`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${state.token}`,
      },
      body: JSON.stringify({
        fileName: file.name,
        totalChunks,
      }),
    });

    if (!initRes.ok) {
      const txt = await initRes.text();
      throw new Error(`Init upload failed: ${txt}`);
    }

    const initData = await initRes.json();
    const uploadId = initData.uploadId;

    console.log("[CHUNK] UploadId:", uploadId);

    // Step 2 - get status for resume
    const statusRes = await fetch(
      `${config.API_BASE}/message/upload/status?uploadId=${uploadId}`,
      {
        headers: { Authorization: `Bearer ${state.token}` },
      }
    );
    if (!statusRes.ok) {
      // If status endpoint fails, we assume no chunks uploaded yet
      console.warn(
        "[CHUNK] Status check failed, assuming no chunks uploaded yet"
      );
    }
    const statusData = (await statusRes.ok)
      ? await statusRes.json()
      : { uploadedChunks: [] };
    const uploadedChunks = new Set(statusData.uploadedChunks || []);

    // Step 3 - upload each chunk
    for (let i = 0; i < totalChunks; i++) {
      if (uploadedChunks.has(i)) {
        // update progress
        if (typeof onProgress === "function") {
          onProgress(Math.round(((i + 1) / totalChunks) * 100), i, totalChunks);
        }
        console.log(`[CHUNK] Skip ${i} (exists)`);
        continue;
      }

      const start = i * CHUNK_SIZE;
      const end = Math.min(start + CHUNK_SIZE, file.size);
      const chunk = file.slice(start, end);

      const form = new FormData();
      form.append("uploadId", uploadId);
      form.append("chunkIndex", i);
      form.append("fileChunk", chunk);

      let attempt = 0;
      let uploaded = false;
      while (!uploaded && attempt < MAX_RETRY) {
        attempt++;
        try {
          console.log(
            `[CHUNK] Uploading chunk ${i}/${totalChunks} attempt ${attempt}`
          );
          const chunkRes = await fetch(
            `${config.API_BASE}/message/upload/chunk`,
            {
              method: "POST",
              headers: { Authorization: `Bearer ${state.token}` },
              body: form,
            }
          );
          if (!chunkRes.ok) {
            const txt = await chunkRes.text();
            throw new Error(`Chunk ${i} failed: ${txt}`);
          }
          uploaded = true;
        } catch (err) {
          console.warn(`[CHUNK] chunk ${i} attempt ${attempt} failed`, err);
          if (attempt >= MAX_RETRY) {
            throw new Error(
              `Chunk ${i} failed after ${MAX_RETRY} attempts: ${err.message}`
            );
          }
          // small delay before retry
          await new Promise((r) => setTimeout(r, 500 * attempt));
        }
      }

      // update progress
      if (typeof onProgress === "function") {
        onProgress(Math.round(((i + 1) / totalChunks) * 100), i, totalChunks);
      }
    }

    // Step 4 - request complete
    const completeRes = await fetch(
      `${config.API_BASE}/message/upload/complete`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${state.token}`,
        },
        body: JSON.stringify({ uploadId }),
      }
    );

    if (!completeRes.ok) {
      const txt = await completeRes.text();
      throw new Error(`Complete upload failed: ${txt}`);
    }

    const completeData = await completeRes.json();
    console.log("[CHUNK] DONE:", completeData.fileUrl);

    return { fileUrl: completeData.fileUrl, uploadId };
  }

  function appendTempUploadingMessage(fileName, uploadId = null) {
    const bubble = document.createElement("div");
    bubble.className = "message-right uploading-message";
    if (uploadId) bubble.dataset.uploadTempId = uploadId;

    bubble.innerHTML = `
            <div style="font-weight:600;margin-bottom:6px;">Đang gửi tệp: ${escapeHtml(
              fileName
            )}</div>
            <div class="upload-progress" style="font-size:13px;color:#666">Đang tải lên... 0%</div>
        `;

    app.dom.messages.appendChild(bubble);
    scrollMessagesToBottom();
    return bubble;
  }

  function updateTempUploadProgress(bubble, percent) {
    if (!bubble) return;
    const node = bubble.querySelector(".upload-progress");
    if (node) node.textContent = `Đang tải lên... ${percent}%`;
  }

  function markTempUploadSuccess(bubble) {
    if (!bubble) return;
    const node = bubble.querySelector(".upload-progress");
    if (node) node.textContent = `Đã tải xong ✔`;
    bubble.classList.add("upload-success");
  }

  function markTempUploadError(bubble, msg = "Lỗi upload") {
    if (!bubble) return;
    const node = bubble.querySelector(".upload-progress");
    if (node) node.textContent = `${msg} ❌`;
    bubble.classList.add("upload-error");
  }

  function removeTempUploadBubble(bubble) {
    try {
      if (bubble && bubble.parentNode) bubble.parentNode.removeChild(bubble);
    } catch (e) {
      /* ignore */
    }
  }

  function escapeHtml(str) {
    if (!str) return "";
    return String(str)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }
  // ---------- End chunk helpers ----------

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
        connection
          .invoke("JoinGroup", state.currentChatId.toString())
          .catch(console.error);
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
        headers: { Authorization: `Bearer ${state.token}` },
      });
      const messages = await res.json();
      (Array.isArray(messages) ? messages : []).forEach((msg) =>
        appendMessage(msg)
      );
      scrollMessagesToBottom();
      if (
        state.connection &&
        state.connection.state === signalR.HubConnectionState.Connected
      ) {
        await state.connection
          .invoke("JoinGroup", chatId.toString())
          .catch(console.error);
        await state.connection
          .invoke("MarkMessagesAsRead", Number(chatId))
          .catch(console.error);
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
    state.currentReceiverName =
      contact.fullName || contact.phoneNumber || "Liên hệ mới";
    state.currentReceiverPresence = null;
    state.isNewConversation = true;
    app.dom.chatTitle.textContent = state.currentReceiverName;
    app.dom.chatSubtitle.textContent = "Chưa có tin nhắn - bắt đầu trò chuyện";
    app.dom.messages.innerHTML = "";
    resetRenderedMessageCache();
    scrollMessagesToBottom();
    app.dom.chatItems
      ?.querySelectorAll(".chat-item")
      .forEach((el) => el.classList.remove("active"));
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
    const isMine =
      msg.senderId === state.userId || msg.SenderId === state.userId;
    bubble.className = isMine ? "message-right" : "message-left";
    const messageId = msg.id ?? msg.Id ?? null;
    if (messageId) bubble.dataset.messageId = String(messageId);
    const chunks = [];
    if (!isMine) {
      const senderLabel = msg.senderName || msg.SenderName || "";
      if (senderLabel) {
        chunks.push(
          `<div style="font-size:12px;color:#4a4a4a;margin-bottom:4px;">${app.utils.sanitize(
            senderLabel
          )}</div>`
        );
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
        chunks.push(
          `<div class="message-attachment"><img src="${safeUrl}" alt="image" loading="lazy"></div>`
        );
      } else if (/\.(mp4|webm|ogg)$/i.test(fileUrl)) {
        chunks.push(
          `<div class="message-attachment"><video controls src="${safeUrl}"></video></div>`
        );
      } else {
        chunks.push(
          `<div class="message-attachment"><a href="${safeUrl}" target="_blank" rel="noopener noreferrer">Tải tệp</a></div>`
        );
      }
    }
    const stamp = app.utils.formatTimestamp(
      app.utils.resolveMessageTimestamp(msg)
    );
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
      metaSegments.push(
        `<span class="msg-meta-sent" title="${stamp.absoluteFull}">Gửi lúc ${absoluteLabel}</span>`
      );
    }
    if (stamp && relativeLabel) {
      metaSegments.push(`<span class="msg-meta-ago">${relativeLabel}</span>`);
    }
    if (statusDescriptor) {
      const statusTitle = app.utils.sanitize(
        statusDescriptor.tooltip || statusDescriptor.label
      );
      const statusLabel = app.utils.sanitize(statusDescriptor.label);
      metaSegments.push(
        `<span class="msg-status ${statusDescriptor.code}" title="${statusTitle}">${statusLabel}</span>`
      );
    }
    const meta = metaSegments.length
      ? `<div class="msg-meta">${metaSegments.join(
          '<span class="meta-divider">•</span>'
        )}</div>`
      : "";
    bubble.innerHTML = `${chunks.join("") || "(Tin nhắn trống)"}${meta}`;
    app.dom.messages.appendChild(bubble);
    rememberRenderedMessage(messageKey);
    return true;
  }

  function handleIncomingMessage(msg) {
    const matchesActiveChat =
      Boolean(state.currentChatId) && msg.chatId === state.currentChatId;
    const matchesAdHoc =
      !state.currentChatId &&
      state.isNewConversation &&
      (msg.senderId === state.currentReceiverId ||
        msg.receiverId === state.currentReceiverId);
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
      isSeen: normalized.isSeen,
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

    // NOTE: do NOT block the send button while uploading large files.
    // Allow the user to keep sending text or other small files.

    const MAX_CLIENT_UPLOAD_BYTES = 1 * 1024 * 1024 * 1024; // 1 GB
    if (file && file.size > MAX_CLIENT_UPLOAD_BYTES) {
      alert(
        `Tệp quá lớn (${(file.size / (1024 * 1024)).toFixed(
          1
        )} MB). Kích thước tối đa cho phép là ${(
          MAX_CLIENT_UPLOAD_BYTES /
          (1024 * 1024)
        ).toFixed(0)} MB.`
      );
      return;
    }

    const form = new FormData();
    const existingChat = Boolean(state.currentChatId);
    if (existingChat) form.append("ChatId", state.currentChatId);
    form.append("SenderId", state.userId);
    form.append("ReceiverId", state.currentReceiverId);
    form.append("Content", content);

    // We'll support two flows:
    // - Small files (< 5MB): send via existing /message/send with FormData File
    // - Large files (>= 5MB): upload in chunks, then call /message/send with FileUrl

    let uploadedFileUrl = null;
    let tempBubble = null;

    try {
      if (file) {
        if (file.size >= 5 * 1024 * 1024) {
          // Use chunk upload
          console.log("[SEND] Using chunk upload for file:", file.name);
          // Append a temporary upload message to UI
          tempBubble = appendTempUploadingMessage(file.name);

          // Progress callback updates UI
          const progressCallback = (percent) => {
            updateTempUploadProgress(tempBubble, percent);
          };

          // Do chunk upload (this may resume if interrupted)
          const { fileUrl, uploadId } = await uploadFileInChunks(
            file,
            (percent, chunkIndex, totalChunks) => {
              // percent from uploadFileInChunks
              updateTempUploadProgress(tempBubble, percent);
            }
          );

          uploadedFileUrl = fileUrl;

          markTempUploadSuccess(tempBubble);
          // wait a short time so user sees success, then remove temp
          setTimeout(() => removeTempUploadBubble(tempBubble), 700);
        } else {
          // small file - keep original behavior
          let type = "file";
          if (file.type.startsWith("image/")) type = "image";
          else if (file.type.startsWith("video/")) type = "video";
          form.append("FileType", type);
          form.append("File", file);
        }
      } else {
        form.append("FileType", "text");
      }

      // If we have fileUrl (chunk upload), send that instead of raw file
      if (uploadedFileUrl) {
        // keep same FileType detection
        let type = "file";
        if (file && file.type.startsWith("image/")) type = "image";
        else if (file && file.type.startsWith("video/")) type = "video";
        form.append("FileType", type);
        // IMPORTANT: this uses your existing /message/send API which expects form data.
        // We add FileUrl field; BE should read FileUrl when present and skip file uploading.
        form.append("FileUrl", uploadedFileUrl);
      }

      const endpoint = `${config.API_BASE}/message/send`;
      const res = await fetch(endpoint, {
        method: "POST",
        headers: { Authorization: `Bearer ${state.token}` },
        body: form,
      });

      // Read response text first to surface detailed server errors
      const resText = await res.text();
      let data = null;
      try {
        data = resText ? JSON.parse(resText) : null;
      } catch (e) {
        data = resText;
      }
      if (!res.ok) {
        const serverMsg =
          data && data.message
            ? data.message
            : typeof data === "string"
            ? data
            : JSON.stringify(data);
        throw new Error(serverMsg || "Không thể gửi tin nhắn");
      }
      if (!existingChat && data?.chatId) {
        state.currentChatId = data.chatId;
        state.isNewConversation = false;
        if (
          state.connection &&
          state.connection.state === signalR.HubConnectionState.Connected
        ) {
          state.connection
            .invoke("JoinGroup", data.chatId.toString())
            .catch(console.error);
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
      // If we created a temp bubble and upload failed, mark error
      if (tempBubble) {
        markTempUploadError(tempBubble, error.message || "Lỗi upload");
        // allow user to retry by leaving bubble visible
      }
      alert("Gửi tin nhắn thất bại: " + (error.message || ""));
    } finally {
      // NOTE: we intentionally do NOT toggle sendBtn.disabled here in order
      // to avoid blocking user input while uploads happen concurrently.
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
      const inlineDotClass = presence.isOnline
        ? "presence-inline-dot online"
        : "presence-inline-dot";
      app.dom.chatSubtitle.innerHTML = `<span class="${inlineDotClass}"></span>${app.utils.sanitize(
        presence.text
      )}`;
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
          status:
            entry.status ||
            entry.Status ||
            entry.messageStatus ||
            entry.MessageStatus ||
            null,
          readAt:
            entry.readAt ||
            entry.ReadAt ||
            entry.seenAt ||
            entry.SeenAt ||
            null,
          isRead: entry.isRead ?? entry.IsRead ?? null,
          isSeen: entry.isSeen ?? entry.IsSeen ?? null,
        };
      }
      return null;
    }
    if (args.length >= 2) {
      const [messageId, status, rest] = args;
      let readAt = null;
      if (typeof rest === "object" && rest !== null) {
        readAt =
          rest.readAt || rest.ReadAt || rest.seenAt || rest.SeenAt || null;
      }
      return {
        messageId,
        status: typeof status === "string" ? status : null,
        readAt,
      };
    }
    return null;
  }

  function updateMessageStatusBubble(messageId, descriptor) {
    if (!messageId) return;
    const bubble = app.dom.messages?.querySelector(
      `[data-message-id="${messageId}"]`
    );
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
    const chatKey =
      msg.chatId ?? msg.ChatId ?? state.currentChatId ?? "unknown";
    const senderKey = msg.senderId ?? msg.SenderId ?? "unknown";
    const stampKey =
      msg.sentTime ||
      msg.SentTime ||
      msg.timestamp ||
      msg.Timestamp ||
      msg.createdAt ||
      msg.CreatedAt ||
      Date.now();
    const contentKey = (
      msg.content ||
      msg.Content ||
      msg.fileUrl ||
      msg.FileUrl ||
      ""
    ).slice(-24);
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
    scrollMessagesToBottom,
  };
})(window.ChatApp);
