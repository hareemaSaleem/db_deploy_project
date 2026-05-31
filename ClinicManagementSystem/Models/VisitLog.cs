using ClinicManagementSystem.Pages.Receptionist;

namespace ClinicManagementSystem.Models
{
    public class VisitLog
    {
        public int VisitLogId { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime VisitDate {  get; set; }
        public int VisitCountInLast10Days { get; set; }
        public bool IsFrequencyVisitWarning { get; set; }
        public string VisitType { get; set; }   
        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
        public VisitLog() 
        {
            VisitDate = DateTime.Now;
            IsFrequencyVisitWarning = false;
        }
        public VisitLog(int patientId , int appointmentId , string visitType)
        {
            PatientId = patientId;
            AppointmentId = appointmentId;
            VisitType = visitType;
            IsFrequencyVisitWarning = false;
            VisitDate = DateTime.Now;
        }
    }
}
