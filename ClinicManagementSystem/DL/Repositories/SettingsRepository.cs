using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using System.Threading.Tasks;

namespace ClinicManagementSystem.DL.Repositories
{
    public class SettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicSettings> GetSettingsAsync()
        {
            var settings = await _context.ClinicSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new ClinicSettings
                {
                    ClinicName = "City Care Clinic",
                    Email = "info@citycareclinic.com",
                    Phone = "+92 300 1234567",
                    Address = "123 Main Street, Gulberg, Lahore",
                    City = "Lahore",
                    ZipCode = "54000",
                    DateFormat = "MMM DD, YYYY",
                    TimeFormat = "12 Hours (hh:mm AM/PM)",
                    Currency = "PKR (Pakistani Rupee)",
                    ItemsPerPage = 10,
                    Language = "English",
                    ThemeMode = "Light"
                };
                _context.ClinicSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }

        public async Task<bool> UpdateSettingsAsync(ClinicSettings settings)
        {
            _context.ClinicSettings.Update(settings);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }
    }
}