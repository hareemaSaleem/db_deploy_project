using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class DoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.OrderBy(d => d.Name).ToListAsync();
        }
        public async Task<List<Doctor>> GetActiveDoctorsAsync()
        {
            return await _context.Doctors.Where(d => d.Status == "Active").OrderBy(d => d.Name).ToListAsync();
        }
        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == id);
        }
        public async Task<Doctor> GetDoctorByEmailAsync(string email)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.Email.Trim().ToLower() == email.Trim().ToLower());
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Doctors.AnyAsync(d => d.Email == email);
        }
        public async Task<int> AddDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor.DoctorId;
        }
        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await GetDoctorByIdAsync(id);
            if (doctor == null) return false;

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Doctor> GetDoctorByUserIdAsync(int userId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        }
        public async Task<int> GetTotalDoctorsCountAsync()
        {
            return await _context.Doctors.CountAsync();
        }
        public async Task<int> GetActiveDoctorsCountAsync()
        {
            return await _context.Doctors.CountAsync(d => d.Status == "Active");
        }
    }
}