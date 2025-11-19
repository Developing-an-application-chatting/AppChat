package com.example.mychatapp.model.modelData

/**
 * ================================
 * Chat.kt (Đã chuyển ID sang Int)
 * ================================
 */
data class Chat(
    val id: Int, // 👈 Đã đổi từ String sang Int
    val name: String,
    val lastMessage: String,
    val time: String,
    val avatarUrl: String?
)