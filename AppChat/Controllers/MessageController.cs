using AppChat.Data;
using AppChat.Hubs;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Repositories;
using AppChat.Services;
using AppChat.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMessageRepository _msgRepo;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MessageController> _logger;

        public MessageController(
            IMessageRepository msgRepo,
            IHubContext<ChatHub> hub,
            IWebHostEnvironment env,
            AppDbContext context,
            ILogger<MessageController> logger)
        {
            _msgRepo = msgRepo;
            _hub = hub;
            _env = env;
            _context = context;
            _logger = logger;
        }

        // GET: /message/{chatId}
        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            try
            {
                var messages = await _msgRepo.GetMessagesByChatIdAsync(chatId);
                // Check if messages are null?
                if (messages == null || !messages.Any())
                {
                    return Ok(new List<Message>()); 
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        // POST: /message/send
        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            try
            {
                _logger.LogInformation("Received request: ChatId: {ChatId}, SenderId: {SenderId}, ReceiverId: {ReceiverId}, Content: {Content}",
                    dto.ChatId, dto.SenderId, dto.ReceiverId, dto.Content);

                _logger.LogInformation("Start processing file upload for message from user {SenderId} to {ReceiverId}.", dto.SenderId, dto.ReceiverId);

                string? fileUrl = null;

                // ============================
                // 1) Xử lý upload file nếu có
                // ============================
                if (dto.FileType != "text" && dto.File != null)
                {
                    var wwwRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsDir = Path.Combine(wwwRoot, "uploads");

                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    var fileName = Path.GetFileName(dto.File.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    fileUrl = $"/uploads/{fileName}";
                }

                // ====================================
                // 2) Nếu ChatId null → tìm hoặc tạo chat
                // ====================================
                int chatId;
                _logger.LogInformation("Check chatId from dto sending", dto.ChatId);


                if (dto.ChatId == null)
                {
                    _logger.LogInformation("Checking if a chat exists between sender {SenderId} and receiver {ReceiverId}.", dto.SenderId, dto.ReceiverId);

                    // tìm xem A và B đã có chat chưa
                    var existingChat = await _context.Chats
                        .FirstOrDefaultAsync(c =>
                            (c.UserAId == dto.SenderId && c.UserBId == dto.ReceiverId) ||
                            (c.UserAId == dto.ReceiverId && c.UserBId == dto.SenderId)
                        );

                    if (existingChat == null)
                    {
                        // tạo chat mới
                        _logger.LogInformation("Creating new chat between sender {SenderId} and receiver {ReceiverId}.", dto.SenderId, dto.ReceiverId);

                        existingChat = new Chat
                        {
                            UserAId = dto.SenderId,
                            UserBId = dto.ReceiverId,
                            LastMessage = "",
                            LastMessageTime = DateTime.UtcNow,
                            UnreadCount = 0
                        };

                        _context.Chats.Add(existingChat);
                        await _context.SaveChangesAsync();
                    }

                    chatId = existingChat.Id;
                }
                else
                {
                    chatId = dto.ChatId.Value;
                }

                // ============================
                // 3) Tạo và lưu Message
                // ============================
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = dto.SenderId,
                    FileType = dto.FileType,
                    Content = dto.FileType == "text" ? dto.Content : null,
                    FileUrl = fileUrl,
                    SentAt = DateTime.UtcNow,
                    Status = "sent"
                };

                _context.Messages.Add(message);
                _logger.LogInformation("Message created with ChatId: {ChatId}, SenderId: {SenderId}, Content: {Content}", chatId, dto.SenderId, dto.Content ?? "No content (file message)");

                // ============================
                // 4) Cập nhật Chat (last message)
                // ============================
                var chatUpdate = await _context.Chats.FindAsync(chatId);

                if (dto.FileType == "text")
                    chatUpdate.LastMessage = dto.Content ?? "";
                else
                    chatUpdate.LastMessage = $"[{dto.FileType}]";

                chatUpdate.LastMessageTime = DateTime.UtcNow;

                // tăng unread count cho người nhận
                if (dto.SenderId == chatUpdate.UserAId)
                    chatUpdate.UnreadCount += 1;
                else
                    chatUpdate.UnreadCount += 1;

                await _context.SaveChangesAsync();

                // ============================
                // 5) Build MessageDto
                // ============================
                var sender = await _context.Users.FindAsync(dto.SenderId);
                var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

                var messageDto = new MessageDto
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    SenderId = message.SenderId,
                    SenderName = senderName,
                    Content = message.Content,
                    FileUrl = message.FileUrl,
                    FileType = message.FileType,
                    SentTime = TimeHelper.ConvertToVietnamTime(message.SentAt),
                    Status = message.Status
                };

                // ============================
                // 6) Gửi realtime SignalR
                // ============================
                _logger.LogInformation("Sending message {MessageId} to chat group {ChatId}.", message.Id, chatId);

                // Realtime to Sender and Receiver
                await _hub.Clients.User(chatUpdate.UserBId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                await _hub.Clients.User(chatUpdate.UserAId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                // ============================
                // 7) Trả về FE
                // ============================
                return Ok(new
                {
                    chatId = chatId,
                    message = messageDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi gửi tin nhắn", error = ex.Message });
            }
        }
    }
}
