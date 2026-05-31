using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class PatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.OrderBy(p => p.Name).ToListAsync();
        }
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        }
        public async Task<Patient> GetPatientByCNICAsync(string cnic)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.CNIC == cnic);
        }
        public async Task<List<Patient>> SearchPatientsAsync(string keyword)
        {
            return await _context.Patients
                .Where(p => p.Name.Contains(keyword) ||
                            p.CNIC.Contains(keyword) ||
                            p.Phone.Contains(keyword))
                .ToListAsync();
        }
        public async Task<bool> IsCnicExistsAsync(string cnic)
        {
            return await _context.Patients.AnyAsync(p => p.CNIC == cnic);
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return await _context.Patients.AnyAsync(p => p.Email == email);
        }
        public async Task<int> AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient.PatientId;
        }
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }
        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await GetPatientByIdAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Patient>> GetPatientsWithTodaysAppointmentsAsync(int doctorId)
        {
            var today = DateTime.Today;
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate.Date == today.Date
                         && a.Status != "Cancelled")  
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            if (patientIds == null || patientIds.Count == 0)
            {
                return new List<Patient>();
            }

            // Get the patient details
            var patients = await _context.Patients
                .Where(p => patientIds.Contains(p.PatientId))
                .OrderBy(p => p.Name)
                .ToListAsync();

            return patients;
        }
        public async Task<List<Patient>> GetPatientsWithAppointmentsForDateAsync(int doctorId, DateTime date)
        {
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            if (patientIds == null || patientIds.Count == 0)
            {
                return new List<Patient>();
            }

            var patients = await _context.Patients
                .Where(p => patientIds.Contains(p.PatientId))
                .OrderBy(p => p.Name)
                .ToListAsync();

            return patients;
        }
        public async Task<List<Patient>> GetHighRiskPatientsAsync()
        {
            return await _context.Patients.Where(p => p.RiskFlag == true).ToListAsync();
        }
        public async Task<int> GetTotalPatientsCountAsync()
        {
            return await _context.Patients.CountAsync();
        }
    }
}