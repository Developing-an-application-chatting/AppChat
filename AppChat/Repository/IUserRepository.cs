using AppChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
