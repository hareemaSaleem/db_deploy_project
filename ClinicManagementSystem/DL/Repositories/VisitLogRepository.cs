using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class VisitLogRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VisitLog>> GetAllVisitLogsAsync()
        {
            return await _context.VisitLogs.OrderByDescending(v => v.VisitDate).ToListAsync();
        }

        public async Task<int> AddVisitLogAsync(VisitLog visitLog)
        {
            _context.VisitLogs.Add(visitLog);
            await _context.SaveChangesAsync();
            return visitLog.VisitLogId;
        }
    }
}