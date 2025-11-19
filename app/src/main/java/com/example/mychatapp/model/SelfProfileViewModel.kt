package com.example.mychatapp.model

import android.content.Context
import android.net.Uri
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.mychatapp.data.sampleSelfProfile
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

/**
 * SelfProfileViewModel
 * - Quản lý dữ liệu hồ sơ người dùng (phần "More" → "Account")
 * - Sử dụng Flow để cập nhật realtime lên UI
 */
data class SelfProfile(
    val id: String,
    val name: String,
    val phoneNumber: String,
    val imageUrl: String?,
    val bio: String
)

class SelfProfileViewModel : ViewModel() {

    private val _uiState = MutableStateFlow(sampleSelfProfile)
    val uiState: StateFlow<SelfProfile> = _uiState

    fun loadSelfProfile() {
        viewModelScope.launch {
            _uiState.value = sampleSelfProfile
        }
    }

    /**
     * Cập nhật profile người dùng
     * @param name: tên mới
     * @param imageUri: ảnh mới (URI local)
     * @param context: dùng cho xử lý upload file
     */
    fun updateProfile(name: String, imageUri: Uri?, context: Context) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(name = name)
            // TODO: nếu có repo thì gọi repo.updateSelf(...)
            // upload imageUri lên server nếu cần
        }
    }
}
