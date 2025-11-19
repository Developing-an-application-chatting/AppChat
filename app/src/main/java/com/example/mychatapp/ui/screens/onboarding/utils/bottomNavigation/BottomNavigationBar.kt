package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.Message
import androidx.compose.material.icons.filled.AccountCircle
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavHostController
import androidx.navigation.compose.currentBackStackEntryAsState
import com.example.mychatapp.R

@Composable
fun BottomNavigationBar(navController: NavHostController) {
    val bottomNavItems = listOf(
        BottomNavItem(
            title = "Contacts",
            selectedIcon = Icons.Filled.Person,
            unselectedIconRes = R.drawable.contact,
            route = "contacts"
        ),
        BottomNavItem(
            title = "Messages",
            selectedIcon = Icons.AutoMirrored.Filled.Message,
            unselectedIconRes = R.drawable.chat,
            route = "messages"
        ),
        BottomNavItem(
            title = "Profile",
            selectedIcon = Icons.Filled.AccountCircle,
            unselectedIconRes = R.drawable.profile,
            route = "profile"
        )
    )

    val navBackStackEntry = navController.currentBackStackEntryAsState()
    val currentRoute = navBackStackEntry.value?.destination?.route

    Box(
        modifier = Modifier
            .fillMaxWidth()
            .background(Color.White)
            .border(width = 0.6.dp, color = Color(0xFFE0E0E0)) // đường phân cách mờ phía trên
            .padding(vertical = 8.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(horizontal = 32.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            bottomNavItems.forEach { item ->
                val isSelected = currentRoute == item.route

                Column(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    verticalArrangement = Arrangement.Center,
                    modifier = Modifier
                        .clip(CircleShape)
                        .clickable(
                            indication = null, // bỏ hiệu ứng ripple tím mặc định
                            interactionSource = remember { MutableInteractionSource() }
                        ) {
                            navController.navigate(item.route) {
                                popUpTo(navController.graph.startDestinationId) { saveState = true }
                                launchSingleTop = true
                                restoreState = true
                            }
                        }
                        .background(
                            if (isSelected) Color.Transparent
                            else Color.Transparent
                        )
                        .padding(8.dp)
                ) {
                    // Icon
                    Icon(
                        painter = painterResource(
                            id = if (isSelected)
                                item.selectedIconRes ?: item.unselectedIconRes!!
                            else
                                item.unselectedIconRes!!
                        ),
                        contentDescription = item.title,
                        tint = Color(0xFF1E1E1E),
                        modifier = Modifier.size(26.dp)
                    )

                    // Chỉ hiển thị text + chấm khi được chọn
                    if (isSelected) {
                        Spacer(modifier = Modifier.height(2.dp))
                        Text(
                            text = item.title,
                            color = Color(0xFF1E1E1E),
                            fontSize = 12.sp
                        )
                        Spacer(modifier = Modifier.height(2.dp))
                        Box(
                            modifier = Modifier
                                .size(5.dp)
                                .clip(CircleShape)
                                .background(Color(0xFF1E1E1E))
                        )
                    }
                }
            }
        }
    }
}