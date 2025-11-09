using AppChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConversationController : Controller
    {
        private readonly AppDbContext _context;

        public ConversationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConver()
        {
            var convers = await _context.Conversations.ToListAsync();
            return Ok(convers);
        }

        [Authorize]
        [HttpGet("myconversations")]   // https://localhost:5047/conversation/myconversations
        public async Task<IActionResult> GetConverById()
        {
            try
            {
                // Lấy userId từ token
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                // Lấy danh sách Conver của user
                var conversations = await _context.Conversations
                    .Where(c => c.User1Id == userId || c.User2Id == userId)
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                if (!conversations.Any())
                    return NotFound(new { message = "Không tìm thấy cuộc trò chuyện nào." });

                // Lấy danh sách các UserId tham gia phòng chat
                var otherUserIds = conversations
                    .Select(c => c.User1Id == userId ? c.User2Id : c.User1Id)
                    .Distinct()
                    .ToList();

                // Lấy thông tin người dùng (UserName)
                var users = await _context.Users
                    .Where(u => otherUserIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName);

                // Tạo kết quả trả về
                var result = conversations.Select(c => new
                {
                    c.Id,
                    UserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                    UserName = users.ContainsKey(c.User1Id == userId ? c.User2Id : c.User1Id)
                        ? users[c.User1Id == userId ? c.User2Id : c.User1Id]
                        : "(Người dùng đã bị xóa)"
                    //c.LastMessage,
                    //c.LastMessageTime,
                    //c.UnreadCount
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
