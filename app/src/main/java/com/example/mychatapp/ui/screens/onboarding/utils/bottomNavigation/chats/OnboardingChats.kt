package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.chats

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import coil.compose.AsyncImage
import com.example.mychatapp.model.modelData.Chat
import com.example.mychatapp.model.viewModel.ChatViewModel

/**
 * ================================
 * 📱 OnboardingChats.kt
 * ================================
 * Màn hình hiển thị danh sách các đoạn hội thoại.
 *
 * CLIENT (App – Jetpack Compose):
 * - Hiển thị danh sách các đoạn chat gần nhất (tạm dùng dữ liệu giả).
 * - Khi có backend, ViewModel sẽ gọi API thật để lấy dữ liệu.
 *
 * BACKEND (ASP.NET C#):
 * - Cần cung cấp REST API và SignalR Hub:
 *      GET /api/chat                   → Lấy danh sách đoạn chat gần nhất
 *      GET /api/chat/{friendId}        → Lấy lịch sử tin nhắn
 *      WebSocket /chathub (SignalR)    → Gửi/nhận tin nhắn real-time
 */

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OnboardingChats(
    navController: NavController,
    viewModel: ChatViewModel = viewModel()
) {
    var searchQuery by remember { mutableStateOf("") }

    // Lấy danh sách đoạn chat từ ViewModel (dữ liệu giả / thật từ API)
    val chats by viewModel.chats.collectAsState()

    LaunchedEffect(Unit) {
        viewModel.refreshChats()
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Chats") },
                //thêm vào chat nhóm nếu phát triển kịp
//                actions = {
//                    IconButton(onClick = { /* TODO: Thêm icon tạo nhóm / tin nhắn mới */ }) {
//                        Icon(Icons.Default.Search, contentDescription = "Search Icon")
//                    }
//                }
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .background(Color.White)
                .padding(horizontal = 16.dp, vertical = 8.dp)
        ) {
            //Thanh tìm kiếm
            SearchBar(
                query = searchQuery,
                onQueryChange = { searchQuery = it }
            )
            Spacer(modifier = Modifier.height(8.dp))

            LazyColumn(
                modifier = Modifier.fillMaxSize(),
                contentPadding = PaddingValues(vertical = 8.dp)
            ) {
                // Lọc danh sách theo tên người
                val filteredChats = chats.filter {
                    it.name.contains(searchQuery, ignoreCase = true)
                }

                // Lọc danh sách theo ô tìm kiếm
                items(filteredChats) { chat ->
                    ChatItem(
                        chat = chat,
                        onClick = {
                            // Khi bấm vào một đoạn chat:
                            // Điều hướng đến màn hình chi tiết chat
                            // Ở đó App sẽ gọi API:
                            //    GET /api/chat/{chat.id}
                            //    hoặc kết nối SignalR /chathub
                            navController.navigate("chat_detail/${chat.id}/${chat.name}")
                        }
                    )
                }
            }
        }
    }
}

@Composable
fun SearchBar(
    query: String,
    onQueryChange: (String) -> Unit,
    modifier: Modifier = Modifier
) {
    OutlinedTextField(
        value = query,
        onValueChange = onQueryChange,
        leadingIcon = {
            Icon(
                imageVector = Icons.Default.Search,
                contentDescription = "Search Icon",
                tint = Color(0xFF9CA3AF),
                modifier = Modifier.size(20.dp)
            )
        },
        placeholder = {
            Text(
                text = "Search chats",
                color = Color(0xFF9CA3AF),
                fontSize = 14.sp
            )
        },
        singleLine = true,
        modifier = modifier
            .fillMaxWidth()
            .height(54.dp),
        shape = RoundedCornerShape(12.dp),
        colors = TextFieldDefaults.colors(
            focusedContainerColor = Color(0xFFF6F6F6),
            unfocusedContainerColor = Color(0xFFF6F6F6),
            disabledContainerColor = Color(0xFFF6F6F6),
            focusedIndicatorColor = Color.Transparent,
            unfocusedIndicatorColor = Color.Transparent,
            disabledIndicatorColor = Color.Transparent,
            cursorColor = Color.Gray,
            //test thử
            focusedLeadingIconColor = Color.Gray,
            unfocusedLeadingIconColor = Color.Gray
        ),
        textStyle = MaterialTheme.typography.bodyMedium.copy(fontSize = 14.sp)
    )
}
/**
 * Thành phần hiển thị một đoạn chat (avatar, tên, tin nhắn cuối, thời gian)
 *
 * BACKEND:
 *  - Cần trả về cấu trúc dữ liệu Chat:
 *      {
 *          "id": "1",
 *          "name": "Athalia Putri",
 *          "lastMessage": "Hey, how are you?",
 *          "time": "09:45 AM",
 *          "avatarUrl": "https://i.pravatar.cc/150?img=1"
 *      }
 */
@Composable
fun ChatItem(
    chat: Chat, onClick: () -> Unit
) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 10.dp)
            .clip(RoundedCornerShape(12.dp))
            .background(Color(0xFFF9FAFB))
            .padding(horizontal = 12.dp, vertical = 10.dp)
            .clickable { onClick() },
        verticalAlignment = Alignment.CenterVertically
    ) {
        Box(modifier = Modifier.size(50.dp)) {
            // Ảnh đại diện hoặc ký tự viết tắt
            if (chat.avatarUrl != null) {
                AsyncImage(
                    model = chat.avatarUrl,
                    contentDescription = chat.name,
                    modifier = Modifier
                        .size(50.dp)
                        .clip(CircleShape),
                    contentScale = ContentScale.Crop
                )
            } else {
                Box(
                    modifier = Modifier
                        .size(50.dp)
                        .clip(CircleShape)
                        .background(Color(0xFFE5E9FF)),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        text = chat.name.split(" ").map { it.first() }.take(2).joinToString(""),
                        color = Color(0xFF1E40AF),
                        fontWeight = FontWeight.Bold
                    )
                }
            }
        }
        Spacer(modifier = Modifier.width(16.dp))

        Column(modifier = Modifier.weight(1f)) {
            Text(
                text = chat.name,
                fontWeight = FontWeight.SemiBold,
                fontSize = 16.sp,
                maxLines = 1,
                overflow = TextOverflow.Ellipsis
            )
            Text(
                text = chat.lastMessage,
                style = MaterialTheme.typography.bodySmall,
                color = Color.Gray,
//                fontSize = 14.sp,
                maxLines = 1,
                overflow = TextOverflow.Ellipsis
            )
        }

        // Hiển thị thời gian tin nhắn cuối cùng
        Text(
            text = chat.time,
            fontWeight = FontWeight.SemiBold,
            fontSize = 12.sp,
            maxLines = 1,
            overflow = TextOverflow.Ellipsis,
        )
    }
}
