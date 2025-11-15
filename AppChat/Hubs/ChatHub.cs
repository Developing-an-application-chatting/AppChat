using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly MessageService _msgService;

        public ChatHub(AppDbContext context, MessageService service)
        {
            _context = context;
            _msgService = service;
        }

        public string UserId => Context.UserIdentifier ?? "NULL";

        public override Task OnConnectedAsync()
        {
            var user = Context.User; // Lấy thông tin người dùng từ JWT
            Console.WriteLine(user);
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            return base.OnConnectedAsync();
        }

        //public async Task SendMessage(string converId, string user, string message, string senderId)
        //{
        //    // Lưu tin nhắn, lấy tên người gửi, và gửi tin nhắn đến các client
        //    var msg = await _msgService.SaveMessage(converId, senderId, message);

        //    var sender = await _context.Users
        //        .Where(u => u.Id == int.Parse(senderId))
        //        .FirstOrDefaultAsync();

        //    if (sender != null)
        //    {
        //        var messageDto = new MessageDto
        //        {
        //            Id = msg.Id,
        //            SenderId = msg.SenderId,
        //            SenderName = $"{sender.FirstName} {sender.LastName}",
        //            Content = msg.Content,
        //            SentTime = msg.SentAt.ToString("HH:mm:ss dd/MM/yyyy"),
        //            Status = msg.Status
        //        };

        //        await Clients.User(user).SendAsync("ReceiveMessage", messageDto);
        //        await Clients.Caller.SendAsync("ReceiveMessage", messageDto);
        //    }
        //}

        public async Task SendMessage(SendMessageDto dto)
        {
            string? fileUrl = null;

            // Nếu là file, decode base64 và lưu
            if (dto.FileType != "text" && dto.FileBase64 != null)
            {
                var base64Data = dto.FileBase64.Split(",")[1];
                var bytes = Convert.FromBase64String(base64Data);

                var ext = Path.GetExtension(dto.FileName) ?? ".bin";
                var fileName = $"{Guid.NewGuid()}{ext}";
                var uploadsDir = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var filePath = Path.Combine(uploadsDir, fileName);
                await File.WriteAllBytesAsync(filePath, bytes);

                fileUrl = $"/uploads/{fileName}";
            }

            // Lưu DB
            var msg = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId,
                FileType = dto.FileType,
                Content = dto.FileType == "text" ? dto.Content : null,
                FileUrl = fileUrl
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Lấy thông tin sender
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.SenderId);

            var messageDto = new MessageDto
            {
                Id = msg.Id,
                SenderId = msg.SenderId,
                SenderName = $"{sender?.FirstName} {sender?.LastName}",
                Content = msg.Content,
                FileUrl = msg.FileUrl,
                FileType = msg.FileType,
                SentTime = msg.SentAt.ToString("HH:mm:ss dd/MM/yyyy"),
                Status = msg.Status
            };

            // Gửi realtime tới group
            await Clients.Group(dto.ChatId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }

        public async Task UserTyping(string conversationId, string userId, string userName)
        {
            await Clients.OthersInGroup(conversationId)
                .SendAsync("ReceiveTyping", conversationId, userId, userName);
        }

        public async Task MessageSeen(string conversationId, string messageId)
        {
            await Clients.OthersInGroup(conversationId)
                .SendAsync("MessageStatusUpdated", messageId, "seen");
        }

        public async Task JoinGroup(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }
    }
}
