using AppChat.Data;
using AppChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Services
{
    public class UserService
    {

        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User newUser)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException(nameof(newUser), "New user cannot be null");
            }

            try { 
            
                await _context.Users.AddAsync(newUser);

                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception e)
            {
                throw new Exception("Error creating user: " + e.Message);
            }
        }

        public async Task<User> GetUserById(int Id)
        {
            try
            {
                var user = await _context.Users.FindAsync(Id);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found!");
                }
                return user;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching user: " + e.Message);
            }

        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching all users: " + e.Message);
            }
        }

        public async Task<bool> UpdateUser(int Id, User updatedUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(Id);
                if (user == null) throw new KeyNotFoundException("User not found!");

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.AvatarUrl = updatedUser.AvatarUrl;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error updating user: " + e.Message);

            }
        }

        public async Task<bool> DeleteUser(int Id)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(Id);
                if (existingUser == null) throw new KeyNotFoundException("User not found!");

                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();

                return true;
            }
            catch(Exception e)
            {
                throw new Exception("Error deleting user: " + e.Message);
            }
            
        }


    }
}
