package com.example.mychatapp.ui.screens.onboarding

import android.app.Activity
import com.example.mychatapp.ui.screens.onboarding.utils.PhoneValidator
import android.widget.Toast
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
//import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.imePadding
//import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.wrapContentWidth
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.material3.TextFieldDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.navigation.NavController
import com.google.firebase.FirebaseException
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PhoneAuthCredential
import com.google.firebase.auth.PhoneAuthOptions
import com.google.firebase.auth.PhoneAuthProvider
import java.util.concurrent.TimeUnit

@Composable
fun PhoneNumberAuthentication(navController: NavController) {
//    mutableStateOf("") tạo ra một state có thể thay đổi (biến trạng thái) với giá trị ban đầu là chuỗi rỗng "".
//
//    remember { ... } giúp lưu giữ (ghi nhớ) giá trị state này trong suốt quá trình recomposition (vẽ lại UI).
//    Nếu không có remember, mỗi lần Compose vẽ lại, biến đó sẽ bị khởi tạo lại, khiến giá trị người dùng nhập (ví dụ trong TextField) bị mất.
    var phoneNumber by remember { mutableStateOf("") }
    var countryCode by remember { mutableStateOf("+84") }
    val context = LocalContext.current

    val activity = LocalContext.current as Activity
    var isLoading by remember { mutableStateOf(false) }
    val auth = FirebaseAuth.getInstance()

    val callbacks = remember {
        object : PhoneAuthProvider.OnVerificationStateChangedCallbacks() {

            // Khi gửi OTP thành công
            override fun onCodeSent(
                verificationId: String,
                token: PhoneAuthProvider.ForceResendingToken
            ) {
                isLoading = false
                Toast.makeText(context, "OTP Sent!", Toast.LENGTH_SHORT).show()
                // Điều hướng sang màn hình OTP, mang theo SĐT và ID phiên
                val fullPhone = "$countryCode${phoneNumber.trim().dropWhile { it == '0' }}"
                navController.navigate("NumberCode/$fullPhone/$verificationId")
            }

            // Khi xác thực thất bại
            override fun onVerificationFailed(e: FirebaseException) {
                isLoading = false
                Toast.makeText(context, "Failed: ${e.message}", Toast.LENGTH_LONG).show()
            }

            // (Trường hợp hiếm) Tự động xác thực thành công (không cần OTP)
            override fun onVerificationCompleted(credential: PhoneAuthCredential) {
                // Chúng ta sẽ xử lý việc đăng nhập ở màn hình OTP
                // Nhưng nếu nó tự động xong, ta cũng có thể cho qua
                isLoading = false
                Toast.makeText(context, "Auto-Verified!", Toast.LENGTH_SHORT).show()
                // TODO: Xử lý đăng nhập ngay tại đây nếu muốn
            }
        }
    }

    Column(
        modifier = Modifier
            .padding(16.dp)
            .imePadding(),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Spacer(modifier = Modifier.height(120.dp))

        Text(
            text = "Enter Your Phone Number",
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
            text = "Please confirm your country code and enter \nyour phone number",
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
            ){
                AndroidView(
                    factory = { context ->
                        com.hbb20.CountryCodePicker(context).apply {
                            setAutoDetectedCountry(true)
                            setShowPhoneCode(true)
                            setCcpDialogShowPhoneCode(true)
                            ccpDialogShowFlag = true
                            ccpDialogShowNameCode = true

                            textAlignment = android.view.View.TEXT_ALIGNMENT_CENTER

                            setPadding(0,0,0,0)

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
            ){
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
                var phone = phoneNumber.trim()
                if (phone.startsWith("0")) phone = phone.drop(1)

                val fullPhone = "$countryCode$phone"
                val error = PhoneValidator.validatePhoneNumber(phoneNumber, countryCode)

                if (error != null) {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                } else {
                    isLoading = true
                    val options = PhoneAuthOptions.newBuilder(auth)
                        .setPhoneNumber(fullPhone)
                        .setTimeout(60L, TimeUnit.SECONDS) // Thời gian chờ
                        .setActivity(activity) // Activity
                        .setCallbacks(callbacks) // Callbacks để lắng nghe
                        .build()
                    PhoneAuthProvider.verifyPhoneNumber(options)
                }
            },
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp),
            enabled = !isLoading,
            colors = ButtonDefaults.buttonColors(
                disabledContainerColor = Color.Gray
            )
        )
        {
            Text(
                text = if (isLoading) "Sending..." else "Continue",
                color = Color.White,
                fontSize = 16.sp,
                fontWeight = FontWeight.Medium
            )
        }
    }
}
