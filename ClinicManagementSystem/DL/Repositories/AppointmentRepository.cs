using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class AppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all appointments
        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        // Get appointment by ID
        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        // Get appointments by doctor for a specific date
        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        // Get today's appointments for a doctor
        public async Task<List<Appointment>> GetTodaysAppointmentsAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        // Get appointments by patient
        public async Task<List<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        // Check if time slot is available
        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                a.AppointmentTime == time &&
                a.Status != "Cancelled");
        }

        // Book new appointment
        public async Task<int> BookAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment.AppointmentId;
        }

        // Cancel appointment
        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await GetAppointmentByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // Update appointment status
        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            var appointment = await GetAppointmentByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // Get appointment counts by status for a date
        public async Task<Dictionary<string, int>> GetAppointmentCountsByStatusAsync(int doctorId, DateTime date)
        {
            var appointments = await GetAppointmentsByDoctorAndDateAsync(doctorId, date);

            var counts = new Dictionary<string, int>();
            counts["all"] = appointments.Count;
            counts["completed"] = appointments.Count(a => a.Status == "Completed");
            counts["scheduled"] = appointments.Count(a => a.Status == "Scheduled");
            counts["cancelled"] = appointments.Count(a => a.Status == "Cancelled");
            counts["no-show"] = appointments.Count(a => a.Status == "No-Show");

            return counts;
        }
    }
}