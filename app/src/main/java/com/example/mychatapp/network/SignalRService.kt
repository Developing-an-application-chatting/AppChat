package com.example.mychatapp.network

import android.util.Log
import com.example.mychatapp.network.dto.MessageResponseDto
import com.example.mychatapp.network.dto.SendMessageRequestDto
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import com.microsoft.signalr.HubConnectionState
import io.reactivex.rxjava3.core.Single
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch

object SignalRService {

    // Đổi localhost thành 10.0.2.2 nếu chạy máy ảo Android, hoặc IP LAN nếu chạy máy thật
    private const val HUB_URL = "http://192.168.242.221:5047/chatHub"

    private var hubConnection: HubConnection? = null

    // Flow để bắn tin nhắn mới nhận được sang Repository/ViewModel
    private val _incomingMessages = MutableSharedFlow<MessageResponseDto>()
    val incomingMessages = _incomingMessages.asSharedFlow()

    // Hàm khởi tạo kết nối (Gọi khi Login thành công)
    fun startConnection(token: String) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) return

        try {
            val urlWithToken = "$HUB_URL?access_token=$token"

            hubConnection = HubConnectionBuilder.create(urlWithToken).build()

            // Lắng nghe sự kiện "ReceiveMessage" từ Backend (ChatHub.cs dòng 97)
            hubConnection?.on("ReceiveMessage", { messageDto ->
                Log.d("SignalR", "Nhận tin nhắn: ${messageDto.content}")
                CoroutineScope(Dispatchers.IO).launch {
                    _incomingMessages.emit(messageDto)
                }
            }, MessageResponseDto::class.java)

            // Bắt đầu kết nối
            hubConnection?.start()?.blockingAwait()
            Log.d("SignalR", "Kết nối thành công! ID: ${hubConnection?.connectionId}")

        } catch (e: Exception) {
            Log.e("SignalR", "Lỗi kết nối: ${e.message}")
        }
    }

    fun stopConnection() {
        hubConnection?.stop()
        hubConnection = null
    }

    // Backend yêu cầu JoinGroup để nhận tin nhắn (ChatHub.cs dòng 116)
    fun joinChatGroup(chatId: String) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) {
            hubConnection?.invoke("JoinGroup", chatId)
            Log.d("SignalR", "Đã join vào phòng: $chatId")
        }
    }

    // Gửi tin nhắn (ChatHub.cs dòng 56 - SendMessage(SendMessageDto dto))
    fun sendMessage(request: SendMessageRequestDto) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) {
            hubConnection?.invoke("SendMessage", request)
        } else {
            Log.e("SignalR", "Chưa kết nối, không thể gửi tin!")
        }
    }
}