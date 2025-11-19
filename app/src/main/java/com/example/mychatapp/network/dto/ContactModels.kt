package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

// 1. Dùng để hứng list user khi tìm kiếm (API: /User/list)
data class UserDto(
    val id: Int,

    @SerializedName("firstName", alternate = ["FirstName"]) // Nhận cả chữ hoa và thường
    val firstName: String?,

    @SerializedName("lastName", alternate = ["LastName"])
    val lastName: String?,

    val email: String?,

    @SerializedName("avatarUrl", alternate = ["UserAva", "AvatarUrl"])
    val avatarUrl: String?,

    val isOnline: Boolean
)

data class ContactItemDto(
    @SerializedName("Id") val relationshipId: Int, // ID của bảng Contact
    val contactInfo: UserDto // Thông tin người bạn
)

