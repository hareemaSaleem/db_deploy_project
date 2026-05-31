namespace ClinicManagementSystem.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BloodGroup { get; set; }
        public string MaritalStatus { get; set; }
        public string? ChronicDiseases { get; set; }
        public bool RiskFlag { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Appointment> Appointments { get; set; }
        public List<Billing> Billings { get; set; }
        public List<Prescription> Prescriptions { get; set; }

        public Patient()
        {
            RiskFlag = false;
            CreatedAt = DateTime.Now;
            Appointments = new List<Appointment>();
            Billings = new List<Billing>();
            Prescriptions = new List<Prescription>();
        }

        public Patient(string name, string cnic, string phone, string email)
        {
            Name = name;
            CNIC = cnic;
            Phone = phone;
            Email = email;
            RiskFlag = false;
            CreatedAt = DateTime.Now;
            Appointments = new List<Appointment>();
            Billings = new List<Billing>();
            Prescriptions = new List<Prescription>();
        }
    }
}
