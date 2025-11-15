using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
                if (existingUser != null) return BadRequest("Email already taken");

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Created(string.Empty, user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpPost("login")]
        public async Task <IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PhoneNumber == dto.PhoneNumber);

                if (user == null) return Unauthorized("Invalid credentials");

                var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.PhoneNumber);

                return Ok(new { accessToken, user.Id});
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
   
        }
    }
}
