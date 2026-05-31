namespace ClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ReceptionistId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int Duration { get; set; }
        public string Reason { get; set; }
        public string PriorityLevel { get; set; }
        public string Status { get; set; }
        public int? QueuePosition { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public Appointment()
        {
            Duration = 20;
            PriorityLevel = "Normal";
            Status = "Scheduled";
            CreatedAt = DateTime.Now;
        }

        public Appointment(int patientId, int doctorId, DateTime appointmentDate, TimeSpan appointmentTime)
        {
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            Duration = 20;
            PriorityLevel = "Normal";
            Status = "Scheduled";
            CreatedAt = DateTime.Now;
        }
    }
}
