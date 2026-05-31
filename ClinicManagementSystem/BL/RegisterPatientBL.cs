using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class RegisterPatientBL : IPatientRegister
    {
        private readonly ILoggingService _logger;
        public RegisterPatientBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool RegisterPatient(Patient patient, List<Patient> existingPatients, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"RegisterPatient started for: {patient.Name}");
                var validationErrors = ValidatePatient(patient);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }
                if (IsDuplicateCnic(patient.CNIC, existingPatients))
                {
                    errorMessage = "CNIC already registered";
                    _logger.LogWarning($"Duplicate CNIC: {patient.CNIC}");
                    return false;
                }
                if (!string.IsNullOrEmpty(patient.Email))
                {
                    if (IsDuplicateEmail(patient.Email, existingPatients))
                    {
                        errorMessage = "Email already registered";
                        _logger.LogWarning($"Duplicate Email: {patient.Email}");
                        return false;
                    }
                }

                var preparedPatient = PreparePatientForSave(patient);
                _logger.LogAudit("Patient Registered", "System", $"Name: {preparedPatient.Name}");
                _logger.LogInfo($"Patient validated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterPatient", ex);
                errorMessage = "An unexpected error occurred. Please try again.";
                return false;
            }
        }
        public bool UpdatePatient(Patient patient, List<Patient> existingPatients, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"UpdatePatient started for ID: {patient.PatientId}");
                Patient existingPatient = null;
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patient.PatientId)
                    {
                        existingPatient = p;
                        break;
                    }
                }
                if (existingPatient == null)
                {
                    errorMessage = "Patient not found";
                    _logger.LogWarning($"Patient not found. ID: {patient.PatientId}");
                    return false;
                }

                var validationErrors = ValidatePatient(patient);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                foreach (var p in existingPatients)
                {
                    if (p.PatientId != patient.PatientId && p.CNIC == patient.CNIC)
                    {
                        errorMessage = "CNIC already registered to another patient";
                        _logger.LogWarning($"Duplicate CNIC: {patient.CNIC}");
                        return false;
                    }
                }

                _logger.LogAudit("Patient Updated", "System", $"PatientId: {patient.PatientId}, Name: {patient.Name}");
                _logger.LogInfo($"Patient validated successfully. ID: {patient.PatientId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdatePatient", ex);
                errorMessage = "An unexpected error occurred. Please try again.";
                return false;
            }
        }

        public bool DeletePatient(int patientId, List<Patient> existingPatients, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeletePatient started for ID: {patientId}");

                bool patientExists = false;
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                    {
                        patientExists = true;
                        break;
                    }
                }

                if (!patientExists)
                {
                    errorMessage = "Patient not found";
                    _logger.LogWarning($"Patient not found. ID: {patientId}");
                    return false;
                }

                _logger.LogAudit("Patient Deleted", "System", $"PatientId: {patientId}");
                _logger.LogInfo($"Patient validated for deletion. ID: {patientId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeletePatient", ex);
                errorMessage = "An unexpected error occurred. Please try again.";
                return false;
            }
        }

        public Patient GetPatientById(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientById", ex);
                return null;
            }
        }

        public List<Patient> GetAllPatients(List<Patient> existingPatients)
        {
            try
            {
                List<Patient> sortedPatients = new List<Patient>();
                foreach (var p in existingPatients)
                {
                    sortedPatients.Add(p);
                }

                for (int i = 0; i < sortedPatients.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedPatients.Count; j++)
                    {
                        if (string.Compare(sortedPatients[i].Name, sortedPatients[j].Name) > 0)
                        {
                            Patient temp = sortedPatients[i];
                            sortedPatients[i] = sortedPatients[j];
                            sortedPatients[j] = temp;
                        }
                    }
                }

                return sortedPatients;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllPatients", ex);
                return new List<Patient>();
            }
        }

        public List<Patient> SearchPatients(string keyword, List<Patient> existingPatients)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return existingPatients;

                string searchTerm = keyword.ToLower();
                List<Patient> results = new List<Patient>();

                foreach (var p in existingPatients)
                {
                    if (p.Name.ToLower().Contains(searchTerm) ||
                        p.CNIC.Contains(searchTerm) ||
                        p.Phone.Contains(searchTerm) ||
                        (p.Email != null && p.Email.ToLower().Contains(searchTerm)) ||
                        (p.City != null && p.City.ToLower().Contains(searchTerm)))
                    {
                        results.Add(p);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SearchPatients", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> GetHighRiskPatients(List<Patient> existingPatients)
        {
            try
            {
                List<Patient> highRiskPatients = new List<Patient>();

                foreach (var p in existingPatients)
                {
                    if (p.RiskFlag)
                        highRiskPatients.Add(p);
                }

                return highRiskPatients;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetHighRiskPatients", ex);
                return new List<Patient>();
            }
        }
        public List<string> ValidatePatient(Patient patient)
        {
            List<string> errors = new List<string>();
            string errorMessage;

            if (!Validators.IsNameValid(patient.Name, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsCnicValid(patient.CNIC, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsPhoneValid(patient.Phone, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsEmailValid(patient.Email, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsDateOfBirthValid(patient.DateOfBirth, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsGenderValid(patient.Gender, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsMaritalStatusValid(patient.MaritalStatus, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsBloodGroupValid(patient.BloodGroup, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsAddressValid(patient.Address, out errorMessage))
                errors.Add(errorMessage);

            if (!Validators.IsCityValid(patient.City, out errorMessage))
                errors.Add(errorMessage);

            return errors;
        }

        public bool IsDuplicateCnic(string cnic, List<Patient> existingPatients)
        {
            if (string.IsNullOrWhiteSpace(cnic))
                return false;

            string cleanCnic = cnic.Replace("-", "");

            foreach (var p in existingPatients)
            {
                string existingCleanCnic = p.CNIC.Replace("-", "");
                if (existingCleanCnic == cleanCnic)
                    return true;
            }

            return false;
        }

        public bool IsDuplicateEmail(string email, List<Patient> existingPatients)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string emailLower = email.ToLower();

            foreach (var p in existingPatients)
            {
                if (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower() == emailLower)
                    return true;
            }

            return false;
        }

        public int CalculateAge(DateTime dateOfBirth)
        {
            return Validators.CalculateAgeFromDateOfBirth(dateOfBirth);
        }

        public bool DetermineRiskFlag(string chronicDiseases)
        {
            return Validators.DetermineRiskFlag(chronicDiseases);
        }

        public Patient PreparePatientForSave(Patient patient)
        {
            Patient preparedPatient = new Patient();

            preparedPatient.Name = patient.Name;
            preparedPatient.CNIC = patient.CNIC;
            preparedPatient.DateOfBirth = patient.DateOfBirth;
            preparedPatient.Gender = patient.Gender;
            preparedPatient.Phone = patient.Phone;
            preparedPatient.Email = patient.Email;
            preparedPatient.Address = patient.Address;
            preparedPatient.City = patient.City;
            preparedPatient.BloodGroup = patient.BloodGroup;
            preparedPatient.MaritalStatus = patient.MaritalStatus;
            preparedPatient.ChronicDiseases = patient.ChronicDiseases;
            preparedPatient.CreatedAt = DateTime.Now;

            if (patient.Age == 0 && patient.DateOfBirth != default)
            {
                preparedPatient.Age = CalculateAge(patient.DateOfBirth);
            }
            else
            {
                preparedPatient.Age = patient.Age;
            }

            preparedPatient.RiskFlag = DetermineRiskFlag(patient.ChronicDiseases);

            return preparedPatient;
        }
    }
}