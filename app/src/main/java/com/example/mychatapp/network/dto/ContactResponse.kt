package com.example.mychatapp.network.dto

data class ContactResponse(
    val userId: Int,
    val contacts: List<ContactItem>
)

data class ContactItem(
    val id: Int,
    val user: ContactUser
)

data class ContactUser(
    val id: Int,
    val fullName: String,
    val isOnline: Boolean,
    val avatarUrl: String?
)
