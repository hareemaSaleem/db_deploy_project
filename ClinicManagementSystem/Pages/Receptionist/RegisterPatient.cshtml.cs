using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class RegisterPatientModel : PageModel
    {
        private readonly PatientRepository _patientRepo;

        public RegisterPatientModel(PatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string CNIC { get; set; }

        [BindProperty]
        public DateTime DateOfBirth { get; set; }

        [BindProperty]
        public int Age { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string MaritalStatus { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string BloodGroup { get; set; }

        [BindProperty]
        public string ChronicDiseases { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            // Empty form
            DateOfBirth = new DateTime(1990, 1, 1);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validation
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(CNIC) || string.IsNullOrEmpty(PhoneNumber))
            {
                ErrorMessage = "Please fill all required fields";
                return Page();
            }

            // Check if CNIC already exists
            bool cnicExists = await _patientRepo.IsCnicExistsAsync(CNIC);
            if (cnicExists)
            {
                ErrorMessage = "CNIC already registered";
                return Page();
            }

            // Create new patient object
            var patient = new Patient
            {
                Name = FullName,
                CNIC = CNIC,
                DateOfBirth = DateOfBirth,
                Age = Age,
                Gender = Gender,
                Phone = PhoneNumber,
                Email = Email,
                MaritalStatus = MaritalStatus,
                Address = Address,
                City = City,
                BloodGroup = BloodGroup,
                ChronicDiseases = ChronicDiseases,
                RiskFlag = !string.IsNullOrEmpty(ChronicDiseases),
                UserId = 3,
                CreatedAt = DateTime.Now
            };

            // Save to database
            int newId = await _patientRepo.AddPatientAsync(patient);

            if (newId > 0)
            {
                SuccessMessage = $"Patient {FullName} registered successfully!";
                ModelState.Clear();
                FullName = "";
                CNIC = "";
                PhoneNumber = "";
                return Page();
            }

            ErrorMessage = "Failed to register patient";
            return Page();
        }
    }
}