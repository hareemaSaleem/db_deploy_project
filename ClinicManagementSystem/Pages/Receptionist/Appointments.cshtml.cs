using ClinicManagementSystem.Data;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoctorModel = ClinicManagementSystem.Models.Doctor;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class AppointmentsModel : PageModel
    {
        private readonly AppointmentRepository _appointmentRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly PatientRepository _patientRepo;

        private readonly ApplicationDbContext _context;

        public AppointmentsModel(
            AppointmentRepository appointmentRepo,
            DoctorRepository doctorRepo,
            PatientRepository patientRepo,
            ApplicationDbContext context)
        {
            _appointmentRepo = appointmentRepo;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _context = context;
        }
        public List<DoctorModel> Doctors { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Appointment> TodayAppointments { get; set; }
        public Dictionary<string, object> AppointmentsData { get; set; }

        [BindProperty]
        public int PatientId { get; set; }

        [BindProperty]
        public int DoctorId { get; set; }

        [BindProperty]
        public DateTime AppointmentDate { get; set; }

        [BindProperty]
        public TimeSpan AppointmentTime { get; set; }

        [BindProperty]
        public string PriorityLevel { get; set; }

        [BindProperty]
        public string Reason { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            var doctorsFromDb = await _doctorRepo.GetAllDoctorsAsync();
            Doctors = new List<DoctorModel>();
            foreach (var d in doctorsFromDb)
            {
                Doctors.Add(d);
            }

            Patients = await _patientRepo.GetAllPatientsAsync();

            AppointmentDate = DateTime.Today;
            AppointmentTime = new TimeSpan(9, 0, 0);
            PriorityLevel = "Normal";

            AppointmentsData = new Dictionary<string, object>();
        }
        public List<string> GenerateTimeSlots()
        {
            var slots = new List<string>();
            for (int hour = 8; hour <= 16; hour++)
            {
                for (int minute = 0; minute < 60; minute += 20)
                {
                    if (hour == 16 && minute > 40) continue;
                    DateTime time = new DateTime(1, 1, 1, hour, minute, 0);
                    slots.Add(time.ToString("h:mm tt"));
                }
            }
            return slots;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PatientId <= 0 || DoctorId <= 0)
            {
                ErrorMessage = "Please select patient and doctor";
                await OnGetAsync();
                return Page();
            }

            bool isAvailable = await _appointmentRepo.IsTimeSlotAvailableAsync(DoctorId, AppointmentDate, AppointmentTime);
            if (!isAvailable)
            {
                ErrorMessage = "Selected time slot is already booked";
                await OnGetAsync();
                return Page();
            }

            var appointment = new Appointment
            {
                PatientId = PatientId,
                DoctorId = DoctorId,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime,
                PriorityLevel = PriorityLevel,
                Reason = Reason,
                Status = "Scheduled",
                Duration = 20,
                CreatedAt = DateTime.Now
            };

            int newId = await _appointmentRepo.BookAppointmentAsync(appointment);

            if (newId > 0)
            {
                SuccessMessage = "Appointment booked successfully!";
                ModelState.Clear();
            }
            else
            {
                ErrorMessage = "Failed to book appointment";
            }

            await OnGetAsync();
            return Page();

        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            bool result = await _appointmentRepo.CancelAppointmentAsync(id);
            if (result)
            {
                SuccessMessage = "Appointment cancelled successfully";
            }
            else
            {
                ErrorMessage = "Failed to cancel appointment";
            }
            return RedirectToPage();
        }
        public async Task<JsonResult> OnGetDoctorSlotsAsync(int doctorId)
        {
            var todayAppts = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            var timeSlots = GenerateTimeSlots();
            var data = new Dictionary<string, object>();

            foreach (var slot in timeSlots)
            {
                var appt = todayAppts.FirstOrDefault(a =>
                {
                    var dt = new DateTime(1, 1, 1, a.AppointmentTime.Hours, a.AppointmentTime.Minutes, 0);
                    return dt.ToString("h:mm tt") == slot;
                });

                data[slot] = new
                {
                    booked = appt != null,
                    patient = appt?.Patient?.Name ?? "",
                    priority = appt?.PriorityLevel ?? ""
                };
            }

            return new JsonResult(data);
        }
    }
}