namespace AppChat.Models.DTOs
{
    public class RegisterDTO
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
    }

}
