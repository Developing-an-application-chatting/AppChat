package com.example.mychatapp.network

import com.example.mychatapp.network.dto.LoginResponseDto
import com.example.mychatapp.model.modelData.Chat
import com.example.mychatapp.model.modelData.ChatMessage
import com.example.mychatapp.network.dto.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    // --- AUTH ---
    @POST("Auth/login")
    suspend fun login(@Body loginDto: LoginRequestDto): Response<LoginResponseDto>

    // Xóa register nếu backend không có endpoint riêng, hoặc dùng lại login để tạo user
    @POST("Auth/login") // login sẽ tự tạo user nếu chưa có
    suspend fun register(@Body registerDto: RegisterRequestDto): Response<LoginResponseDto>

    // --- USER & CONTACTS ---
    @GET("User/list")
    suspend fun getAllUsers(): List<UserDto>
    @GET("Contact")
    suspend fun getMyContacts(): Response<ContactResponse>
    @POST("Contact/add")
    suspend fun addFriend(@Body request: AddContactRequest): Response<Unit>


    // --- CHAT ---
    @GET("api/chat")
    suspend fun getChatConversations(): List<Chat>

    @GET("api/chat/{friendId}/messages")
    suspend fun getChatHistory(@Path("friendId") friendId: String): List<ChatMessage>

    @GET("User/phone/{phone}")
    suspend fun getUserByPhone(@Path("phone") phone: String): Response<UserDto>

    @GET("Message/history/{friendId}")
    suspend fun getChatHistory(@Path("friendId") friendId: Int): Response<MessageHistoryResponseDto>
}
