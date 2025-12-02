namespace AppChat.Models.DTOs
{
    public class SendMessageDto
    {
        public int? ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string FileType { get; set; } = "text";
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
        public string? FileBase64 { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
