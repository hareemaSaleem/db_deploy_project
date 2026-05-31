namespace ClinicManagementSystem.Models
{
    public class Billing
    {
        public int BillingId { get; set; }
        public int PatientId { get; set; }
        public int ReceptionistId { get; set; }
        public string BillNumber { get; set; }
        public decimal ConsultationFee { get; set; }
        public decimal MedicineCharges { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime BillingDate { get; set; }
        public string Notes { get; set; }

        public Patient Patient { get; set; }
        public int Bonus { get; set; }

        public Billing()
        {
            PaymentMethod = "Cash";
            PaymentStatus = "Pending";
            BillingDate = DateTime.Now;
        }

        public Billing(int patientId, decimal consultationFee, decimal medicineCharges)
        {
            PatientId = patientId;
            ConsultationFee = consultationFee;
            MedicineCharges = medicineCharges;
            PaymentMethod = "Cash";
            PaymentStatus = "Pending";
            BillingDate = DateTime.Now;
            TotalAmount = consultationFee + medicineCharges;
        }
    }
}
