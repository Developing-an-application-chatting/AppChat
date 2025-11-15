using AppChat.Data;
using AppChat.Hubs;
using AppChat.Models;
using AppChat.Models.DTOs;
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
        private readonly IHubContext<ChatHub> _hub;
        private readonly IWebHostEnvironment _env;

        public MessageController(AppDbContext context,
                                 IHubContext<ChatHub> hubContext,
                                 IWebHostEnvironment env)
        {
            _context = context;
            _hub = hubContext;
            _env = env;
        }

        // GET: message/1
        [HttpGet("{ChatId}")]
        public async Task<IActionResult> GetMessages(int ChatId)
        {
            try
            {
                // Lấy userId từ token
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                //    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                //if (!int.TryParse(userIdClaim, out int userId))
                //    return BadRequest("Invalid user ID in token.");
                int userId = 1; // FIXED tạm cho test

                bool isParticipant = await _context.Chats
                    .AnyAsync(c => c.Id == ChatId && (c.UserAId == userId || c.UserBId == userId));

                if (!isParticipant)
                    return Forbid("Bạn không có quyền truy cập vào cuộc trò chuyện này.");

                var messages = await _context.Messages
                    .Where(m => m.ChatId == ChatId)
                    .Join(_context.Users,
                          m => m.SenderId,
                          u => u.Id,
                          (m, u) => new
                          {
                              m.Id,
                              m.SenderId,
                              SenderName = $"{u.FirstName} {u.LastName}",
                              m.Content,
                              m.FileUrl,
                              m.FileType,
                              SentTime = m.SentAt.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy")
                          })
                    .OrderBy(m => m.Id)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        // POST: message/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            try
            {
                string? fileUrl = null;

                // 1) Xử lý file upload nếu là file/image/video
                if (dto.FileType != "text" && dto.File != null)
                {
                    // đảm bảo wwwroot tồn tại
                    var wwwRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsDir = Path.Combine(wwwRoot, "uploads");

                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    // URL cho FE truy cập (tương lai deploy vẫn OK)
                    fileUrl = $"/uploads/{fileName}";
                }

                // 2) Tạo Message entity
                var message = new Message
                {
                    ChatId = dto.ChatId,
                    SenderId = dto.SenderId,
                    FileType = dto.FileType,
                    Content = dto.FileType == "text" ? dto.Content : null,
                    FileUrl = fileUrl,
                    SentAt = DateTime.UtcNow,
                    Status = "sent"
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // 3) Lấy tên người gửi (có thể tương lai lấy từ User table)
                var sender = await _context.Users.FindAsync(dto.SenderId);
                var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

                // 4) Trả về MessageDto
                var messageDto = new MessageDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = senderName,
                    Content = message.Content,
                    FileUrl = message.FileUrl,
                    FileType = message.FileType,
                    SentTime = message.SentAt.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy"),
                    Status = message.Status
                };

                // 5) Gửi realtime SignalR
                await _hub.Clients.Group(dto.ChatId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                return Ok(messageDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi gửi tin nhắn", error = ex.Message });
            }
        }

    }
}
