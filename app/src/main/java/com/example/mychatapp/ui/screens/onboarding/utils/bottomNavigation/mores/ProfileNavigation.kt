package com.example.mychatapp.ui.screens.onboarding.utils.bottomNavigation.mores

import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.Icons.Default
import androidx.compose.material.icons.automirrored.filled.Help
import androidx.compose.material.icons.automirrored.filled.Logout
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.mychatapp.R

/**
 * Component chứa danh sách navigation item dùng cho màn "More"
 * Có thể tái sử dụng trong OnboardingSelfProfile và OnboardingProfile.
 *
 * Mỗi item truyền callback onClick để NavHost xử lý điều hướng thực tế.
 */

@Composable
fun ProfileNavigationList(
    onAccountClick: () -> Unit,
    onPrivacyClick: () -> Unit,
    onHelpClick: () -> Unit,
    onLogoutClick: () -> Unit
) {
    Column(
        modifier = Modifier.fillMaxWidth(),
        verticalArrangement = Arrangement.spacedBy(6.dp)
    ) {
        //them logo anh
        ProfileNavItem(icon = Default.Person, title = "Account", onClick = onAccountClick)
        ProfileNavItem(icon = Default.Security, title = "Privacy", onClick = onPrivacyClick)
        HorizontalDivider(modifier = Modifier.padding(vertical = 8.dp))
        ProfileNavItem(icon = Icons.AutoMirrored.Filled.Help, title = "Help", onClick = onHelpClick)

        HorizontalDivider(modifier = Modifier.padding(vertical = 8.dp))
        ProfileNavItem(
            icon = Icons.AutoMirrored.Filled.Logout,
            title = "Logout",
            onClick = onLogoutClick
        )
    }
}

@Composable
fun ProfileNavItem(
    icon: androidx.compose.ui.graphics.vector.ImageVector,
    title: String,
    onClick: () -> Unit
) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick)
            .padding(vertical = 14.dp, horizontal = 8.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Icon(icon, contentDescription = title, modifier = Modifier.size(24.dp))
        Spacer(modifier = Modifier.width(16.dp))
        Text(
            title,
            style = MaterialTheme.typography.bodyLarge.copy(fontSize = 17.sp),
            modifier = Modifier.weight(1f)
        )
        Image(
            painter = painterResource(id = R.drawable.vector1),
            contentDescription = "Arrow",
            modifier = Modifier.size(20.dp)
        )
    }
}
