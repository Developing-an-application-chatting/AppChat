using AppChat.Data;
using AppChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        //[Authorize]
        [HttpGet]  // https://localhost:5047/Contact
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                //    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                //if (!int.TryParse(userIdClaim, out int userId))
                //    return BadRequest("Invalid user ID in token.");
                int userId = 2;

                var friendList = await _context.Contacts
                    .Where(f => f.UserIdContactA == userId || f.UserIdContactB == userId)
                    .ToListAsync();

                if (friendList == null) return NotFound();

                // Result
                var result = new
                {
                    userId = userId,
                    contacts = friendList.Select(contact => new
                    {
                        id = contact.Id,
                        contactWith = contact.UserIdContactA == userId
                            ? contact.UserIdContactB
                            : contact.UserIdContactA
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }
    }
}
