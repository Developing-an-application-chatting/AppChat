using AppChat.Data;
using AppChat.Hubs;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Repositories;
using AppChat.Services;
using AppChat.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _msgRepo;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MessageController> _logger;

        public MessageController(
            IMessageRepository msgRepo,
            IHubContext<ChatHub> hub,
            IWebHostEnvironment env,
            ILogger<MessageController> logger)
        {
            _msgRepo = msgRepo;
            _hub = hub;
            _env = env;
            _logger = logger;
        }

        // GET: /message/{chatId}
        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            try
            {
                var messages = await _msgRepo.GetMessagesByChatIdAsync(chatId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        // POST: /message/send
        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            try
            {
                _logger.LogInformation("DTO Received: ChatId={ChatId}, SenderId={SenderId}, ReceiverId={ReceiverId}, Content='{Content}', FileType={FileType}",
                    dto.ChatId, dto.SenderId, dto.ReceiverId, dto.Content, dto.FileType);

                string? fileUrl = null;

                // xử lý file nếu có
                if (dto.File != null && dto.FileType != "text")
                {
                    var wwwRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsDir = Path.Combine(wwwRoot, "uploads");
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    var fileName = Path.GetFileName(dto.File.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    fileUrl = $"/uploads/{fileName}";
                }

                // lưu message qua repository
                var message = await _msgRepo.SendMessageAsync(dto.ChatId, dto.SenderId, dto.ReceiverId, dto.Content, dto.FileType, fileUrl);

                // gửi realtime SignalR
                await _hub.Clients.Group(message.ChatId.ToString())
                    .SendAsync("ReceiveMessage", message);

                // trả về FE
                return Ok(new { chatId = message.ChatId, message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi gửi tin nhắn", error = ex.Message });
            }
        }
    }
}
