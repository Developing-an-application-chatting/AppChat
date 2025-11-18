using AppChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetContactsByUserId(int userId);
    }
}
