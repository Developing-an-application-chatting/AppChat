package com.example.mychatapp.network.dto

data class RegisterRequestDto(
    val firstName: String,
    val lastName: String,
    val phoneNumber: String
)