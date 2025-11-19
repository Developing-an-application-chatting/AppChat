using AppChat.Models;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ContactController(ContactService service)
        {
            _service = service;
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
    }
}
