package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.mores

import android.widget.Toast
import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import coil.compose.rememberAsyncImagePainter
import com.example.mychatapp.model.SelfProfileViewModel
import com.hbb20.CountryCodePicker
import androidx.compose.ui.viewinterop.AndroidView
import androidx.navigation.NavController
import com.example.mychatapp.ui.screens.onboarding.utils.SessionManager
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OnboardingSelfProfile(
    mainNavController: NavController,
    bottomNavController: NavController,
    viewModel: SelfProfileViewModel = viewModel(),
    onBackClick: (() -> Unit)? = null,
    // navigation callbacks cho từng mục
    onAccountClick: () -> Unit = {},
    onPrivacyClick: () -> Unit = {},
    onHelpClick: () -> Unit = {}
) {
    val profile by viewModel.uiState.collectAsState()
    val context = LocalContext.current

    val sessionManager = remember { SessionManager(context) }
    val coroutineScope = rememberCoroutineScope()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Profile", fontSize = 20.sp) },
                navigationIcon = {
                    onBackClick?.let {
                        IconButton(onClick = it) {
                            Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                        }
                    }
                }
            )
        }
    ) { innerPadding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(innerPadding)
                .padding(horizontal = 20.dp)
        ) {
            // Box hiển thị thông tin bản thân (avatar, tên, phone)
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(vertical = 16.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                // Avatar
                Image(
                    painter = rememberAsyncImagePainter(profile.imageUrl ?: ""),
                    contentDescription = "Avatar",
                    modifier = Modifier
                        .size(60.dp)
                        .clip(CircleShape)
                )

                Spacer(modifier = Modifier.width(16.dp))

                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = profile.name,
                        style = MaterialTheme.typography.titleMedium
                    )

                    Spacer(modifier = Modifier.height(6.dp))

                    // CountryCodePicker để hiển thị cờ và định dạng số điện thoại
                    AndroidView(
                        factory = { ctx ->
                            CountryCodePicker(ctx).apply {
                                setAutoDetectedCountry(true)
                                fullNumber = profile.phoneNumber
                                isClickable = false
                                setCcpClickable(false)
                            }
                        },
                        modifier = Modifier
                            .height(40.dp)
                    )
                }
            }

            HorizontalDivider()

            Spacer(modifier = Modifier.height(8.dp))

            // Gọi component navigation list (ProfileNavigation.kt)
            ProfileNavigationList(
                onAccountClick = {
                    bottomNavController.navigate("selfAccount")
                    onAccountClick()
                },
                onPrivacyClick = { onPrivacyClick() },
                onHelpClick = {
                    Toast.makeText(context, "Open Help", Toast.LENGTH_SHORT).show()
                    onHelpClick()
                },
                onLogoutClick = {
                    coroutineScope.launch {
                        // 1. Xóa session
                        sessionManager.logout()

                        // 2. Dùng mainNavController để quay về Onboarding
                        mainNavController.navigate("onboarding") {
                            // Xóa toàn bộ back stack
                            popUpTo(0) { inclusive = true }
                            launchSingleTop = true
                        }
                    }
                }
            )
        }
    }
}
