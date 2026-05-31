namespace ClinicManagementSystem.Models
{
    public class Receptionist
    {
        public int ReceptionistId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ContactInfo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Education { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Designation { get; set; }
        public string CNIC { get; set; }

        public Receptionist()
        {
            Status = "Active";
            CreatedAt = DateTime.Now;
        }

        public Receptionist(string firstName, string lastName, string username, string password, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            Email = email;
            Status = "Active";
            CreatedAt = DateTime.Now;
        }
    }
}
