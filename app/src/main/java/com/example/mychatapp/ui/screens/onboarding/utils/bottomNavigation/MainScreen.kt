package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation

import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.NavController
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.addFriendScreen.OnboardingAddFriend
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.addFriendScreen.OnboardingContacts
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.addFriendScreen.friendInformation.FriendInformationScreen
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.chats.OnboardingChatDetail
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.chats.OnboardingChats
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.mores.OnboardingSelfProfile
import com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.mores.account.ProfileSelfAccount

@Composable
fun MainScreen(navController: NavController) {
    // ⚠️ Tạo NavController riêng cho bottom bar
    val bottomNavController = rememberNavController()

    val navBackStackEntry by bottomNavController.currentBackStackEntryAsState()
    val currentRoute = navBackStackEntry?.destination?.route

    Scaffold(
        bottomBar = {
            if (currentRoute != "addFriend" && currentRoute?.startsWith("chat_detail") != true) {
                BottomNavigationBar(bottomNavController)
            }
        }
    ) { innerPadding ->
        NavHost(
            navController = bottomNavController,
            startDestination = "contacts",
            modifier = Modifier.padding(innerPadding)
        ) {
            composable("contacts") {
                OnboardingContacts(bottomNavController)
            }
            composable("messages") {
                OnboardingChats(bottomNavController)
            }
            composable("profile") {
                OnboardingSelfProfile(
                    mainNavController = navController,
                    bottomNavController = bottomNavController
                )
            }
            composable("addFriend") {
                OnboardingAddFriend(bottomNavController)
            }
            composable("friendInformation/{friendPhoneNumber}") { backStackEntry ->
                val phone = backStackEntry.arguments?.getString("friendPhoneNumber") ?: ""
                FriendInformationScreen(navController = bottomNavController, friendPhoneNumber = phone)
            }
            composable("chats") { OnboardingChats(navController) }
            composable(
                route = "chat_detail/{friendId}/{friendName}",
                arguments = listOf(
                    navArgument("friendId") { type = NavType.StringType },
                    navArgument("friendName") { type = NavType.StringType }
                )
            ) { backStackEntry ->
                // 🧩 Lấy dữ liệu truyền qua NavController
                val friendId = backStackEntry.arguments?.getString("friendId") ?: ""
                val friendName = backStackEntry.arguments?.getString("friendName") ?: ""

                // Vì ContactRepository.currentUserId là Int, cần toString() nếu tham số yêu cầu String
                val myId = com.example.mychatapp.model.viewModel.ContactRepository.currentUserId.toString()

                OnboardingChatDetail(
                    navController = bottomNavController,
                    senderId = "1", // TODO: sau này thay bằng userId từ session login thật
                    receiverId = friendId,
                    receiverName = friendName
                )
            }
            //selfAccount
            composable("selfAccount"){
                ProfileSelfAccount(bottomNavController)
            }

        }
    }
}