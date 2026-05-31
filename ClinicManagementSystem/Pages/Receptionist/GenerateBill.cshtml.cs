using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class GenerateBillModel : PageModel
    {
        private readonly BillingRepository _billingRepo;
        private readonly PatientRepository _patientRepo;

        public GenerateBillModel(BillingRepository billingRepo, PatientRepository patientRepo)
        {
            _billingRepo = billingRepo;
            _patientRepo = patientRepo;
        }

        public List<Patient> Patients { get; set; }
        public List<Billing> Bills { get; set; }

        [BindProperty]
        public int PatientId { get; set; }

        [BindProperty]
        public decimal ConsultationFee { get; set; }

        [BindProperty]
        public decimal MedicineCharges { get; set; }

        [BindProperty]
        public decimal OtherCharges { get; set; }

        [BindProperty]
        public decimal Discount { get; set; }

        [BindProperty]
        public decimal TotalAmount { get; set; }

        [BindProperty]
        public string PaymentMethod { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            // Get all patients for dropdown
            Patients = await _patientRepo.GetAllPatientsAsync();

            // Get all bills for history
            Bills = await _billingRepo.GetAllBillingsAsync();

            // Set default values
            ConsultationFee = 500;
            MedicineCharges = 1000;
            OtherCharges = 0;
            Discount = 0;
            PaymentMethod = "Cash";
            CalculateTotal();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PatientId <= 0)
            {
                ErrorMessage = "Please select a patient";
                await OnGetAsync();
                return Page();
            }

            TotalAmount = ConsultationFee + MedicineCharges + OtherCharges - Discount;

            if (TotalAmount <= 0)
            {
                ErrorMessage = "Total amount must be greater than 0";
                await OnGetAsync();
                return Page();
            }

            var billing = new Billing
            {
                PatientId = PatientId,
                ConsultationFee = ConsultationFee,
                MedicineCharges = MedicineCharges,
                OtherCharges = OtherCharges,
                Discount = Discount,
                TotalAmount = TotalAmount,
                PaymentMethod = PaymentMethod,
                PaymentStatus = "Paid",
                BillingDate = DateTime.Now,
                BillNumber = $"BILL{DateTime.Now:yyyyMMddHHmmss}"
            };

            int newId = await _billingRepo.AddBillingAsync(billing);

            if (newId > 0)
            {
                SuccessMessage = $"Bill generated successfully! Total: PKR {TotalAmount:F2}";

                // Reset form
                PatientId = 0;
                ConsultationFee = 500;
                MedicineCharges = 1000;
                OtherCharges = 0;
                Discount = 0;
                PaymentMethod = "Cash";
                ModelState.Clear();
            }
            else
            {
                ErrorMessage = "Failed to generate bill";
            }

            // Refresh data
            await OnGetAsync();
            return Page();
        }

        private void CalculateTotal()
        {
            TotalAmount = ConsultationFee + MedicineCharges + OtherCharges - Discount;
        }
    }
}