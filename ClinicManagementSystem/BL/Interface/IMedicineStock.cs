using System;
using System.Collections.Generic;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IMedicineStock
    {
        // CRUD Operations (receive existingMedicines as parameter)
        bool AddMedicine(Medicine medicine, List<Medicine> existingMedicines, out string errorMessage);
        bool UpdateMedicine(int medicineId, Medicine updatedMedicine, List<Medicine> existingMedicines, out string errorMessage);
        bool DeleteMedicine(int medicineId, List<Medicine> existingMedicines, out string errorMessage);

        // Get Operations (receive existingMedicines as parameter)
        Medicine GetMedicineById(int medicineId, List<Medicine> existingMedicines);
        List<Medicine> GetAllMedicines(List<Medicine> existingMedicines);
        List<Medicine> GetMedicinesByCategory(string category, List<Medicine> existingMedicines);
        List<Medicine> GetMedicinesByManufacturer(string manufacturer, List<Medicine> existingMedicines);
        List<Medicine> GetLowStockMedicines(int threshold, List<Medicine> existingMedicines);
        List<Medicine> GetExpiringMedicines(int daysThreshold, List<Medicine> existingMedicines);
        List<Medicine> SearchMedicines(string keyword, List<Medicine> existingMedicines);

        // Stock Operations (receive existingMedicines as parameter)
        bool UpdateStock(int medicineId, int quantityToAdd, List<Medicine> existingMedicines, out string errorMessage);
        bool ReduceStock(int medicineId, int quantityToReduce, List<Medicine> existingMedicines, out string errorMessage);

        // Validation (pure logic - no storage)
        bool IsDuplicateMedicineName(string medicineName, List<Medicine> existingMedicines);
        void UpdateLowStockAlert(Medicine medicine);
        List<string> ValidateMedicine(Medicine medicine);

        // Statistics (receive existingMedicines as parameter)
        int GetTotalStockValue(List<Medicine> existingMedicines);
        Dictionary<string, int> GetStockSummaryByCategory(List<Medicine> existingMedicines);
    }
}