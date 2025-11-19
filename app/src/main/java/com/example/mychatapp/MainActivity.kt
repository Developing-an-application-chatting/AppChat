package com.example.mychatapp

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.mychatapp.ui.screens.onboarding.AuthenticationEnterCode
import com.example.mychatapp.ui.screens.onboarding.LoginUserProfileScreen
import com.example.mychatapp.ui.screens.onboarding.OnboardingScreen
import com.example.mychatapp.ui.screens.onboarding.PhoneNumberAuthentication
import com.example.mychatapp.ui.screens.onboarding.utils.SessionManager
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.MainScreen
import com.example.mychatapp.ui.theme.MyChatAppTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MyChatAppTheme {
                Surface (
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ){
                    MyChatApp()
                }
            }
        }
    }
}

@Composable
fun MyChatApp() {
    val context = LocalContext.current
    // 1. Khởi tạo SessionManager
    val sessionManager = SessionManager(context)

    // 2. Lắng nghe trạng thái đăng nhập từ DataStore
    // initial = null nghĩa là "chưa biết", đang load
    val isUserLoggedIn by sessionManager.isLoggedIn.collectAsState(initial = null)

    // 3. Xử lý hiển thị dựa trên trạng thái load
    when (isUserLoggedIn) {
        null -> {
            // TRẠNG THÁI CHỜ: Hiển thị vòng xoay loading hoặc màn hình trắng có logo
            // Điều này ngăn NavHost khởi chạy sai đường dẫn
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
        }
        else -> {
            // KHI ĐÃ CÓ DỮ LIỆU: Khởi tạo Navigation
            // Nếu loggedIn = true -> vào mainScreen
            // Nếu loggedIn = false -> vào onboarding
            val startRoute = if (isUserLoggedIn == true) "mainScreen" else "onboarding"

            MainNavigation(startDestination = startRoute)
        }
    }
}

@Composable
fun MainNavigation(startDestination:String) {
val navController = rememberNavController()
    NavHost(
       navController = navController,
            //startDestination = "onboarding"
       startDestination = startDestination

    ) {
            composable("onboarding") {
                OnboardingScreen(navController)
            }
            composable ("PhoneNumber"){
                PhoneNumberAuthentication(navController)
            }
            composable (
                route = "NumberCode/{phoneNumber}/{verificationId}",
                arguments = listOf(
                    navArgument("phoneNumber") { type = NavType.StringType },
                    navArgument("verificationId") { type = NavType.StringType }
                    )
            ){ backStackEntry ->
                val phoneNumber = backStackEntry.arguments?.getString("phoneNumber") ?: ""
                val verificationId = backStackEntry.arguments?.getString("verificationId") ?: ""

                AuthenticationEnterCode(navController, phoneNumber,verificationId)
            }
            composable("UserProfile/{phoneNumber}") { backStackEntry ->
                val phoneNumber = backStackEntry.arguments?.getString("phoneNumber") ?: ""
                LoginUserProfileScreen(
                    navController = navController,
                    phoneNumber = phoneNumber
                )
            }
            composable("mainScreen"){
                MainScreen(navController)
            }
            composable("home") {
                OnboardingScreen(navController)
            }
        }
    }


