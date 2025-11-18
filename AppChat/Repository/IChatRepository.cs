using AppChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IChatRepository
    {
        Task<List<Chat>> GetChatsByUserId(int userId);
        Task<Chat> GetChatBetweenUsers(int userAId, int userBId);
        Task<Chat> AddChat(Chat chat);
        Task UpdateChat(Chat chat);
    }
}
