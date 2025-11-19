using AppChat.Models;
using AppChat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class ChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;

        public ChatService(IChatRepository chatRepo, IUserRepository userRepo, IMessageRepository messageRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
        }

        public async Task<List<object>> GetConversationsAsync(int userId)
        {
            var chats = await _chatRepo.GetChatsByUserId(userId);
            var otherUserIds = chats.Select(c => c.UserAId == userId ? c.UserBId : c.UserAId).Distinct().ToList();
            var usersDict = new Dictionary<int, object>();

            foreach (var id in otherUserIds)
            {
                var u = await _userRepo.GetUserById(id);
                if (u != null)
                    usersDict[id] = new
                    {
                        u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        u.PhoneNumber
                    };
            }

            var result = new List<object>();
            foreach (var chat in chats)
            {
                var messages = await _messageRepo.GetMessagesByChatIdAsync(chat.Id);
                var lastMessage = messages.LastOrDefault();
                var partnerId = chat.UserAId == userId ? chat.UserBId : chat.UserAId;
                var partnerInfo = usersDict.ContainsKey(partnerId) ? usersDict[partnerId] : null;

                int unreadCount = messages
                    .Count(m => m.Status == "sent" && m.SenderId != userId);

                result.Add(new
                {
                    Chat = new
                    {
                        chat.Id,
                        LastMessage = lastMessage?.Content ?? "(Không có tin nhắn)",
                        LastMessageTime = lastMessage?.SentTime,
                        UnreadCount = unreadCount
                    },
                    Info = partnerInfo
                });                                                                         
            }

            return result;
        }
    }
}
