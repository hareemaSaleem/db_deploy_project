using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class AdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all admins
        public async Task<List<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins.OrderBy(a => a.Name).ToListAsync();
        }

        // Get admin by ID
        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == id);
        }

        // Get admin by email
        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }

        // Check if email exists
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Admins.AnyAsync(a => a.Email == email);
        }

        // Add new admin
        public async Task<int> AddAdminAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin.AdminId;
        }

        // Update admin
        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        // Delete admin
        public async Task<bool> DeleteAdminAsync(int id)
        {
            var admin = await GetAdminByIdAsync(id);
            if (admin == null) return false;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}