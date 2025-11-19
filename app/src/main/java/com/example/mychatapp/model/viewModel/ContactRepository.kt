package com.example.mychatapp.model.viewModel

import android.util.Log
import com.example.mychatapp.model.modelData.Contact
import com.example.mychatapp.network.RetrofitInstance
import com.example.mychatapp.network.dto.AddContactRequest
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

object ContactRepository {

    private val api = RetrofitInstance.api

    // Thay bằng IP máy của bạn (như file ChatRepository)
    private const val BASE_URL = "http://192.168.242.221:8080"

    // ID của user hiện tại (set khi Login)
    var currentUserId: Int = 1

    // 1. Danh sách bạn bè (Đã kết bạn)
    private val _contacts = MutableStateFlow<List<Contact>>(emptyList())
    val contacts = _contacts.asStateFlow()

    // 2. Danh sách tất cả user (Để tìm kiếm kết bạn)
    private val _allUsers = MutableStateFlow<List<Contact>>(emptyList())
    val allUsers = _allUsers.asStateFlow()

    // === API: Lấy danh bạ ===
    fun fetchContacts() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val response = api.getMyContacts()
                if (response.isSuccessful && response.body() != null) {
                    val wrapper = response.body()!!

                    val mappedList = wrapper.contacts.map { item ->
                        val u = item.user
                        Contact(
                            id = u.id,
                            name = u.fullName,
                            avatarUrl = u.avatarUrl?.let { "$BASE_URL$it" },
                            isOnline = u.isOnline,
                            status = if (u.isOnline) "Online" else "Offline"
                        )
                    }
                    _contacts.value = mappedList
                } else {
                    Log.e("ContactRepo", "Fetch contacts failed: ${response.code()} ${response.message()}")
                }
            } catch (e: Exception) {
                Log.e("ContactRepo", "Lỗi lấy danh bạ: ${e.message}")
            }
        }
    }

    // === API: Lấy tất cả user để tìm kiếm ===
    fun fetchAllUsers() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val dtoList = api.getAllUsers()

                val mappedList = dtoList.map { u ->
                    val avatar = u.avatarUrl?.let { path ->
                        if (path.startsWith("http")) path else "$BASE_URL$path"
                    }

                    Contact(
                        id = u.id,
                        name = "${u.lastName} ${u.firstName}",
                        avatarUrl = avatar,
                        isOnline = u.isOnline,
                        status = if (u.isOnline) "Online" else "Offline"
                    )
                }

                _allUsers.value = mappedList.filter { it.id != currentUserId }

            } catch (e: Exception) {
                Log.e("ContactRepo", "Lỗi lấy list user: ${e.message}")
            }
        }
    }

    // === API: Thêm bạn ===
    fun addFriend(friendId: String, onSuccess: () -> Unit, onError: () -> Unit) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val request = AddContactRequest(
                    friendId = friendId.toInt()
                )
                val response = api.addFriend(request)

                if (response.isSuccessful) {
                    // Tải lại danh bạ để cập nhật
                    fetchContacts()

                    // 👇 QUAN TRỌNG: Chuyển về luồng Main để gọi Navigation
                    withContext(Dispatchers.Main) {
                        Log.d("ContactRepo", "Kết bạn thành công!")
                        onSuccess()
                    }
                } else {
                    withContext(Dispatchers.Main) {
                        Log.e("ContactRepo", "Lỗi kết bạn: ${response.code()}")
                        onError()
                    }
                }
            } catch (e: Exception) {
                e.printStackTrace()
                withContext(Dispatchers.Main) {
                    onError()
                }
            }
        }
    }

    // Hàm clear dữ liệu khi logout
    fun clear() {
        _contacts.value = emptyList()
        _allUsers.value = emptyList()
    }

    fun addSelfToLocal(user: Contact) {
        val currentList = _contacts.value.toMutableList()
        currentList.removeIf { it.id == user.id }
        currentList.add(0, user)
        _contacts.value = currentList
    }
}