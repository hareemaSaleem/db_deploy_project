using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IPatientRegister
    {
        bool RegisterPatient(Patient patient, List<Patient> existingPatients, out string errorMessage);
        bool UpdatePatient(Patient patient, List<Patient> existingPatients, out string errorMessage);
        bool DeletePatient(int patientId, List<Patient> existingPatients, out string errorMessage);
        Patient GetPatientById(int patientId, List<Patient> existingPatients);
        List<Patient> GetAllPatients(List<Patient> existingPatients);
        List<Patient> SearchPatients(string keyword, List<Patient> existingPatients);
        List<Patient> GetHighRiskPatients(List<Patient> existingPatients);
        List<string> ValidatePatient(Patient patient);
        bool IsDuplicateCnic(string cnic, List<Patient> existingPatients);
        bool IsDuplicateEmail(string email, List<Patient> existingPatients);
        int CalculateAge(DateTime dateOfBirth);
        bool DetermineRiskFlag(string chronicDiseases);
        Patient PreparePatientForSave(Patient patient);
    }
}