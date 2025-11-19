package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

// Class này hứng toàn bộ cục JSON trả về từ API getHistory
data class MessageHistoryResponseDto(
    @SerializedName("chatId") val chatId: Int,
    @SerializedName("messages") val messages: List<MessageResponseDto>
)

// Class này hứng từng tin nhắn lẻ (Đã có thể có rồi, kiểm tra lại)
/* data class MessageResponseDto(...) -> Giữ nguyên cái cũ của bạn */