using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class MedicineStockBL : IMedicineStock
    {
        private readonly ILoggingService _logger;
        private static int _nextMedicineId = 1;

        public MedicineStockBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool AddMedicine(Medicine medicine, List<Medicine> existingMedicines, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"AddMedicine called for: {medicine.Name}");
                var validationErrors = ValidateMedicine(medicine);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }
                if (IsDuplicateMedicineName(medicine.Name, existingMedicines))
                {
                    errorMessage = "Medicine with this name already exists";
                    _logger.LogWarning($"Duplicate medicine name: {medicine.Name}");
                    return false;
                }

                if (medicine.StockQuantity < 0)
                {
                    errorMessage = "Stock quantity cannot be negative";
                    _logger.LogWarning($"Negative stock quantity: {medicine.StockQuantity}");
                    return false;
                }

                if (medicine.Price < 0)
                {
                    errorMessage = "Price cannot be negative";
                    _logger.LogWarning($"Negative price: {medicine.Price}");
                    return false;
                }

                if (medicine.ExpiryDate < DateTime.Now)
                {
                    errorMessage = "Expiry date cannot be in the past";
                    _logger.LogWarning($"Expired date: {medicine.ExpiryDate}");
                    return false;
                }

                // Generate ID (UI/DL will use this)
                medicine.MedicineId = _nextMedicineId++;
                medicine.CreatedAt = DateTime.Now;
                medicine.LowStockAlert = medicine.StockQuantity < 20;

                _logger.LogAudit("Medicine Added", "System",
                    $"Name: {medicine.Name}, Stock: {medicine.StockQuantity}, Price: {medicine.Price}");
                _logger.LogInfo($"Medicine validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddMedicine", ex);
                errorMessage = "An unexpected error occurred while adding medicine";
                return false;
            }
        }

        public bool UpdateMedicine(int medicineId, Medicine updatedMedicine, List<Medicine> existingMedicines, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateMedicine called for ID: {medicineId}");

                // Find existing medicine in the provided list
                Medicine existingMedicine = null;
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId == medicineId)
                    {
                        existingMedicine = med;
                        break;
                    }
                }

                if (existingMedicine == null)
                {
                    errorMessage = "Medicine not found";
                    _logger.LogWarning($"Medicine not found. ID: {medicineId}");
                    return false;
                }

                var validationErrors = ValidateMedicine(updatedMedicine);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                // Check duplicate name (excluding current medicine)
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId != medicineId && med.Name == updatedMedicine.Name)
                    {
                        errorMessage = "Medicine with this name already exists";
                        _logger.LogWarning($"Duplicate medicine name: {updatedMedicine.Name}");
                        return false;
                    }
                }

                if (updatedMedicine.ExpiryDate < DateTime.Now)
                {
                    errorMessage = "Expiry date cannot be in the past";
                    _logger.LogWarning($"Expired date: {updatedMedicine.ExpiryDate}");
                    return false;
                }

                _logger.LogAudit("Medicine Updated", "System", $"MedicineId: {medicineId}, Name: {updatedMedicine.Name}");
                _logger.LogInfo($"Medicine validated successfully. ID: {medicineId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateMedicine", ex);
                errorMessage = "An unexpected error occurred while updating medicine";
                return false;
            }
        }

        public bool DeleteMedicine(int medicineId, List<Medicine> existingMedicines, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeleteMedicine called for ID: {medicineId}");

                // Check if medicine exists
                bool medicineExists = false;
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId == medicineId)
                    {
                        medicineExists = true;
                        break;
                    }
                }

                if (!medicineExists)
                {
                    errorMessage = "Medicine not found";
                    _logger.LogWarning($"Medicine not found. ID: {medicineId}");
                    return false;
                }

                _logger.LogAudit("Medicine Deleted", "System", $"MedicineId: {medicineId}");
                _logger.LogInfo($"Medicine validated for deletion. ID: {medicineId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteMedicine", ex);
                errorMessage = "An unexpected error occurred while deleting medicine";
                return false;
            }
        }

        // ============== GET OPERATIONS ==============

        public Medicine GetMedicineById(int medicineId, List<Medicine> existingMedicines)
        {
            try
            {
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId == medicineId)
                        return med;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMedicineById", ex);
                return null;
            }
        }

        public List<Medicine> GetAllMedicines(List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> sortedMedicines = new List<Medicine>();
                foreach (var med in existingMedicines)
                {
                    sortedMedicines.Add(med);
                }

                // Sort by name
                for (int i = 0; i < sortedMedicines.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedMedicines.Count; j++)
                    {
                        if (string.Compare(sortedMedicines[i].Name, sortedMedicines[j].Name) > 0)
                        {
                            Medicine temp = sortedMedicines[i];
                            sortedMedicines[i] = sortedMedicines[j];
                            sortedMedicines[j] = temp;
                        }
                    }
                }

                return sortedMedicines;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllMedicines", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> GetMedicinesByCategory(string category, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();

                foreach (var med in existingMedicines)
                {
                    if (med.Category == category)
                    {
                        result.Add(med);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMedicinesByCategory", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> GetMedicinesByManufacturer(string manufacturer, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();

                foreach (var med in existingMedicines)
                {
                    if (med.Manufacturer == manufacturer)
                    {
                        result.Add(med);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMedicinesByManufacturer", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> GetLowStockMedicines(int threshold, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();

                foreach (var med in existingMedicines)
                {
                    if (med.StockQuantity < threshold)
                    {
                        result.Add(med);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetLowStockMedicines", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> GetExpiringMedicines(int daysThreshold, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();
                DateTime expiryThreshold = DateTime.Now.AddDays(daysThreshold);

                foreach (var med in existingMedicines)
                {
                    if (med.ExpiryDate <= expiryThreshold)
                    {
                        result.Add(med);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetExpiringMedicines", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> SearchMedicines(string keyword, List<Medicine> existingMedicines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return existingMedicines;

                string searchTerm = keyword.ToLower();
                List<Medicine> results = new List<Medicine>();

                foreach (var med in existingMedicines)
                {
                    if (med.Name.ToLower().Contains(searchTerm) ||
                        med.Category.ToLower().Contains(searchTerm) ||
                        med.Manufacturer.ToLower().Contains(searchTerm))
                    {
                        results.Add(med);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SearchMedicines", ex);
                return new List<Medicine>();
            }
        }

        // ============== STOCK OPERATIONS ==============

        public bool UpdateStock(int medicineId, int quantityToAdd, List<Medicine> existingMedicines, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateStock called for MedicineId: {medicineId}, Quantity to add: {quantityToAdd}");

                Medicine medicine = null;
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId == medicineId)
                    {
                        medicine = med;
                        break;
                    }
                }

                if (medicine == null)
                {
                    errorMessage = "Medicine not found";
                    _logger.LogWarning($"Medicine not found. ID: {medicineId}");
                    return false;
                }

                if (quantityToAdd < 0)
                {
                    errorMessage = "Quantity to add cannot be negative";
                    return false;
                }

                _logger.LogAudit("Stock Updated", "System", $"MedicineId: {medicineId}, Added: {quantityToAdd}");
                _logger.LogInfo($"Stock update validated successfully. MedicineId: {medicineId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateStock", ex);
                errorMessage = "An unexpected error occurred while updating stock";
                return false;
            }
        }

        public bool ReduceStock(int medicineId, int quantityToReduce, List<Medicine> existingMedicines, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"ReduceStock called for MedicineId: {medicineId}, Quantity to reduce: {quantityToReduce}");

                Medicine medicine = null;
                foreach (var med in existingMedicines)
                {
                    if (med.MedicineId == medicineId)
                    {
                        medicine = med;
                        break;
                    }
                }

                if (medicine == null)
                {
                    errorMessage = "Medicine not found";
                    _logger.LogWarning($"Medicine not found. ID: {medicineId}");
                    return false;
                }

                if (quantityToReduce < 0)
                {
                    errorMessage = "Quantity to reduce cannot be negative";
                    return false;
                }

                if (medicine.StockQuantity < quantityToReduce)
                {
                    errorMessage = "Insufficient stock available";
                    _logger.LogWarning($"Insufficient stock. Available: {medicine.StockQuantity}, Requested: {quantityToReduce}");
                    return false;
                }

                _logger.LogAudit("Stock Reduced", "System", $"MedicineId: {medicineId}, Reduced: {quantityToReduce}");
                _logger.LogInfo($"Stock reduction validated successfully. MedicineId: {medicineId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ReduceStock", ex);
                errorMessage = "An unexpected error occurred while reducing stock";
                return false;
            }
        }

        // ============== VALIDATION METHODS ==============

        public bool IsDuplicateMedicineName(string medicineName, List<Medicine> existingMedicines)
        {
            if (string.IsNullOrWhiteSpace(medicineName))
                return false;

            foreach (var med in existingMedicines)
            {
                if (med.Name.ToLower() == medicineName.ToLower())
                    return true;
            }
            return false;
        }

        public void UpdateLowStockAlert(Medicine medicine)
        {
            medicine.LowStockAlert = medicine.StockQuantity < 20;
        }

        public List<string> ValidateMedicine(Medicine medicine)
        {
            List<string> errors = new List<string>();
            string errorMessage;

            if (!Validators.IsNameValid(medicine.Name, out errorMessage))
                errors.Add($"Medicine Name: {errorMessage}");

            if (string.IsNullOrWhiteSpace(medicine.Category))
            {
                errors.Add("Category is required");
            }

            if (string.IsNullOrWhiteSpace(medicine.Manufacturer))
            {
                errors.Add("Manufacturer is required");
            }
            else if (!Validators.IsNameValid(medicine.Manufacturer, out errorMessage))
            {
                errors.Add($"Manufacturer: {errorMessage}");
            }

            if (medicine.StockQuantity < 0)
            {
                errors.Add("Stock quantity cannot be negative");
            }

            if (medicine.Price < 0)
            {
                errors.Add("Price cannot be negative");
            }

            if (medicine.Price > 1000000)
            {
                errors.Add("Price cannot exceed 1,000,000");
            }

            if (medicine.ExpiryDate == default)
            {
                errors.Add("Expiry date is required");
            }

            return errors;
        }

        // ============== STATISTICS ==============

        public int GetTotalStockValue(List<Medicine> existingMedicines)
        {
            try
            {
                int totalValue = 0;

                foreach (var med in existingMedicines)
                {
                    totalValue += (int)(med.StockQuantity * med.Price);
                }

                return totalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalStockValue", ex);
                return 0;
            }
        }

        public Dictionary<string, int> GetStockSummaryByCategory(List<Medicine> existingMedicines)
        {
            try
            {
                Dictionary<string, int> summary = new Dictionary<string, int>();

                foreach (var med in existingMedicines)
                {
                    if (summary.ContainsKey(med.Category))
                    {
                        summary[med.Category] += med.StockQuantity;
                    }
                    else
                    {
                        summary[med.Category] = med.StockQuantity;
                    }
                }

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStockSummaryByCategory", ex);
                return new Dictionary<string, int>();
            }
        }
    }
}