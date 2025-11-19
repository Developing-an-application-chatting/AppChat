package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

data class LoginRequestDto(
    // 💡 Đổi từ "token" sang "PhoneNumber" để khớp C#
    @SerializedName("PhoneNumber")
    val phoneNumber: String
)