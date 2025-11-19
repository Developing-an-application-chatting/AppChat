package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation

import androidx.compose.ui.graphics.vector.ImageVector
//import okhttp3.Route

//BottomNavItem in Communicate
data class BottomNavItem(
    val title: String,
    val selectedIconRes: Int? = null,
    val selectedIcon: ImageVector? = null,
    val unselectedIconRes: Int? = null,
    val route: String
)