namespace ClinicManagementSystem.Models
{
    public class PriorityQueue
    {
        public int QueueId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int QueuePosition { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; }

        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }

        public PriorityQueue()
        {
            PriorityLevel = "Normal";
            Status = "Waiting";
            ArrivalTime = DateTime.Now;
        }

        public PriorityQueue(int appointmentId, int patientId, string priorityLevel)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            PriorityLevel = priorityLevel;
            Status = "Waiting";
            ArrivalTime = DateTime.Now;
        }
    }
}
