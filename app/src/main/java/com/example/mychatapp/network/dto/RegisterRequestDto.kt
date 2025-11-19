package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

data class RegisterRequestDto(
    @SerializedName("firstName")
    val firstName: String,
    @SerializedName("lastName")
    val lastName: String,
    @SerializedName("phoneNumber")
    val phoneNumber: String,
    @SerializedName("lastSeen")
    val lastSeen: String = "2025-11-16T12:00:00Z" // Có thể gán cứng
)