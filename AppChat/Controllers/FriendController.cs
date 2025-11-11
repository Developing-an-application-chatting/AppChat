using AppChat.Data;
using AppChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FriendController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]  // https://localhost:5047/friend
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                var friendList = await _context.Friends
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                if (friendList == null) return NotFound();

                return Ok(friendList);
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }
    }
}
