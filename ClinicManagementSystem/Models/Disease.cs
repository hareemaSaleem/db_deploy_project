namespace ClinicManagementSystem.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }
        public string CommonSymptoms { get; set; }
        public string SuggestedMedicines { get; set; }
        public int OccurrenceCount { get; set; }
        public DateTime LastOccurrence { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Trend { get; set; }
        public List<DiagnosisHistory> DiagnosisHistories { get; set; }

        public Disease()
        {
            OccurrenceCount = 0;
            CreatedAt = DateTime.Now;
            DiagnosisHistories = new List<DiagnosisHistory>();
        }

        public Disease(string diseaseName, string commonSymptoms)
        {
            DiseaseName = diseaseName;
            CommonSymptoms = commonSymptoms;
            OccurrenceCount = 0;
            CreatedAt = DateTime.Now;
            DiagnosisHistories = new List<DiagnosisHistory>();
        }
    }
}
