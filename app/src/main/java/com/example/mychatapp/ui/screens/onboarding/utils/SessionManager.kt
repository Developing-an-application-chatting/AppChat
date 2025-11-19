package com.example.mychatapp.ui.screens.onboarding.utils

import android.content.Context
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

val Context.dataStore by preferencesDataStore("user_prefs")

class SessionManager(private val context: Context) {

    companion object {
        val IS_LOGGED_IN = booleanPreferencesKey("is_logged_in")
        val USER_ID = stringPreferencesKey("user_id")
        val USER_NAME = stringPreferencesKey("user_name")
        val USER_TOKEN = stringPreferencesKey("user_token")
        val USER_AVATAR = stringPreferencesKey("user_avatar") // nếu cần lưu avatar
    }

    // Luồng (Flow) cho trạng thái đăng nhập
    val isLoggedIn: Flow<Boolean> = context.dataStore.data.map {
        it[IS_LOGGED_IN] ?: false
    }

    val userId: Flow<String?> = context.dataStore.data.map { it[USER_ID] }
    val userName: Flow<String?> = context.dataStore.data.map { it[USER_NAME] }
    val userToken: Flow<String?> = context.dataStore.data.map { it[USER_TOKEN] }
    val userAvatar: Flow<String?> = context.dataStore.data.map { it[USER_AVATAR] }

    /**
     * Lưu toàn bộ phiên đăng nhập
     * @param avatarUrl nếu chưa có, truyền ""
     */
    suspend fun saveUserSession(id: String, name: String, token: String, avatarUrl: String = "") {
        context.dataStore.edit { prefs ->
            prefs[IS_LOGGED_IN] = true
            prefs[USER_ID] = id
            prefs[USER_NAME] = name
            prefs[USER_TOKEN] = token
            prefs[USER_AVATAR] = avatarUrl
        }
    }

    /**
     * Đăng xuất
     */
    suspend fun logout() {
        context.dataStore.edit { prefs ->
            prefs.clear()
        }
    }

    /**
     * Cập nhật trạng thái đăng nhập
     */
    suspend fun setLoggedIn(value: Boolean) {
        context.dataStore.edit { prefs ->
            prefs[IS_LOGGED_IN] = value
        }
    }
}
