package com.example.mychatapp.ui.screens.onboarding

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.example.mychatapp.R
import com.example.mychatapp.ui.screens.onboarding.utils.SessionManager

@Composable
fun OnboardingScreen(navController: NavController) {
    val context = LocalContext.current
    val sessionManager = remember { SessionManager(context) }

    val isLoggedIn by sessionManager.isLoggedIn.collectAsState(initial = false)
//C2: để thoát ra contacts và test UI
//    LaunchedEffect(isLoggedIn) {
//        if (isLoggedIn) {
//            navController.navigate("contacts") {
//                popUpTo("onboarding") { inclusive = false }
//            }
//        }
//    }
//C1:Để thoát ra contacts và test UI
//    LaunchedEffect(Unit) {
//        sessionManager.setLoggedIn(false)
//    }

    if (!isLoggedIn) {

        Column(
            modifier = Modifier
                .fillMaxSize()
                .background(Color.White)
                .padding(horizontal = 24.dp, vertical = 16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {

            // ⬆️ Spacer trên cùng - để đẩy nội dung tổng thể lên/xuống
            Spacer(modifier = Modifier.height(40.dp)) // 👈 tuỳ chỉnh giá trị này

            // 🟦 Illustration + Text
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier.fillMaxWidth()
            ) {
                // Illustration area
                BoxWithConstraints(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(300.dp), // Hoặc weight(0.4f) nếu muốn responsive
                    contentAlignment = Alignment.Center
                ) {
                    val screenWidth = maxWidth
                    val screenHeight = maxHeight

                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        // Left illustration
                        Box(
                            modifier = Modifier
                                .width(screenWidth * 0.45f)
                                .aspectRatio(1f),
                            contentAlignment = Alignment.Center
                        ) {
                            Image(
                                painter = painterResource(id = R.drawable.circlebg1),
                                contentDescription = "Circle Left",
                                modifier = Modifier
                                    .fillMaxSize(0.8f)
                                    .align(Alignment.Center)
                                    .offset(x = screenWidth * 0.05f, y = screenHeight * 0.02f)
                            )

                            Image(
                                painter = painterResource(id = R.drawable.member1),
                                contentDescription = "Hình 1",
                                modifier = Modifier
                                    .fillMaxWidth(0.95f)
                                    .aspectRatio(0.5f)
                                    .align(Alignment.BottomStart)
                                    .offset(x = screenWidth * 0.08f, y = screenHeight * 0.25f)
                            )

                            Image(
                                painter = painterResource(id = R.drawable.chat1),
                                contentDescription = "Chat 1",
                                modifier = Modifier
                                    .fillMaxWidth(0.35f)
                                    .aspectRatio(1f)
                                    .align(Alignment.TopEnd)
                                    .offset(x = screenWidth * 0.03f, y = screenHeight * 0.02f)
                            )
                        }

                        // Right illustration
                        Box(
                            modifier = Modifier
                                .width(screenWidth * 0.45f)
                                .aspectRatio(1f),
                            contentAlignment = Alignment.Center
                        ) {
                            Image(
                                painter = painterResource(id = R.drawable.circlebg2),
                                contentDescription = "Circle Right",
                                modifier = Modifier
                                    .fillMaxSize(0.7f)
                                    .align(Alignment.Center)
                                    .offset(x = -screenWidth * 0.01f, y = -screenHeight * 0.23f)
                            )

                            Image(
                                painter = painterResource(id = R.drawable.chat2),
                                contentDescription = "Chat 2",
                                modifier = Modifier
                                    .fillMaxWidth(0.35f)
                                    .aspectRatio(1f)
                                    .align(Alignment.TopEnd)
                                    .offset(x = -screenWidth * 0.3f, y = -screenHeight * 0.18f)
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(90.dp)) // khoảng cách illustration ↔ text

                // 🟨 Text Section
                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(
                        text = "Connect easily with \nyour family and friends \nover countries",
                        fontWeight = FontWeight.Bold,
                        fontSize = 25.sp,
                        textAlign = TextAlign.Center,
                        letterSpacing = 0.5.sp,
                        color = Color.Black,
                        fontStyle = FontStyle.Normal,
                        lineHeight = 30.sp
                    )

                    Spacer(modifier = Modifier.weight(1f))

                    Text(
                        text = "Terms & Privacy Policy",
                        style = MaterialTheme.typography.bodySmall.copy(fontSize = 12.sp),
                        color = Color.Gray
                    )

                    Spacer(modifier = Modifier.height(16.dp))

                    // 🟩 Bottom Button
                    Button(
                        onClick = { navController.navigate("PhoneNumber") },
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(56.dp),
                        shape = RoundedCornerShape(28.dp),
                        colors = ButtonDefaults.buttonColors(
                            containerColor = Color(0xFF0A5CFF)
                        )
                    ) {
                        Text(
                            text = "Start Messaging",
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Medium,
                            color = Color.White
                        )
                    }
                }
            }
        }
    }
}

