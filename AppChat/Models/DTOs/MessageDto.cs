namespace AppChat.Models.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string SentTime { get; set; }
        public string Status { get; set; }
    }
}
