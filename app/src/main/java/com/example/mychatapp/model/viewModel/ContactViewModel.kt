package com.example.mychatapp.model.viewModel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.mychatapp.model.modelData.Contact
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

/**
 * ================================
 * ContactViewModel.kt (Đã sửa lỗi)
 * ================================
 */
class ContactViewModel : ViewModel() {

    // 1. Trỏ thẳng đến StateFlow của Repository để UI tự cập nhật
    val contacts: StateFlow<List<Contact>> = ContactRepository.contacts

    // 2. Init Block: Tự động gọi API lấy danh bạ ngay khi ViewModel khởi tạo
    init {
        refreshContacts()
    }

    fun refreshContacts() {
        viewModelScope.launch {
            ContactRepository.fetchContacts()
        }
    }

    /**
     * 3. Hàm thêm bạn
     * Đã sửa: Gọi 'addFriend' của Repository thay vì 'addContact'
     */
    fun addFriend(friendId: String, onSuccess: () -> Unit, onError: () -> Unit) {
        viewModelScope.launch {
            ContactRepository.addFriend(
                friendId = friendId,
                onSuccess = onSuccess,
                onError = onError
            )
        }
    }
}