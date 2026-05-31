using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.ClinicOwner
{
    public class RegisterDoctorModel : PageModel
    {
        private readonly DoctorRepository _doctorRepo;
        private readonly UserRepository _userRepo;

        public RegisterDoctorModel(
            DoctorRepository doctorRepo,
            UserRepository userRepo)
        {
            _doctorRepo = doctorRepo;
            _userRepo = userRepo;
        }

        // Statistics
        public int TotalDoctorsCount { get; set; }
        public int ActiveDoctorsCount { get; set; }
        public int InactiveDoctorsCount { get; set; }
        public int TotalSpecializationsCount { get; set; }

        // Doctors List
        public List<Models.Doctor> DoctorsList { get; set; }

        // Form properties
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Cnic { get; set; }
        [BindProperty]

        public string Specialization { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Qualification { get; set; }

        [BindProperty]
        public int Experience { get; set; }

        [BindProperty]
        public decimal ConsultationFee { get; set; }

        [BindProperty]
        public string Status { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostAddDoctorAsync()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Please enter doctor's full name";
                await LoadDashboardData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Specialization))
            {
                ErrorMessage = "Please select specialization";
                await LoadDashboardData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter email address";
                await LoadDashboardData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                ErrorMessage = "Please enter phone number";
                await LoadDashboardData();
                return Page();
            }

            // Check if email already exists
            bool emailExists = await _doctorRepo.IsEmailExistsAsync(Email);
            if (emailExists)
            {
                ErrorMessage = "A doctor with this email already exists";
                await LoadDashboardData();
                return Page();
            }

            // Create username from email (part before @)
            string username = Email.Split('@')[0];

            // Check if username exists, append numbers if needed
            string baseUsername = username;
            int counter = 1;
            while (await _userRepo.IsUsernameExistsAsync(username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            // Create User account
            var user = new User
            {
                Username = username,
                Password = Phone, // Using phone as default password (should be changed by user later)
                Role = "Doctor",
                Email = Email,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            int userId = await _userRepo.AddUserAsync(user);

            // Create Doctor
            var doctor = new Models.Doctor
            {
                UserId = userId,
                Name = FullName,
                CNIC = Cnic,
                Email = Email,
                Password = Phone, // Temporary password
                Specialization = Specialization,
                Phone = Phone,
                Qualification = Qualification,
                Experience = Experience,
                ConsultationFee = ConsultationFee,
                Status = Status,
                DateOfJoining = DateTime.Today,
                CreatedAt = DateTime.Now
            };

            int doctorId = await _doctorRepo.AddDoctorAsync(doctor);

            if (doctorId > 0)
            {
                SuccessMessage = $"Doctor {FullName} added successfully! Username: {username}, Password: {Phone}";
                ClearForm();
            }
            else
            {
                ErrorMessage = "Failed to add doctor. Please try again.";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepo.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                ErrorMessage = "Doctor not found";
                await LoadDashboardData();
                return Page();
            }

            // Delete the doctor
            bool deleted = await _doctorRepo.DeleteDoctorAsync(id);

            // Also delete the associated user
            if (deleted && doctor.UserId > 0)
            {
                await _userRepo.DeleteUserAsync(doctor.UserId);
            }

            if (deleted)
            {
                SuccessMessage = $"Doctor {doctor.Name} deleted successfully!";
            }
            else
            {
                ErrorMessage = "Failed to delete doctor";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnGetSearchDoctorsAsync(string searchTerm)
        {
            var allDoctors = await _doctorRepo.GetAllDoctorsAsync();

            List<Models.Doctor> filteredDoctors;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredDoctors = allDoctors;
            }
            else
            {
                var term = searchTerm.ToLower();
                filteredDoctors = allDoctors
                    .Where(d => d.Name.ToLower().Contains(term) ||
                                d.Email.ToLower().Contains(term) ||
                                d.Phone.Contains(term) ||
                                d.Specialization.ToLower().Contains(term))
                    .ToList();
            }

            var result = filteredDoctors.Select(d => new
            {
                doctorId = d.DoctorId,
                initials = GetInitials(d.Name),
                name = d.Name,
                specialization = d.Specialization,
                email = d.Email,
                phone = d.Phone,
                status = d.Status,
                qualification = d.Qualification,
                experience = d.Experience,
                fee = d.ConsultationFee
            }).ToList();

            return new JsonResult(result);
        }

        private async Task LoadDashboardData()
        {
            var allDoctors = await _doctorRepo.GetAllDoctorsAsync();

            TotalDoctorsCount = allDoctors.Count;
            ActiveDoctorsCount = allDoctors.Count(d => d.Status == "Active");
            InactiveDoctorsCount = allDoctors.Count(d => d.Status == "Inactive");

            // Get unique specializations count
            var uniqueSpecializations = allDoctors.Select(d => d.Specialization).Distinct().ToList();
            TotalSpecializationsCount = uniqueSpecializations.Count;

            DoctorsList = allDoctors.OrderBy(d => d.Name).ToList();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "DR";

            var nameParts = name.Split(' ');
            if (nameParts.Length >= 2)
            {
                return (nameParts[0][0].ToString() + nameParts[1][0].ToString()).ToUpper();
            }
            else
            {
                return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
            }
        }

        private void ClearForm()
        {
            FullName = null;
            Specialization = null;
            Email = null;
            Phone = null;
            Qualification = null;
            Experience = 0;
            ConsultationFee = 0;
            Status = "Active";
        }
    }
}