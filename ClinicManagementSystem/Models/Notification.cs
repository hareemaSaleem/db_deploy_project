namespace ClinicManagementSystem.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int PrescriptionId { get; set; }
        public string NotificationType { get; set; }  
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? SentDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Prescription Prescription { get; set; }

        public Notification()
        {
            Status = "Pending";
            CreatedAt = DateTime.Now;
            ScheduledDate = DateTime.Now;
        }
    }
}
