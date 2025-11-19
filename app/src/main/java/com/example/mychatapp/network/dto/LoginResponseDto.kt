package com.example.mychatapp.network.dto

data class LoginResponseDto(
    val userId: Int,
    val accessToken: String,
    val name: String?,
    val avatarUrl: String?
)
