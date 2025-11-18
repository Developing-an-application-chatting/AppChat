using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MessageDto>> GetMessagesByChatIdAsync(int chatId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Join(_context.Users,
                      m => m.SenderId,
                      u => u.Id,
                      (m, u) => new MessageDto
                      {
                          Id = m.Id,
                          ChatId = m.ChatId,
                          SenderId = m.SenderId,
                          SenderName = $"{u.FirstName} {u.LastName}",
                          Content = m.Content,
                          FileUrl = m.FileUrl,
                          FileType = m.FileType,
                          Status = m.Status,
                          SentTime = TimeHelper.ConvertToVietnamTime(m.SentAt)
                      })
                .OrderBy(m => m.Id)
                .ToListAsync();
        }

        public async Task<MessageDto> SendMessageAsync(int? chatId, int senderId, int receiverId, string? content, string fileType, string? fileUrl)
        {
            // 1) Lấy hoặc tạo Chat
            Chat chat;
            if (chatId == null || chatId == 0)
            {
                chat = await _context.Chats
                    .FirstOrDefaultAsync(c =>
                        (c.UserAId == senderId && c.UserBId == receiverId) ||
                        (c.UserAId == receiverId && c.UserBId == senderId));

                if (chat == null)
                {
                    chat = new Chat
                    {
                        UserAId = senderId,
                        UserBId = receiverId,
                        LastMessage = "",
                        LastMessageTime = DateTime.UtcNow,
                        UnreadCount = 0
                    };
                    _context.Chats.Add(chat);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                chat = await _context.Chats.FindAsync(chatId.Value)
                       ?? throw new Exception("Chat not found");
            }

            // 2) Tạo Message
            var msg = new Message
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Content = fileType == "text" ? (content ?? "") : null, // <--- fix null thành ""
                FileUrl = fileUrl,
                FileType = fileType,
                SentAt = DateTime.UtcNow,
                Status = "sent"
            };

            _context.Messages.Add(msg);

            // 3) Cập nhật Chat
            chat.LastMessage = fileType == "text" ? (content ?? "") : $"[{fileType}]";
            chat.LastMessageTime = DateTime.UtcNow;
            chat.UnreadCount += 1;

            await _context.SaveChangesAsync();

            // 4) Tạo MessageDto
            var sender = await _context.Users.FindAsync(senderId);
            var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

            return new MessageDto
            {
                Id = msg.Id,
                ChatId = msg.ChatId,
                SenderId = msg.SenderId,
                SenderName = senderName,
                Content = msg.Content,
                FileUrl = msg.FileUrl,
                FileType = msg.FileType,
                SentTime = TimeHelper.ConvertToVietnamTime(msg.SentAt),
                Status = msg.Status
            };
        }
    }
}
