using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.DL.Repositories
{
    public class MedicineRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all medicines
        public async Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return await _context.Medicines.OrderBy(m => m.Name).ToListAsync();
        }

        // Get medicine by ID
        public async Task<Medicine> GetMedicineByIdAsync(int id)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.MedicineId == id);
        }

        // Get medicine by name
        public async Task<Medicine> GetMedicineByNameAsync(string name)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.Name == name);
        }

        // Check if medicine name exists
        public async Task<bool> IsMedicineNameExistsAsync(string name)
        {
            return await _context.Medicines.AnyAsync(m => m.Name == name);
        }

        // Get low stock medicines (stock < threshold)
        public async Task<List<Medicine>> GetLowStockMedicinesAsync(int threshold = 20)
        {
            return await _context.Medicines
                .Where(m => m.StockQuantity < threshold)
                .OrderBy(m => m.StockQuantity)
                .ToListAsync();
        }

        // Get expiring medicines (within days threshold)
        public async Task<List<Medicine>> GetExpiringMedicinesAsync(int daysThreshold = 90)
        {
            DateTime expiryThreshold = DateTime.Now.AddDays(daysThreshold);

            return await _context.Medicines
                .Where(m => m.ExpiryDate <= expiryThreshold)
                .OrderBy(m => m.ExpiryDate)
                .ToListAsync();
        }

        // Search medicines
        public async Task<List<Medicine>> SearchMedicinesAsync(string keyword)
        {
            return await _context.Medicines
                .Where(m => m.Name.Contains(keyword) ||
                            m.Category.Contains(keyword) ||
                            m.Manufacturer.Contains(keyword))
                .ToListAsync();
        }

        // Add new medicine
        public async Task<int> AddMedicineAsync(Medicine medicine)
        {
            medicine.LowStockAlert = medicine.StockQuantity < 20;
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();
            return medicine.MedicineId;
        }

        // Update medicine
        public async Task<bool> UpdateMedicineAsync(Medicine medicine)
        {
            medicine.LowStockAlert = medicine.StockQuantity < 20;
            _context.Medicines.Update(medicine);
            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        // Update stock quantity
        public async Task<bool> UpdateStockAsync(int medicineId, int quantityToAdd)
        {
            var medicine = await GetMedicineByIdAsync(medicineId);
            if (medicine == null) return false;

            medicine.StockQuantity += quantityToAdd;
            medicine.LowStockAlert = medicine.StockQuantity < 20;

            await _context.SaveChangesAsync();
            return true;
        }

        // Reduce stock (when prescribed)
        public async Task<bool> ReduceStockAsync(int medicineId, int quantityToReduce)
        {
            var medicine = await GetMedicineByIdAsync(medicineId);
            if (medicine == null) return false;

            if (medicine.StockQuantity < quantityToReduce) return false;

            medicine.StockQuantity -= quantityToReduce;
            medicine.LowStockAlert = medicine.StockQuantity < 20;

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete medicine
        public async Task<bool> DeleteMedicineAsync(int id)
        {
            var medicine = await GetMedicineByIdAsync(id);
            if (medicine == null) return false;

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
            return true;
        }
        // Update stock when medicine samples are added
        public async Task<bool> AddOrUpdateStockFromSample(string medicineName, int quantity)
        {
            var existingMedicine = await GetMedicineByNameAsync(medicineName);

            if (existingMedicine != null)
            {
                // Medicine exists - update stock
                existingMedicine.StockQuantity += quantity;
                existingMedicine.LowStockAlert = existingMedicine.StockQuantity < 20;
                _context.Medicines.Update(existingMedicine);
            }
            else
            {
                // New medicine - create entry
                var newMedicine = new Medicine
                {
                    Name = medicineName,
                    Category = "Sample",
                    Manufacturer = "Medical Rep Sample",
                    StockQuantity = quantity,
                    Price = 0,
                    ExpiryDate = DateTime.Now.AddYears(2),
                    LowStockAlert = quantity < 20,
                    CreatedAt = DateTime.Now
                };
                _context.Medicines.Add(newMedicine);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}