package com.example.mychatapp.network.dto

import com.google.gson.annotations.SerializedName

/**
 * DTO (Data Transfer Object)
 * Nhận Token tùy chỉnh (C#) TỪ server.
 */
data class LoginResponseDto(
    @SerializedName("accessToken")
    val token: String,

    // Sửa "userId" -> "id" để khớp C#
    @SerializedName("id")
    val userId: String,

    // C# không gửi trường này, nên ta cho phép nó null (thêm ?)
    @SerializedName("name")
    val name: String?
    // Thêm bất kỳ thông tin nào khác mà C# trả về khi đăng nhập
)