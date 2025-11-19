package com.example.mychatapp.data

import com.example.mychatapp.model.SelfProfile


/**
 * ==========================================
 * SAMPLE DATA: SelfProfile (mô phỏng dữ liệu người dùng hiện tại)
 * ==========================================
 *
 * ✅ LUỒNG DỮ LIỆU THẬT KHI LOGIN:
 * 1. Người dùng đăng nhập (POST /auth/login)
 *    -> Backend kiểm tra thông tin trong database
 *    -> Nếu hợp lệ: trả về JSON chứa thông tin user + token
 *
 * 2. Ứng dụng nhận dữ liệu đó, lưu vào local database (Room / DataStore):
 *    - userId
 *    - name
 *    - phoneNumber (nên lưu ở dạng E.164, ví dụ: +62130917101920)
 *    - imageUrl
 *    - bio
 *    - authToken
 *
 * 3. Khi người dùng mở màn hình “More” (OnboardingSelfProfile):
 *    - ViewModel.loadSelfProfile():
 *         (a) Lấy dữ liệu từ local DB hiển thị nhanh
 *         (b) Gọi API GET /user/self (Authorization: Bearer <token>)
 *             -> Backend truy xuất database -> trả về user JSON
 *         (c) Cập nhật lại local DB & StateFlow
 *         (d) UI tự động hiển thị thông tin mới nhất
 *
 * ✅ API MẪU (backend):
 *    GET /user/self
 *    Headers: Authorization: Bearer <token>
 *
 *    RESPONSE:
 *    {
 *       "id": "self_01",
 *       "name": "Almayra Zamzamy",
 *       "phoneNumber": "+62130917101920",
 *       "imageUrl": "https://cdn.example.com/avatar/almayra.jpg",
 *       "bio": "Loves design and cats 🐱"
 *    }
 *
 * ✅ GHI NHỚ:
 * - Lưu số điện thoại chuẩn quốc tế (E.164) để CountryCodePicker hiển thị đúng.
 * - Token nên được lưu an toàn (EncryptedSharedPreferences hoặc DataStore).
 * - Khi offline, app vẫn hiển thị dữ liệu local.
 */

val sampleSelfProfile = SelfProfile(
    id = "self_001",
    name = "Huy Trương",
    phoneNumber = "+84886828499", // 🇻🇳 Số điện thoại định dạng chuẩn quốc tế
    imageUrl = null,
    bio = ""
)
