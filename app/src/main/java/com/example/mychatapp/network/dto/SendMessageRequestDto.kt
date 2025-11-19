package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

data class SendMessageRequestDto(
    @SerializedName("chatId") val chatId: Int,
    @SerializedName("senderId") val senderId: Int,
    @SerializedName("receiverId") val receiverId: Int, // 👈 THÊM DÒNG NÀY

    val content: String?,
    val fileBase64: String? = null,
    val fileName: String? = null,
    val fileType: String = "text"
)