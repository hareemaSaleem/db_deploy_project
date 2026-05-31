using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Doctor
{
    public class PrescriptionModel : PageModel
    {
        private readonly PatientRepository _patientRepo;
        private readonly PrescriptionRepository _prescriptionRepo;
        private readonly MedicineRepository _medicineRepo;
        private readonly DoctorRepository _doctorRepo;

        public PrescriptionModel(
            PatientRepository patientRepo,
            PrescriptionRepository prescriptionRepo,
            MedicineRepository medicineRepo,
            DoctorRepository doctorRepo)
        {
            _patientRepo = patientRepo;
            _prescriptionRepo = prescriptionRepo;
            _medicineRepo = medicineRepo;
            _doctorRepo = doctorRepo;
            _doctorRepo = doctorRepo;
        }

        public List<Patient> Patients { get; set; }
        public List<Medicine> Medicines { get; set; }
        public int DoctorId { get; set; }
        public string TodayDate { get; set; }
        public string DoctorName { get; set; }
        [BindProperty]
        public int PatientId { get; set; }

        [BindProperty]
        public DateTime VisitDate { get; set; }

        [BindProperty]
        public string Diagnosis { get; set; }

        [BindProperty]
        public DateTime? FollowUpDate { get; set; }

        [BindProperty]
        public string Symptoms { get; set; }

        [BindProperty]
        public string Notes { get; set; }

        [BindProperty]
        public List<PrescribedMedicine> MedicinesList { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string InfoMessage { get; set; }

        public async Task<JsonResult> OnGetPatientPrescriptions(int patientId)
        {
            var prescriptions = await _prescriptionRepo.GetPrescriptionsByPatientAsync(patientId);

            var result = new List<object>();
            foreach (var pres in prescriptions)
            {
                var medicinesList = new List<string>();
                if (pres.PrescribedMedicines != null)
                {
                    foreach (var med in pres.PrescribedMedicines)
                    {
                        medicinesList.Add($"{med.MedicineName} {med.Dosage} for {med.Duration} days");
                    }
                }

                result.Add(new
                {
                    visitDate = pres.VisitDate.ToString("dd MMM yyyy"),
                    diagnosis = pres.Diagnosis,
                    medicines = string.Join(", ", medicinesList),
                    followUpDate = pres.FollowUpDate?.ToString("dd MMM yyyy") ?? "Not scheduled"
                });
            }

            return new JsonResult(result);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please login first";
                return RedirectToPage("/Shared/Login");
            }

            if (userRole != "Doctor")
            {
                TempData["ErrorMessage"] = "Access denied. Doctor only area.";
                return RedirectToPage("/Shared/Login");
            }

            int loggedInUserId = int.Parse(userId);
            var doctor = await _doctorRepo.GetDoctorByUserIdAsync(loggedInUserId);

            if (doctor != null)
            {
                DoctorId = doctor.DoctorId;  
                DoctorName = doctor.Name;   
            }
            else
            {
                DoctorId = loggedInUserId;
            }

            TodayDate = DateTime.Today.ToString("dddd, MMMM dd, yyyy");
            Patients = await _patientRepo.GetPatientsWithTodaysAppointmentsAsync(DoctorId);
            Medicines = await _medicineRepo.GetAllMedicinesAsync();
            VisitDate = DateTime.Today;

            if (Patients == null || Patients.Count == 0)
            {
                InfoMessage = "You have no appointments scheduled for today. Please check your schedule.";
            }

            return Page();
        }
        public async Task<JsonResult> OnGetCheckRepeatMedicine(int patientId, string medicineName)
        {
            var count = await _prescriptionRepo.GetMedicinePrescriptionCountAsync(patientId, medicineName);
            return new JsonResult(new { count = count, isRepeated = count >= 2 });
        }

       
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                DoctorId = int.Parse(userId);
            }

            if (PatientId <= 0)
            {
                ErrorMessage = "Please select a patient";
                await OnGetAsync();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Diagnosis))
            {
                ErrorMessage = "Please enter diagnosis";
                await OnGetAsync();
                return Page();
            }

            if (MedicinesList == null || MedicinesList.Count == 0)
            {
                ErrorMessage = "Please add at least one medicine";
                await OnGetAsync();
                return Page();
            }

            var prescription = new Prescription
            {
                PatientId = PatientId,
                DoctorId = DoctorId,
                VisitDate = VisitDate,
                Diagnosis = Diagnosis,
                Symptoms = Symptoms,
                FollowUpDate = FollowUpDate,
                Notes = Notes,
                CreatedAt = DateTime.Now,
                PrescribedMedicines = new List<PrescribedMedicine>()
            };

            foreach (var med in MedicinesList)
            {
                prescription.PrescribedMedicines.Add(new PrescribedMedicine
                {
                    MedicineName = med.MedicineName,
                    Dosage = med.Dosage,
                    Duration = med.Duration
                });
            }

            int newId = await _prescriptionRepo.AddPrescriptionAsync(prescription);

            if (newId > 0)
            {
                await _prescriptionRepo.UpdateOrCreateDiseaseAsync(Diagnosis, Symptoms);

                SuccessMessage = "Prescription saved successfully!";
                ModelState.Clear();
                MedicinesList = new List<PrescribedMedicine>();
                PatientId = 0;
                Diagnosis = "";
                Symptoms = "";
                FollowUpDate = null;
                Notes = "";
            }
            else
            {
                ErrorMessage = "Failed to save prescription";
            }

            await OnGetAsync();
            return Page();
        }
    }
}