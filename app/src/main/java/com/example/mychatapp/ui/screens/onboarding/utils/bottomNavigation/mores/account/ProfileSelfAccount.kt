package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.mores.account

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.provider.MediaStore
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
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
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import coil.compose.rememberAsyncImagePainter
import com.example.mychatapp.R
import com.example.mychatapp.model.SelfProfileViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ProfileSelfAccount(
    navController: NavController,
    viewModel: SelfProfileViewModel = viewModel()
) {
    val context = LocalContext.current
    val profile by viewModel.uiState.collectAsState()

    var selectedImageUri by remember { mutableStateOf<Uri?>(null) }
    var name by remember { mutableStateOf(profile.name) }

    val imagePicker = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.StartActivityForResult()
    ) { result ->
        if (result.resultCode == Activity.RESULT_OK) {
            selectedImageUri = result.data?.data
        }
    }

    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = {
                    Text(
                        text = "Account",
                        color = Color.Black,
                        fontWeight = FontWeight.SemiBold,
                        fontSize = 23.sp
                    )
                },
                navigationIcon = {
                    IconButton(onClick = { navController.navigate("profile") }) {
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
                        .height(200.dp) // kéo dài khung xuống một chút
                        .clip(RoundedCornerShape(20.dp))
                        .background(Color(0xFFF3F3F3)),
                    contentAlignment = Alignment.TopCenter
                ) {
                    Column(
                        modifier = Modifier.padding(top = 80.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        OutlinedTextField(
                            value = name,
                            onValueChange = { name = it },
                            placeholder = { Text("Your Name") },
                            singleLine = true,
                            textStyle = LocalTextStyle.current.copy(
                                textAlign = TextAlign.Center
                            ),
                            modifier = Modifier.width(200.dp)
                        )
                        Spacer(modifier = Modifier.height(10.dp))
                        Text(
                            text = profile.phoneNumber,
                            color = Color.Gray,
                            fontSize = 14.sp
                        )
                    }
                }

                // Avatar + Nút Add nằm ngoài
                Box(
                    modifier = Modifier
                        .size(130.dp)
                        .align(Alignment.TopCenter),
                    contentAlignment = Alignment.Center
                ) {
                    // Avatar tròn
                    Box(
                        modifier = Modifier
                            .size(120.dp)
                            .clip(CircleShape)
                            .background(Color(0xFF5FD482)),
                        contentAlignment = Alignment.Center
                    ) {
                        Image(
                            painter = when {
                                selectedImageUri != null -> rememberAsyncImagePainter(selectedImageUri)
                                profile.imageUrl != null -> rememberAsyncImagePainter(profile.imageUrl)
                                else -> painterResource(id = R.drawable.vector)
                            },
                            contentDescription = "Avatar",
                            contentScale = ContentScale.Crop,
                            modifier = Modifier.fillMaxSize()
                        )
                    }

                    // Nút Add ra ngoài avatar
                    Box(
                        modifier = Modifier
                            .size(30.dp)
                            .align(Alignment.BottomEnd)
                            .offset(x = -(5).dp, y = -(5).dp) // đẩy ra ngoài mép avatar
                            .background(Color.White, CircleShape)
                            .clickable {
                                val intent = Intent(
                                    Intent.ACTION_PICK,
                                    MediaStore.Images.Media.EXTERNAL_CONTENT_URI
                                )
                                imagePicker.launch(intent)
                            },
                        contentAlignment = Alignment.Center
                    ) {
                        Icon(
                            painter = painterResource(id = R.drawable.vectoradd),
                            contentDescription = "Add photo",
                            tint = Color.Black,
                            modifier = Modifier.size(18.dp)
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(60.dp))

            // Nút Apply
            Button(
                onClick = {
                    viewModel.updateProfile(
                        name = name,
                        imageUri = selectedImageUri,
                        context = context
                    )
                },
                colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF0057FF)),
                shape = RoundedCornerShape(12.dp),
                modifier = Modifier
                    .fillMaxWidth(0.85f)
                    .height(50.dp)
            ) {
                Text("Apply", fontSize = 18.sp, fontWeight = FontWeight.Medium)
            }
        }
    }
}
