package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName


// ==========================================
// RESPONSE: Nhận về (Server -> Client)
// ==========================================
data class MessageResponseDto(
    @SerializedName("id")
    val id: Int,

    @SerializedName("chatId")
    val chatId: Int,

    @SerializedName("senderId")
    val senderId: Int,

    @SerializedName("senderName")
    val senderName: String,

    @SerializedName("content")
    val content: String?,

    @SerializedName("fileUrl")
    val fileUrl: String?,

    @SerializedName("fileType")
    val fileType: String,

    @SerializedName("sentTime")
    val sentTime: String,

    @SerializedName("status")
    val status: String
)