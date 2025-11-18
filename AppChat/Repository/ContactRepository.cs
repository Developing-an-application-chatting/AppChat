using AppChat.Data;
using AppChat.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext context) => _context = context;

        public async Task<List<Contact>> GetContactsByUserId(int userId)
        {
            return await _context.Contacts
                .Where(f => f.UserIdContactA == userId || f.UserIdContactB == userId)
                .ToListAsync();
        }
    }
}
