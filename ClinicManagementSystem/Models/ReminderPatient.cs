namespace ClinicManagementSystem.Models
{
    public class ReminderPatient
    {
        public string Name { get; set; }
        public int PatientId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FollowUpDate { get; set; }
        public int PrescriptionId { get; set; }
    }
}
