using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class MedicineStockModel : PageModel
    {
        private readonly MedicineRepository _medicineRepo;

        public MedicineStockModel(MedicineRepository medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        public List<Medicine> Medicines { get; set; }

        public async Task OnGetAsync()
        {
            // ✅ Get medicines from database
            Medicines = await _medicineRepo.GetAllMedicinesAsync();
        }

        // ✅ For adding medicine via AJAX
        public async Task<IActionResult> OnPostAddMedicine([FromBody] Medicine medicine)
        {
            if (medicine == null)
                return new JsonResult(new { success = false });

            medicine.CreatedAt = DateTime.Now;
            medicine.LowStockAlert = medicine.StockQuantity < 20;

            int newId = await _medicineRepo.AddMedicineAsync(medicine);

            return new JsonResult(new { success = newId > 0, id = newId });
        }

        // ✅ For updating medicine via AJAX
        public async Task<IActionResult> OnPostUpdateMedicine([FromBody] Medicine medicine)
        {
            if (medicine == null)
                return new JsonResult(new { success = false });

            medicine.LowStockAlert = medicine.StockQuantity < 20;

            bool updated = await _medicineRepo.UpdateMedicineAsync(medicine);

            return new JsonResult(new { success = updated });
        }
    }
}