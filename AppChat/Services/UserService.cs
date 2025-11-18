using AppChat.Models;
using AppChat.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public async Task<User> CreateUser(User newUser) => await _repo.CreateUser(newUser);

        public async Task<User> GetUserById(int id) => await _repo.GetUserById(id);

        public async Task<List<User>> GetAllUsersAsync() => await _repo.GetAllUsersAsync();

        public async Task<bool> UpdateUser(int id, User updatedUser)
        {
            var user = await _repo.GetUserById(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.AvatarUrl = updatedUser.AvatarUrl;
            return await _repo.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id) => await _repo.DeleteUser(id);
    }
}
