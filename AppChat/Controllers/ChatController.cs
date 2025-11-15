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
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllConver()
        //{
        //    var convers = await _context.Conversations.ToListAsync();
        //    return Ok(convers);
        //}

        //[Authorize]
        [HttpGet]   // https://localhost:5047/Chat
        public async Task<IActionResult> GetConverById()
        {
            try
            {
                // Lấy UserIdContactA từ token
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                //    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                //if (!int.TryParse(userIdClaim, out int userId))
                //    return BadRequest("Invalid user ID in token.");

                int userId = 1;

                // Lấy danh sách Conver của user
                var conversations = await _context.Chats
                    .Where(c => c.UserAId == userId || c.UserBId == userId)
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                if (!conversations.Any())
                    return NotFound(new { message = "Không tìm thấy cuộc trò chuyện nào." });

                // Lấy danh sách các UserIdContactA tham gia phòng chat
                var otherUserIds = conversations
                    .Select(c => c.UserAId == userId ? c.UserBId : c.UserAId)
                    .Distinct()
                    .ToList();

                // Lấy thông tin người dùng (UserName)
                var users = await _context.Users
                    .Where(u => otherUserIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        FullName = u.FirstName + " " + u.LastName,
                        u.AvatarUrl,
                        u.IsOnline,
                        u.LastSeen
                    });

                // Tạo kết quả trả 
                var result = new List<object>();

                foreach (var Chat in conversations)
                {
                    // Lấy tin nhắn cuối cùng của cuộc trò chuyện
                    var lastMessage = await _context.Messages
                        .Where(m => m.ChatId == Chat.Id)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefaultAsync();

                    var partnerId = Chat.UserAId == userId ? Chat.UserBId : Chat.UserAId;
                    var partnerInfo = users.ContainsKey(partnerId) ? users[partnerId] : null;

                    var lastMessageContent = lastMessage?.Content ?? "(Không có tin nhắn)";
                    var lastMessageTime = lastMessage?.SentAt ?? DateTime.MinValue;

                    result.Add(new
                    {
                        Chat = new
                        {
                            Chat.Id,
                            LastMessage = lastMessageContent,
                            LastMessageTime = lastMessageTime.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy"),
                            UnreadCount = Chat.UnreadCount
                        },
                        
                        Info = new
                        {
                            Id = partnerId,
                            FullName = partnerInfo.FullName ?? "(Người dùng không tồn tại)",
                            AvatarUrl = partnerInfo?.AvatarUrl,
                            isOnline = partnerInfo?.IsOnline,
                            lastSeen = partnerInfo?.LastSeen.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy")
                        }
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
