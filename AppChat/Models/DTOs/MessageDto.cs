namespace AppChat.Models.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public string FileType { get; set; } = "text";
        public string SentTime { get; set; } = string.Empty;
        public string Status { get; set; } = "sent";
    }

}
