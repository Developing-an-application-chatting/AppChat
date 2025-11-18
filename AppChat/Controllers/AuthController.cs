using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharpLearning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(TokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("login")]  // Change the logic to check existing first and then register if it not
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                // 1. Tìm user theo phone
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                // 2. Nếu chưa có → tạo user mới
                if (user == null)
                {
                    user = new User
                    {
                        PhoneNumber = dto.PhoneNumber,
                        FirstName = "",
                        LastName = "",
                        AvatarUrl = ""
                    };

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }

                // 3. Tạo token
                var token = _tokenService.GenerateToken(
                    user.Id.ToString(),
                    user.PhoneNumber
                );

                // 4. Trả response
                return Ok(new
                {
                    accessToken = token,
                    userId = user.Id
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Update user information after register
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
        {
            var userId = User.FindFirst("id")?.Value;

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.AvatarUrl = dto.AvatarUrl;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
