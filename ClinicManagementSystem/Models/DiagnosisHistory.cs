namespace ClinicManagementSystem.Models
{
    public class DiagnosisHistory
    {
        public int DiagnosisHistoryId { get; set; }
        public int PrescriptionId { get; set; }
        public int DiseaseId { get; set; }
        public int PatientId { get; set; }
        public DateTime DiagnosisDate { get; set; }

        public Prescription Prescription { get; set; }
        public Disease Disease { get; set; }
        public Patient Patient { get; set; }

        public DiagnosisHistory()
        {
            DiagnosisDate = DateTime.Now;
        }

        public DiagnosisHistory(int prescriptionId, int diseaseId, int patientId)
        {
            PrescriptionId = prescriptionId;
            DiseaseId = diseaseId;
            PatientId = patientId;
            DiagnosisDate = DateTime.Now;
        }
    }
}
