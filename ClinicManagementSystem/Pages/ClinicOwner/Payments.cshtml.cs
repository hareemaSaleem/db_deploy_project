using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.ClinicOwner
{
    public class PaymentsModel : PageModel
    {
        private readonly BillingRepository _billingRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly ReceptionistRepository _receptionistRepo;

        public PaymentsModel(
            BillingRepository billingRepo,
            DoctorRepository doctorRepo,
            ReceptionistRepository receptionistRepo)
        {
            _billingRepo = billingRepo;
            _doctorRepo = doctorRepo;
            _receptionistRepo = receptionistRepo;
        }

        public List<PaymentDisplay> PaymentsList { get; set; }
        public List<Models.Doctor> DoctorsList { get; set; }
        public List<Models.Receptionist> StaffList { get; set; }

        // Statistics
        public decimal TotalPaymentsThisMonth { get; set; }
        public int TotalPaymentsCount { get; set; }

        // Form properties
        [BindProperty]
        public string PaidToType { get; set; }

        [BindProperty]
        public int RecipientId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public decimal Bonus { get; set; }

        [BindProperty]
        public string PaymentMethod { get; set; }

        [BindProperty]
        public string Notes { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostAddPaymentAsync()
        {
            // Validation
            if (string.IsNullOrEmpty(PaidToType))
            {
                ErrorMessage = "Please select payment type";
                await LoadDashboardData();
                return Page();
            }

            if (RecipientId <= 0)
            {
                ErrorMessage = "Please select recipient";
                await LoadDashboardData();
                return Page();
            }

            if (Amount <= 0)
            {
                ErrorMessage = "Please enter valid amount";
                await LoadDashboardData();
                return Page();
            }

            if (string.IsNullOrEmpty(PaymentMethod))
            {
                ErrorMessage = "Please select payment method";
                await LoadDashboardData();
                return Page();
            }

            // Generate bill number
            string billNumber = GenerateBillNumber();

            // Get recipient name based on type
            string recipientName = "";
            decimal consultationFee = 0;
            decimal medicineCharges = 0;
            decimal otherCharges = 0;

            if (PaidToType == "doctor")
            {
                var doctor = await _doctorRepo.GetDoctorByIdAsync(RecipientId);
                recipientName = doctor?.Name ?? "Unknown";
                consultationFee = Amount;
            }
            else if (PaidToType == "staff")
            {
                var staff = await _receptionistRepo.GetReceptionistByIdAsync(RecipientId);
                recipientName = $"{staff?.FirstName} {staff?.LastName}" ?? "Unknown";
                medicineCharges = Amount;
            }

            // Create billing record
            var billing = new Billing
            {
                PatientId = 1,
                ReceptionistId = 1,
                BillNumber = billNumber,
                ConsultationFee = consultationFee,
                MedicineCharges = medicineCharges,
                OtherCharges = otherCharges,
                Discount = 0,
                TotalAmount = Amount + Bonus,
                PaymentMethod = PaymentMethod,
                PaymentStatus = "Paid",
                BillingDate = DateTime.Now,
                Notes = $"Payment to: {recipientName} ({PaidToType})",
                Bonus = (int)Bonus
            };

            int billingId = await _billingRepo.AddBillingAsync(billing);

            if (billingId > 0)
            {
                SuccessMessage = $"Payment of Rs. {Amount:N0} to {recipientName} recorded successfully!";
                ClearForm();
            }
            else
            {
                ErrorMessage = "Failed to record payment. Please try again.";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<JsonResult> OnGetDoctorsAsync()
        {
            var doctors = await _doctorRepo.GetActiveDoctorsAsync();
            var result = doctors.Select(d => new
            {
                id = d.DoctorId,
                name = d.Name
            }).ToList();
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetStaffAsync()
        {
            var staff = await _receptionistRepo.GetActiveReceptionistsAsync();
            var result = staff.Select(s => new
            {
                id = s.ReceptionistId,
                name = $"{s.FirstName} {s.LastName}"
            }).ToList();
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetSearchPaymentsAsync(string searchTerm)
        {
            var allPayments = await GetAllPaymentsAsync();

            var filteredPayments = allPayments;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                filteredPayments = allPayments
                    .Where(p => p.Name.ToLower().Contains(term) || p.PaidTo.ToLower().Contains(term))
                    .ToList();
            }

            return new JsonResult(filteredPayments);
        }

        private async Task LoadDashboardData()
        {
            // Get all payments
            PaymentsList = await GetAllPaymentsAsync();

            // Get doctors and staff for dropdowns
            DoctorsList = await _doctorRepo.GetActiveDoctorsAsync();
            StaffList = await _receptionistRepo.GetActiveReceptionistsAsync();

            // ✅ FIXED: Calculate statistics without dynamic
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var thisMonthPayments = PaymentsList
                .Where(p => p.Date.Month == currentMonth && p.Date.Year == currentYear)
                .ToList();

            TotalPaymentsThisMonth = thisMonthPayments.Sum(p => p.Amount + p.Bonus);
            TotalPaymentsCount = PaymentsList.Count;
        }

        private async Task<List<PaymentDisplay>> GetAllPaymentsAsync()
        {
            var allPayments = new List<PaymentDisplay>();

            // Get all billings
            var allBillings = await _billingRepo.GetAllBillingsAsync();

            foreach (var billing in allBillings.Where(b => b.PaymentStatus == "Paid"))
            {
                string paidTo = "Patient";
                string name = billing.Patient?.Name ?? "Unknown";

                // Check if this is a doctor payment (ConsultationFee > 0 and Notes contains "Doctor")
                if (billing.ConsultationFee > 0 && billing.Notes != null && billing.Notes.Contains("Doctor"))
                {
                    paidTo = "Doctor";
                    name = ExtractNameFromNotes(billing.Notes);
                }
                // Check if this is a staff payment (MedicineCharges > 0 and Notes contains "Staff")
                else if (billing.MedicineCharges > 0 && billing.Notes != null && billing.Notes.Contains("Staff"))
                {
                    paidTo = "Staff";
                    name = ExtractNameFromNotes(billing.Notes);
                }

                allPayments.Add(new PaymentDisplay
                {
                    Id = billing.BillingId,
                    Date = billing.BillingDate,
                    PaidTo = paidTo,
                    Name = name,
                    Method = billing.PaymentMethod,
                    Amount = paidTo == "Doctor" ? billing.ConsultationFee :
                            (paidTo == "Staff" ? billing.MedicineCharges : billing.TotalAmount),
                    Bonus = billing.Bonus,
                    Status = billing.PaymentStatus,
                    Type = paidTo.ToLower()
                });
            }

            return allPayments.OrderByDescending(p => p.Date).ToList();
        }

        private string ExtractNameFromNotes(string notes)
        {
            if (string.IsNullOrEmpty(notes)) return "Unknown";

            // Extract name from pattern: " - Payment to: Name (Type)"
            var pattern = " - Payment to: ";
            var startIndex = notes.IndexOf(pattern);
            if (startIndex >= 0)
            {
                var namePart = notes.Substring(startIndex + pattern.Length);
                var endIndex = namePart.IndexOf(" (");
                if (endIndex >= 0)
                {
                    return namePart.Substring(0, endIndex);
                }
            }
            return "Unknown";
        }

        private string GenerateBillNumber()
        {
            string prefix = "PAY";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string randomPart = new Random().Next(1000, 9999).ToString();
            return $"{prefix}-{datePart}-{randomPart}";
        }

        private void ClearForm()
        {
            PaidToType = null;
            RecipientId = 0;
            Amount = 0;
            Bonus = 0;
            PaymentMethod = null;
            Notes = null;
        }
    }

    public class PaymentDisplay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PaidTo { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public decimal Bonus { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}