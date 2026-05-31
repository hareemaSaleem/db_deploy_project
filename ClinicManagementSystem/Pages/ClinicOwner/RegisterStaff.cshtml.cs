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
    public class RegisterStaffModel : PageModel
    {
        private readonly ReceptionistRepository _receptionistRepo;
        private readonly UserRepository _userRepo;

        public RegisterStaffModel(
            ReceptionistRepository receptionistRepo,
            UserRepository userRepo)
        {
            _receptionistRepo = receptionistRepo;
            _userRepo = userRepo;
        }

        // Statistics
        public int TotalStaffCount { get; set; }
        public int ActiveStaffCount { get; set; }
        public int InactiveStaffCount { get; set; }
        public int TotalDepartmentsCount { get; set; }

        // Staff List
        public List<Models.Receptionist> StaffList { get; set; }

        // Form Properties for Add/Update
        [BindProperty]
        public int StaffId { get; set; }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string Cnic { get; set; }

        [BindProperty]
        public string Designation { get; set; }


        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public DateTime DateOfBirth { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public DateTime JoiningDate { get; set; }

        [BindProperty]
        public string Qualification { get; set; }

        [BindProperty]
        public decimal Salary { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public string Status { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostAddStaffAsync()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Please enter staff member's full name";
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
            bool emailExists = await _receptionistRepo.IsEmailExistsAsync(Email);
            if (emailExists)
            {
                ErrorMessage = "A staff member with this email already exists";
                await LoadDashboardData();
                return Page();
            }

            // Calculate age from DateOfBirth
            int age = CalculateAge(DateOfBirth);

            // Create username from email
            string username = Email.Split('@')[0];
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
                Password = Phone,
                Role = "Receptionist",
                Email = Email,
                IsActive = Status == "Active",
                CreatedAt = DateTime.Now
            };

            int userId = await _userRepo.AddUserAsync(user);

            // Create Receptionist/Staff
            var staff = new Models.Receptionist
            {
                UserId = userId,
                FirstName = FullName.Split(' ')[0],
                LastName = FullName.Contains(' ') ? FullName.Substring(FullName.IndexOf(' ') + 1) : "",
                Username = username,
                Password = Phone,
                Gender = Gender,
                CNIC = Cnic,
                Age = age,
                ContactInfo = Phone,
                Email = Email,
                Address = Address ?? "",
                DateOfJoining = JoiningDate,
                Education = Qualification,
                Salary = Salary,
                Status = Status,
                CreatedAt = DateTime.Now
            };

            int staffId = await _receptionistRepo.AddReceptionistAsync(staff);

            if (staffId > 0)
            {
                SuccessMessage = $"Staff member {FullName} added successfully! Username: {username}, Password: {Phone}";
                ClearForm();
            }
            else
            {
                ErrorMessage = "Failed to add staff member. Please try again.";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStaffAsync()
        {
            if (StaffId <= 0)
            {
                ErrorMessage = "Invalid staff ID";
                await LoadDashboardData();
                return Page();
            }

            var existingStaff = await _receptionistRepo.GetReceptionistByIdAsync(StaffId);
            if (existingStaff == null)
            {
                ErrorMessage = "Staff member not found";
                await LoadDashboardData();
                return Page();
            }

            // Calculate age
            int age = CalculateAge(DateOfBirth);

            // Update staff
            existingStaff.FirstName = FullName.Split(' ')[0];
            existingStaff.LastName = FullName.Contains(' ') ? FullName.Substring(FullName.IndexOf(' ') + 1) : "";
            existingStaff.Gender = Gender;
            existingStaff.Age = age;
            existingStaff.ContactInfo = Phone;
            existingStaff.Email = Email;
            existingStaff.Address = Address ?? "";
            existingStaff.DateOfJoining = JoiningDate;
            existingStaff.Education = Qualification;
            existingStaff.Salary = Salary;
            existingStaff.Status = Status;

            bool updated = await _receptionistRepo.UpdateReceptionistAsync(existingStaff);

            if (updated)
            {
                SuccessMessage = $"Staff member {FullName} updated successfully!";
                ClearForm();
            }
            else
            {
                ErrorMessage = "Failed to update staff member";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteStaffAsync(int id)
        {
            var staff = await _receptionistRepo.GetReceptionistByIdAsync(id);
            if (staff == null)
            {
                ErrorMessage = "Staff member not found";
                await LoadDashboardData();
                return Page();
            }

            bool deleted = await _receptionistRepo.DeleteReceptionistAsync(id);

            if (deleted && staff.UserId > 0)
            {
                await _userRepo.DeleteUserAsync(staff.UserId);
            }

            if (deleted)
            {
                SuccessMessage = $"Staff member {staff.FirstName} {staff.LastName} deleted successfully!";
            }
            else
            {
                ErrorMessage = "Failed to delete staff member";
            }

            await LoadDashboardData();
            return Page();
        }

        public async Task<JsonResult> OnGetSearchStaffAsync(string searchTerm)
        {
            var allStaff = await _receptionistRepo.GetAllReceptionistsAsync();

            var filteredStaff = allStaff;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                filteredStaff = allStaff
                    .Where(s => (s.FirstName + " " + s.LastName).ToLower().Contains(term) ||
                                s.Email.ToLower().Contains(term) ||
                                s.ContactInfo.Contains(term) ||
                                (s.Education?.ToLower().Contains(term) ?? false))
                    .ToList();
            }

            var result = filteredStaff.Select(s => new
            {
                staffId = s.ReceptionistId,
                initials = GetInitials(s.FirstName, s.LastName),
                name = $"{s.FirstName} {s.LastName}",
                cnic = s.CNIC ?? "N/A",
                designation = s.Education ?? "Staff",
                age = s.Age,
                email = s.Email,
                phone = s.ContactInfo,
                joiningDate = s.DateOfJoining.ToString("yyyy-MM-dd"),
                status = s.Status,
                qualification = s.Education,
                salary = s.Salary,
                address = s.Address,
                gender = s.Gender,
                dob = CalculateDateOfBirthFromAge(s.Age)
            }).ToList();

            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetStaffByIdAsync(int id)
        {
            var staff = await _receptionistRepo.GetReceptionistByIdAsync(id);
            if (staff == null)
            {
                return new JsonResult(null);
            }

            var result = new
            {
                staffId = staff.ReceptionistId,
                name = $"{staff.FirstName} {staff.LastName}",
                cnic = staff.CNIC ?? "",
                designation = staff.Education ?? "",
                email = staff.Email,
                phone = staff.ContactInfo,
                joiningDate = staff.DateOfJoining.ToString("yyyy-MM-dd"),
                status = staff.Status,
                qualification = staff.Education ?? "",
                salary = staff.Salary,
                address = staff.Address ?? "",
                gender = staff.Gender,
                age = staff.Age
            };

            return new JsonResult(result);
        }

        private async Task LoadDashboardData()
        {
            var allStaff = await _receptionistRepo.GetAllReceptionistsAsync();

            TotalStaffCount = allStaff.Count;
            ActiveStaffCount = allStaff.Count(s => s.Status == "Active");
            InactiveStaffCount = allStaff.Count(s => s.Status == "Inactive");

            var uniqueDepartments = allStaff.Select(s => s.Education).Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
            TotalDepartmentsCount = uniqueDepartments.Count;

            StaffList = allStaff.OrderBy(s => s.FirstName).ToList();
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        private string CalculateDateOfBirthFromAge(int age)
        {
            var today = DateTime.Today;
            var dob = today.AddYears(-age);
            return dob.ToString("yyyy-MM-dd");
        }

        private string GetInitials(string firstName, string lastName)
        {
            string initial1 = string.IsNullOrEmpty(firstName) ? "S" : firstName[0].ToString().ToUpper();
            string initial2 = string.IsNullOrEmpty(lastName) ? "T" : lastName[0].ToString().ToUpper();
            return initial1 + initial2;
        }

        private void ClearForm()
        {
            FullName = null;
            Cnic = null;
            Designation = null;
            Email = null;
            Phone = null;
            DateOfBirth = DateTime.MinValue;
            Gender = null;
            JoiningDate = DateTime.Today;
            Qualification = null;
            Salary = 0;
            Address = null;
            Status = "Active";
        }
    }
}