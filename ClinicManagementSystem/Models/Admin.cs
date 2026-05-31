namespace ClinicManagementSystem.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int WorkExperience { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Education { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public Admin()
        {
            Role = "Admin";
            CreatedAt = DateTime.Now;
        }

        public Admin(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Name = firstName + " " + lastName;
            Email = email;
            Password = password;
            Role = "Admin";
            CreatedAt = DateTime.Now;
        }
    }
}
