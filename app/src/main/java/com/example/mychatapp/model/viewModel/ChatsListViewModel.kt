package com.example.mychatapp.model.viewModel

import android.net.Uri
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.mychatapp.model.modelData.ChatMessage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class ChatListViewModel : ViewModel() {

    private val _messages = MutableStateFlow<List<ChatMessage>>(emptyList())
    val messages: StateFlow<List<ChatMessage>> = _messages.asStateFlow()

    private var messageCollectionJob: Job? = null

    // Biến này để lưu ID của chat đang xem
    private var currentChatId: String? = null


    /**
     * Gửi tin nhắn text
     */
    fun sendTextMessage(senderId: String, receiverId: String, text: String) {
        viewModelScope.launch {
            ChatRepository.sendTextMessage(senderId, receiverId, text)

            // Sau khi gửi, chúng ta "ép" ViewModel
            // hủy luồng cũ và tải lại luồng mới
            currentChatId?.let {
                loadChatHistory(it)
            }
        }
    }

    /**
     * ✅ Gửi tin nhắn hình
     */
    fun uploadImageMessage(context: android.content.Context, localUri: Uri, senderId: Int, receiverId: Int) {
        viewModelScope.launch(Dispatchers.IO) {
            try {
                // 1. Đọc file ảnh từ Uri và nén/chuyển sang Base64
                val inputStream = context.contentResolver.openInputStream(localUri)
                val bytes = inputStream?.readBytes()
                inputStream?.close()

                if (bytes != null) {
                    // Chuyển byte[] sang Base64 String
                    val base64String = android.util.Base64.encodeToString(bytes, android.util.Base64.NO_WRAP)
                    val fileName = "image_${System.currentTimeMillis()}.jpg"

                    // 2. Gọi Repository gửi đi
                    ChatRepository.sendImageMessage(senderId, receiverId, base64String, fileName)
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    /**
     * ✅ Lấy lịch sử chat
     */
    fun loadChatHistory(chatId: String) {
        viewModelScope.launch {
            val msgsFlow = ChatRepository.getMessagesForChat(chatId)
            msgsFlow.collect { list -> _messages.value = list }
        }
    }
}