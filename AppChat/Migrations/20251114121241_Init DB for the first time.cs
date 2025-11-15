using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppChat.Migrations
{
    /// <inheritdoc />
    public partial class InitDBforthefirsttime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAId = table.Column<int>(type: "int", nullable: false),
                    UserBId = table.Column<int>(type: "int", nullable: false),
                    LastMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastMessageTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnreadCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdContactA = table.Column<int>(type: "int", nullable: false),
                    UserIdContactB = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Id", "LastMessage", "LastMessageTime", "UnreadCount", "UserAId", "UserBId" },
                values: new object[,]
                {
                    { 1, "Gặp sau nha 👋", new DateTime(2025, 10, 28, 10, 44, 0, 0, DateTimeKind.Unspecified), 0, 1, 2 },
                    { 2, "Ok mai gặp!", new DateTime(2025, 10, 27, 21, 27, 0, 0, DateTimeKind.Unspecified), 0, 1, 3 },
                    { 3, "Đi xem phim nhé 🎬", new DateTime(2025, 10, 26, 20, 16, 0, 0, DateTimeKind.Unspecified), 0, 1, 4 },
                    { 4, "Gửi file rồi đó!", new DateTime(2025, 10, 25, 18, 20, 0, 0, DateTimeKind.Unspecified), 0, 1, 5 },
                    { 5, "Cảm ơn nha 😁", new DateTime(2025, 10, 25, 19, 24, 0, 0, DateTimeKind.Unspecified), 1, 2, 6 },
                    { 6, "Tối nhớ học bài 😆", new DateTime(2025, 10, 28, 8, 14, 0, 0, DateTimeKind.Unspecified), 0, 3, 7 },
                    { 7, "Mai gặp nhé 💕", new DateTime(2025, 10, 27, 9, 0, 0, 0, DateTimeKind.Unspecified), 0, 4, 8 },
                    { 8, "Haha vui quá 😁", new DateTime(2025, 10, 27, 18, 45, 0, 0, DateTimeKind.Unspecified), 0, 5, 7 },
                    { 9, "Ok bro 😎", new DateTime(2025, 10, 28, 22, 15, 0, 0, DateTimeKind.Unspecified), 0, 6, 8 },
                    { 10, "Đi học thôi!", new DateTime(2025, 10, 28, 9, 18, 0, 0, DateTimeKind.Unspecified), 0, 2, 5 }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "UserIdContactA", "UserIdContactB" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 2, 1, 3 },
                    { 3, 1, 4 },
                    { 4, 1, 5 },
                    { 5, 1, 6 },
                    { 6, 2, 1 },
                    { 7, 2, 3 },
                    { 8, 2, 5 },
                    { 9, 2, 6 },
                    { 10, 2, 7 },
                    { 11, 3, 1 },
                    { 12, 3, 2 },
                    { 13, 3, 4 },
                    { 14, 3, 7 },
                    { 15, 3, 8 },
                    { 16, 4, 1 },
                    { 17, 4, 3 },
                    { 18, 4, 5 },
                    { 19, 4, 6 },
                    { 20, 4, 8 },
                    { 21, 5, 1 },
                    { 22, 5, 2 },
                    { 23, 5, 4 },
                    { 24, 5, 6 },
                    { 25, 5, 7 },
                    { 26, 6, 1 },
                    { 27, 6, 2 },
                    { 28, 6, 4 },
                    { 29, 6, 5 },
                    { 30, 6, 8 },
                    { 31, 7, 2 },
                    { 32, 7, 3 },
                    { 33, 7, 5 },
                    { 34, 7, 8 },
                    { 35, 7, 1 },
                    { 36, 8, 3 },
                    { 37, 8, 4 },
                    { 38, 8, 6 },
                    { 39, 8, 7 },
                    { 40, 8, 1 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "Content", "FileType", "FileUrl", "SenderId", "SentAt", "Status" },
                values: new object[,]
                {
                    { 1, 1, "Chào Phuc, hôm nay rảnh không?", null, null, 1, new DateTime(2025, 10, 25, 9, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 2, 1, "Rảnh nè, đi uống cà phê không ☕", null, null, 2, new DateTime(2025, 10, 25, 9, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 3, 1, "Ok, quán cũ nha!", null, null, 1, new DateTime(2025, 10, 25, 9, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 4, 1, "Tới luôn 😁", null, null, 2, new DateTime(2025, 10, 25, 9, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 5, 1, "Đang đi nè 🚶‍♂️", null, null, 1, new DateTime(2025, 10, 25, 9, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 6, 1, "Tới rồi nha", null, null, 2, new DateTime(2025, 10, 25, 9, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 7, 1, "Thấy rồi 👋", null, null, 1, new DateTime(2025, 10, 25, 9, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 8, 1, "Uống gì đây?", null, null, 2, new DateTime(2025, 10, 25, 9, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 9, 1, "Cho ly đen đá 😆", null, null, 1, new DateTime(2025, 10, 25, 9, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 10, 2, "Khoa ơi, API chạy chưa?", null, null, 3, new DateTime(2025, 10, 26, 9, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 11, 2, "Chạy rồi, đang test thêm chút!", null, null, 1, new DateTime(2025, 10, 26, 9, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 12, 2, "Good job 💪", null, null, 3, new DateTime(2025, 10, 26, 9, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 13, 2, "Cảm ơn nhé 😎", null, null, 1, new DateTime(2025, 10, 26, 9, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 14, 2, "Mai review nha", null, null, 3, new DateTime(2025, 10, 26, 9, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 15, 2, "Ok deal!", null, null, 1, new DateTime(2025, 10, 26, 9, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 16, 2, "Nhớ đem laptop 😅", null, null, 3, new DateTime(2025, 10, 26, 9, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 17, 2, "Haha tất nhiên rồi", null, null, 1, new DateTime(2025, 10, 26, 9, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 18, 2, "Ok, gặp sáng mai nhé 👋", null, null, 3, new DateTime(2025, 10, 26, 9, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 19, 3, "Cuối tuần đi xem phim nhé 🎬", null, null, 4, new DateTime(2025, 10, 26, 20, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 20, 3, "Phim gì vậy?", null, null, 1, new DateTime(2025, 10, 26, 20, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 21, 3, "Marvel mới ra đó 😁", null, null, 4, new DateTime(2025, 10, 26, 20, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 22, 3, "Ok, đặt vé nhé", null, null, 1, new DateTime(2025, 10, 26, 20, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 23, 3, "Đặt 2 vé rồi nha ❤️", null, null, 4, new DateTime(2025, 10, 26, 20, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 24, 3, "Nice! hẹn gặp 😄", null, null, 1, new DateTime(2025, 10, 26, 20, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 25, 3, "Mai 7h nhé!", null, null, 4, new DateTime(2025, 10, 26, 20, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 26, 3, "Okeee 😎", null, null, 1, new DateTime(2025, 10, 26, 20, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 27, 3, "Ngủ sớm nha 😆", null, null, 4, new DateTime(2025, 10, 26, 20, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 28, 4, "Khoa gửi tài liệu chưa?", null, null, 5, new DateTime(2025, 10, 25, 18, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 29, 4, "Đang gửi nè 📎", null, null, 1, new DateTime(2025, 10, 25, 18, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 30, 4, "Ok thấy rồi", null, null, 5, new DateTime(2025, 10, 25, 18, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 31, 4, "Check giúp nha", null, null, 1, new DateTime(2025, 10, 25, 18, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 32, 4, "Ổn hết 👍", null, null, 5, new DateTime(2025, 10, 25, 18, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 33, 4, "Tốt quá 😁", null, null, 1, new DateTime(2025, 10, 25, 18, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 34, 4, "Mai nộp nha", null, null, 5, new DateTime(2025, 10, 25, 18, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 35, 4, "Ok!", null, null, 1, new DateTime(2025, 10, 25, 18, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 36, 4, "Thanks bro 😆", null, null, 5, new DateTime(2025, 10, 25, 18, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 37, 5, "Tuan ơi mai học môn gì nhỉ?", null, null, 2, new DateTime(2025, 10, 25, 19, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 38, 5, "Toán cao cấp đó 😅", null, null, 6, new DateTime(2025, 10, 25, 19, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 39, 5, "Ôi lại nữa à", null, null, 2, new DateTime(2025, 10, 25, 19, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 40, 5, "Chuẩn bị bài kỹ nha!", null, null, 6, new DateTime(2025, 10, 25, 19, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 41, 5, "Ok, tối nay làm bài chung nhé", null, null, 2, new DateTime(2025, 10, 25, 19, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 42, 5, "Được luôn 😎", null, null, 6, new DateTime(2025, 10, 25, 19, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 43, 5, "8h call nhé", null, null, 2, new DateTime(2025, 10, 25, 19, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 44, 5, "Ok deal!", null, null, 6, new DateTime(2025, 10, 25, 19, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 45, 5, "Cảm ơn nha 😁", null, null, 2, new DateTime(2025, 10, 25, 19, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 46, 6, "Han ơi làm bài tập xong chưa?", null, null, 3, new DateTime(2025, 10, 27, 8, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 47, 6, "Sắp xong rồi 😅", null, null, 7, new DateTime(2025, 10, 27, 8, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 48, 6, "Cần giúp không?", null, null, 3, new DateTime(2025, 10, 27, 8, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 49, 6, "Có chứ 😁", null, null, 7, new DateTime(2025, 10, 27, 8, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 50, 6, "Ok để mình gọi nha", null, null, 3, new DateTime(2025, 10, 27, 8, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 51, 6, "Cảm ơn nha 😍", null, null, 7, new DateTime(2025, 10, 27, 8, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 52, 6, "Không có chi 😉", null, null, 3, new DateTime(2025, 10, 27, 8, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 53, 6, "Tối gặp nhé", null, null, 7, new DateTime(2025, 10, 27, 8, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 54, 6, "Ok luôn 😆", null, null, 3, new DateTime(2025, 10, 27, 8, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 55, 7, "Nam ơi mai có họp nhóm không?", null, null, 4, new DateTime(2025, 10, 27, 9, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 56, 7, "Có nha, 9h bắt đầu", null, null, 8, new DateTime(2025, 10, 27, 9, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 57, 7, "Ok, chuẩn bị tài liệu nhé", null, null, 4, new DateTime(2025, 10, 27, 9, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 58, 7, "Mình lo phần đó rồi 😎", null, null, 8, new DateTime(2025, 10, 27, 9, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 59, 7, "Good, cảm ơn nha ❤️", null, null, 4, new DateTime(2025, 10, 27, 9, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 60, 7, "Không có gì 😄", null, null, 8, new DateTime(2025, 10, 27, 9, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 61, 7, "Mai gặp nhé 👋", null, null, 4, new DateTime(2025, 10, 27, 9, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 62, 7, "See you!", null, null, 8, new DateTime(2025, 10, 27, 9, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 63, 7, "Nhớ đem laptop nha 💻", null, null, 4, new DateTime(2025, 10, 27, 9, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 64, 8, "Han ơi bài tập nhóm sao rồi?", null, null, 5, new DateTime(2025, 10, 27, 18, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 65, 8, "Gần xong rồi 😁", null, null, 7, new DateTime(2025, 10, 27, 18, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 66, 8, "Cần mình giúp gì không?", null, null, 5, new DateTime(2025, 10, 27, 18, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 67, 8, "Ok gửi phần cậu qua nhé", null, null, 7, new DateTime(2025, 10, 27, 18, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 68, 8, "Gửi rồi đó", null, null, 5, new DateTime(2025, 10, 27, 18, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 69, 8, "Nhận được rồi ✅", null, null, 7, new DateTime(2025, 10, 27, 18, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 70, 8, "Tối call nhé", null, null, 5, new DateTime(2025, 10, 27, 18, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 71, 8, "Ok deal 😆", null, null, 7, new DateTime(2025, 10, 27, 18, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 72, 8, "Cảm ơn nha", null, null, 5, new DateTime(2025, 10, 27, 18, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 73, 9, "Nam ơi tối chơi game không?", null, null, 6, new DateTime(2025, 10, 28, 21, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 74, 9, "Có luôn bro 😎", null, null, 8, new DateTime(2025, 10, 28, 21, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 75, 9, "Rank hay thường?", null, null, 6, new DateTime(2025, 10, 28, 21, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 76, 9, "Rank chứ 😁", null, null, 8, new DateTime(2025, 10, 28, 21, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 77, 9, "Ok vào phòng nhé", null, null, 6, new DateTime(2025, 10, 28, 21, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 78, 9, "Tới liền 🔥", null, null, 8, new DateTime(2025, 10, 28, 21, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 79, 9, "GG ez 😆", null, null, 6, new DateTime(2025, 10, 28, 21, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 80, 9, "Haha vui quá 😂", null, null, 8, new DateTime(2025, 10, 28, 21, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 81, 9, "Mai nữa nha!", null, null, 6, new DateTime(2025, 10, 28, 21, 40, 0, 0, DateTimeKind.Unspecified), "" },
                    { 82, 10, "Bao ơi đi học chưa?", null, null, 2, new DateTime(2025, 10, 28, 8, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 83, 10, "Sắp tới rồi 😆", null, null, 5, new DateTime(2025, 10, 28, 8, 5, 0, 0, DateTimeKind.Unspecified), "" },
                    { 84, 10, "Nhớ mang slide nha", null, null, 2, new DateTime(2025, 10, 28, 8, 10, 0, 0, DateTimeKind.Unspecified), "" },
                    { 85, 10, "Ok mình in rồi ✅", null, null, 5, new DateTime(2025, 10, 28, 8, 15, 0, 0, DateTimeKind.Unspecified), "" },
                    { 86, 10, "Tốt quá!", null, null, 2, new DateTime(2025, 10, 28, 8, 20, 0, 0, DateTimeKind.Unspecified), "" },
                    { 87, 10, "Hẹn ở lớp nha", null, null, 5, new DateTime(2025, 10, 28, 8, 25, 0, 0, DateTimeKind.Unspecified), "" },
                    { 88, 10, "Ok đi liền đây 🚶‍♂️", null, null, 2, new DateTime(2025, 10, 28, 8, 30, 0, 0, DateTimeKind.Unspecified), "" },
                    { 89, 10, "Thấy rồi 👋", null, null, 5, new DateTime(2025, 10, 28, 8, 35, 0, 0, DateTimeKind.Unspecified), "" },
                    { 90, 10, "Đi học thôi 😁", null, null, 2, new DateTime(2025, 10, 28, 8, 40, 0, 0, DateTimeKind.Unspecified), "" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "FirstName", "IsOnline", "LastName", "LastSeen", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "", "Dang", true, "Khoa", new DateTime(2025, 11, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "0901000001" },
                    { 2, "", "Minh", false, "Phuc", new DateTime(2025, 11, 3, 11, 55, 0, 0, DateTimeKind.Unspecified), "0901000002" },
                    { 3, "", "Hai", false, "Dang", new DateTime(2025, 11, 3, 11, 50, 0, 0, DateTimeKind.Unspecified), "0901000003" },
                    { 4, "", "Thao", true, "Nguyen", new DateTime(2025, 11, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "0901000004" },
                    { 5, "", "Bao", false, "Tran", new DateTime(2025, 11, 3, 11, 0, 0, 0, DateTimeKind.Unspecified), "0901000005" },
                    { 6, "", "Anh", true, "Tuan", new DateTime(2025, 11, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "0901000006" },
                    { 7, "", "Ngoc", false, "Han", new DateTime(2025, 11, 3, 11, 30, 0, 0, DateTimeKind.Unspecified), "0901000007" },
                    { 8, "", "Le", false, "Nam", new DateTime(2025, 11, 3, 11, 45, 0, 0, DateTimeKind.Unspecified), "0901000008" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
