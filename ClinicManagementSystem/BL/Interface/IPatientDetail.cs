using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IPatientDetail
    {
        Patient GetPatientDetails(int patientId, List<Patient> existingPatients);
        List<VisitLog> GetPatientVisitHistory(int patientId, List<VisitLog> existingVisitLogs);
        List<Prescription> GetPatientPrescriptions(int patientId, List<Prescription> existingPrescriptions);
        List<Billing> GetPatientBillingHistory(int patientId, List<Billing> existingBills);
        string GetPatientFullName(int patientId, List<Patient> existingPatients);
        string GetPatientCNIC(int patientId, List<Patient> existingPatients);
        string GetPatientPhone(int patientId, List<Patient> existingPatients);
        int GetPatientAge(int patientId, List<Patient> existingPatients);
        string GetPatientGender(int patientId, List<Patient> existingPatients);
        string GetPatientEmail(int patientId, List<Patient> existingPatients);
        string GetPatientMaritalStatus(int patientId, List<Patient> existingPatients);
        string GetPatientAddress(int patientId, List<Patient> existingPatients);
        string GetPatientCity(int patientId, List<Patient> existingPatients);
        string GetPatientBloodGroup(int patientId, List<Patient> existingPatients);
        string GetPatientChronicDiseases(int patientId, List<Patient> existingPatients);
        DateTime GetPatientDateOfBirth(int patientId, List<Patient> existingPatients);
        bool IsPatientHighRisk(int patientId, List<Patient> existingPatients);
        int GetTotalVisits(int patientId, List<VisitLog> existingVisitLogs);
        int GetTotalPrescriptions(int patientId, List<Prescription> existingPrescriptions);
        decimal GetTotalBilledAmount(int patientId, List<Billing> existingBills);
    }
}