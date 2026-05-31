using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class MedicalRepRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalRepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all medical rep visits
        public async Task<List<MedicalRep>> GetAllMedicalRepsAsync()
        {
            return await _context.MedicalReps
                .Include(m => m.MedicineSamples)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        // Get medical rep by ID
        public async Task<MedicalRep> GetMedicalRepByIdAsync(int id)
        {
            return await _context.MedicalReps
                .Include(m => m.MedicineSamples)
                .FirstOrDefaultAsync(m => m.MedicalRepId == id);
        }

        // Get visits by company name
        public async Task<List<MedicalRep>> GetVisitsByCompanyAsync(string companyName)
        {
            return await _context.MedicalReps
                .Where(m => m.CompanyName.Contains(companyName))
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        // Get visits by date range
        public async Task<List<MedicalRep>> GetVisitsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.MedicalReps
                .Where(m => m.VisitDate.Date >= fromDate.Date && m.VisitDate.Date <= toDate.Date)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        // Add new medical rep visit
        public async Task<int> AddMedicalRepAsync(MedicalRep medicalRep)
        {
            _context.MedicalReps.Add(medicalRep);
            await _context.SaveChangesAsync();
            return medicalRep.MedicalRepId;
        }

        // Update medical rep visit
        public async Task<bool> UpdateMedicalRepAsync(MedicalRep medicalRep)
        {
            _context.MedicalReps.Update(medicalRep);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        // Delete medical rep visit
        public async Task<bool> DeleteMedicalRepAsync(int id)
        {
            var medicalRep = await GetMedicalRepByIdAsync(id);
            if (medicalRep == null) return false;

            _context.MedicalReps.Remove(medicalRep);
            await _context.SaveChangesAsync();
            return true;
        }

        // Add medicine sample to a visit
        public async Task<int> AddMedicineSampleAsync(MedicineSample sample)
        {
            _context.MedicineSamples.Add(sample);
            await _context.SaveChangesAsync();
            return sample.SampleId;
        }

        // Get samples by visit ID
        public async Task<List<MedicineSample>> GetSamplesByVisitIdAsync(int medicalRepId)
        {
            return await _context.MedicineSamples
                .Where(s => s.MedicalRepId == medicalRepId)
                .ToListAsync();
        }
    }
}