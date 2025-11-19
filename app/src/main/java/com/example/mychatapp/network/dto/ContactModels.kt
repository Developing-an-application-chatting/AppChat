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

// 2. Dùng để hứng response của API /Contact
data class ContactResponseWrapper(
    val userId: Int,
    val contacts: List<ContactItemDto>
)

data class ContactItemDto(
    @SerializedName("Id") val relationshipId: Int, // ID của bảng Contact
    val contactInfo: UserDto // Thông tin người bạn
)

// 3. Dùng để gửi request kết bạn
data class AddContactRequest(
    val userId: Int,   // Tạm thời gửi ID của mình
    val friendId: Int
)