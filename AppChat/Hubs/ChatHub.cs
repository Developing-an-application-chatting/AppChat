using AppChat.Data;
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
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string converId, string user, string message, string senderId)
        {
            // Lưu tin nhắn, lấy tên người gửi, và gửi tin nhắn đến các client
            var msg = await _msgService.SaveMessage(converId, senderId, message);

            var sender = await _context.Users
                .Where(u => u.Id == int.Parse(senderId))
                .FirstOrDefaultAsync();

            if (sender != null)
            {
                var messageDto = new MessageDto
                {
                    Id = msg.Id,
                    SenderId = msg.SenderId,
                    SenderName = $"{sender.FirstName} {sender.LastName}",
                    Content = msg.Content,
                    SentTime = msg.SentAt.ToString("HH:mm:ss dd/MM/yyyy"),
                    Status = msg.Status
                };

                await Clients.User(user).SendAsync("ReceiveMessage", messageDto);
                await Clients.Caller.SendAsync("ReceiveMessage", messageDto);
            }
        }

        public async Task UserTyping(string conversationId, string userId)
        {
            await Clients.OthersInGroup(conversationId)
                .SendAsync("ReceiveTyping", conversationId, userId);
        }

        public async Task MessageSeen(string conversationId, string messageId)
        {
            await Clients.OthersInGroup(conversationId)
                .SendAsync("MessageStatusUpdated", messageId, "seen");
        }
    }
}
