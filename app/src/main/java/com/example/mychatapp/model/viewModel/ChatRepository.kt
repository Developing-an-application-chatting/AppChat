package com.example.mychatapp.model.viewModel

import android.util.Log
import com.example.mychatapp.model.modelData.Chat
import com.example.mychatapp.model.modelData.ChatMessage
import com.example.mychatapp.model.modelData.Contact
import com.example.mychatapp.network.RetrofitInstance
import com.example.mychatapp.network.dto.SendMessageRequestDto
import com.example.mychatapp.network.SignalRService
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import java.util.*

object ChatRepository {

    private val api = RetrofitInstance.api
    // Thay đổi IP của bạn ở đây
    private const val BASE_URL = "http://192.168.242.221:5047"

    private var currentUserId: Int = 0

    // == DANH SÁCH CHAT ==
    private val _chats = MutableStateFlow<List<Chat>>(emptyList())
    val chats: StateFlow<List<Chat>> = _chats.asStateFlow()

    // == TIN NHẮN CHI TIẾT ==
    private val _messages = mutableMapOf<String, MutableStateFlow<List<ChatMessage>>>()

    // Map để lưu quan hệ: FriendId (UI) -> ChatId (Server)
    private val friendToChatIdMap = mutableMapOf<Int, Int>()

    init {
        // Lắng nghe tin nhắn từ SignalR trả về
        CoroutineScope(Dispatchers.IO).launch {
            SignalRService.incomingMessages.collect { msgDto ->

                // 1. Convert DTO -> UI Model
                val newMessage = ChatMessage(
                    id = msgDto.id,
                    senderId = msgDto.senderId,
                    receiverId = msgDto.chatId, // Server trả về ChatId
                    type = if (msgDto.fileType == "text") "text" else "image",
                    content = msgDto.content,
                    imageUrl = msgDto.fileUrl?.let { "$BASE_URL$it" },
                    timestamp = msgDto.sentTime,
                    isRead = false,
                    isSentByMe = (msgDto.senderId == currentUserId)
                )

                // 2. Tìm FriendId tương ứng với ChatId này (để update UI)
                val friendId = friendToChatIdMap.entries.find { it.value == msgDto.chatId }?.key

                // 3. Update vào danh sách tin nhắn (theo ChatId của Server)
                updateMessageList(msgDto.chatId.toString(), newMessage)

                // 4. Nếu tìm thấy FriendId -> Update thêm vào danh sách tin nhắn theo FriendId
                if (friendId != null) {
                    updateMessageList(friendId.toString(), newMessage)
                }
            }
        }
    }

    fun initialize(token: String, myId: Int) {
        currentUserId = myId
        CoroutineScope(Dispatchers.IO).launch {
            SignalRService.startConnection(token)
        }
    }

    fun getMessagesForChat(chatIdOrFriendId: String): MutableStateFlow<List<ChatMessage>> {
        if (!_messages.containsKey(chatIdOrFriendId)) {
            _messages[chatIdOrFriendId] = MutableStateFlow(emptyList())
        }
        return _messages[chatIdOrFriendId]!!
    }

    // Hàm này dùng nội bộ để update list & last message
    private fun updateMessageList(key: String, message: ChatMessage) {
        val flow = getMessagesForChat(key)
        val currentList = flow.value.toMutableList()
        currentList.add(0, message)
        flow.value = currentList

        // Quan trọng: Cập nhật dòng chat bên ngoài
        updateLastMessage(key, message)
    }

    // 🔥 Logic tải tin nhắn theo ID BẠN BÈ
    fun fetchMessages(friendIdString: String) {
        val friendId = friendIdString.toIntOrNull() ?: return

        CoroutineScope(Dispatchers.IO).launch {
            try {
                val response = api.getChatHistory(friendId)

                if (response.isSuccessful && response.body() != null) {
                    val data = response.body()!!

                    // Lưu ChatId thật lại
                    friendToChatIdMap[friendId] = data.chatId

                    // Join Group
                    SignalRService.joinChatGroup(data.chatId.toString())

                    // Map dữ liệu
                    val uiMessages = data.messages.map { msgDto ->
                        ChatMessage(
                            id = msgDto.id,
                            senderId = msgDto.senderId,
                            receiverId = friendId,
                            type = if (msgDto.fileType == "text") "text" else "image",
                            content = msgDto.content,
                            imageUrl = msgDto.fileUrl?.let { "$BASE_URL$it" },
                            timestamp = msgDto.sentTime,
                            isRead = true,
                            isSentByMe = (msgDto.senderId == currentUserId)
                        )
                    }
                    getMessagesForChat(friendIdString).value = uiMessages
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    // Gửi tin nhắn TEXT
    fun sendTextMessage(senderId: String, friendIdString: String, text: String) {
        val senderIdInt = senderId.toIntOrNull() ?: return
        val friendIdInt = friendIdString.toIntOrNull() ?: return

        // Nếu chưa có ChatId thì tạm dùng 0 -> Server sẽ tự tạo
        val realChatId = friendToChatIdMap[friendIdInt] ?: 0

        // 1. Update UI ngay (Optimistic)
        val tempMessage = ChatMessage(
            id = System.currentTimeMillis().toInt(),
            senderId = senderIdInt,
            receiverId = friendIdInt,
            type = "text",
            content = text,
            imageUrl = null,
            timestamp = "Sending...",
            isRead = false,
            isSentByMe = true
        )
        updateMessageList(friendIdString, tempMessage)

        // 2. Gửi Server
        val request = SendMessageRequestDto(
            chatId = realChatId,
            senderId = senderIdInt,
            receiverId = friendIdInt, // Gửi ID người nhận để Server tạo chat
            content = text,
            fileType = "text"
        )
        CoroutineScope(Dispatchers.IO).launch {
            SignalRService.sendMessage(request)
        }
    }

    // Gửi tin nhắn ẢNH
    fun sendImageMessage(senderId: Int, friendId: Int, base64: String, fileName: String) {
        val realChatId = friendToChatIdMap[friendId] ?: 0

        val request = SendMessageRequestDto(
            chatId = realChatId,
            senderId = senderId,
            receiverId = friendId,
            content = null,
            fileBase64 = "data:image/jpeg;base64,$base64",
            fileName = fileName,
            fileType = "image"
        )
        CoroutineScope(Dispatchers.IO).launch {
            SignalRService.sendMessage(request)
        }
    }

    // Hàm cập nhật tin nhắn cuối cùng & xử lý chat mới
    private fun updateLastMessage(chatIdOrFriendId: String, message: ChatMessage) {
        val currentChats = _chats.value.toMutableList()

        // Cố gắng tìm theo ID (có thể là ChatId hoặc FriendId tùy ngữ cảnh)
        // Logic tốt nhất: Tìm theo FriendId
        // Nhưng ChatId của server trả về là ID cuộc hội thoại, không phải ID người bạn
        // => Chúng ta cần fetchChats() để server trả về danh sách chuẩn nhất

        // Logic đơn giản: Nếu tìm thấy ID này trong list chat -> Update
        val chatIndex = currentChats.indexOfFirst {
            it.id.toString() == chatIdOrFriendId ||
                    (friendToChatIdMap[it.id] != null && friendToChatIdMap[it.id].toString() == chatIdOrFriendId)
        }

        if (chatIndex != -1) {
            val oldChat = currentChats[chatIndex]
            val updatedChat = oldChat.copy(
                lastMessage = message.content ?: "📷 Hình ảnh",
                time = "Vừa xong"
            )
            currentChats.removeAt(chatIndex)
            currentChats.add(0, updatedChat)
            _chats.value = currentChats
        } else {
            // 🚨 Chat mới -> Tải lại danh sách
            Log.d("ChatRepo", "Chat mới -> Refresh list")
            fetchChats()
        }
    }

    fun fetchChats() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val response = api.getChatConversations()
                _chats.value = response

                // Cache lại ID nếu cần thiết để map sau này
                response.forEach { chat ->
                    // Lưu ý: Chat model cần có field friendId nếu muốn map chuẩn
                    // friendToChatIdMap[chat.friendId] = chat.id
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    fun addChat(friend: Contact) {
        val currentChats = _chats.value.toMutableList()
        if (currentChats.none { it.id == friend.id }) {
            val newChat = Chat(
                id = friend.id,
                name = friend.name,
                lastMessage = "Say hello 👋",
                time = "Now",
                avatarUrl = friend.avatarUrl
            )
            currentChats.add(newChat)
            _chats.value = currentChats

            if (!_messages.containsKey(friend.id.toString())) {
                _messages[friend.id.toString()] = MutableStateFlow(emptyList())
            }
        }
    }

    fun clearData() {
        _chats.value = emptyList()
        _messages.clear()
        friendToChatIdMap.clear()
        currentUserId = 0
        SignalRService.stopConnection()
    }
}