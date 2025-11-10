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
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friend> Friends { get; set; }

        // Seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USERS =====
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Dang", LastName = "Khoa", PhoneNumber = "0901000001", AvatarUrl = "", IsOnline = true, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0) },
                new User { Id = 2, FirstName = "Minh", LastName = "Phuc", PhoneNumber = "0901000002", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 55, 0) },
                new User { Id = 3, FirstName = "Hai", LastName = "Dang", PhoneNumber = "0901000003", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 50, 0) },
                new User { Id = 4, FirstName = "Thao", LastName = "Nguyen", PhoneNumber = "0901000004", AvatarUrl = "", IsOnline = true, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0) },
                new User { Id = 5, FirstName = "Bao", LastName = "Tran", PhoneNumber = "0901000005", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 0, 0) },
                new User { Id = 6, FirstName = "Anh", LastName = "Tuan", PhoneNumber = "0901000006", AvatarUrl = "", IsOnline = true, LastSeen = new DateTime(2025, 11, 3, 12, 0, 0) },
                new User { Id = 7, FirstName = "Ngoc", LastName = "Han", PhoneNumber = "0901000007", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 30, 0) },
                new User { Id = 8, FirstName = "Le", LastName = "Nam", PhoneNumber = "0901000008", AvatarUrl = "", IsOnline = false, LastSeen = new DateTime(2025, 11, 3, 11, 45, 0) }
            );


            // ===== CONVERSATIONS =====
            // Mỗi user có 4–5 cuộc trò chuyện (với các bạn bè ở trên)
            modelBuilder.Entity<Conversation>().HasData(
                new Conversation { Id = 1, User1Id = 1, User2Id = 2, LastMessage = "Gặp sau nha 👋", LastMessageTime = new DateTime(2025, 10, 28, 10, 44, 0), UnreadCount = 0 },
                new Conversation { Id = 2, User1Id = 1, User2Id = 3, LastMessage = "Ok mai gặp!", LastMessageTime = new DateTime(2025, 10, 27, 21, 27, 0), UnreadCount = 0 },
                new Conversation { Id = 3, User1Id = 1, User2Id = 4, LastMessage = "Đi xem phim nhé 🎬", LastMessageTime = new DateTime(2025, 10, 26, 20, 16, 0), UnreadCount = 0 },
                new Conversation { Id = 4, User1Id = 1, User2Id = 5, LastMessage = "Gửi file rồi đó!", LastMessageTime = new DateTime(2025, 10, 25, 18, 20, 0), UnreadCount = 0 },
                new Conversation { Id = 5, User1Id = 2, User2Id = 6, LastMessage = "Cảm ơn nha 😁", LastMessageTime = new DateTime(2025, 10, 25, 19, 24, 0), UnreadCount = 1 },
                new Conversation { Id = 6, User1Id = 3, User2Id = 7, LastMessage = "Tối nhớ học bài 😆", LastMessageTime = new DateTime(2025, 10, 28, 8, 14, 0), UnreadCount = 0 },
                new Conversation { Id = 7, User1Id = 4, User2Id = 8, LastMessage = "Mai gặp nhé 💕", LastMessageTime = new DateTime(2025, 10, 27, 9, 0, 0), UnreadCount = 0 },
                new Conversation { Id = 8, User1Id = 5, User2Id = 7, LastMessage = "Haha vui quá 😁", LastMessageTime = new DateTime(2025, 10, 27, 18, 45, 0), UnreadCount = 0 },
                new Conversation { Id = 9, User1Id = 6, User2Id = 8, LastMessage = "Ok bro 😎", LastMessageTime = new DateTime(2025, 10, 28, 22, 15, 0), UnreadCount = 0 },
                new Conversation { Id = 10, User1Id = 2, User2Id = 5, LastMessage = "Đi học thôi!", LastMessageTime = new DateTime(2025, 10, 28, 9, 18, 0), UnreadCount = 0 }
            );



            // ===== MESSAGES =====
            modelBuilder.Entity<Message>().HasData(
                // === Conversation 1: Khoa (1) - Phuc (2)
                new Message { Id = 1, ConversationId = 1, SenderId = 1, Content = "Chào Phuc, hôm nay rảnh không?", SentAt = new DateTime(2025, 10, 25, 9, 0, 0) },
                new Message { Id = 2, ConversationId = 1, SenderId = 2, Content = "Rảnh nè, đi uống cà phê không ☕", SentAt = new DateTime(2025, 10, 25, 9, 5, 0) },
                new Message { Id = 3, ConversationId = 1, SenderId = 1, Content = "Ok, quán cũ nha!", SentAt = new DateTime(2025, 10, 25, 9, 10, 0) },
                new Message { Id = 4, ConversationId = 1, SenderId = 2, Content = "Tới luôn 😁", SentAt = new DateTime(2025, 10, 25, 9, 15, 0) },
                new Message { Id = 5, ConversationId = 1, SenderId = 1, Content = "Đang đi nè 🚶‍♂️", SentAt = new DateTime(2025, 10, 25, 9, 20, 0) },
                new Message { Id = 6, ConversationId = 1, SenderId = 2, Content = "Tới rồi nha", SentAt = new DateTime(2025, 10, 25, 9, 25, 0) },
                new Message { Id = 7, ConversationId = 1, SenderId = 1, Content = "Thấy rồi 👋", SentAt = new DateTime(2025, 10, 25, 9, 30, 0) },
                new Message { Id = 8, ConversationId = 1, SenderId = 2, Content = "Uống gì đây?", SentAt = new DateTime(2025, 10, 25, 9, 35, 0) },
                new Message { Id = 9, ConversationId = 1, SenderId = 1, Content = "Cho ly đen đá 😆", SentAt = new DateTime(2025, 10, 25, 9, 40, 0) },

                // === Conversation 2: Khoa (1) - Dang (3)
                new Message { Id = 10, ConversationId = 2, SenderId = 3, Content = "Khoa ơi, API chạy chưa?", SentAt = new DateTime(2025, 10, 26, 9, 0, 0) },
                new Message { Id = 11, ConversationId = 2, SenderId = 1, Content = "Chạy rồi, đang test thêm chút!", SentAt = new DateTime(2025, 10, 26, 9, 5, 0) },
                new Message { Id = 12, ConversationId = 2, SenderId = 3, Content = "Good job 💪", SentAt = new DateTime(2025, 10, 26, 9, 10, 0) },
                new Message { Id = 13, ConversationId = 2, SenderId = 1, Content = "Cảm ơn nhé 😎", SentAt = new DateTime(2025, 10, 26, 9, 15, 0) },
                new Message { Id = 14, ConversationId = 2, SenderId = 3, Content = "Mai review nha", SentAt = new DateTime(2025, 10, 26, 9, 20, 0) },
                new Message { Id = 15, ConversationId = 2, SenderId = 1, Content = "Ok deal!", SentAt = new DateTime(2025, 10, 26, 9, 25, 0) },
                new Message { Id = 16, ConversationId = 2, SenderId = 3, Content = "Nhớ đem laptop 😅", SentAt = new DateTime(2025, 10, 26, 9, 30, 0) },
                new Message { Id = 17, ConversationId = 2, SenderId = 1, Content = "Haha tất nhiên rồi", SentAt = new DateTime(2025, 10, 26, 9, 35, 0) },
                new Message { Id = 18, ConversationId = 2, SenderId = 3, Content = "Ok, gặp sáng mai nhé 👋", SentAt = new DateTime(2025, 10, 26, 9, 40, 0) },

                // === Conversation 3: Khoa (1) - Thao (4)
                new Message { Id = 19, ConversationId = 3, SenderId = 4, Content = "Cuối tuần đi xem phim nhé 🎬", SentAt = new DateTime(2025, 10, 26, 20, 0, 0) },
                new Message { Id = 20, ConversationId = 3, SenderId = 1, Content = "Phim gì vậy?", SentAt = new DateTime(2025, 10, 26, 20, 5, 0) },
                new Message { Id = 21, ConversationId = 3, SenderId = 4, Content = "Marvel mới ra đó 😁", SentAt = new DateTime(2025, 10, 26, 20, 10, 0) },
                new Message { Id = 22, ConversationId = 3, SenderId = 1, Content = "Ok, đặt vé nhé", SentAt = new DateTime(2025, 10, 26, 20, 15, 0) },
                new Message { Id = 23, ConversationId = 3, SenderId = 4, Content = "Đặt 2 vé rồi nha ❤️", SentAt = new DateTime(2025, 10, 26, 20, 20, 0) },
                new Message { Id = 24, ConversationId = 3, SenderId = 1, Content = "Nice! hẹn gặp 😄", SentAt = new DateTime(2025, 10, 26, 20, 25, 0) },
                new Message { Id = 25, ConversationId = 3, SenderId = 4, Content = "Mai 7h nhé!", SentAt = new DateTime(2025, 10, 26, 20, 30, 0) },
                new Message { Id = 26, ConversationId = 3, SenderId = 1, Content = "Okeee 😎", SentAt = new DateTime(2025, 10, 26, 20, 35, 0) },
                new Message { Id = 27, ConversationId = 3, SenderId = 4, Content = "Ngủ sớm nha 😆", SentAt = new DateTime(2025, 10, 26, 20, 40, 0) },

                // === Conversation 4: Khoa (1) - Bao (5)
                new Message { Id = 28, ConversationId = 4, SenderId = 5, Content = "Khoa gửi tài liệu chưa?", SentAt = new DateTime(2025, 10, 25, 18, 0, 0) },
                new Message { Id = 29, ConversationId = 4, SenderId = 1, Content = "Đang gửi nè 📎", SentAt = new DateTime(2025, 10, 25, 18, 5, 0) },
                new Message { Id = 30, ConversationId = 4, SenderId = 5, Content = "Ok thấy rồi", SentAt = new DateTime(2025, 10, 25, 18, 10, 0) },
                new Message { Id = 31, ConversationId = 4, SenderId = 1, Content = "Check giúp nha", SentAt = new DateTime(2025, 10, 25, 18, 15, 0) },
                new Message { Id = 32, ConversationId = 4, SenderId = 5, Content = "Ổn hết 👍", SentAt = new DateTime(2025, 10, 25, 18, 20, 0) },
                new Message { Id = 33, ConversationId = 4, SenderId = 1, Content = "Tốt quá 😁", SentAt = new DateTime(2025, 10, 25, 18, 25, 0) },
                new Message { Id = 34, ConversationId = 4, SenderId = 5, Content = "Mai nộp nha", SentAt = new DateTime(2025, 10, 25, 18, 30, 0) },
                new Message { Id = 35, ConversationId = 4, SenderId = 1, Content = "Ok!", SentAt = new DateTime(2025, 10, 25, 18, 35, 0) },
                new Message { Id = 36, ConversationId = 4, SenderId = 5, Content = "Thanks bro 😆", SentAt = new DateTime(2025, 10, 25, 18, 40, 0) },

                // === Conversation 5: Phuc (2) - Tuan (6)
                new Message { Id = 37, ConversationId = 5, SenderId = 2, Content = "Tuan ơi mai học môn gì nhỉ?", SentAt = new DateTime(2025, 10, 25, 19, 0, 0) },
                new Message { Id = 38, ConversationId = 5, SenderId = 6, Content = "Toán cao cấp đó 😅", SentAt = new DateTime(2025, 10, 25, 19, 5, 0) },
                new Message { Id = 39, ConversationId = 5, SenderId = 2, Content = "Ôi lại nữa à", SentAt = new DateTime(2025, 10, 25, 19, 10, 0) },
                new Message { Id = 40, ConversationId = 5, SenderId = 6, Content = "Chuẩn bị bài kỹ nha!", SentAt = new DateTime(2025, 10, 25, 19, 15, 0) },
                new Message { Id = 41, ConversationId = 5, SenderId = 2, Content = "Ok, tối nay làm bài chung nhé", SentAt = new DateTime(2025, 10, 25, 19, 20, 0) },
                new Message { Id = 42, ConversationId = 5, SenderId = 6, Content = "Được luôn 😎", SentAt = new DateTime(2025, 10, 25, 19, 25, 0) },
                new Message { Id = 43, ConversationId = 5, SenderId = 2, Content = "8h call nhé", SentAt = new DateTime(2025, 10, 25, 19, 30, 0) },
                new Message { Id = 44, ConversationId = 5, SenderId = 6, Content = "Ok deal!", SentAt = new DateTime(2025, 10, 25, 19, 35, 0) },
                new Message { Id = 45, ConversationId = 5, SenderId = 2, Content = "Cảm ơn nha 😁", SentAt = new DateTime(2025, 10, 25, 19, 40, 0) },

                // === Conversation 6: Dang (3) - Han (7)
                new Message { Id = 46, ConversationId = 6, SenderId = 3, Content = "Han ơi làm bài tập xong chưa?", SentAt = new DateTime(2025, 10, 27, 8, 0, 0) },
                new Message { Id = 47, ConversationId = 6, SenderId = 7, Content = "Sắp xong rồi 😅", SentAt = new DateTime(2025, 10, 27, 8, 5, 0) },
                new Message { Id = 48, ConversationId = 6, SenderId = 3, Content = "Cần giúp không?", SentAt = new DateTime(2025, 10, 27, 8, 10, 0) },
                new Message { Id = 49, ConversationId = 6, SenderId = 7, Content = "Có chứ 😁", SentAt = new DateTime(2025, 10, 27, 8, 15, 0) },
                new Message { Id = 50, ConversationId = 6, SenderId = 3, Content = "Ok để mình gọi nha", SentAt = new DateTime(2025, 10, 27, 8, 20, 0) },
                new Message { Id = 51, ConversationId = 6, SenderId = 7, Content = "Cảm ơn nha 😍", SentAt = new DateTime(2025, 10, 27, 8, 25, 0) },
                new Message { Id = 52, ConversationId = 6, SenderId = 3, Content = "Không có chi 😉", SentAt = new DateTime(2025, 10, 27, 8, 30, 0) },
                new Message { Id = 53, ConversationId = 6, SenderId = 7, Content = "Tối gặp nhé", SentAt = new DateTime(2025, 10, 27, 8, 35, 0) },
                new Message { Id = 54, ConversationId = 6, SenderId = 3, Content = "Ok luôn 😆", SentAt = new DateTime(2025, 10, 27, 8, 40, 0) },

                // === Conversation 7: Thao (4) - Nam (8)
                new Message { Id = 55, ConversationId = 7, SenderId = 4, Content = "Nam ơi mai có họp nhóm không?", SentAt = new DateTime(2025, 10, 27, 9, 0, 0) },
                new Message { Id = 56, ConversationId = 7, SenderId = 8, Content = "Có nha, 9h bắt đầu", SentAt = new DateTime(2025, 10, 27, 9, 5, 0) },
                new Message { Id = 57, ConversationId = 7, SenderId = 4, Content = "Ok, chuẩn bị tài liệu nhé", SentAt = new DateTime(2025, 10, 27, 9, 10, 0) },
                new Message { Id = 58, ConversationId = 7, SenderId = 8, Content = "Mình lo phần đó rồi 😎", SentAt = new DateTime(2025, 10, 27, 9, 15, 0) },
                new Message { Id = 59, ConversationId = 7, SenderId = 4, Content = "Good, cảm ơn nha ❤️", SentAt = new DateTime(2025, 10, 27, 9, 20, 0) },
                new Message { Id = 60, ConversationId = 7, SenderId = 8, Content = "Không có gì 😄", SentAt = new DateTime(2025, 10, 27, 9, 25, 0) },
                new Message { Id = 61, ConversationId = 7, SenderId = 4, Content = "Mai gặp nhé 👋", SentAt = new DateTime(2025, 10, 27, 9, 30, 0) },
                new Message { Id = 62, ConversationId = 7, SenderId = 8, Content = "See you!", SentAt = new DateTime(2025, 10, 27, 9, 35, 0) },
                new Message { Id = 63, ConversationId = 7, SenderId = 4, Content = "Nhớ đem laptop nha 💻", SentAt = new DateTime(2025, 10, 27, 9, 40, 0) },

                // === Conversation 8: Bao (5) - Han (7)
                new Message { Id = 64, ConversationId = 8, SenderId = 5, Content = "Han ơi bài tập nhóm sao rồi?", SentAt = new DateTime(2025, 10, 27, 18, 0, 0) },
                new Message { Id = 65, ConversationId = 8, SenderId = 7, Content = "Gần xong rồi 😁", SentAt = new DateTime(2025, 10, 27, 18, 5, 0) },
                new Message { Id = 66, ConversationId = 8, SenderId = 5, Content = "Cần mình giúp gì không?", SentAt = new DateTime(2025, 10, 27, 18, 10, 0) },
                new Message { Id = 67, ConversationId = 8, SenderId = 7, Content = "Ok gửi phần cậu qua nhé", SentAt = new DateTime(2025, 10, 27, 18, 15, 0) },
                new Message { Id = 68, ConversationId = 8, SenderId = 5, Content = "Gửi rồi đó", SentAt = new DateTime(2025, 10, 27, 18, 20, 0) },
                new Message { Id = 69, ConversationId = 8, SenderId = 7, Content = "Nhận được rồi ✅", SentAt = new DateTime(2025, 10, 27, 18, 25, 0) },
                new Message { Id = 70, ConversationId = 8, SenderId = 5, Content = "Tối call nhé", SentAt = new DateTime(2025, 10, 27, 18, 30, 0) },
                new Message { Id = 71, ConversationId = 8, SenderId = 7, Content = "Ok deal 😆", SentAt = new DateTime(2025, 10, 27, 18, 35, 0) },
                new Message { Id = 72, ConversationId = 8, SenderId = 5, Content = "Cảm ơn nha", SentAt = new DateTime(2025, 10, 27, 18, 40, 0) },

                // === Conversation 9: Tuan (6) - Nam (8)
                new Message { Id = 73, ConversationId = 9, SenderId = 6, Content = "Nam ơi tối chơi game không?", SentAt = new DateTime(2025, 10, 28, 21, 0, 0) },
                new Message { Id = 74, ConversationId = 9, SenderId = 8, Content = "Có luôn bro 😎", SentAt = new DateTime(2025, 10, 28, 21, 5, 0) },
                new Message { Id = 75, ConversationId = 9, SenderId = 6, Content = "Rank hay thường?", SentAt = new DateTime(2025, 10, 28, 21, 10, 0) },
                new Message { Id = 76, ConversationId = 9, SenderId = 8, Content = "Rank chứ 😁", SentAt = new DateTime(2025, 10, 28, 21, 15, 0) },
                new Message { Id = 77, ConversationId = 9, SenderId = 6, Content = "Ok vào phòng nhé", SentAt = new DateTime(2025, 10, 28, 21, 20, 0) },
                new Message { Id = 78, ConversationId = 9, SenderId = 8, Content = "Tới liền 🔥", SentAt = new DateTime(2025, 10, 28, 21, 25, 0) },
                new Message { Id = 79, ConversationId = 9, SenderId = 6, Content = "GG ez 😆", SentAt = new DateTime(2025, 10, 28, 21, 30, 0) },
                new Message { Id = 80, ConversationId = 9, SenderId = 8, Content = "Haha vui quá 😂", SentAt = new DateTime(2025, 10, 28, 21, 35, 0) },
                new Message { Id = 81, ConversationId = 9, SenderId = 6, Content = "Mai nữa nha!", SentAt = new DateTime(2025, 10, 28, 21, 40, 0) },

                // === Conversation 10: Phuc (2) - Bao (5)
                new Message { Id = 82, ConversationId = 10, SenderId = 2, Content = "Bao ơi đi học chưa?", SentAt = new DateTime(2025, 10, 28, 8, 0, 0) },
                new Message { Id = 83, ConversationId = 10, SenderId = 5, Content = "Sắp tới rồi 😆", SentAt = new DateTime(2025, 10, 28, 8, 5, 0) },
                new Message { Id = 84, ConversationId = 10, SenderId = 2, Content = "Nhớ mang slide nha", SentAt = new DateTime(2025, 10, 28, 8, 10, 0) },
                new Message { Id = 85, ConversationId = 10, SenderId = 5, Content = "Ok mình in rồi ✅", SentAt = new DateTime(2025, 10, 28, 8, 15, 0) },
                new Message { Id = 86, ConversationId = 10, SenderId = 2, Content = "Tốt quá!", SentAt = new DateTime(2025, 10, 28, 8, 20, 0) },
                new Message { Id = 87, ConversationId = 10, SenderId = 5, Content = "Hẹn ở lớp nha", SentAt = new DateTime(2025, 10, 28, 8, 25, 0) },
                new Message { Id = 88, ConversationId = 10, SenderId = 2, Content = "Ok đi liền đây 🚶‍♂️", SentAt = new DateTime(2025, 10, 28, 8, 30, 0) },
                new Message { Id = 89, ConversationId = 10, SenderId = 5, Content = "Thấy rồi 👋", SentAt = new DateTime(2025, 10, 28, 8, 35, 0) },
                new Message { Id = 90, ConversationId = 10, SenderId = 2, Content = "Đi học thôi 😁", SentAt = new DateTime(2025, 10, 28, 8, 40, 0) }
            );



            // ===== FRIENDS =====
            // Mỗi người có 4–5 bạn bè (2 chiều)
            modelBuilder.Entity<Friend>()
                .HasData(
                // Khoa (1)
                new Friend { Id = 1, UserId = 1, FriendId = 2 },
                new Friend { Id = 2, UserId = 1, FriendId = 3 },
                new Friend { Id = 3, UserId = 1, FriendId = 4 },
                new Friend { Id = 4, UserId = 1, FriendId = 5 },
                new Friend { Id = 5, UserId = 1, FriendId = 6 },

                // Phuc (2)
                new Friend { Id = 6, UserId = 2, FriendId = 1 },
                new Friend { Id = 7, UserId = 2, FriendId = 3 },
                new Friend { Id = 8, UserId = 2, FriendId = 5 },
                new Friend { Id = 9, UserId = 2, FriendId = 6 },
                new Friend { Id = 10, UserId = 2, FriendId = 7 },

                // Dang (3)
                new Friend { Id = 11, UserId = 3, FriendId = 1 },
                new Friend { Id = 12, UserId = 3, FriendId = 2 },
                new Friend { Id = 13, UserId = 3, FriendId = 4 },
                new Friend { Id = 14, UserId = 3, FriendId = 7 },
                new Friend { Id = 15, UserId = 3, FriendId = 8 },

                // Thao (4)
                new Friend { Id = 16, UserId = 4, FriendId = 1 },
                new Friend { Id = 17, UserId = 4, FriendId = 3 },
                new Friend { Id = 18, UserId = 4, FriendId = 5 },
                new Friend { Id = 19, UserId = 4, FriendId = 6 },
                new Friend { Id = 20, UserId = 4, FriendId = 8 },

                // Bao (5)
                new Friend { Id = 21, UserId = 5, FriendId = 1 },
                new Friend { Id = 22, UserId = 5, FriendId = 2 },
                new Friend { Id = 23, UserId = 5, FriendId = 4 },
                new Friend { Id = 24, UserId = 5, FriendId = 6 },
                new Friend { Id = 25, UserId = 5, FriendId = 7 },

                // Tuan (6)
                new Friend { Id = 26, UserId = 6, FriendId = 1 },
                new Friend { Id = 27, UserId = 6, FriendId = 2 },
                new Friend { Id = 28, UserId = 6, FriendId = 4 },
                new Friend { Id = 29, UserId = 6, FriendId = 5 },
                new Friend { Id = 30, UserId = 6, FriendId = 8 },

                // Han (7)
                new Friend { Id = 31, UserId = 7, FriendId = 2 },
                new Friend { Id = 32, UserId = 7, FriendId = 3 },
                new Friend { Id = 33, UserId = 7, FriendId = 5 },
                new Friend { Id = 34, UserId = 7, FriendId = 8 },
                new Friend { Id = 35, UserId = 7, FriendId = 1 },

                // Nam (8)
                new Friend { Id = 36, UserId = 8, FriendId = 3 },
                new Friend { Id = 37, UserId = 8, FriendId = 4 },
                new Friend { Id = 38, UserId = 8, FriendId = 6 },
                new Friend { Id = 39, UserId = 8, FriendId = 7 },
                new Friend { Id = 40, UserId = 8, FriendId = 1 }
            );

        }

    }
}
