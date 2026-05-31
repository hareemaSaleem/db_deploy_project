using System.Numerics;

namespace ClinicManagementSystem.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; }
        public string Symptoms { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public List<PrescribedMedicine> PrescribedMedicines { get; set; }

        public Prescription()
        {
            VisitDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            PrescribedMedicines = new List<PrescribedMedicine>();
        }

        public Prescription(int patientId, int doctorId, string diagnosis)
        {
            PatientId = patientId;
            DoctorId = doctorId;
            Diagnosis = diagnosis;
            VisitDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            PrescribedMedicines = new List<PrescribedMedicine>();
        }
    }
}
