using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.DL.Repositories
{
    public class NotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification.NotificationId;
        }

        public async Task<bool> IsReminderSentTodayAsync(int patientId, DateTime followUpDate)
        {
            return await _context.Notifications
                .AnyAsync(n => n.PatientId == patientId
                            && n.ScheduledDate.Date == DateTime.Today.Date
                            && n.Status == "Sent");
        }

        public async Task<Notification> GetLastReminderAsync(int patientId)
        {
            return await _context.Notifications
                .Where(n => n.PatientId == patientId && n.Status == "Sent")
                .OrderByDescending(n => n.SentDate)
                .FirstOrDefaultAsync();
        }

        public async Task<ReminderStats> GetReminderStatsAsync(int doctorId)
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var stats = new ReminderStats
            {
                SentToday = await _context.Notifications
                    .CountAsync(n => n.DoctorId == doctorId && n.SentDate != null && n.SentDate.Value.Date == today.Date),

                ScheduledForTomorrow = await _context.Notifications
                    .CountAsync(n => n.DoctorId == doctorId && n.Status == "Pending" && n.ScheduledDate.Date == today.AddDays(1).Date),

                UpcomingNext7Days = await _context.Notifications
                    .CountAsync(n => n.DoctorId == doctorId && n.Status == "Pending" && n.ScheduledDate.Date >= today.Date && n.ScheduledDate.Date <= today.AddDays(7).Date),

                TotalSentThisMonth = await _context.Notifications
                    .CountAsync(n => n.DoctorId == doctorId && n.SentDate != null && n.SentDate.Value.Date >= startOfMonth.Date)
            };

            return stats;
        }
    }
}