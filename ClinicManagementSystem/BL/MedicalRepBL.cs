using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class MedicalRepBL : IMedicalRep
    {
        private readonly ILoggingService _logger;
        private static int _nextSampleId = 1;

        public MedicalRepBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool AddMedicalRepVisit(MedicalRep medicalRep, List<MedicalRep> existingVisits, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"AddMedicalRepVisit called for Rep: {medicalRep.RepName}, Company: {medicalRep.CompanyName}");
                var validationErrors = ValidateMedicalRep(medicalRep);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                if (medicalRep.MedicineSamples == null || medicalRep.MedicineSamples.Count == 0)
                {
                    errorMessage = "At least one medicine sample is required";
                    _logger.LogWarning("No medicine samples provided");
                    return false;
                }

                foreach (var sample in medicalRep.MedicineSamples)
                {
                    var sampleErrors = ValidateMedicineSample(sample);
                    if (sampleErrors.Count > 0)
                    {
                        errorMessage = $"Sample validation failed: {string.Join(", ", sampleErrors)}";
                        _logger.LogWarning($"Sample validation failed: {errorMessage}");
                        return false;
                    }
                }

                if (medicalRep.VisitDate > DateTime.Now)
                {
                    errorMessage = "Visit date cannot be in the future";
                    _logger.LogWarning($"Future visit date: {medicalRep.VisitDate}");
                    return false;
                }

                // Generate sample IDs
                foreach (var sample in medicalRep.MedicineSamples)
                {
                    if (sample.SampleId == 0)
                    {
                        sample.SampleId = _nextSampleId++;
                    }
                }

                _logger.LogAudit("Medical Rep Visit Added", "System",
                    $"Rep: {medicalRep.RepName}, Company: {medicalRep.CompanyName}, Samples: {medicalRep.MedicineSamples.Count}");
                _logger.LogInfo($"Medical rep visit validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddMedicalRepVisit", ex);
                errorMessage = "An unexpected error occurred while adding medical rep visit";
                return false;
            }
        }

        public bool UpdateMedicalRepVisit(int visitId, MedicalRep updatedRep, List<MedicalRep> existingVisits, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateMedicalRepVisit called for ID: {visitId}");

                // Find existing visit in the provided list
                MedicalRep existingVisit = null;
                foreach (var visit in existingVisits)
                {
                    if (visit.MedicalRepId == visitId)
                    {
                        existingVisit = visit;
                        break;
                    }
                }

                if (existingVisit == null)
                {
                    errorMessage = "Medical rep visit not found";
                    _logger.LogWarning($"Visit not found. ID: {visitId}");
                    return false;
                }

                var validationErrors = ValidateMedicalRep(updatedRep);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                if (updatedRep.VisitDate > DateTime.Now)
                {
                    errorMessage = "Visit date cannot be in the future";
                    _logger.LogWarning($"Future visit date: {updatedRep.VisitDate}");
                    return false;
                }

                _logger.LogAudit("Medical Rep Visit Updated", "System", $"RepId: {visitId}, Rep: {updatedRep.RepName}");
                _logger.LogInfo($"Medical rep visit validated successfully. ID: {visitId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateMedicalRepVisit", ex);
                errorMessage = "An unexpected error occurred while updating medical rep visit";
                return false;
            }
        }

        public bool DeleteMedicalRepVisit(int visitId, List<MedicalRep> existingVisits, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeleteMedicalRepVisit called for ID: {visitId}");

                // Check if visit exists
                bool visitExists = false;
                foreach (var visit in existingVisits)
                {
                    if (visit.MedicalRepId == visitId)
                    {
                        visitExists = true;
                        break;
                    }
                }

                if (!visitExists)
                {
                    errorMessage = "Medical rep visit not found";
                    _logger.LogWarning($"Visit not found. ID: {visitId}");
                    return false;
                }

                _logger.LogAudit("Medical Rep Visit Deleted", "System", $"RepId: {visitId}");
                _logger.LogInfo($"Medical rep visit validated for deletion. ID: {visitId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteMedicalRepVisit", ex);
                errorMessage = "An unexpected error occurred while deleting medical rep visit";
                return false;
            }
        }

        // ============== GET OPERATIONS ==============

        public MedicalRep GetMedicalRepVisitById(int visitId, List<MedicalRep> existingVisits)
        {
            try
            {
                foreach (var visit in existingVisits)
                {
                    if (visit.MedicalRepId == visitId)
                        return visit;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMedicalRepVisitById", ex);
                return null;
            }
        }

        public List<MedicalRep> GetAllMedicalRepVisits(List<MedicalRep> existingVisits)
        {
            try
            {
                List<MedicalRep> sortedVisits = new List<MedicalRep>();
                foreach (var visit in existingVisits)
                {
                    sortedVisits.Add(visit);
                }

                // Sort by date (newest first)
                for (int i = 0; i < sortedVisits.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedVisits.Count; j++)
                    {
                        if (sortedVisits[i].VisitDate < sortedVisits[j].VisitDate)
                        {
                            MedicalRep temp = sortedVisits[i];
                            sortedVisits[i] = sortedVisits[j];
                            sortedVisits[j] = temp;
                        }
                    }
                }

                return sortedVisits;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllMedicalRepVisits", ex);
                return new List<MedicalRep>();
            }
        }

        public List<MedicalRep> GetVisitsByCompany(string companyName, List<MedicalRep> existingVisits)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(companyName))
                    return new List<MedicalRep>();

                List<MedicalRep> result = new List<MedicalRep>();
                string searchTerm = companyName.ToLower();

                foreach (var visit in existingVisits)
                {
                    if (visit.CompanyName.ToLower().Contains(searchTerm))
                    {
                        result.Add(visit);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetVisitsByCompany", ex);
                return new List<MedicalRep>();
            }
        }

        public List<MedicalRep> GetVisitsByDateRange(DateTime fromDate, DateTime toDate, List<MedicalRep> existingVisits)
        {
            try
            {
                List<MedicalRep> result = new List<MedicalRep>();

                foreach (var visit in existingVisits)
                {
                    if (visit.VisitDate.Date >= fromDate.Date && visit.VisitDate.Date <= toDate.Date)
                    {
                        result.Add(visit);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetVisitsByDateRange", ex);
                return new List<MedicalRep>();
            }
        }

        // ============== SAMPLE MANAGEMENT ==============

        public bool AddMedicineSample(int visitId, MedicineSample sample, List<MedicalRep> existingVisits, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"AddMedicineSample called for VisitId: {visitId}, Medicine: {sample.MedicineName}");

                // Find the visit
                MedicalRep visit = null;
                foreach (var v in existingVisits)
                {
                    if (v.MedicalRepId == visitId)
                    {
                        visit = v;
                        break;
                    }
                }

                if (visit == null)
                {
                    errorMessage = "Medical rep visit not found";
                    _logger.LogWarning($"Visit not found. ID: {visitId}");
                    return false;
                }

                var sampleErrors = ValidateMedicineSample(sample);
                if (sampleErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", sampleErrors);
                    _logger.LogWarning($"Sample validation failed: {errorMessage}");
                    return false;
                }

                // Generate sample ID
                sample.SampleId = _nextSampleId++;

                _logger.LogAudit("Medicine Sample Added", "System", $"VisitId: {visitId}, Medicine: {sample.MedicineName}, Quantity: {sample.Quantity}");
                _logger.LogInfo($"Medicine sample validated successfully. SampleId: {sample.SampleId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddMedicineSample", ex);
                errorMessage = "An unexpected error occurred while adding medicine sample";
                return false;
            }
        }

        public List<MedicineSample> GetSamplesByVisit(int visitId, List<MedicalRep> existingVisits)
        {
            try
            {
                MedicalRep visit = null;
                foreach (var v in existingVisits)
                {
                    if (v.MedicalRepId == visitId)
                    {
                        visit = v;
                        break;
                    }
                }

                if (visit == null || visit.MedicineSamples == null)
                    return new List<MedicineSample>();

                return visit.MedicineSamples;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetSamplesByVisit", ex);
                return new List<MedicineSample>();
            }
        }

        // ============== VALIDATION METHODS (Pure Logic) ==============

        public List<string> ValidateMedicalRep(MedicalRep medicalRep)
        {
            List<string> errors = new List<string>();
            string errorMessage;

            if (!Validators.IsNameValid(medicalRep.RepName, out errorMessage))
                errors.Add($"Rep Name: {errorMessage}");

            if (!Validators.IsNameValid(medicalRep.CompanyName, out errorMessage))
                errors.Add($"Company Name: {errorMessage}");

            if (medicalRep.VisitDate == default)
            {
                errors.Add("Visit Date is required");
            }

            return errors;
        }

        public List<string> ValidateMedicineSample(MedicineSample sample)
        {
            List<string> errors = new List<string>();
            string errorMessage;

            if (!Validators.IsNameValid(sample.MedicineName, out errorMessage))
                errors.Add($"Medicine Name: {errorMessage}");

            if (sample.Quantity <= 0)
            {
                errors.Add("Quantity must be greater than 0");
            }

            if (sample.Quantity > 10000)
            {
                errors.Add("Quantity cannot exceed 10,000");
            }

            if (sample.ExpiryDate != null && sample.ExpiryDate < DateTime.Now)
            {
                errors.Add("Expiry date cannot be in the past");
            }

            return errors;
        }

        // ============== CALCULATIONS ==============

        public int CalculateTotalSamplesCount(int visitId, List<MedicalRep> existingVisits)
        {
            try
            {
                MedicalRep visit = null;
                foreach (var v in existingVisits)
                {
                    if (v.MedicalRepId == visitId)
                    {
                        visit = v;
                        break;
                    }
                }

                if (visit == null || visit.MedicineSamples == null)
                    return 0;

                int total = 0;
                foreach (var sample in visit.MedicineSamples)
                {
                    total += sample.Quantity;
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CalculateTotalSamplesCount", ex);
                return 0;
            }
        }

        public List<string> GetAllMedicineNamesFromSamples(List<MedicalRep> existingVisits)
        {
            try
            {
                List<string> medicineNames = new List<string>();

                foreach (var visit in existingVisits)
                {
                    if (visit.MedicineSamples != null)
                    {
                        foreach (var sample in visit.MedicineSamples)
                        {
                            bool exists = false;
                            foreach (var name in medicineNames)
                            {
                                if (name == sample.MedicineName)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                            {
                                medicineNames.Add(sample.MedicineName);
                            }
                        }
                    }
                }
                for (int i = 0; i < medicineNames.Count - 1; i++)
                {
                    for (int j = i + 1; j < medicineNames.Count; j++)
                    {
                        if (string.Compare(medicineNames[i], medicineNames[j]) > 0)
                        {
                            string temp = medicineNames[i];
                            medicineNames[i] = medicineNames[j];
                            medicineNames[j] = temp;
                        }
                    }
                }
                return medicineNames;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllMedicineNamesFromSamples", ex);
                return new List<string>();
            }
        }
    }
}