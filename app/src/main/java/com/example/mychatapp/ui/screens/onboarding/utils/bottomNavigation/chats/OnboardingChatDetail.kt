package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.chats

import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import com.example.mychatapp.R
import com.example.mychatapp.model.viewModel.ChatListViewModel
import kotlinx.coroutines.launch

/**
 * =====================================
 * OnboardingChatDetail.kt
 * =====================================
 * Màn hình hội thoại chi tiết giữa 2 người dùng.
 *
 * FRONTEND:
 * - Hiển thị danh sách tin nhắn, khung nhập, gửi text / ảnh
 * BACKEND:
 * - Gọi API thật để lấy/gửi tin nhắn
 * - Kết nối SignalR (C#) để cập nhật real-time
 */

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OnboardingChatDetail(
    navController: NavController,
    receiverId: String,
    receiverName: String,
    senderId: String,
    viewModel: ChatListViewModel = viewModel(),
) {
    val messages by viewModel.messages.collectAsState()
    var messageText by remember { mutableStateOf("") }
    val coroutineScope = rememberCoroutineScope()

    val context = androidx.compose.ui.platform.LocalContext.current
    val photoPickerLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.PickMultipleVisualMedia(),
        onResult = { uris ->
            // 'uris' là một List<Uri> chứa tất cả ảnh người dùng đã chọn
            if (uris.isNotEmpty()) {
                coroutineScope.launch {
                    uris.forEach { uri ->
                        // Gọi ViewModel để xử lý từng ảnh
                        viewModel.uploadImageMessage(context, uri, senderId.toInt(), receiverId.toInt())                    }
                }
            }
        }
    )

    // ✅ Lấy lịch sử tin nhắn khi mở khung chat
    LaunchedEffect(receiverId) {
        viewModel.loadChatHistory(receiverId)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(receiverName) },
                navigationIcon = {
                    IconButton(onClick = { navController.popBackStack() }) {
                        Icon(
                            painter = painterResource(id = R.drawable.vector),
                            contentDescription = "Back",
                            modifier = Modifier.size(15.dp),
                            tint = Color.Black
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(containerColor = Color.White)
            )
        },
        bottomBar = {
            ChatInputBar(
                messageText = messageText,
                onMessageChange = { messageText = it },
                onSendClick = {
                    if (messageText.isNotBlank()) {
                        coroutineScope.launch {
                            viewModel.sendTextMessage(senderId, receiverId, messageText)
                            messageText = ""
                        }
                    }
                },
                onAttachClick = {
                    // Mở ImagePicker thật → chọn ảnh
                    // Gọi viewModel.uploadImageMessage(selectedImageUri, senderId, receiverId)
                    photoPickerLauncher.launch(
                        PickVisualMediaRequest(ActivityResultContracts.PickVisualMedia.ImageOnly)
                    )
                }
            )
        }
    ) { padding ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .background(Color(0xFFF6F6F6)),
            reverseLayout = true
        ) {
            items(messages) { msg ->
                ChatBubble(message = msg)
            }
        }
    }
}
