using AppChat.Data;
using AppChat.Hubs;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Repositories;
using AppChat.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMessageRepository _msgRepo;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MessageController> _logger;

        public MessageController(
            IMessageRepository msgRepo,
            IHubContext<ChatHub> hub,
            IWebHostEnvironment env,
            AppDbContext context,
            ILogger<MessageController> logger)
        {
            _msgRepo = msgRepo;
            _hub = hub;
            _env = env;
            _context = context;
            _logger = logger;
        }

        // GET: /message/{chatId}
        [Authorize]
        [HttpGet("{chatId}")]   // https://localhost:5047/message/{chatId}
        public async Task<IActionResult> GetMessages(int chatId)
        {
            try
            {
                var messages = await _msgRepo.GetMessagesByChatIdAsync(chatId);
                // Check if messages are null?
                if (messages == null || !messages.Any())
                {
                    return Ok(new List<Message>());
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        // POST: /message/send
        [RequestSizeLimit(2147483648)]
        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            try
            {
                const long MAX_FILE_BYTES = 2L * 1024 * 1024 * 1024; // 2 GB
                if (dto.File != null && dto.File.Length > MAX_FILE_BYTES)
                {
                    return StatusCode(413, new { message = "Tệp quá lớn. Kích thước tối đa là 2 GB." });
                }

                string? fileUrl = null;

                if (!string.IsNullOrEmpty(dto.FileUrl))
                {
                    fileUrl = dto.FileUrl;
                }
                // ============================
                // 1) Xử lý upload file nếu có
                // ============================
                else if (dto.FileType != "text" && dto.File != null)
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

                    fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                }

                // ====================================
                // 2) Nếu ChatId null → tìm hoặc tạo chat
                // ====================================
                int chatId;

                if (dto.ChatId == null)
                {

                    // tìm xem A và B đã có chat chưa
                    var existingChat = await _context.Chats
                        .FirstOrDefaultAsync(c =>
                            (c.UserAId == dto.SenderId && c.UserBId == dto.ReceiverId) ||
                            (c.UserAId == dto.ReceiverId && c.UserBId == dto.SenderId)
                        );

                    if (existingChat == null)
                    {
                        // tạo chat mới

                        existingChat = new Chat
                        {
                            UserAId = dto.SenderId,
                            UserBId = dto.ReceiverId,
                            LastMessage = "",
                            LastMessageTime = DateTime.UtcNow,
                            UnreadCount = 0
                        };

                        _context.Chats.Add(existingChat);
                        await _context.SaveChangesAsync();
                    }

                    chatId = existingChat.Id;
                }
                else
                {
                    chatId = dto.ChatId.Value;
                }

                // ============================
                // 3) Tạo và lưu Message
                // ============================
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = dto.SenderId,
                    FileType = dto.FileType,
                    Content = dto.FileType == "text" ? dto.Content : null,
                    FileUrl = fileUrl,
                    SentAt = DateTime.UtcNow,
                    Status = "sent"
                };

                _context.Messages.Add(message);

                // ============================
                // 4) Cập nhật Chat (last message)
                // ============================
                var chatUpdate = await _context.Chats.FindAsync(chatId);

                if (dto.FileType == "text")
                    chatUpdate.LastMessage = dto.Content ?? "";
                else
                    chatUpdate.LastMessage = $"[{dto.FileType}]";

                chatUpdate.LastMessageTime = DateTime.UtcNow;

                // tăng unread count cho người nhận
                if (dto.SenderId == chatUpdate.UserAId)
                    chatUpdate.UnreadCount += 1;
                else
                    chatUpdate.UnreadCount += 1;

                await _context.SaveChangesAsync();

                // ============================
                // 5) Build MessageDto
                // ============================
                var sender = await _context.Users.FindAsync(dto.SenderId);
                var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

                var messageDto = new MessageDto
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    SenderId = message.SenderId,
                    SenderName = senderName,
                    Content = message.Content,
                    FileUrl = message.FileUrl,
                    FileType = message.FileType,
                    SentTime = TimeHelper.ConvertToVietnamTime(message.SentAt),
                    Status = message.Status
                };

                // ============================
                // 6) Gửi realtime SignalR
                // ============================

                // Realtime to Sender and Receiver
                await _hub.Clients.User(chatUpdate.UserBId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                await _hub.Clients.User(chatUpdate.UserAId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                // ============================
                // 7) Trả về FE
                // ============================
                return Ok(new
                {
                    chatId = chatId,
                    message = messageDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi gửi tin nhắn", error = ex.Message });
            }
        }



        [HttpPost("upload/init")]
        public IActionResult InitUpload([FromBody] InitUploadRequest req)
        {
            var uploadId = Guid.NewGuid().ToString("N");
            _logger.LogInformation("[UPLOAD INIT] FileName={FileName}, TotalChunks={TotalChunks}, UploadId={UploadId}",
                req.FileName, req.TotalChunks, uploadId);

            var tempDir = Path.Combine(_env.WebRootPath, "uploads_temp", uploadId);
            Directory.CreateDirectory(tempDir);

            System.IO.File.WriteAllText(
                Path.Combine(tempDir, "info.json"),
                JsonSerializer.Serialize(req)
            );

            _logger.LogInformation("[UPLOAD INIT] Created temp folder: {TempDir}", tempDir);

            return Ok(new
            {
                uploadId = uploadId,
                chunkSize = 2 * 1024 * 1024
            });
        }

        public class InitUploadRequest
        {
            public string FileName { get; set; }
            public int TotalChunks { get; set; }
        }



        [HttpPost("upload/chunk")]
        public async Task<IActionResult> UploadChunk(
            [FromForm] string uploadId,
            [FromForm] int chunkIndex,
            [FromForm] IFormFile fileChunk)
        {
            _logger.LogInformation("[UPLOAD CHUNK] UploadId={UploadId}, ChunkIndex={ChunkIndex}, Size={Size}",
                uploadId, chunkIndex, fileChunk?.Length);

            var tempDir = Path.Combine(_env.WebRootPath, "uploads_temp", uploadId);

            if (!Directory.Exists(tempDir))
            {
                _logger.LogWarning("[UPLOAD CHUNK] FAILED - Upload folder not found. UploadId={UploadId}", uploadId);
                return BadRequest("UploadId không tồn tại");
            }

            var chunkPath = Path.Combine(tempDir, $"chunk_{chunkIndex}");

            _logger.LogInformation("[UPLOAD CHUNK] Saving chunk to: {ChunkPath}", chunkPath);

            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await fileChunk.CopyToAsync(stream);
            }

            _logger.LogInformation("[UPLOAD CHUNK] SUCCESS - UploadId={UploadId}, ChunkIndex={ChunkIndex}", uploadId, chunkIndex);

            return Ok(new { received = true });
        }



        [HttpGet("upload/status")]
        public IActionResult GetUploadStatus([FromQuery] string uploadId)
        {
            var tempDir = Path.Combine(_env.WebRootPath, "uploads_temp", uploadId);

            _logger.LogInformation("[UPLOAD STATUS] Checking status for UploadId={UploadId}", uploadId);

            if (!Directory.Exists(tempDir))
            {
                _logger.LogWarning("[UPLOAD STATUS] FAILED - Folder not found. UploadId={UploadId}", uploadId);
                return BadRequest("UploadId không tồn tại");
            }

            var chunks = Directory.GetFiles(tempDir, "chunk_*")
                                  .Select(path => int.Parse(Path.GetFileName(path).Replace("chunk_", "")))
                                  .OrderBy(x => x)
                                  .ToList();

            _logger.LogInformation("[UPLOAD STATUS] UploadId={UploadId}, ReceivedChunks={Count}", uploadId, chunks.Count);

            return Ok(new { uploadedChunks = chunks });
        }



        [HttpPost("upload/complete")]
        public IActionResult CompleteUpload([FromBody] CompleteUploadRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.UploadId))
                    return BadRequest("UploadId không hợp lệ");

                var tempDir = Path.Combine(_env.WebRootPath, "uploads_temp", req.UploadId);

                _logger.LogInformation("[UPLOAD COMPLETE] UploadId={UploadId}, TempDir={TempDir}",
                    req.UploadId, tempDir);

                if (!Directory.Exists(tempDir))
                {
                    _logger.LogWarning("[UPLOAD COMPLETE] Folder not found for {UploadId}", req.UploadId);
                    return BadRequest("UploadId không tồn tại");
                }

                // Đọc metadata từ info.json
                var metaPath = Path.Combine(tempDir, "info.json");
                if (!System.IO.File.Exists(metaPath))
                {
                    _logger.LogWarning("[UPLOAD COMPLETE] info.json missing for {UploadId}", req.UploadId);
                    return BadRequest("Thiếu metadata upload");
                }

                var metaJson = System.IO.File.ReadAllText(metaPath);
                var meta = JsonSerializer.Deserialize<InitUploadRequest>(metaJson);

                if (meta == null)
                    return BadRequest("Lỗi metadata upload");

                // Chuẩn bị thư mục uploads
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var finalPath = Path.Combine(uploadsDir, meta.FileName);

                _logger.LogInformation("[UPLOAD COMPLETE] Merging file: {FinalPath}", finalPath);

                // Ghép file từ các chunk
                using (var finalStream = new FileStream(finalPath, FileMode.Create))
                {
                    for (int i = 0; i < meta.TotalChunks; i++)
                    {
                        var chunkPath = Path.Combine(tempDir, $"chunk_{i}");

                        if (!System.IO.File.Exists(chunkPath))
                        {
                            _logger.LogError("[UPLOAD COMPLETE] Missing chunk {Index} for UploadId={UploadId}",
                                i, req.UploadId);
                            return BadRequest($"Thiếu chunk thứ {i}");
                        }

                        var bytes = System.IO.File.ReadAllBytes(chunkPath);
                        finalStream.Write(bytes, 0, bytes.Length);
                    }
                }

                // Tạo URL trả về FE
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{meta.FileName}";

                _logger.LogInformation("[UPLOAD COMPLETE] Success. FileUrl={FileUrl}", fileUrl);

                // Xoá thư mục tạm
                Directory.Delete(tempDir, true);

                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UPLOAD COMPLETE] Exception");
                return BadRequest(new { message = "Lỗi hoàn tất upload", error = ex.Message });
            }
        }

        public class CompleteUploadRequest
        {
            public string UploadId { get; set; }
        }


    }
}
