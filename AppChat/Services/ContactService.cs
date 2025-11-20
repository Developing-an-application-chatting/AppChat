using AppChat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class ContactService
    {
        private readonly IContactRepository _repo;
        private readonly IUserRepository _userRepo;

        public ContactService(IContactRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task<List<object>> GetContactsByUserIdAsync(int userId)
        {
            var friendList = await _repo.GetContactsByUserId(userId);
            var result = new List<object>();

            foreach (var contact in friendList)
            {
                var contactUserId = contact.UserIdContactA == userId ? contact.UserIdContactB : contact.UserIdContactA;
                var u = await _userRepo.GetUserById(contactUserId);

                if (u != null)
                {
                    result.Add(new
                    {
                        contact.Id,
                        User = new
                        {
                            u.Id,
                            FullName = u.LastName + " " + u.FirstName,
                            u.IsOnline,
                            u.PhoneNumber,
                        }
                    });
                }
            }

            return result;
        }
    }
}
