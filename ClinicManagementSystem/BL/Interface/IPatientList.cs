using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IPatientList
    {
        List<Patient> GetAllPatients(List<Patient> existingPatients);
        Patient GetPatientById(int patientId, List<Patient> existingPatients);
        Patient GetPatientByCNIC(string cnic, List<Patient> existingPatients);
        List<Patient> SearchPatients(string keyword, List<Patient> existingPatients);
        List<Patient> FilterPatientsByGender(string gender, List<Patient> existingPatients);
        List<Patient> FilterPatientsByMaritalStatus(string maritalStatus, List<Patient> existingPatients);
        List<Patient> FilterPatientsByChronicDisease(string chronicDisease, List<Patient> existingPatients);
        List<Patient> FilterPatientsByDateRange(DateTime fromDate, DateTime toDate, List<Patient> existingPatients);
        List<Patient> FilterPatientsByAgeRange(int minAge, int maxAge, List<Patient> existingPatients);
        List<Patient> GetFilteredPatients(string gender, string maritalStatus, string chronicDisease, string searchTerm, List<Patient> existingPatients);
        int GetTotalPatientsCount(List<Patient> existingPatients);
        int GetPatientsCountByGender(string gender, List<Patient> existingPatients);
        int GetPatientsCountByChronicDisease(string disease, List<Patient> existingPatients);
        Dictionary<string, int> GetChronicDiseaseStatistics(List<Patient> existingPatients);
        Dictionary<string, int> GetGenderStatistics(List<Patient> existingPatients);
        Dictionary<int, int> GetAgeGroupStatistics(List<Patient> existingPatients);
        List<Patient> GetPatientsForExport(DateTime fromDate, DateTime toDate, List<Patient> existingPatients);
        List<string> ValidatePatientSearch(string keyword);
    }
}