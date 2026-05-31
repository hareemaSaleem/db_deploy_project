namespace ClinicManagementSystem.Models
{
    public class MedicineSample
    {
        public int SampleId { get; set; }
        public int MedicalRepId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public MedicalRep MedicalRep { get; set; }
        public MedicineSample()
        { }
        public MedicineSample(int medicalRepId, string medicineName, int quantity)
        {
            MedicalRepId = medicalRepId;
            MedicineName = medicineName;
            Quantity = quantity;
        }
    }
}
