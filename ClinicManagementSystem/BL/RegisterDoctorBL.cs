using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class RegisterDoctorBL : IRegisterDoctor
    {
        private readonly ILoggingService _logger;

        public RegisterDoctorBL(ILoggingService logger)
        {
            _logger = logger;
        }

        public List<Doctor> GetAllDoctors(List<Doctor> existingDoctors)
        {
            try
            {
                _logger.LogDebug("GetAllDoctors called");

                List<Doctor> sortedDoctors = new List<Doctor>();
                foreach (var doctor in existingDoctors)
                {
                    sortedDoctors.Add(doctor);
                }

                for (int i = 0; i < sortedDoctors.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedDoctors.Count; j++)
                    {
                        if (string.Compare(sortedDoctors[i].Name, sortedDoctors[j].Name) > 0)
                        {
                            Doctor temp = sortedDoctors[i];
                            sortedDoctors[i] = sortedDoctors[j];
                            sortedDoctors[j] = temp;
                        }
                    }
                }

                return sortedDoctors;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllDoctors", ex);
                return new List<Doctor>();
            }
        }

        public List<Doctor> GetActiveDoctors(List<Doctor> existingDoctors)
        {
            try
            {
                _logger.LogDebug("GetActiveDoctors called");

                List<Doctor> result = new List<Doctor>();

                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Status == "Active")
                    {
                        result.Add(doctor);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetActiveDoctors", ex);
                return new List<Doctor>();
            }
        }

        public List<Doctor> GetInactiveDoctors(List<Doctor> existingDoctors)
        {
            try
            {
                _logger.LogDebug("GetInactiveDoctors called");

                List<Doctor> result = new List<Doctor>();

                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Status == "Inactive")
                    {
                        result.Add(doctor);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetInactiveDoctors", ex);
                return new List<Doctor>();
            }
        }

        public List<Doctor> SearchDoctors(string keyword, List<Doctor> existingDoctors)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllDoctors(existingDoctors);

                string searchTerm = keyword.ToLower();
                List<Doctor> results = new List<Doctor>();

                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Name.ToLower().Contains(searchTerm) ||
                        doctor.Email.ToLower().Contains(searchTerm) ||
                        doctor.Phone.Contains(searchTerm) ||
                        doctor.Specialization.ToLower().Contains(searchTerm))
                    {
                        results.Add(doctor);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SearchDoctors", ex);
                return new List<Doctor>();
            }
        }

        public Doctor GetDoctorById(int doctorId, List<Doctor> existingDoctors)
        {
            try
            {
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.DoctorId == doctorId)
                    {
                        return doctor;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetDoctorById", ex);
                return null;
            }
        }

        public Doctor GetDoctorByEmail(string email, List<Doctor> existingDoctors)
        {
            try
            {
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Email.ToLower() == email.ToLower())
                    {
                        return doctor;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetDoctorByEmail", ex);
                return null;
            }
        }

        public bool AddDoctor(Doctor doctor, List<Doctor> existingDoctors, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"AddDoctor called for: {doctor.Name}");

                var validationErrors = ValidateDoctor(doctor);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                if (IsDuplicateEmail(doctor.Email, existingDoctors))
                {
                    errorMessage = "Email already exists";
                    _logger.LogWarning($"Duplicate email: {doctor.Email}");
                    return false;
                }

                if (IsDuplicatePhone(doctor.Phone, existingDoctors))
                {
                    errorMessage = "Phone number already exists";
                    _logger.LogWarning($"Duplicate phone: {doctor.Phone}");
                    return false;
                }

                if (doctor.ConsultationFee < 0)
                {
                    errorMessage = "Consultation fee cannot be negative";
                    _logger.LogWarning($"Negative fee: {doctor.ConsultationFee}");
                    return false;
                }

                if (doctor.Experience < 0)
                {
                    errorMessage = "Experience cannot be negative";
                    _logger.LogWarning($"Negative experience: {doctor.Experience}");
                    return false;
                }

                doctor.CreatedAt = DateTime.Now;

                _logger.LogAudit("Doctor Added", "System",
                    $"Doctor: {doctor.Name}, Specialization: {doctor.Specialization}");
                _logger.LogInfo($"Doctor validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddDoctor", ex);
                errorMessage = "An unexpected error occurred while adding doctor";
                return false;
            }
        }

        public bool UpdateDoctor(int doctorId, Doctor updatedDoctor, List<Doctor> existingDoctors, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateDoctor called for ID: {doctorId}");

                Doctor existingDoctor = null;
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.DoctorId == doctorId)
                    {
                        existingDoctor = doctor;
                        break;
                    }
                }

                if (existingDoctor == null)
                {
                    errorMessage = "Doctor not found";
                    _logger.LogWarning($"Doctor not found. ID: {doctorId}");
                    return false;
                }

                var validationErrors = ValidateDoctor(updatedDoctor);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                foreach (var doctor in existingDoctors)
                {
                    if (doctor.DoctorId != doctorId && doctor.Email == updatedDoctor.Email)
                    {
                        errorMessage = "Email already exists for another doctor";
                        _logger.LogWarning($"Duplicate email: {updatedDoctor.Email}");
                        return false;
                    }
                }

                if (updatedDoctor.ConsultationFee < 0)
                {
                    errorMessage = "Consultation fee cannot be negative";
                    return false;
                }

                _logger.LogAudit("Doctor Updated", "System",
                    $"DoctorId: {doctorId}, Name: {updatedDoctor.Name}");
                _logger.LogInfo($"Doctor validated successfully. ID: {doctorId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateDoctor", ex);
                errorMessage = "An unexpected error occurred while updating doctor";
                return false;
            }
        }

        public bool DeleteDoctor(int doctorId, List<Doctor> existingDoctors, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeleteDoctor called for ID: {doctorId}");
                bool doctorExists = false;
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.DoctorId == doctorId)
                    {
                        doctorExists = true;
                        break;
                    }
                }
                if (!doctorExists)
                {
                    errorMessage = "Doctor not found";
                    _logger.LogWarning($"Doctor not found. ID: {doctorId}");
                    return false;
                }
                _logger.LogAudit("Doctor Deleted", "System", $"DoctorId: {doctorId}");
                _logger.LogInfo($"Doctor validated for deletion. ID: {doctorId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteDoctor", ex);
                errorMessage = "An unexpected error occurred while deleting doctor";
                return false;
            }
        }
        public List<string> ValidateDoctor(Doctor doctor)
        {
            List<string> errors = new List<string>();
            string errorMessage;
            if (!Validators.IsNameValid(doctor.Name, out errorMessage))
            {
                errors.Add($"Doctor Name: {errorMessage}");
            }
            if (string.IsNullOrWhiteSpace(doctor.Specialization))
            {
                errors.Add("Specialization is required");
            }
            if (string.IsNullOrWhiteSpace(doctor.Email))
            {
                errors.Add("Email is required");
            }
            else if (!Validators.IsEmailValid(doctor.Email, out errorMessage))
            {
                errors.Add($"Email: {errorMessage}");
            }
            if (string.IsNullOrWhiteSpace(doctor.Phone))
            {
                errors.Add("Phone number is required");
            }
            else if (!Validators.IsPhoneValid(doctor.Phone, out errorMessage))
            {
                errors.Add($"Phone: {errorMessage}");
            }
            if (string.IsNullOrWhiteSpace(doctor.Qualification))
            {
                errors.Add("Qualification is required");
            }
            return errors;
        }
        public bool IsDuplicateEmail(string email, List<Doctor> existingDoctors)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            foreach (var doctor in existingDoctors)
            {
                if (doctor.Email.ToLower() == email.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsDuplicatePhone(string phone, List<Doctor> existingDoctors)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            foreach (var doctor in existingDoctors)
            {
                if (doctor.Phone == phone)
                {
                    return true;
                }
            }
            return false;
        }
        public int GetTotalDoctorsCount(List<Doctor> existingDoctors)
        {
            try
            {
                return existingDoctors.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalDoctorsCount", ex);
                return 0;
            }
        }
        public int GetActiveDoctorsCount(List<Doctor> existingDoctors)
        {
            try
            {
                int count = 0;
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Status == "Active")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetActiveDoctorsCount", ex);
                return 0;
            }
        }
        public int GetInactiveDoctorsCount(List<Doctor> existingDoctors)
        {
            try
            {
                int count = 0;
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Status == "Inactive")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetInactiveDoctorsCount", ex);
                return 0;
            }
        }
        public int GetTotalSpecializationsCount(List<Doctor> existingDoctors)
        {
            try
            {
                List<string> specializations = new List<string>();
                foreach (var doctor in existingDoctors)
                {
                    bool exists = false;
                    foreach (var spec in specializations)
                    {
                        if (spec == doctor.Specialization)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists && !string.IsNullOrWhiteSpace(doctor.Specialization))
                    {
                        specializations.Add(doctor.Specialization);
                    }
                }
                return specializations.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalSpecializationsCount", ex);
                return 0;
            }
        }
    }
}