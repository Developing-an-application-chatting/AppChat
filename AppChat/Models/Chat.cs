using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Chats")]
    public class Chat
    {
        public int Id { get; set; }
        public int UserAId { get; set; }
        public int UserBId { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
