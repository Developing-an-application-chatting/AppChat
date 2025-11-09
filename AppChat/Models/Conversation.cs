using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Conversations")]
    public class Conversation
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
