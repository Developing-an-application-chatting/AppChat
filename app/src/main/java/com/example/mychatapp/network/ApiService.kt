package com.example.mychatapp.network

import com.example.mychatapp.model.modelData.Chat
import com.example.mychatapp.model.modelData.ChatMessage
import com.example.mychatapp.network.dto.AddContactRequest
import com.example.mychatapp.network.dto.ContactResponseWrapper
import com.example.mychatapp.network.dto.UserDto
import com.example.mychatapp.network.dto.* // Import hết DTO
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    // --- AUTH ---
    @POST("Auth/login")
    suspend fun login(@Body loginDto: LoginRequestDto): Response<LoginResponseDto>

    @POST("Auth/register")
    suspend fun register(@Body registerDto: RegisterRequestDto): Response<LoginResponseDto>

    // --- USER & CONTACTS ---

    // 1. Lấy danh sách tất cả User (để tìm kiếm và kết bạn)
    @GET("User/list")
    suspend fun getAllUsers(): List<UserDto>

    // 2. Lấy danh sách bạn bè đã kết bạn
    @GET("Contact")
    suspend fun getMyContacts(): Response<ContactResponseWrapper>

    // 3. Thêm bạn mới
    @POST("Contact/add")
    suspend fun addFriend(@Body request: AddContactRequest): Response<Any>

    // --- CHAT ---
    @GET("api/chat")
    suspend fun getChatConversations(): List<Chat>

    @GET("api/chat/{friendId}/messages")
    suspend fun getChatHistory(@Path("friendId") friendId: String): List<ChatMessage>

    @GET("User/phone/{phone}")
    suspend fun getUserByPhone(@Path("phone") phone: String): Response<UserDto>

    // Cần tạo thêm MessageHistoryResponseDto (xem bước 3)
    @GET("Message/history/{friendId}")
    suspend fun getChatHistory(@Path("friendId") friendId: Int): Response<MessageHistoryResponseDto>
}