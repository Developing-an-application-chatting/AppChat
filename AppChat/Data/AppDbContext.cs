using AppChat.Models;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Define DB sets for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        // Seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USERS =====
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Dang", LastName = "Khoa", PhoneNumber = "0901000001", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0, DateTimeKind.Utc) },
                new User { Id = 2, FirstName = "Phuoc", LastName = "Vo", PhoneNumber = "0792051912", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 55, 0, DateTimeKind.Utc) },
                new User { Id = 3, FirstName = "Huy", LastName = "Truong", PhoneNumber = "0886828499", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 50, 0, DateTimeKind.Utc) },
                new User { Id = 4, FirstName = "Thao", LastName = "Nguyen", PhoneNumber = "0901000004", Password = "123", AvatarUrl = "", IsOnline = true, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0, DateTimeKind.Utc) },
                new User { Id = 5, FirstName = "Bao", LastName = "Tran", PhoneNumber = "0901000005", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 0, 0, DateTimeKind.Utc) },
                new User { Id = 6, FirstName = "Anh", LastName = "Tuan", PhoneNumber = "0901000006", Password = "123", AvatarUrl = "", IsOnline = true, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0, DateTimeKind.Utc) },
                new User { Id = 7, FirstName = "Ngoc", LastName = "Han", PhoneNumber = "0901000007", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 30, 0, DateTimeKind.Utc) },
                new User { Id = 8, FirstName = "Le", LastName = "Nam", PhoneNumber = "0901000008", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 45, 0, DateTimeKind.Utc) },
                new User { Id = 9, FirstName = "Minh", LastName = "Phuc", PhoneNumber = "0901000009", Password = "123", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 45, 0, DateTimeKind.Utc) }

            );


            // ===== CONVERSATIONS =====
            // Mỗi user có 4–5 cuộc trò chuyện (với các bạn bè ở trên)
            modelBuilder.Entity<Chat>().HasData(
                new Chat { Id = 1, UserAId = 1, UserBId = 2, LastMessage = "Gặp sau nha 👋", LastMessageTime = new DateTime(2025, 10, 28, 10, 44, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 2, UserAId = 1, UserBId = 3, LastMessage = "Ok mai gặp!", LastMessageTime = new DateTime(2025, 10, 27, 21, 27, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 3, UserAId = 1, UserBId = 4, LastMessage = "Đi xem phim nhé 🎬", LastMessageTime = new DateTime(2025, 10, 26, 20, 16, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 4, UserAId = 1, UserBId = 5, LastMessage = "Gửi file rồi đó!", LastMessageTime = new DateTime(2025, 10, 25, 18, 20, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 5, UserAId = 2, UserBId = 6, LastMessage = "Cảm ơn nha 😁", LastMessageTime = new DateTime(2025, 10, 25, 19, 24, 0, DateTimeKind.Utc), UnreadCount = 1 },
                new Chat { Id = 6, UserAId = 3, UserBId = 7, LastMessage = "Tối nhớ học bài 😆", LastMessageTime = new DateTime(2025, 10, 28, 8, 14, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 7, UserAId = 4, UserBId = 8, LastMessage = "Mai gặp nhé 💕", LastMessageTime = new DateTime(2025, 10, 27, 9, 0, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 8, UserAId = 5, UserBId = 7, LastMessage = "Haha vui quá 😁", LastMessageTime = new DateTime(2025, 10, 27, 18, 45, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 9, UserAId = 6, UserBId = 8, LastMessage = "Ok bro 😎", LastMessageTime = new DateTime(2025, 10, 28, 22, 15, 0, DateTimeKind.Utc), UnreadCount = 0 },
                new Chat { Id = 10, UserAId = 2, UserBId = 5, LastMessage = "Đi học thôi!", LastMessageTime = new DateTime(2025, 10, 28, 9, 18, 0, DateTimeKind.Utc), UnreadCount = 0 }

            );



            // ===== MESSAGES =====
            modelBuilder.Entity<Message>().HasData(
                // === Chat 1: Khoa (1) - Phuc (2)
                new Message { Id = 1, ChatId = 1, SenderId = 1, Content = "Chào Phuc, hôm nay rảnh không?", SentAt = new DateTime(2025, 10, 25, 9, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 2, ChatId = 1, SenderId = 2, Content = "Rảnh nè, đi uống cà phê không ☕", SentAt = new DateTime(2025, 10, 25, 9, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 3, ChatId = 1, SenderId = 1, Content = "Ok, quán cũ nha!", SentAt = new DateTime(2025, 10, 25, 9, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 4, ChatId = 1, SenderId = 2, Content = "Tới luôn 😁", SentAt = new DateTime(2025, 10, 25, 9, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 5, ChatId = 1, SenderId = 1, Content = "Đang đi nè 🚶‍♂️", SentAt = new DateTime(2025, 10, 25, 9, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 6, ChatId = 1, SenderId = 2, Content = "Tới rồi nha", SentAt = new DateTime(2025, 10, 25, 9, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 7, ChatId = 1, SenderId = 1, Content = "Thấy rồi 👋", SentAt = new DateTime(2025, 10, 25, 9, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 8, ChatId = 1, SenderId = 2, Content = "Uống gì đây?", SentAt = new DateTime(2025, 10, 25, 9, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 9, ChatId = 1, SenderId = 1, Content = "Cho ly đen đá 😆", SentAt = new DateTime(2025, 10, 25, 9, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 2: Khoa (1) - Dang (3)
                new Message { Id = 10, ChatId = 2, SenderId = 3, Content = "Khoa ơi, API chạy chưa?", SentAt = new DateTime(2025, 10, 26, 9, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 11, ChatId = 2, SenderId = 1, Content = "Chạy rồi, đang test thêm chút!", SentAt = new DateTime(2025, 10, 26, 9, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 12, ChatId = 2, SenderId = 3, Content = "Good job 💪", SentAt = new DateTime(2025, 10, 26, 9, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 13, ChatId = 2, SenderId = 1, Content = "Cảm ơn nhé 😎", SentAt = new DateTime(2025, 10, 26, 9, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 14, ChatId = 2, SenderId = 3, Content = "Mai review nha", SentAt = new DateTime(2025, 10, 26, 9, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 15, ChatId = 2, SenderId = 1, Content = "Ok deal!", SentAt = new DateTime(2025, 10, 26, 9, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 16, ChatId = 2, SenderId = 3, Content = "Nhớ đem laptop 😅", SentAt = new DateTime(2025, 10, 26, 9, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 17, ChatId = 2, SenderId = 1, Content = "Haha tất nhiên rồi", SentAt = new DateTime(2025, 10, 26, 9, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 18, ChatId = 2, SenderId = 3, Content = "Ok, gặp sáng mai nhé 👋", SentAt = new DateTime(2025, 10, 26, 9, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 3: Khoa (1) - Thao (4)
                new Message { Id = 19, ChatId = 3, SenderId = 4, Content = "Cuối tuần đi xem phim nhé 🎬", SentAt = new DateTime(2025, 10, 26, 20, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 20, ChatId = 3, SenderId = 1, Content = "Phim gì vậy?", SentAt = new DateTime(2025, 10, 26, 20, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 21, ChatId = 3, SenderId = 4, Content = "Marvel mới ra đó 😁", SentAt = new DateTime(2025, 10, 26, 20, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 22, ChatId = 3, SenderId = 1, Content = "Ok, đặt vé nhé", SentAt = new DateTime(2025, 10, 26, 20, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 23, ChatId = 3, SenderId = 4, Content = "Đặt 2 vé rồi nha ❤️", SentAt = new DateTime(2025, 10, 26, 20, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 24, ChatId = 3, SenderId = 1, Content = "Nice! hẹn gặp 😄", SentAt = new DateTime(2025, 10, 26, 20, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 25, ChatId = 3, SenderId = 4, Content = "Mai 7h nhé!", SentAt = new DateTime(2025, 10, 26, 20, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 26, ChatId = 3, SenderId = 1, Content = "Okeee 😎", SentAt = new DateTime(2025, 10, 26, 20, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 27, ChatId = 3, SenderId = 4, Content = "Ngủ sớm nha 😆", SentAt = new DateTime(2025, 10, 26, 20, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 4: Khoa (1) - Bao (5)
                new Message { Id = 28, ChatId = 4, SenderId = 5, Content = "Khoa gửi tài liệu chưa?", SentAt = new DateTime(2025, 10, 25, 18, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 29, ChatId = 4, SenderId = 1, Content = "Đang gửi nè 📎", SentAt = new DateTime(2025, 10, 25, 18, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 30, ChatId = 4, SenderId = 5, Content = "Ok thấy rồi", SentAt = new DateTime(2025, 10, 25, 18, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 31, ChatId = 4, SenderId = 1, Content = "Check giúp nha", SentAt = new DateTime(2025, 10, 25, 18, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 32, ChatId = 4, SenderId = 5, Content = "Ổn hết 👍", SentAt = new DateTime(2025, 10, 25, 18, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 33, ChatId = 4, SenderId = 1, Content = "Tốt quá 😁", SentAt = new DateTime(2025, 10, 25, 18, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 34, ChatId = 4, SenderId = 5, Content = "Mai nộp nha", SentAt = new DateTime(2025, 10, 25, 18, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 35, ChatId = 4, SenderId = 1, Content = "Ok!", SentAt = new DateTime(2025, 10, 25, 18, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 36, ChatId = 4, SenderId = 5, Content = "Thanks bro 😆", SentAt = new DateTime(2025, 10, 25, 18, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 5: Phuc (2) - Tuan (6)
                new Message { Id = 37, ChatId = 5, SenderId = 2, Content = "Tuan ơi mai học môn gì nhỉ?", SentAt = new DateTime(2025, 10, 25, 19, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 38, ChatId = 5, SenderId = 6, Content = "Toán cao cấp đó 😅", SentAt = new DateTime(2025, 10, 25, 19, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 39, ChatId = 5, SenderId = 2, Content = "Ôi lại nữa à", SentAt = new DateTime(2025, 10, 25, 19, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 40, ChatId = 5, SenderId = 6, Content = "Chuẩn bị bài kỹ nha!", SentAt = new DateTime(2025, 10, 25, 19, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 41, ChatId = 5, SenderId = 2, Content = "Ok, tối nay làm bài chung nhé", SentAt = new DateTime(2025, 10, 25, 19, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 42, ChatId = 5, SenderId = 6, Content = "Được luôn 😎", SentAt = new DateTime(2025, 10, 25, 19, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 43, ChatId = 5, SenderId = 2, Content = "8h call nhé", SentAt = new DateTime(2025, 10, 25, 19, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 44, ChatId = 5, SenderId = 6, Content = "Ok deal!", SentAt = new DateTime(2025, 10, 25, 19, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 45, ChatId = 5, SenderId = 2, Content = "Cảm ơn nha 😁", SentAt = new DateTime(2025, 10, 25, 19, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 6: Dang (3) - Han (7)
                new Message { Id = 46, ChatId = 6, SenderId = 3, Content = "Han ơi làm bài tập xong chưa?", SentAt = new DateTime(2025, 10, 27, 8, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 47, ChatId = 6, SenderId = 7, Content = "Sắp xong rồi 😅", SentAt = new DateTime(2025, 10, 27, 8, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 48, ChatId = 6, SenderId = 3, Content = "Cần giúp không?", SentAt = new DateTime(2025, 10, 27, 8, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 49, ChatId = 6, SenderId = 7, Content = "Có chứ 😁", SentAt = new DateTime(2025, 10, 27, 8, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 50, ChatId = 6, SenderId = 3, Content = "Ok để mình gọi nha", SentAt = new DateTime(2025, 10, 27, 8, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 51, ChatId = 6, SenderId = 7, Content = "Cảm ơn nha 😍", SentAt = new DateTime(2025, 10, 27, 8, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 52, ChatId = 6, SenderId = 3, Content = "Không có chi 😉", SentAt = new DateTime(2025, 10, 27, 8, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 53, ChatId = 6, SenderId = 7, Content = "Tối gặp nhé", SentAt = new DateTime(2025, 10, 27, 8, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 54, ChatId = 6, SenderId = 3, Content = "Ok luôn 😆", SentAt = new DateTime(2025, 10, 27, 8, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 7: Thao (4) - Nam (8)
                new Message { Id = 55, ChatId = 7, SenderId = 4, Content = "Nam ơi mai có họp nhóm không?", SentAt = new DateTime(2025, 10, 27, 9, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 56, ChatId = 7, SenderId = 8, Content = "Có nha, 9h bắt đầu", SentAt = new DateTime(2025, 10, 27, 9, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 57, ChatId = 7, SenderId = 4, Content = "Ok, chuẩn bị tài liệu nhé", SentAt = new DateTime(2025, 10, 27, 9, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 58, ChatId = 7, SenderId = 8, Content = "Mình lo phần đó rồi 😎", SentAt = new DateTime(2025, 10, 27, 9, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 59, ChatId = 7, SenderId = 4, Content = "Good, cảm ơn nha ❤️", SentAt = new DateTime(2025, 10, 27, 9, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 60, ChatId = 7, SenderId = 8, Content = "Không có chi 😉", SentAt = new DateTime(2025, 10, 27, 9, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 61, ChatId = 7, SenderId = 4, Content = "Hẹn 9h sáng mai 😁", SentAt = new DateTime(2025, 10, 27, 9, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 62, ChatId = 7, SenderId = 8, Content = "Ok luôn 😎", SentAt = new DateTime(2025, 10, 27, 9, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 63, ChatId = 7, SenderId = 4, Content = "Chuẩn bị tinh thần nha 😆", SentAt = new DateTime(2025, 10, 27, 9, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 8: Bao (5) - Linh (9)
                new Message { Id = 64, ChatId = 8, SenderId = 5, Content = "Linh ơi có xem bài mới chưa?", SentAt = new DateTime(2025, 10, 28, 10, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 65, ChatId = 8, SenderId = 9, Content = "Có xem rồi 😎", SentAt = new DateTime(2025, 10, 28, 10, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 66, ChatId = 8, SenderId = 5, Content = "Có khó không?", SentAt = new DateTime(2025, 10, 28, 10, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 67, ChatId = 8, SenderId = 9, Content = "Bình thường thôi 😁", SentAt = new DateTime(2025, 10, 28, 10, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 68, ChatId = 8, SenderId = 5, Content = "Ok, chuẩn bị ôn bài nào", SentAt = new DateTime(2025, 10, 28, 10, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 69, ChatId = 8, SenderId = 9, Content = "Được luôn 😆", SentAt = new DateTime(2025, 10, 28, 10, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 70, ChatId = 8, SenderId = 5, Content = "8h tối call nha", SentAt = new DateTime(2025, 10, 28, 10, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 71, ChatId = 8, SenderId = 9, Content = "Ok luôn 😎", SentAt = new DateTime(2025, 10, 28, 10, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 72, ChatId = 8, SenderId = 5, Content = "Thanks nha ❤️", SentAt = new DateTime(2025, 10, 28, 10, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 9: Nam (8) - Hieu (10)
                new Message { Id = 73, ChatId = 9, SenderId = 8, Content = "Hieu ơi bài tập xong chưa?", SentAt = new DateTime(2025, 10, 28, 11, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 74, ChatId = 9, SenderId = 10, Content = "Sắp xong 😅", SentAt = new DateTime(2025, 10, 28, 11, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 75, ChatId = 9, SenderId = 8, Content = "Cần giúp không?", SentAt = new DateTime(2025, 10, 28, 11, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 76, ChatId = 9, SenderId = 10, Content = "Có chút 😁", SentAt = new DateTime(2025, 10, 28, 11, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 77, ChatId = 9, SenderId = 8, Content = "Ok để mình gọi nha", SentAt = new DateTime(2025, 10, 28, 11, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 78, ChatId = 9, SenderId = 10, Content = "Cảm ơn nha 😍", SentAt = new DateTime(2025, 10, 28, 11, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 79, ChatId = 9, SenderId = 8, Content = "Không có chi 😉", SentAt = new DateTime(2025, 10, 28, 11, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 80, ChatId = 9, SenderId = 10, Content = "Tối gặp nhé", SentAt = new DateTime(2025, 10, 28, 11, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 81, ChatId = 9, SenderId = 8, Content = "Ok luôn 😆", SentAt = new DateTime(2025, 10, 28, 11, 40, 0, DateTimeKind.Utc), Status = "sent" },

                // === Chat 10: Linh (9) - Hieu (10)
                new Message { Id = 82, ChatId = 10, SenderId = 9, Content = "Hieu, tối nay đi ăn không?", SentAt = new DateTime(2025, 10, 29, 12, 0, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 83, ChatId = 10, SenderId = 10, Content = "Đi chứ 😎", SentAt = new DateTime(2025, 10, 29, 12, 5, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 84, ChatId = 10, SenderId = 9, Content = "Ăn gì đây?", SentAt = new DateTime(2025, 10, 29, 12, 10, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 85, ChatId = 10, SenderId = 10, Content = "Bún chả nhé 😁", SentAt = new DateTime(2025, 10, 29, 12, 15, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 86, ChatId = 10, SenderId = 9, Content = "Ok, hẹn 7h", SentAt = new DateTime(2025, 10, 29, 12, 20, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 87, ChatId = 10, SenderId = 10, Content = "Ok luôn 😎", SentAt = new DateTime(2025, 10, 29, 12, 25, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 88, ChatId = 10, SenderId = 9, Content = "Đừng quên đặt bàn nha 😆", SentAt = new DateTime(2025, 10, 29, 12, 30, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 89, ChatId = 10, SenderId = 10, Content = "Ok luôn ❤️", SentAt = new DateTime(2025, 10, 29, 12, 35, 0, DateTimeKind.Utc), Status = "sent" },
                new Message { Id = 90, ChatId = 10, SenderId = 9, Content = "Tối gặp nha 😁", SentAt = new DateTime(2025, 10, 29, 12, 40, 0, DateTimeKind.Utc), Status = "sent" }
            );




            // ===== FRIENDS =====
            // Mỗi người có 4–5 bạn bè (2 chiều)
            modelBuilder.Entity<Contact>()
               .HasData(
                   // User 1 (Khoa)
                   new Contact { Id = 1, UserIdContactA = 1, UserIdContactB = 2 },
                   new Contact { Id = 2, UserIdContactA = 1, UserIdContactB = 3 },
                   new Contact { Id = 3, UserIdContactA = 1, UserIdContactB = 4 },
                   new Contact { Id = 4, UserIdContactA = 1, UserIdContactB = 5 },
                   new Contact { Id = 5, UserIdContactA = 1, UserIdContactB = 6 },
                   new Contact { Id = 6, UserIdContactA = 1, UserIdContactB = 7 },
                   new Contact { Id = 7, UserIdContactA = 1, UserIdContactB = 8 },

                   // User 2 (Phuc)
                   new Contact { Id = 8, UserIdContactA = 2, UserIdContactB = 3 },
                   new Contact { Id = 9, UserIdContactA = 2, UserIdContactB = 5 },
                   new Contact { Id = 10, UserIdContactA = 2, UserIdContactB = 6 },
                   new Contact { Id = 11, UserIdContactA = 2, UserIdContactB = 7 },

                   // User 3 (Dang)
                   new Contact { Id = 12, UserIdContactA = 3, UserIdContactB = 4 },
                   new Contact { Id = 13, UserIdContactA = 3, UserIdContactB = 7 },
                   new Contact { Id = 14, UserIdContactA = 3, UserIdContactB = 8 },

                   // User 4 (Thao)
                   new Contact { Id = 15, UserIdContactA = 4, UserIdContactB = 5 },
                   new Contact { Id = 16, UserIdContactA = 4, UserIdContactB = 6 },
                   new Contact { Id = 17, UserIdContactA = 4, UserIdContactB = 8 },

                   // User 5 (Bao)
                   new Contact { Id = 18, UserIdContactA = 5, UserIdContactB = 7 },

                   // User 6 (Tuan)
                   new Contact { Id = 19, UserIdContactA = 6, UserIdContactB = 8 },

                   // User 7 (Han)
                   new Contact { Id = 20, UserIdContactA = 7, UserIdContactB = 8 }
               );
        }
    }
}
