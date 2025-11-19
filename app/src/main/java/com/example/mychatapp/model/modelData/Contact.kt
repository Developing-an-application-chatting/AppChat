package com.example.mychatapp.model.modelData

data class Contact(
    val id: Int, // 👈 ĐỔI TỪ String SANG Int Ở ĐÂY
    val name: String,
    val lastMessage: String? = null,
    val time: String? = null,
    val avatarUrl: String?,
    val isOnline: Boolean = false,
    val status: String = "Offline"
)

