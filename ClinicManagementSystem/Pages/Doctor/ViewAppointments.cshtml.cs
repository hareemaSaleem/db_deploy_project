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
    public class ViewAppointmentsModel : PageModel
    {
        private readonly AppointmentRepository _appointmentRepo;
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;

        public ViewAppointmentsModel(
            AppointmentRepository appointmentRepo,
            PatientRepository patientRepo,
            DoctorRepository doctorRepo)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }

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
                Specialization = doctor.Specialization; 
            }
            else
            {
                DoctorName = HttpContext.Session.GetString("Username") ?? "Doctor";
                Specialization = "General Physician";
            }

            return Page();
        }

        public async Task<JsonResult> OnGetFilteredAppointmentsAsync(string date, string status)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new List<object>());
            }

            int loggedInUserId = int.Parse(userId);
            var doctor = await _doctorRepo.GetDoctorByUserIdAsync(loggedInUserId);
            if (doctor == null)
            {
                return new JsonResult(new List<object>());
            }

            int doctorId = doctor.DoctorId;

            var allAppointments = await _appointmentRepo.GetAllAppointmentsAsync();
            var allPatients = await _patientRepo.GetAllPatientsAsync();

            var filteredAppointments = new List<Appointment>();
            foreach (var apt in allAppointments)
            {
                if (apt.DoctorId == doctorId)  
                {
                    filteredAppointments.Add(apt);
                }
            }

            if (!string.IsNullOrEmpty(date))
            {
                DateTime filterDate = DateTime.Parse(date);
                var dateFiltered = new List<Appointment>();
                foreach (var apt in filteredAppointments)
                {
                    if (apt.AppointmentDate.Date == filterDate.Date)
                    {
                        dateFiltered.Add(apt);
                    }
                }
                filteredAppointments = dateFiltered;
            }

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                var statusFiltered = new List<Appointment>();
                foreach (var apt in filteredAppointments)
                {
                    if (apt.Status.ToLower() == status.ToLower())
                    {
                        statusFiltered.Add(apt);
                    }
                }
                filteredAppointments = statusFiltered;
            }

            var result = new List<object>();
            foreach (var apt in filteredAppointments)
            {
                Patient patient = null;
                foreach (var p in allPatients)
                {
                    if (p.PatientId == apt.PatientId)
                    {
                        patient = p;
                        break;
                    }
                }

                result.Add(new
                {
                    id = apt.AppointmentId,
                    patientId = apt.PatientId,
                    patientName = patient?.Name ?? "Unknown",
                    age = patient?.Age ?? 0,
                    gender = patient?.Gender ?? "N/A",
                    contact = patient?.Phone ?? "N/A",
                    date = apt.AppointmentDate.ToString("yyyy-MM-dd"),
                    time = apt.AppointmentTime.ToString(@"hh\:mm"),
                    duration = $"{apt.Duration} min",
                    reason = apt.Reason ?? "N/A",
                    priority = apt.PriorityLevel,
                    status = apt.Status.ToLower()
                });
            }

            return new JsonResult(result);
        }
    }
}