using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class PrescriptionBL : IPrescription
    {
        private readonly ILoggingService _logger;
        public PrescriptionBL(ILoggingService logger)
        {
            _logger = logger;
        }

        public List<Patient> GetAllPatients(List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug("GetAllPatients called");
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

        public List<Prescription> GetPatientPrescriptions(int patientId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug($"GetPatientPrescriptions called for PatientId: {patientId}");
                List<Prescription> result = new List<Prescription>();
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.PatientId == patientId)
                    {
                        result.Add(pres);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientPrescriptions", ex);
                return new List<Prescription>();
            }
        }

        public List<Prescription> GetPatientPrescriptionsSorted(int patientId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug($"GetPatientPrescriptionsSorted called for PatientId: {patientId}");
                List<Prescription> result = new List<Prescription>();
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.PatientId == patientId)
                    {
                        result.Add(pres);
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].VisitDate < result[j].VisitDate)
                        {
                            Prescription temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientPrescriptionsSorted", ex);
                return new List<Prescription>();
            }
        }

        public int GetMedicinePrescriptionCount(int patientId, string medicineName, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug($"GetMedicinePrescriptionCount called for PatientId: {patientId}, Medicine: {medicineName}");

                int count = 0;

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.PatientId == patientId && pres.PrescribedMedicines != null)
                    {
                        foreach (var med in pres.PrescribedMedicines)
                        {
                            if (med.MedicineName.ToLower() == medicineName.ToLower())
                            {
                                count++;
                            }
                        }
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMedicinePrescriptionCount", ex);
                return 0;
            }
        }

        public bool IsMedicineRepeated(int patientId, string medicineName, List<Prescription> existingPrescriptions, int threshold)
        {
            try
            {
                int count = GetMedicinePrescriptionCount(patientId, medicineName, existingPrescriptions);
                return count >= threshold;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in IsMedicineRepeated", ex);
                return false;
            }
        }

        public List<string> ValidatePrescription(Prescription prescription, List<Medicine> existingMedicines)
        {
            List<string> errors = new List<string>();
            string errorMessage;

            if (prescription.PatientId <= 0)
            {
                errors.Add("Valid patient is required");
            }
            if (prescription.DoctorId <= 0)
            {
                errors.Add("Valid doctor is required");
            }
            if (string.IsNullOrWhiteSpace(prescription.Diagnosis))
            {
                errors.Add("Diagnosis is required");
            }
            if (!Validators.IsDateNotInFuture(prescription.VisitDate, out errorMessage))
            {
                errors.Add($"Visit Date: {errorMessage}");
            }
            if (prescription.PrescribedMedicines == null || prescription.PrescribedMedicines.Count == 0)
            {
                errors.Add("At least one medicine is required");
            }
            else
            {
                foreach (var med in prescription.PrescribedMedicines)
                {
                    if (string.IsNullOrWhiteSpace(med.MedicineName))
                    {
                        errors.Add("Medicine name cannot be empty");
                        break;
                    }
                    // ✅ FIXED: Removed incorrect null check for int type
                    if (med.Duration <= 0)
                    {
                        errors.Add($"Duration for {med.MedicineName} must be greater than 0");
                    }
                }
            }
            return errors;
        }

        public Prescription PreparePrescriptionForSave(Prescription prescription, int doctorId)
        {
            prescription.DoctorId = doctorId;
            prescription.CreatedAt = DateTime.Now;

            if (prescription.VisitDate == default)
            {
                prescription.VisitDate = DateTime.Now;
            }

            return prescription;
        }

        public bool SavePrescription(Prescription prescription, List<PrescribedMedicine> prescribedMedicines, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"SavePrescription called for PatientId: {prescription.PatientId}");
                var validationErrors = ValidatePrescription(prescription, new List<Medicine>());
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }
                if (prescribedMedicines == null || prescribedMedicines.Count == 0)
                {
                    errorMessage = "At least one medicine is required";
                    _logger.LogWarning("No medicines provided");
                    return false;
                }
                foreach (var med in prescribedMedicines)
                {
                    if (string.IsNullOrWhiteSpace(med.MedicineName))
                    {
                        errorMessage = "Medicine name cannot be empty";
                        _logger.LogWarning("Empty medicine name");
                        return false;
                    }
                    // ✅ FIXED: Removed incorrect null check for int type
                    if (med.Duration <= 0)
                    {
                        errorMessage = $"Duration for {med.MedicineName} must be greater than 0";
                        _logger.LogWarning($"Invalid duration for {med.MedicineName}");
                        return false;
                    }
                }
                _logger.LogAudit("Prescription Saved", "System",
                    $"PatientId: {prescription.PatientId}, DoctorId: {prescription.DoctorId}, Medicines: {prescribedMedicines.Count}");
                _logger.LogInfo($"Prescription validated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SavePrescription", ex);
                errorMessage = "An unexpected error occurred while saving prescription";
                return false;
            }
        }
    }
}