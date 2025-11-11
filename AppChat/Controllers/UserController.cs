using AppChat.Data;
using AppChat.Models;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserService _service;

        public UserController(AppDbContext context, UserService service)
        {
            _context = context;
            _service = service;
        }

        //[Authorize]
        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            try
            {
                await _service.CreateUser(newUser);
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return BadRequest($"{e.Message} - {e.InnerException?.Message}");
            }
        }

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int Id)
        {
            if (Id == null || Id <= 0) return BadRequest("Invalid Id");
            try { 
            
                return Ok(await _service.GetUserById(Id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        //[Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                return Ok(await _service.GetAllUsersAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        //[Authorize]
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateUser(int Id, User updatedUser)
        {
            if (Id == null || updatedUser == null) throw new ArgumentNullException("Null agruments provided");
            try
            {
                await _service.UpdateUser(Id, updatedUser);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        //[Authorize]
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            if (Id == null || Id <= 0) return BadRequest("Invalid Id");
            try
            {
                await _service.DeleteUser(Id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}



