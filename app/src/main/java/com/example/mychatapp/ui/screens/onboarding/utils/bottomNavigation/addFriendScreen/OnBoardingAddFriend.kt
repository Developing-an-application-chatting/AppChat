package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.addFriendScreen

import android.widget.Toast
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.imePadding
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.wrapContentWidth
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.material3.TextFieldDefaults
import androidx.compose.material3.TopAppBar
import androidx.compose.material3.TopAppBarDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.navigation.NavController
import com.example.mychatapp.R
import com.example.mychatapp.ui.screens.onboarding.utils.PhoneValidator

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OnboardingAddFriend(bottomNavController: NavController) {
    var phoneNumber by remember { mutableStateOf("") }
    // SỬA LỖI LOGIC: Tạo state cho mã quốc gia, mặc định là "VN" (+84)
    var countryCode by remember { mutableStateOf("+84") }
    val context = LocalContext.current

    Scaffold(
        topBar = {
            TopAppBar(
                title = {},
                navigationIcon = {
                    IconButton(onClick = { bottomNavController.navigate("contacts") }) {
                        Icon(
                            painter = painterResource(id = R.drawable.vector),
                            contentDescription = "Back",
                            modifier = Modifier.size(15.dp),
                            tint = Color.Black
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = Color.Transparent,
                    navigationIconContentColor = Color.Black
                )
            )
        }
    ) { innerPading -> // -> 'innerPading' được nhận ở đây
        Column(
            modifier = Modifier
                .padding(innerPading)
                .padding(16.dp)
                .imePadding(),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Spacer(modifier = Modifier.height(120.dp))

            Text(
                text = "Searching for strangers",
                textAlign = TextAlign.Center,
                fontSize = 28.sp,
                fontWeight = FontWeight.SemiBold,
                color = Color(0xFF000000),
                lineHeight = 38.sp,
                modifier = Modifier
                    .width(350.dp)
            )

            Spacer(modifier = Modifier.height(15.dp))

            Text(
                text = "Please confirm stranger's country code and \nenter phone number",
                textAlign = TextAlign.Center,
                fontSize = 15.sp,
                color = Color(0xFF000000),
                lineHeight = 28.sp,
                modifier = Modifier
                    .width(350.dp)

            )

            Spacer(modifier = Modifier.height(55.dp))

            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp),

                verticalAlignment = Alignment.CenterVertically
            ) {
                Box(
                    modifier = Modifier
                        .wrapContentWidth()
                        .height(60.dp)
                        .border(
                            width = 1.dp,
                            color = Color(0xFFADB5BD),
                            shape = RoundedCornerShape(15.dp)
                        ),
                    contentAlignment = Alignment.Center
                ) {
                    AndroidView(
                        factory = { context ->
                            com.hbb20.CountryCodePicker(context).apply {
                                setAutoDetectedCountry(true)
                                setShowPhoneCode(true)
                                setCcpDialogShowPhoneCode(true)
                                ccpDialogShowFlag = true
                                ccpDialogShowNameCode = true

                                textAlignment = android.view.View.TEXT_ALIGNMENT_CENTER

                                setPadding(0, 0, 0, 0)

                                // SỬA LỖI LOGIC: Cập nhật state khi người dùng đổi quốc gia
                                setOnCountryChangeListener {
                                    countryCode = "+" + this.selectedCountryCode
                                    this.post {
                                        this.requestLayout()
                                        this.invalidate()
                                    }
                                }
                            }
                        },
                        modifier = Modifier
                            .width(103.dp)
                            .height(45.dp)
                            .align(Alignment.Center)
                    )
                }
                // Country picker
                // Sử dụng để xác định được ID SĐT của người dùng


                Spacer(modifier = Modifier.width(20.dp))

                // TextField nhập số điện thoại
                Box(
                    modifier = Modifier
                        .wrapContentWidth()
                        .height(60.dp)
                        .border(
                            width = 1.dp,
                            color = Color(0xFFADB5BD),
                            shape = RoundedCornerShape(15.dp)
                        )
                ) {
                    TextField(
                        value = phoneNumber,
                        onValueChange = { phoneNumber = it },
                        placeholder = {
                            Text(
                                text = "Phone Number",
                                color = Color(0xFFADB5BD),
                                fontSize = 15.sp
                            )
                        },
                        singleLine = true,
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                        colors = TextFieldDefaults.colors(
                            unfocusedContainerColor = Color.Transparent,
                            focusedContainerColor = Color.Transparent,
                            unfocusedIndicatorColor = Color.Transparent,
                            focusedIndicatorColor = Color.Transparent,
                            cursorColor = Color(0xFF000000)
                        ),
                    )
                }
            }
            Spacer(modifier = Modifier.height(30.dp))
            Button(
                onClick = {
                    // SỬA LỖI LOGIC: Xóa dòng hard-code
                    // val countryCode = "+84"
                    var phone = phoneNumber.trim()

                    // Xử lý loại bỏ số 0 ở đầu
                    if (phone.startsWith("0")) phone = phone.drop(1)

                    // Sử dụng `countryCode` từ state
                    val friendPhoneNumber = "$countryCode$phone"

                    //
                    val error = PhoneValidator.validatePhoneNumber(phone, countryCode)

                    if (error != null) {
                        Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                    } else {
                        // 👉 TODO: Gọi API hoặc ViewModel để thêm bạn ở đây
                        Toast.makeText(
                            context,
                            "Finding and Adding friend: $friendPhoneNumber",
                            Toast.LENGTH_SHORT
                        ).show()
                        bottomNavController.navigate("friendInformation/$friendPhoneNumber")
                    }
                },
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp)
            ) {
                Text(
                    text = "Find",
                    color = Color.White,
                    fontSize = 16.sp,
                    fontWeight = FontWeight.Medium
                )
            }
        }
    }
}