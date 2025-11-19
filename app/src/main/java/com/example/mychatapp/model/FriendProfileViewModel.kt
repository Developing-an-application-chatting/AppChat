package com.example.mychatapp.model

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.mychatapp.network.RetrofitInstance
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class FriendProfile(
    val id: String,
    val name: String,
    val phoneNumber: String,
    val imageUrl: String?,
    val bio: String? = null
)

class FriendProfileViewModel : ViewModel() {
    private val api = RetrofitInstance.api
    // Thay bằng IP máy bạn
    companion object {
        // Thay bằng IP máy bạn
        private const val BASE_URL = "http://192.168.242.221:8080"
    }

    private val _uiState = MutableStateFlow<FriendProfile?>(null)
    val uiState = _uiState.asStateFlow()

    fun loadFriendProfile(phoneNumber: String) {
        viewModelScope.launch {
            try {
                // Gọi API Backend tìm user theo SĐT
                // Lưu ý: SĐT có dấu '+' (VD: +84909...) cần encode an toàn
                val response = api.getUserByPhone(phoneNumber)

                if (response.isSuccessful && response.body() != null) {
                    val userDto = response.body()!!

                    _uiState.value = FriendProfile(
                        id = userDto.id.toString(),
                        name = "${userDto.lastName} ${userDto.firstName}",
                        phoneNumber = phoneNumber, // Hoặc userDto.phoneNumber
                        imageUrl = userDto.avatarUrl?.let { "$BASE_URL$it" }
                    )
                } else {
                    // Không tìm thấy user hoặc lỗi
                    _uiState.value = null
                }
            } catch (e: Exception) {
                e.printStackTrace()
                _uiState.value = null
            }
        }
    }
}
