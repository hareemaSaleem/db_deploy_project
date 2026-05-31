using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class PatientListBL : IPatientList
    {
        private readonly ILoggingService _logger;

        public PatientListBL(ILoggingService logger)
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

        public Patient GetPatientByCNIC(string cnic, List<Patient> existingPatients)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnic))
                    return null;

                string cleanCnic = cnic.Replace("-", "");

                foreach (var p in existingPatients)
                {
                    string patientCleanCnic = p.CNIC.Replace("-", "");
                    if (patientCleanCnic == cleanCnic)
                        return p;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientByCNIC", ex);
                return null;
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
                        (p.Email != null && p.Email.ToLower().Contains(searchTerm)))
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
        public List<Patient> FilterPatientsByGender(string gender, List<Patient> existingPatients)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gender) || gender == "All")
                    return existingPatients;
                List<Patient> results = new List<Patient>();
                foreach (var p in existingPatients)
                {
                    if (p.Gender == gender)
                    {
                        results.Add(p);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterPatientsByGender", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> FilterPatientsByMaritalStatus(string maritalStatus, List<Patient> existingPatients)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maritalStatus) || maritalStatus == "All")
                    return existingPatients;
                List<Patient> results = new List<Patient>();
                foreach (var p in existingPatients)
                {
                    if (p.MaritalStatus == maritalStatus)
                    {
                        results.Add(p);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterPatientsByMaritalStatus", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> FilterPatientsByChronicDisease(string chronicDisease, List<Patient> existingPatients)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chronicDisease) || chronicDisease == "All")
                    return existingPatients;
                List<Patient> results = new List<Patient>();
                if (chronicDisease == "None")
                {
                    foreach (var p in existingPatients)
                    {
                        if (string.IsNullOrWhiteSpace(p.ChronicDiseases))
                        {
                            results.Add(p);
                        }
                    }
                }
                else
                {
                    foreach (var p in existingPatients)
                    {
                        if (!string.IsNullOrWhiteSpace(p.ChronicDiseases) &&
                            p.ChronicDiseases.ToLower().Contains(chronicDisease.ToLower()))
                        {
                            results.Add(p);
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterPatientsByChronicDisease", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> FilterPatientsByDateRange(DateTime fromDate, DateTime toDate, List<Patient> existingPatients)
        {
            try
            {
                List<Patient> results = new List<Patient>();

                foreach (var p in existingPatients)
                {
                    if (p.CreatedAt.Date >= fromDate.Date && p.CreatedAt.Date <= toDate.Date)
                    {
                        results.Add(p);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterPatientsByDateRange", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> FilterPatientsByAgeRange(int minAge, int maxAge, List<Patient> existingPatients)
        {
            try
            {
                List<Patient> results = new List<Patient>();

                foreach (var p in existingPatients)
                {
                    if (p.Age >= minAge && p.Age <= maxAge)
                    {
                        results.Add(p);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterPatientsByAgeRange", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> GetFilteredPatients(string gender, string maritalStatus, string chronicDisease, string searchTerm, List<Patient> existingPatients)
        {
            try
            {
                List<Patient> results = new List<Patient>();

                foreach (var p in existingPatients)
                {
                    bool matches = true;

                    if (!string.IsNullOrWhiteSpace(gender) && gender != "All")
                    {
                        if (p.Gender != gender)
                            matches = false;
                    }
                    if (matches && !string.IsNullOrWhiteSpace(maritalStatus) && maritalStatus != "All")
                    {
                        if (p.MaritalStatus != maritalStatus)
                            matches = false;
                    }
                    if (matches && !string.IsNullOrWhiteSpace(chronicDisease) && chronicDisease != "All")
                    {
                        if (chronicDisease == "None")
                        {
                            if (!string.IsNullOrWhiteSpace(p.ChronicDiseases))
                                matches = false;
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(p.ChronicDiseases) ||
                                !p.ChronicDiseases.ToLower().Contains(chronicDisease.ToLower()))
                                matches = false;
                        }
                    }
                    if (matches && !string.IsNullOrWhiteSpace(searchTerm))
                    {
                        string term = searchTerm.ToLower();
                        if (!p.Name.ToLower().Contains(term) &&
                            !p.CNIC.Contains(term) &&
                            !p.Phone.Contains(term))
                        {
                            matches = false;
                        }
                    }
                    if (matches)
                    {
                        results.Add(p);
                    }
                }
                for (int i = 0; i < results.Count - 1; i++)
                {
                    for (int j = i + 1; j < results.Count; j++)
                    {
                        if (string.Compare(results[i].Name, results[j].Name) > 0)
                        {
                            Patient temp = results[i];
                            results[i] = results[j];
                            results[j] = temp;
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetFilteredPatients", ex);
                return new List<Patient>();
            }
        }
        public int GetTotalPatientsCount(List<Patient> existingPatients)
        {
            return existingPatients.Count;
        }
        public int GetPatientsCountByGender(string gender, List<Patient> existingPatients)
        {
            int count = 0;
            foreach (var p in existingPatients)
            {
                if (p.Gender == gender)
                    count++;
            }
            return count;
        }
        public int GetPatientsCountByChronicDisease(string disease, List<Patient> existingPatients)
        {
            int count = 0;
            foreach (var p in existingPatients)
            {
                if (!string.IsNullOrWhiteSpace(p.ChronicDiseases) &&
                    p.ChronicDiseases.ToLower().Contains(disease.ToLower()))
                {
                    count++;
                }
            }
            return count;
        }
        public Dictionary<string, int> GetChronicDiseaseStatistics(List<Patient> existingPatients)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();
            stats["Diabetes"] = 0;
            stats["Hypertension"] = 0;
            stats["Asthma"] = 0;
            stats["Thyroid"] = 0;
            stats["None"] = 0;
            foreach (var p in existingPatients)
            {
                if (string.IsNullOrWhiteSpace(p.ChronicDiseases))
                {
                    stats["None"]++;
                }
                else if (p.ChronicDiseases.ToLower().Contains("diabetes"))
                {
                    stats["Diabetes"]++;
                }
                else if (p.ChronicDiseases.ToLower().Contains("hypertension"))
                {
                    stats["Hypertension"]++;
                }
                else if (p.ChronicDiseases.ToLower().Contains("asthma"))
                {
                    stats["Asthma"]++;
                }
                else if (p.ChronicDiseases.ToLower().Contains("thyroid"))
                {
                    stats["Thyroid"]++;
                }
            }

            return stats;
        }

        public Dictionary<string, int> GetGenderStatistics(List<Patient> existingPatients)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();
            stats["Male"] = 0;
            stats["Female"] = 0;
            stats["Other"] = 0;

            foreach (var p in existingPatients)
            {
                if (p.Gender == "Male")
                    stats["Male"]++;
                else if (p.Gender == "Female")
                    stats["Female"]++;
                else
                    stats["Other"]++;
            }
            return stats;
        }
        public Dictionary<int, int> GetAgeGroupStatistics(List<Patient> existingPatients)
        {
            Dictionary<int, int> stats = new Dictionary<int, int>();
            stats[18] = 0;
            stats[35] = 0;
            stats[50] = 0;
            stats[100] = 0;
            foreach (var p in existingPatients)
            {
                if (p.Age <= 18)
                    stats[18]++;
                else if (p.Age <= 35)
                    stats[35]++;
                else if (p.Age <= 50)
                    stats[50]++;
                else
                    stats[100]++;
            }
            return stats;
        }
        public List<Patient> GetPatientsForExport(DateTime fromDate, DateTime toDate, List<Patient> existingPatients)
        {
            return FilterPatientsByDateRange(fromDate, toDate, existingPatients);
        }
        public List<string> ValidatePatientSearch(string keyword)
        {
            List<string> errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword) && keyword.Length < 2 && keyword.Length > 0)
            {
                errors.Add("Search term must be at least 2 characters for better results");
            }
            return errors;
        }
    }
}