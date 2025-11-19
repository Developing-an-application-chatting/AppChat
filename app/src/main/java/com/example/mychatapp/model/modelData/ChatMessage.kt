package com.example.mychatapp.model.modelData

/**
 * ====================================
 * ChatMessage.kt (Phiên bản Int chuẩn hóa)
 * ====================================
 */
data class ChatMessage(
    val id: Int,             // 👈 Đổi String -> Int
    val senderId: Int,       // 👈 Đổi String -> Int
    val receiverId: Int,     // 👈 Đổi String -> Int (Lưu ChatId hoặc UserId)

    val type: String,        // "text" hoặc "image"
    val content: String?,
    val imageUrl: String?,
    val timestamp: String,
    val isRead: Boolean,
    val isSentByMe: Boolean
)