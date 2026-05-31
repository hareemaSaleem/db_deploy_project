using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class RegisterStaffBL
    {
        private readonly ILoggingService _logger;
        public RegisterStaffBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public List<Receptionist> GetAllStaff(List<Receptionist> existingStaff)
        {
            try
            {
                _logger.LogDebug("GetAllStaff called");
                List<Receptionist> sortedStaff = new List<Receptionist>();
                foreach (var staff in existingStaff)
                {
                    sortedStaff.Add(staff);
                }

                for (int i = 0; i < sortedStaff.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedStaff.Count; j++)
                    {
                        if (string.Compare(sortedStaff[i].FirstName, sortedStaff[j].FirstName) > 0)
                        {
                            Receptionist temp = sortedStaff[i];
                            sortedStaff[i] = sortedStaff[j];
                            sortedStaff[j] = temp;
                        }
                    }
                }

                return sortedStaff;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllStaff", ex);
                return new List<Receptionist>();
            }
        }
        public List<Receptionist> GetActiveStaff(List<Receptionist> existingStaff)
        {
            try
            {
                _logger.LogDebug("GetActiveStaff called");
                List<Receptionist> result = new List<Receptionist>();
                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Active")
                    {
                        result.Add(staff);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetActiveStaff", ex);
                return new List<Receptionist>();
            }
        }

        public List<Receptionist> GetInactiveStaff(List<Receptionist> existingStaff)
        {
            try
            {
                _logger.LogDebug("GetInactiveStaff called");

                List<Receptionist> result = new List<Receptionist>();

                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Inactive")
                    {
                        result.Add(staff);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetInactiveStaff", ex);
                return new List<Receptionist>();
            }
        }

        public List<Receptionist> SearchStaff(string keyword, List<Receptionist> existingStaff)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllStaff(existingStaff);

                string searchTerm = keyword.ToLower();
                List<Receptionist> results = new List<Receptionist>();

                foreach (var staff in existingStaff)
                {
                    string fullName = staff.FirstName + " " + (staff.LastName ?? "");
                    if (fullName.ToLower().Contains(searchTerm) ||
                        staff.Email.ToLower().Contains(searchTerm) ||
                        staff.ContactInfo.Contains(searchTerm) ||
                        staff.Designation.ToLower().Contains(searchTerm))
                    {
                        results.Add(staff);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SearchStaff", ex);
                return new List<Receptionist>();
            }
        }

        public Receptionist GetStaffById(int staffId, List<Receptionist> existingStaff)
        {
            try
            {
                foreach (var staff in existingStaff)
                {
                    if (staff.ReceptionistId == staffId)
                    {
                        return staff;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStaffById", ex);
                return null;
            }
        }

        public Receptionist GetStaffByEmail(string email, List<Receptionist> existingStaff)
        {
            try
            {
                foreach (var staff in existingStaff)
                {
                    if (staff.Email.ToLower() == email.ToLower())
                    {
                        return staff;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStaffByEmail", ex);
                return null;
            }
        }

        public bool AddStaff(Receptionist staff, List<Receptionist> existingStaff, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"AddStaff called for: {staff.FirstName} {staff.LastName}");

                var validationErrors = ValidateStaff(staff);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                if (IsDuplicateEmail(staff.Email, existingStaff))
                {
                    errorMessage = "Email already exists";
                    _logger.LogWarning($"Duplicate email: {staff.Email}");
                    return false;
                }

                if (IsDuplicatePhone(staff.ContactInfo, existingStaff))
                {
                    errorMessage = "Phone number already exists";
                    _logger.LogWarning($"Duplicate phone: {staff.ContactInfo}");
                    return false;
                }

                if (staff.Salary < 0)
                {
                    errorMessage = "Salary cannot be negative";
                    _logger.LogWarning($"Negative salary: {staff.Salary}");
                    return false;
                }

                if (staff.Age < 18 || staff.Age > 65)
                {
                    errorMessage = "Age must be between 18 and 65";
                    _logger.LogWarning($"Invalid age: {staff.Age}");
                    return false;
                }

                staff.CreatedAt = DateTime.Now;

                _logger.LogAudit("Staff Added", "System",
                    $"Staff: {staff.FirstName} {staff.LastName}, Designation: {staff.Designation}");
                _logger.LogInfo($"Staff validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddStaff", ex);
                errorMessage = "An unexpected error occurred while adding staff";
                return false;
            }
        }

        public bool UpdateStaff(int staffId, Receptionist updatedStaff, List<Receptionist> existingStaff, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateStaff called for ID: {staffId}");

                Receptionist existingStaffMember = null;
                foreach (var staff in existingStaff)
                {
                    if (staff.ReceptionistId == staffId)
                    {
                        existingStaffMember = staff;
                        break;
                    }
                }

                if (existingStaffMember == null)
                {
                    errorMessage = "Staff member not found";
                    _logger.LogWarning($"Staff not found. ID: {staffId}");
                    return false;
                }

                var validationErrors = ValidateStaff(updatedStaff);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                foreach (var staff in existingStaff)
                {
                    if (staff.ReceptionistId != staffId && staff.Email == updatedStaff.Email)
                    {
                        errorMessage = "Email already exists for another staff member";
                        _logger.LogWarning($"Duplicate email: {updatedStaff.Email}");
                        return false;
                    }
                }

                if (updatedStaff.Salary < 0)
                {
                    errorMessage = "Salary cannot be negative";
                    return false;
                }

                if (updatedStaff.Age < 18 || updatedStaff.Age > 65)
                {
                    errorMessage = "Age must be between 18 and 65";
                    return false;
                }

                _logger.LogAudit("Staff Updated", "System",
                    $"StaffId: {staffId}, Name: {updatedStaff.FirstName} {updatedStaff.LastName}");
                _logger.LogInfo($"Staff validated successfully. ID: {staffId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateStaff", ex);
                errorMessage = "An unexpected error occurred while updating staff";
                return false;
            }
        }

        public bool DeleteStaff(int staffId, List<Receptionist> existingStaff, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeleteStaff called for ID: {staffId}");

                bool staffExists = false;
                foreach (var staff in existingStaff)
                {
                    if (staff.ReceptionistId == staffId)
                    {
                        staffExists = true;
                        break;
                    }
                }

                if (!staffExists)
                {
                    errorMessage = "Staff member not found";
                    _logger.LogWarning($"Staff not found. ID: {staffId}");
                    return false;
                }
                _logger.LogAudit("Staff Deleted", "System", $"StaffId: {staffId}");
                _logger.LogInfo($"Staff validated for deletion. ID: {staffId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteStaff", ex);
                errorMessage = "An unexpected error occurred while deleting staff";
                return false;
            }
        }
        public List<string> ValidateStaff(Receptionist staff)
        {
            List<string> errors = new List<string>();
            string errorMessage;
            string fullName = staff.FirstName + " " + (staff.LastName ?? "");
            if (!Validators.IsNameValid(fullName, out errorMessage))
            {
                errors.Add($"Staff Name: {errorMessage}");
            }
            if (string.IsNullOrWhiteSpace(staff.Designation))
            {
                errors.Add("Designation is required");
            }
            if (string.IsNullOrWhiteSpace(staff.Email))
            {
                errors.Add("Email is required");
            }
            else if (!Validators.IsEmailValid(staff.Email, out errorMessage))
            {
                errors.Add($"Email: {errorMessage}");
            }

            if (string.IsNullOrWhiteSpace(staff.ContactInfo))
            {
                errors.Add("Phone number is required");
            }
            else if (!Validators.IsPhoneValid(staff.ContactInfo, out errorMessage))
            {
                errors.Add($"Phone: {errorMessage}");
            }

            if (string.IsNullOrWhiteSpace(staff.CNIC))
            {
                errors.Add("CNIC is required");
            }
            else if (!Validators.IsCnicValid(staff.CNIC, out errorMessage))
            {
                errors.Add($"CNIC: {errorMessage}");
            }
            if (string.IsNullOrWhiteSpace(staff.Education))
            {
                errors.Add("Qualification is required");
            }
            if (string.IsNullOrWhiteSpace(staff.Gender))
            {
                errors.Add("Gender is required");
            }
            if (staff.DateOfJoining == default)
            {
                errors.Add("Joining Date is required");
            }
            return errors;
        }
        public bool IsDuplicateEmail(string email, List<Receptionist> existingStaff)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            foreach (var staff in existingStaff)
            {
                if (staff.Email.ToLower() == email.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsDuplicatePhone(string phone, List<Receptionist> existingStaff)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            foreach (var staff in existingStaff)
            {
                if (staff.ContactInfo == phone)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsDuplicateCNIC(string cnic, List<Receptionist> existingStaff)
        {
            if (string.IsNullOrWhiteSpace(cnic))
                return false;
            foreach (var staff in existingStaff)
            {
                if (staff.CNIC == cnic)
                {
                    return true;
                }
            }
            return false;
        }
        public int GetTotalStaffCount(List<Receptionist> existingStaff)
        {
            try
            {
                return existingStaff.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalStaffCount", ex);
                return 0;
            }
        }
        public int GetActiveStaffCount(List<Receptionist> existingStaff)
        {
            try
            {
                int count = 0;
                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Active")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetActiveStaffCount", ex);
                return 0;
            }
        }
        public int GetInactiveStaffCount(List<Receptionist> existingStaff)
        {
            try
            {
                int count = 0;
                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Inactive")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetInactiveStaffCount", ex);
                return 0;
            }
        }
        public int GetTotalDepartmentsCount(List<Receptionist> existingStaff)
        {
            try
            {
                List<string> departments = new List<string>();
                foreach (var staff in existingStaff)
                {
                    bool exists = false;
                    foreach (var dept in departments)
                    {
                        if (dept == staff.Designation)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists && !string.IsNullOrWhiteSpace(staff.Designation))
                    {
                        departments.Add(staff.Designation);
                    }
                }
                return departments.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalDepartmentsCount", ex);
                return 0;
            }
        }
        public int CalculateAgeFromDateOfBirth(DateTime dateOfBirth)
        {
            int age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;
            return age;
        }
    }
}