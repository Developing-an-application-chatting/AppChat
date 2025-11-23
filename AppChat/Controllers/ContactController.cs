using AppChat.Data;
using AppChat.Models;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _service;
        private readonly AppDbContext _context;

        public ContactController(ContactService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [Authorize]
        [HttpGet]  // https://localhost:5047/contact
        public async Task<IActionResult> GetByUserId()
        {
            try
            {

                // Lấy userId từ token
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                var contactInfo = await _service.GetContactsByUserIdAsync(userId);
                // Check if contacts are null?
                if (contactInfo == null || !contactInfo.Any())
                {
                    return Ok(new List<Contact>());
                }

                return Ok(new { userId, contacts = contactInfo });
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }

        [HttpPost("add/{phoneNumber}")]
        public async Task<IActionResult> AddContact(string phoneNumber)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
                if (existingUser == null) return Ok(new { message = "User not existed" });

                var existed = await _context.Contacts
                    .AnyAsync(c => c.UserIdContactA == userId
                                && c.UserIdContactB == existingUser.Id);
                if (existed) return Ok(new { message = "Existed" });

                var newContact = new Contact
                {
                    UserIdContactA = userId,
                    UserIdContactB = existingUser.Id
                };

                await _context.AddAsync(newContact);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Add friend successfully!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
            
        }
    }
}
