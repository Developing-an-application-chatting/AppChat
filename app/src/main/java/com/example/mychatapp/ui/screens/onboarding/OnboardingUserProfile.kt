package com.example.mychatapp.ui.screens.onboarding

import androidx.activity.compose.BackHandler
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.example.mychatapp.R
import com.example.mychatapp.model.modelData.Contact
import com.example.mychatapp.model.viewModel.ContactRepository
import com.example.mychatapp.ui.screens.onboarding.utils.SessionManager
import kotlinx.coroutines.launch
import com.example.mychatapp.network.dto.RegisterRequestDto
import com.example.mychatapp.network.RetrofitInstance
import android.widget.Toast
import com.example.mychatapp.network.dto.LoginRequestDto

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginUserProfileScreen(
    navController: NavController,
    phoneNumber: String
) {
    BackHandler { /* chặn back */ }

    var firstName by remember { mutableStateOf("") }
    var lastName by remember { mutableStateOf("") }

    var firstNameError by remember { mutableStateOf(false) }
    var lastNameError by remember { mutableStateOf(false) }

    val context = LocalContext.current
    val coroutineScope = rememberCoroutineScope()
    val sessionManager = remember { SessionManager(context) }
    val contactRepository = ContactRepository
    val api = RetrofitInstance.api

    var isLoading by remember { mutableStateOf(false) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Text(
                        text = "Your Profile",
                        color = Color.Black,
                        fontWeight = FontWeight.SemiBold,
                        fontSize = 23.sp
                    )
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = Color.Transparent,
                    navigationIconContentColor = Color.Black
                )
            )
        }
    ) { innerPadding ->
        Column(
            modifier = Modifier
                .padding(innerPadding)
                .fillMaxSize()
                .padding(horizontal = 24.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {

            Spacer(modifier = Modifier.height(40.dp))

            // Avatar
            Box(contentAlignment = Alignment.BottomEnd) {
                Box(
                    modifier = Modifier
                        .size(110.dp)
                        .clip(CircleShape)
                        .background(Color.White),
                    contentAlignment = Alignment.Center
                ) {
                    Icon(
                        painter = painterResource(id = R.drawable.modellogin),
                        contentDescription = "Profile Icon",
                        tint = Color.Black,
                        modifier = Modifier.size(60.dp)
                    )
                }

                Box(
                    modifier = Modifier
                        .size(28.dp)
                        .clip(CircleShape)
                        .background(Color.Black)
                        .border(2.dp, Color.White, CircleShape)
                        .clickable { /* TODO choose image */ },
                    contentAlignment = Alignment.Center
                ) {
                    Icon(
                        painter = painterResource(id = R.drawable.moremodellogin),
                        contentDescription = "Add Photo",
                        tint = Color.White,
                        modifier = Modifier.size(16.dp)
                    )
                }
            }

            Spacer(modifier = Modifier.height(40.dp))

            // FIRST NAME
            OutlinedTextField(
                value = firstName,
                onValueChange = {
                    firstName = it
                    firstNameError = false
                },
                placeholder = { Text("First Name (Required)") },
                singleLine = true,
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp),
                shape = RoundedCornerShape(12.dp),
                colors = OutlinedTextFieldDefaults.colors(
                    focusedContainerColor = Color(0xFFF9F9F9),
                    unfocusedContainerColor = Color(0xFFF9F9F9),
                    focusedBorderColor = if (firstNameError) Color.Red else Color.Transparent,
                    unfocusedBorderColor = if (firstNameError) Color.Red else Color.Transparent
                ),
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Text)
            )

            if (firstNameError) {
                Text(
                    text = "First name cannot be empty",
                    color = Color.Red,
                    fontSize = 13.sp,
                    modifier = Modifier
                        .align(Alignment.Start)
                        .padding(top = 4.dp, start = 4.dp)
                )
            }

            Spacer(modifier = Modifier.height(16.dp))

            // LAST NAME
            OutlinedTextField(
                value = lastName,
                onValueChange = {
                    lastName = it
                    lastNameError = false
                },
                placeholder = { Text("Last Name (Required)") },
                singleLine = true,
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp),
                shape = RoundedCornerShape(12.dp),
                colors = OutlinedTextFieldDefaults.colors(
                    focusedContainerColor = Color(0xFFF9F9F9),
                    unfocusedContainerColor = Color(0xFFF9F9F9),
                    focusedBorderColor = if (lastNameError) Color.Red else Color.Transparent,
                    unfocusedBorderColor = if (lastNameError) Color.Red else Color.Transparent
                ),
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Text)
            )

            if (lastNameError) {
                Text(
                    text = "Last name cannot be empty",
                    color = Color.Red,
                    fontSize = 13.sp,
                    modifier = Modifier
                        .align(Alignment.Start)
                        .padding(top = 4.dp, start = 4.dp)
                )
            }

            Spacer(modifier = Modifier.height(30.dp))

            // BUTTON SAVE
            Button(
                onClick = {
                    firstNameError = firstName.isBlank()
                    lastNameError = lastName.isBlank()

                    if (firstNameError || lastNameError || isLoading) return@Button

                    isLoading = true

                    val body = RegisterRequestDto(
                        firstName = firstName,
                        lastName = lastName,
                        phoneNumber = phoneNumber
                    )

                    coroutineScope.launch {
                        try {
                            // 1) GỌI API ĐĂNG KÝ
                            val registerRes = api.register(body)

                            if (!registerRes.isSuccessful) {
                                Toast.makeText(context, "Lỗi đăng ký: ${registerRes.code()}", Toast.LENGTH_SHORT).show()
                                isLoading = false
                                return@launch
                            }

                            // 2) GỌI API ĐĂNG NHẬP (Để lấy Token và ID)
                            val loginRes = api.login(LoginRequestDto(phoneNumber))

                            if (!loginRes.isSuccessful) {
                                Toast.makeText(context, "Đăng nhập thất bại: ${loginRes.code()}", Toast.LENGTH_SHORT).show()
                                isLoading = false
                                return@launch
                            }

                            // Lấy dữ liệu từ body trả về
                            val loginData = loginRes.body()
                            if (loginData == null) {
                                Toast.makeText(context, "Lỗi: Dữ liệu server trả về rỗng", Toast.LENGTH_SHORT).show()
                                isLoading = false
                                return@launch
                            }

                            // 3) CẬP NHẬT REPOSITORY (QUAN TRỌNG NHẤT)
                            // Gán ID kiểu Int vào biến kiểu Int -> Hết lỗi Type mismatch
                            ContactRepository.currentUserId = loginData.userId.toInt()

                            // 4) LƯU SESSION VÀO MÁY
                            sessionManager.saveUserSession(
                                id = loginData.userId,
                                name = loginData.name ?: "$firstName $lastName",
                                token = loginData.token,
                                avatarUrl = ""
                            )

                            // 5) CẬP NHẬT DANH BẠ CỤC BỘ (Để hiển thị chính mình)
                            contactRepository.clear()
                            contactRepository.addSelfToLocal(
                                Contact(
                                    // Chuyển ID sang String vì giao diện yêu cầu String -> Hết lỗi Type mismatch
                                    id = loginData.userId.toInt(),
                                    name = loginData.name ?: "$firstName $lastName",
                                    status = "Online",
                                    isOnline = true,
                                    avatarUrl = null
                                )
                            )

                            // 6) CHUYỂN MÀN HÌNH
                            isLoading = false
                            navController.navigate("mainScreen") {
                                popUpTo("user_profile_screen") { inclusive = true }
                                launchSingleTop = true
                            }

                        } catch (e: Exception) {
                            isLoading = false
                            Toast.makeText(context, "Lỗi kết nối: ${e.message}", Toast.LENGTH_SHORT).show()
                            e.printStackTrace()
                        }
                    }
                },
                enabled = !isLoading,
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp),
                shape = RoundedCornerShape(50),
                colors = ButtonDefaults.buttonColors(
                    containerColor = Color(0xFF0056FF),
                    disabledContainerColor = Color(0xFFB0C4FF)
                )
            ) {
                Text(
                    text = "Save",
                    color = Color.White,
                    fontWeight = FontWeight.Medium,
                    fontSize = 16.sp,
                    textAlign = TextAlign.Center
                )
            }
        }
    }
}
