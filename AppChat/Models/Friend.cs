using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Friends")]
    public class Friend
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }
}
