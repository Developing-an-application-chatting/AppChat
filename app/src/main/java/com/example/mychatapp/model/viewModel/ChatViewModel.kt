package com.example.mychatapp.model.viewModel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
// Xóa import 'sampleChats' vì Repository sẽ lo việc đó
import com.example.mychatapp.model.modelData.Chat
import com.example.mychatapp.model.modelData.Contact
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

/**
 * ================================
 * ChatViewModel.kt (Đã sửa đổi)
 * ================================
 * Trách nhiệm:
 * - Giờ đây chỉ cần 'theo dõi' (observe) dữ liệu từ ChatRepository.
 */


class ChatViewModel : ViewModel() {

    // Chỉ cần trỏ thẳng đến StateFlow của Repository
    val chats: StateFlow<List<Chat>> = ChatRepository.chats

    // Xóa init & loadChats() vì Repository đã làm

    /**
     * Hàm 'addChat' giờ sẽ gọi Repository để xử lý
     */
    fun addChat(friend: Contact) {
        viewModelScope.launch {
            // Ủy quyền việc thêm chat cho Repository
            ChatRepository.addChat(friend)
        }
    }

    init {
        viewModelScope.launch {
            ChatRepository.fetchChats()
        }
    }

    fun refreshChats() {
        viewModelScope.launch {
            ChatRepository.fetchChats()
        }
    }
}