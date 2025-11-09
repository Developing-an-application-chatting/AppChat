using AppChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("{conversationId}")]   //https://localhost:5047/message/1
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                bool isParticipant = await _context.Conversations
                    .AnyAsync(c => c.Id == conversationId && (c.User1Id == userId || c.User2Id == userId));

                if (!isParticipant)
                    return Forbid("Bạn không có quyền truy cập vào cuộc trò chuyện này.");

                var messages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId)
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

                if (messages == null || !messages.Any())
                    return Ok(new { message = "Chưa có tin nhắn nào trong phòng này." });

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
    }
}
