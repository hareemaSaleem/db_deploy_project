using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class MedicalRepModel : PageModel
    {
        private readonly MedicalRepRepository _medicalRepRepo;

        public MedicalRepModel(MedicalRepRepository medicalRepRepo)
        {
            _medicalRepRepo = medicalRepRepo;
        }

        [BindProperty]
        public MedicalRep NewMedicalRep { get; set; }

        [BindProperty]
        public List<MedicineSample> Samples { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            NewMedicalRep = new MedicalRep();
            Samples = new List<MedicineSample>();
            // Add two empty samples by default
            Samples.Add(new MedicineSample());
            Samples.Add(new MedicineSample());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NewMedicalRep.RepName))
                {
                    ErrorMessage = "Representative Name is required";
                    await OnGetAsync();
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(NewMedicalRep.CompanyName))
                {
                    ErrorMessage = "Company Name is required";
                    await OnGetAsync();
                    return Page();
                }

                if (NewMedicalRep.VisitDate == default)
                {
                    ErrorMessage = "Visit Date is required";
                    await OnGetAsync();
                    return Page();
                }

                // Collect samples from form
                var samples = new List<MedicineSample>();
                var medNames = Request.Form["medName"];
                var medQtys = Request.Form["medQty"];
                var medExpiries = Request.Form["medExpiry"];

                bool hasValidSample = false;
                for (int i = 0; i < medNames.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(medNames[i]))
                    {
                        int qty = 0;
                        int.TryParse(medQtys[i], out qty);

                        if (qty > 0)
                        {
                            hasValidSample = true;
                            DateTime expiry = DateTime.Now.AddYears(1);
                            if (!string.IsNullOrWhiteSpace(medExpiries[i]))
                            {
                                DateTime.TryParse(medExpiries[i], out expiry);
                            }

                            samples.Add(new MedicineSample
                            {
                                MedicineName = medNames[i],
                                Quantity = qty,
                                ExpiryDate = expiry
                            });
                        }
                    }
                }

                if (!hasValidSample)
                {
                    ErrorMessage = "At least one medicine sample with quantity > 0 is required";
                    await OnGetAsync();
                    return Page();
                }

                // Create Medical Rep object
                var medicalRep = new MedicalRep
                {
                    RepName = NewMedicalRep.RepName,
                    CompanyName = NewMedicalRep.CompanyName,
                    VisitDate = NewMedicalRep.VisitDate,
                    Remarks = NewMedicalRep.Remarks,
                    CreatedAt = DateTime.Now,
                    MedicineSamples = samples
                };

                // Save to database
                int newId = await _medicalRepRepo.AddMedicalRepAsync(medicalRep);

                if (newId > 0)
                {
                    SuccessMessage = $"✅ Visit recorded successfully for {NewMedicalRep.RepName} from {NewMedicalRep.CompanyName}";
                    await OnGetAsync();
                    return Page();
                }
                else
                {
                    ErrorMessage = "Failed to save visit record";
                    await OnGetAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }
    }
}