using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class BillingRepository
    {
        private readonly ApplicationDbContext _context;

        public BillingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all bills
        public async Task<List<Billing>> GetAllBillingsAsync()
        {
            return await _context.Billings
                .Include(b => b.Patient)
                .OrderByDescending(b => b.BillingDate)
                .ToListAsync();
        }

        // Get bill by ID
        public async Task<Billing> GetBillingByIdAsync(int id)
        {
            return await _context.Billings
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.BillingId == id);
        }

        // Get bills by patient
        public async Task<List<Billing>> GetBillingsByPatientAsync(int patientId)
        {
            return await _context.Billings
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillingDate)
                .ToListAsync();
        }

        // Get bills by date range
        public async Task<List<Billing>> GetBillingsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Billings
                .Where(b => b.BillingDate.Date >= fromDate.Date && b.BillingDate.Date <= toDate.Date)
                .OrderByDescending(b => b.BillingDate)
                .ToListAsync();
        }

        // Get pending payments
        public async Task<List<Billing>> GetPendingPaymentsAsync()
        {
            return await _context.Billings
                .Include(b => b.Patient)
                .Where(b => b.PaymentStatus == "Pending")
                .OrderBy(b => b.BillingDate)
                .ToListAsync();
        }

        // Generate new bill
        public async Task<int> AddBillingAsync(Billing billing)
        {
            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();
            return billing.BillingId;
        }

        // Update payment status
        public async Task<bool> UpdatePaymentStatusAsync(int id, string status, string paymentMethod)
        {
            var billing = await GetBillingByIdAsync(id);
            if (billing == null) return false;

            billing.PaymentStatus = status;
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                billing.PaymentMethod = paymentMethod;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Get total revenue by date range
        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Billings
                .Where(b => b.BillingDate.Date >= fromDate.Date &&
                           b.BillingDate.Date <= toDate.Date &&
                           b.PaymentStatus == "Paid")
                .SumAsync(b => b.TotalAmount);
        }
        // Add these methods to your existing BillingRepository

        public async Task<bool> UpdateBillingAsync(Billing billing)
        {
            _context.Billings.Update(billing);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteBillingAsync(int id)
        {
            var billing = await GetBillingByIdAsync(id);
            if (billing == null) return false;

            _context.Billings.Remove(billing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}