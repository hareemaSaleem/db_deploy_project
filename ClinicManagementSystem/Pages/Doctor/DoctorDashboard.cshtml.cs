using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Doctor
{
    public class DoctorDashboardModel : PageModel
    {
        private readonly AppointmentRepository _appointmentRepo;
        private readonly PrescriptionRepository _prescriptionRepo;
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;

        public DoctorDashboardModel(
            AppointmentRepository appointmentRepo,
            PrescriptionRepository prescriptionRepo,
            PatientRepository patientRepo,
            DoctorRepository doctorRepo)
        {
            _appointmentRepo = appointmentRepo;
            _prescriptionRepo = prescriptionRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string ClinicName { get; set; }
        public int TodayAppointmentsCount { get; set; }
        public int TodayPrescriptionsCount { get; set; }
        public int PendingFollowUpsCount { get; set; }
        public int PatientsThisMonthCount { get; set; }
        public List<Prescription> RecentPrescriptions { get; set; }
        public List<Prescription> FollowUpsDue { get; set; }
        public List<Disease> DiseaseTrends { get; set; }
        public Dictionary<string, int> DiseaseDistribution { get; set; }

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
                if (!string.IsNullOrEmpty(doctor.Name))
                {
                    if (doctor.Name.StartsWith("Dr."))
                        DoctorName = doctor.Name;
                    else
                        DoctorName = $"Dr. {doctor.Name}";
                }
                else
                {
                    DoctorName = HttpContext.Session.GetString("Username") ?? "Doctor";
                }

                Specialization = doctor.Specialization ?? "General Physician";
                ClinicName = "City Care Clinic";
            }
            else
            {
                var username = HttpContext.Session.GetString("Username") ?? "Doctor";
                DoctorName = username.StartsWith("dr.", StringComparison.OrdinalIgnoreCase) ?
                    char.ToUpper(username[0]) + username.Substring(1) :
                    $"Dr. {username}";
                Specialization = "General Physician";
                ClinicName = "City Care Clinic";
            }
            var allAppointments = await _appointmentRepo.GetAllAppointmentsAsync();
            var allPrescriptions = await _prescriptionRepo.GetAllPrescriptionsAsync();
            var allPatients = await _patientRepo.GetAllPatientsAsync();
            DiseaseTrends = await _prescriptionRepo.GetDiseaseTrendsAsync(30);
            DiseaseDistribution = await _prescriptionRepo.GetDiseaseDistributionAsync(30);
            TodayAppointmentsCount = 0;
            foreach (var apt in allAppointments)
            {
                if (apt.DoctorId == DoctorId && apt.AppointmentDate.Date == DateTime.Today.Date)
                {
                    TodayAppointmentsCount++;
                }
            }
            TodayPrescriptionsCount = 0;
            foreach (var pres in allPrescriptions)
            {
                if (pres.DoctorId == DoctorId && pres.VisitDate.Date == DateTime.Today.Date)
                {
                    TodayPrescriptionsCount++;
                }
            }
            PendingFollowUpsCount = 0;
            foreach (var pres in allPrescriptions)
            {
                if (pres.DoctorId == DoctorId && pres.FollowUpDate != null && pres.FollowUpDate.Value.Date >= DateTime.Today.Date)
                {
                    PendingFollowUpsCount++;
                }
            }
            var uniquePatients = new List<int>();
            foreach (var apt in allAppointments)
            {
                if (apt.DoctorId == DoctorId && apt.AppointmentDate.Month == DateTime.Today.Month && apt.AppointmentDate.Year == DateTime.Today.Year)
                {
                    bool exists = false;
                    foreach (var id in uniquePatients)
                    {
                        if (id == apt.PatientId)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        uniquePatients.Add(apt.PatientId);
                    }
                }
            }
            PatientsThisMonthCount = uniquePatients.Count;
            RecentPrescriptions = new List<Prescription>();
            var doctorPrescriptions = new List<Prescription>();
            foreach (var pres in allPrescriptions)
            {
                if (pres.DoctorId == DoctorId)
                {
                    foreach (var patient in allPatients)
                    {
                        if (patient.PatientId == pres.PatientId)
                        {
                            pres.Patient = patient;
                            break;
                        }
                    }
                    doctorPrescriptions.Add(pres);
                }
            }
            for (int i = 0; i < doctorPrescriptions.Count - 1; i++)
            {
                for (int j = i + 1; j < doctorPrescriptions.Count; j++)
                {
                    if (doctorPrescriptions[i].VisitDate < doctorPrescriptions[j].VisitDate)
                    {
                        var temp = doctorPrescriptions[i];
                        doctorPrescriptions[i] = doctorPrescriptions[j];
                        doctorPrescriptions[j] = temp;
                    }
                }
            }
            for (int i = 0; i < doctorPrescriptions.Count && i < 5; i++)
            {
                RecentPrescriptions.Add(doctorPrescriptions[i]);
            }
            FollowUpsDue = new List<Prescription>();
            foreach (var pres in allPrescriptions)
            {
                if (pres.DoctorId == DoctorId && pres.FollowUpDate != null && pres.FollowUpDate.Value.Date >= DateTime.Today.Date)
                {
                    foreach (var patient in allPatients)
                    {
                        if (patient.PatientId == pres.PatientId)
                        {
                            pres.Patient = patient;
                            break;
                        }
                    }
                    FollowUpsDue.Add(pres);
                }
            }
            for (int i = 0; i < FollowUpsDue.Count - 1; i++)
            {
                for (int j = i + 1; j < FollowUpsDue.Count; j++)
                {
                    if (FollowUpsDue[i].FollowUpDate > FollowUpsDue[j].FollowUpDate)
                    {
                        var temp = FollowUpsDue[i];
                        FollowUpsDue[i] = FollowUpsDue[j];
                        FollowUpsDue[j] = temp;
                    }
                }
            }

            return Page();
        }
    }
}