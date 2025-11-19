using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
        //public async Task<IActionResult> Register(RegisterDTO dto)
        //{
        //    try
        //    {
        //        // Check if user are existing
        //        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
        //        if (existingUser != null) return Ok(new { message = "User existed" }); // If existing, return

        //        // Create new User
        //        var newUser = new User
        //        {
        //            PhoneNumber = dto.PhoneNumber,
        //            Password = dto.Password,
        //            FirstName = dto.FirstName,
        //            LastName = dto.LastName,
        //            AvatarUrl = dto.AvatarUrl,
        //        };

        //        // Save to DB
        //        await _context.Users.AddAsync(newUser);
        //        await _context.SaveChangesAsync();
        //        return Created(string.Empty, new {message = "User created"});
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(new { message = e.Message });
        //    }
        //}
        public async Task<IActionResult> Register([FromForm] RegisterDTO dto, IFormFile avatar)
        {
            string avatarUrl = null;

            if (avatar != null && avatar.Length > 0)
            {
                var uploads = Path.Combine("wwwroot", "avatars");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }
                avatarUrl = $"{Request.Scheme}://{Request.Host}/avatars/{fileName}";
            }

            var newUser = new User
            {
                PhoneNumber = dto.PhoneNumber,
                Password = dto.Password,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AvatarUrl = avatarUrl
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Created(string.Empty, new { message = "User created" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                // Check user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                // Check if user information not valid
                if (user == null || user.Password != dto.Password)
                {
                    return Ok(new { message = "Invalid credentials" });
                }

                // Create and response token back to client
                var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.PhoneNumber);

                return Ok(new { accessToken, user.Id });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
