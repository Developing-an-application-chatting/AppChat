package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.addFriendScreen.friendInformation

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import coil.compose.rememberAsyncImagePainter
import com.example.mychatapp.R
import com.example.mychatapp.model.viewModel.ChatViewModel
import com.example.mychatapp.model.modelData.Contact
import com.example.mychatapp.model.viewModel.ContactViewModel
// ⚠️ THAY ĐỔI 1: Import ViewModel mới
import com.example.mychatapp.model.FriendProfileViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun FriendInformationScreen(
    navController: NavController,
    friendPhoneNumber: String,
    viewModel: FriendProfileViewModel = viewModel()
) {
    // Sử dụng LaunchedEffect để chỉ gọi 1 lần khi màn hình mở ra
    LaunchedEffect(key1 = friendPhoneNumber) {
        viewModel.loadFriendProfile(friendPhoneNumber)
    }

    val friendProfile by viewModel.uiState.collectAsState()

    val contactViewModel: ContactViewModel = viewModel()
    val chatViewModel: ChatViewModel = viewModel()


    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = {
                    Text(
                        text = friendPhoneNumber, // Giữ SĐT người dùng tìm kiếm trên thanh tiêu đề
                        color = Color.Black,
                        fontWeight = FontWeight.SemiBold,
                        fontSize = 23.sp
                    )
                },
                navigationIcon = {
                    IconButton(onClick = { navController.navigate("addFriend") }) {
                        Icon(
                            painter = painterResource(id = R.drawable.vector),
                            contentDescription = "Back",
                            modifier = Modifier.size(18.dp),
                            tint = Color.Black
                        )
                    }
                },
            )
        }
    ) { padding ->

        if (friendProfile == null) {
            // Hiển thị vòng xoay loading ở giữa màn hình
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator()
            }
        } else {
            // Khi đã có dữ liệu (friendProfile != null)
            // Tạo một biến non-null để dễ sử dụng
            val profile = friendProfile!!

            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Spacer(modifier = Modifier.height(80.dp))

                // Avatar + Card
                Box(contentAlignment = Alignment.TopCenter) {

                    // Khung thông tin
                    Box(
                        modifier = Modifier
                            .padding(top = 70.dp)
                            .fillMaxWidth(0.85f)
                            .height(200.dp)
                            .clip(RoundedCornerShape(20.dp))
                            .background(Color(0xFFF3F3F3)),
                        contentAlignment = Alignment.TopCenter
                    ) {
                        Column(
                            modifier = Modifier.padding(top = 80.dp),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Text(
                                text = profile.name,
                                fontWeight = FontWeight.Bold,
                                fontSize = 20.sp,
                                textAlign = TextAlign.Center
                            )
                            Spacer(modifier = Modifier.height(10.dp))
                            Text(
                                text = profile.phoneNumber,
                                color = Color.Gray,
                                fontSize = 14.sp
                            )
                        }
                    }

                    // Avatar
                    Box(
                        modifier = Modifier
                            .size(120.dp)
                            .align(Alignment.TopCenter),
                        contentAlignment = Alignment.Center
                    ) {
                        Box(
                            modifier = Modifier
                                .size(120.dp)
                                .clip(CircleShape)
                                .background(Color(0xFF5FD482)),
                            contentAlignment = Alignment.Center
                        ) {
                            Image(
                                painter = when {
                                    profile.imageUrl != null -> rememberAsyncImagePainter(profile.imageUrl)
                                    else -> painterResource(id = R.drawable.vector)
                                },
                                contentDescription = "Avatar",
                                contentScale = ContentScale.Crop,
                                modifier = Modifier.fillMaxSize()
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(60.dp))

                // Nút Message
                Button(
                    onClick = {
                        // 1. Gọi API Kết bạn thật sự (Lưu vào DB)
                        contactViewModel.addFriend(
                            friendId = profile.id, // Hoặc ID kiểu Int tùy hàm
                            onSuccess = {

                                val safeId = profile.id.toIntOrNull() ?: 0
                                // 2. Nếu thành công -> Mới chuyển sang màn hình Chat
                                val friend = Contact(
                                    id = safeId,
                                    name = profile.name,
                                    status = "Friend",
                                    isOnline = false,
                                    avatarUrl = profile.imageUrl
                                )
                                // Thêm vào list chat tạm thời để hiện ngay
                                chatViewModel.addChat(friend)

                                // Chuyển hướng
                                navController.navigate("chat_detail/${friend.id}/${friend.name}"){
                                    popUpTo("contacts")
                                }
                            },
                            onError = {
                                // Nếu lỗi (ví dụ đã là bạn bè rồi) -> Vẫn cho vào chat
                                // Logic: Đã là bạn thì cứ vào chat thôi
                                navController.navigate("chat_detail/${profile.id}/${profile.name}")
                            }
                        )
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF0057FF)),
                    shape = RoundedCornerShape(12.dp),
                    modifier = Modifier
                        .fillMaxWidth(0.85f)
                        .height(50.dp)
                ) {
                    Text("Message", fontSize = 18.sp, fontWeight = FontWeight.Medium)
                }
            }
        }
    }
}