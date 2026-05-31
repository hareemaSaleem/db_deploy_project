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
    public class FollowUpModel : PageModel
    {
        private readonly PrescriptionRepository _prescriptionRepo;
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly NotificationRepository _notificationRepo;

        public FollowUpModel(
            PrescriptionRepository prescriptionRepo,
            PatientRepository patientRepo,
            DoctorRepository doctorRepo,
            NotificationRepository notificationRepo)
        {
            _prescriptionRepo = prescriptionRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
            _notificationRepo = notificationRepo;
        }

        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string ClinicName { get; set; }
        public int DoctorId { get; set; }
        public ReminderStats Stats { get; set; }

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
                DoctorName = doctor.Name.StartsWith("Dr.") ? doctor.Name : $"Dr. {doctor.Name}";
                Specialization = doctor.Specialization ?? "General Physician";
                ClinicName = "City Care Clinic";
            }
            else
            {
                var username = HttpContext.Session.GetString("Username") ?? "Doctor";
                DoctorName = $"Dr. {username}";
                Specialization = "General Physician";
                ClinicName = "City Care Clinic";
            }

            Stats = await _notificationRepo.GetReminderStatsAsync(DoctorId);
            return Page();
        }

        public async Task<JsonResult> OnGetPatientsAsync(string date, string searchTerm)
        {
            DateTime filterDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);

            var allPrescriptions = await _prescriptionRepo.GetAllPrescriptionsAsync();
            var allPatients = await _patientRepo.GetAllPatientsAsync();

            var result = new List<object>();

            foreach (var pres in allPrescriptions)
            {
                if (pres.DoctorId == DoctorId
                    && pres.FollowUpDate != null
                    && pres.FollowUpDate.Value.Date == filterDate.Date
                    && pres.FollowUpDate.Value.Date >= DateTime.Today.Date)
                {
                    var patient = allPatients.FirstOrDefault(p => p.PatientId == pres.PatientId);
                    if (patient != null)
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            var term = searchTerm.ToLower();
                            if (!patient.Name.ToLower().Contains(term) &&
                                !(patient.Email?.ToLower().Contains(term) ?? false) &&
                                !patient.Phone.Contains(term))
                            {
                                continue;
                            }
                        }

                        bool reminderSentToday = await _notificationRepo.IsReminderSentTodayAsync(patient.PatientId, pres.FollowUpDate.Value);
                        Notification lastReminder = reminderSentToday ? await _notificationRepo.GetLastReminderAsync(patient.PatientId) : null;

                        result.Add(new
                        {
                            name = patient.Name,
                            patientId = patient.PatientId,
                            email = patient.Email ?? "",
                            phone = patient.Phone,
                            followUpDate = pres.FollowUpDate.Value.ToString("yyyy-MM-dd"),
                            diagnosis = pres.Diagnosis,
                            prescriptionId = pres.PrescriptionId,
                            reminderSentToday = reminderSentToday,
                            lastReminderTime = lastReminder?.SentDate?.ToString("hh:mm tt") ?? "",
                            lastReminderChannel = lastReminder?.NotificationType ?? ""
                        });
                    }
                }
            }

            return new JsonResult(result);
        }

        public async Task<JsonResult> OnPostSaveRemindersAsync([FromBody] SaveRemindersRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { success = false, message = "Not authenticated" });
                }

                int loggedInUserId = int.Parse(userId);
                var doctor = await _doctorRepo.GetDoctorByUserIdAsync(loggedInUserId);
                if (doctor == null)
                {
                    return new JsonResult(new { success = false, message = "Doctor not found" });
                }

                int savedCount = 0;
                int skippedCount = 0;
                DateTime scheduledDate = request.SendNow ? DateTime.Now : request.ScheduledDateTime;

                foreach (var patient in request.Patients)
                {
                    bool alreadySent = await _notificationRepo.IsReminderSentTodayAsync(patient.PatientId, DateTime.Parse(patient.FollowUpDate));

                    if (alreadySent)
                    {
                        skippedCount++;
                        continue;
                    }

                    var notification = new Notification
                    {
                        PatientId = patient.PatientId,
                        DoctorId = doctor.DoctorId,
                        PrescriptionId = patient.PrescriptionId,
                        NotificationType = request.Channel == "email" ? "Email" : "SMS",
                        Recipient = request.Channel == "email" ? patient.Email : patient.Phone,
                        Subject = "Follow-Up Reminder",
                        Message = GenerateReminderMessage(patient.Name, patient.FollowUpDate, doctor.Name),
                        ScheduledDate = scheduledDate,
                        SentDate = request.SendNow ? DateTime.Now : (DateTime?)null,
                        Status = request.SendNow ? "Sent" : "Pending",
                        CreatedAt = DateTime.Now
                    };

                    await _notificationRepo.SaveNotificationAsync(notification);
                    savedCount++;
                }

                string message = $"{savedCount} reminder(s) saved successfully";
                if (skippedCount > 0)
                {
                    message += $". {skippedCount} skipped (reminder already sent today)";
                }

                return new JsonResult(new { success = true, message = message, count = savedCount, skipped = skippedCount });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private string GenerateReminderMessage(string patientName, string followUpDate, string doctorName)
        {
            DateTime formattedDate = DateTime.Parse(followUpDate);
            string dateString = formattedDate.ToString("dddd, MMMM dd, yyyy");

            return $@"Dear {patientName},

This is a friendly reminder about your follow-up appointment on {dateString}.

Please arrive 10 minutes before your scheduled time.

If you need to reschedule, please contact us at the clinic.

Thank you!
— {doctorName}";
        }
    }
}