using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
        public DateTime LastSeen { get; set; }
    }
}
