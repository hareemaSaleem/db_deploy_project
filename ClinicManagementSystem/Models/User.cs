namespace ClinicManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public User ()
        {
            IsActive = true;
            CreatedAt = DateTime.Now;
        }
        public User (string username, string password, string role , string email )
        {
            Username = username;
            Password = password;
            Role = role;
            Email = email;
            IsActive = true;
            CreatedAt = DateTime.Now;
        }

    }
}
