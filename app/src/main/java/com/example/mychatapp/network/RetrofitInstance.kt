package com.example.mychatapp.network

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitInstance {

    /**
     * ⚠️ RẤT QUAN TRỌNG ⚠️
     * Thay thế địa chỉ IP này bằng địa chỉ IP MÁY TÍNH
     * đang chạy server C# của bạn.
     *
     * (Xem hướng dẫn bên dưới)
     */
    private const val BASE_URL = "http://192.168.242.221:5047/"

    // Khởi tạo Retrofit (chỉ một lần)
    private val retrofit by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create()) // Dùng Gson để "dịch" JSON
            .build()
    }

    /**
     * Cung cấp một phiên bản (instance) của ApiService
     * để các Repository có thể sử dụng.
     */
    val api: ApiService by lazy {
        retrofit.create(ApiService::class.java)
    }
}