namespace ClinicManagementSystem.Models
{
    public class PrescribedMedicine
    {
        public int PrescribedMedicineId { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public int Duration { get; set; }

        public Prescription Prescription { get; set; }
        public Medicine Medicine { get; set; }

        public PrescribedMedicine()
        {

        }

        public PrescribedMedicine(int prescriptionId, string medicineName, string dosage, int duration)
        {
            PrescriptionId = prescriptionId;
            MedicineName = medicineName;
            Dosage = dosage;
            Duration = duration;
        }
    }
}
