namespace ClinicManagementSystem.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Specialization { get; set; }
        public string Phone { get; set; }
        public string CNIC { get; set; }
        public string Qualification { get; set; }
        public int Experience { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Status { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Appointment> Appointments { get; set; }
        public List<Prescription> Prescriptions { get; set; }

        public Doctor()
        {
            Status = "Active";
            CreatedAt = DateTime.Now;
            Appointments = new List<Appointment>();
            Prescriptions = new List<Prescription>();
        }

        public Doctor(string name, string email, string password, string specialization)
        {
            Name = name;
            Email = email;
            Password = password;
            Specialization = specialization;
            Status = "Active";
            CreatedAt = DateTime.Now;
            Appointments = new List<Appointment>();
            Prescriptions = new List<Prescription>();
        }
    }
}
