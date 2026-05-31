using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class ReceptionistRepository
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all receptionists
        public async Task<List<Receptionist>> GetAllReceptionistsAsync()
        {
            return await _context.Receptionists.OrderBy(r => r.FirstName).ToListAsync();
        }

        // Get active receptionists
        public async Task<List<Receptionist>> GetActiveReceptionistsAsync()
        {
            return await _context.Receptionists
                .Where(r => r.Status == "Active")
                .OrderBy(r => r.FirstName)
                .ToListAsync();  // ✅ Fixed - added ToListAsync()
        }

        // Get receptionist by ID
        public async Task<Receptionist> GetReceptionistByIdAsync(int id)
        {
            return await _context.Receptionists.FirstOrDefaultAsync(r => r.ReceptionistId == id);
        }

        // Get receptionist by email
        public async Task<Receptionist> GetReceptionistByEmailAsync(string email)
        {
            return await _context.Receptionists.FirstOrDefaultAsync(r => r.Email == email);
        }

        // Check if email exists
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Receptionists.AnyAsync(r => r.Email == email);
        }

        // Add new receptionist
        public async Task<int> AddReceptionistAsync(Receptionist receptionist)
        {
            _context.Receptionists.Add(receptionist);
            await _context.SaveChangesAsync();
            return receptionist.ReceptionistId;
        }

        // Update receptionist
        public async Task<bool> UpdateReceptionistAsync(Receptionist receptionist)
        {
            _context.Receptionists.Update(receptionist);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        // Delete receptionist
        public async Task<bool> DeleteReceptionistAsync(int id)
        {
            var receptionist = await GetReceptionistByIdAsync(id);
            if (receptionist == null) return false;

            _context.Receptionists.Remove(receptionist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}