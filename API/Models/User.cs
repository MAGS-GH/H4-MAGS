namespace API.Models
{
    public class User : Common
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public double? Lat { get; set; }
        public double? Lng { get; set; }

    }
    public class UserDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
