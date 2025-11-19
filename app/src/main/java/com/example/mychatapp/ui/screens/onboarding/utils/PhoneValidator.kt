package com.example.mychatapp.ui.screens.onboarding.utils

object PhoneValidator {

    /**
     * Kiểm tra định dạng cơ bản của số điện thoại.
     * Trả về thông báo lỗi nếu không hợp lệ, null nếu hợp lệ.
     */
    fun validatePhoneNumber(phone: String, countryCode: String?): String? {
        val trimmed = phone.trim()

        // Kiểm tra trống
        if (trimmed.isEmpty()) {
            return "Vui lòng nhập số điện thoại"
        }

        // Kiểm tra có chọn mã quốc gia
        if (countryCode.isNullOrEmpty()) {
            return "Vui lòng chọn mã quốc gia"
        }

        // Kiểm tra chỉ chứa chữ số
        if (!trimmed.all { it.isDigit() }) {
            return "Số điện thoại chỉ được chứa chữ số"
        }

        // Kiểm tra độ dài (ví dụ VN thường 9–10 số)
        if (trimmed.length < 9 || trimmed.length > 11) {
            return "Số điện thoại không hợp lệ (phải có từ 9–11 số)"
        }

        // Kiểm tra trùng ký tự (ví dụ 1111111111)
        if (trimmed.toSet().size == 1) {
            return "Số điện thoại không hợp lệ (các số không thể giống nhau)"
        }

        return null // hợp lệ
    }
}
