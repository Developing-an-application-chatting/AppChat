using AppChat.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]  // https://localhost:5047/Contact
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                int userId = 1; // TODO: Lấy từ token khi Authorize
                var contactInfo = await _service.GetContactsByUserIdAsync(userId);
                return Ok(new { userId, contacts = contactInfo });
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }
    }
}
