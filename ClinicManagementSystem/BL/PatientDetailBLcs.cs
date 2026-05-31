using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class PatientDetailBL : IPatientDetail
    {
        private readonly ILoggingService _logger;
        public PatientDetailBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public Patient GetPatientDetails(int patientId, List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug($"GetPatientDetails called for ID: {patientId}");
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientDetails", ex);
                return null;
            }
        }
        public List<VisitLog> GetPatientVisitHistory(int patientId, List<VisitLog> existingVisitLogs)
        {
            try
            {
                _logger.LogDebug($"GetPatientVisitHistory called for PatientId: {patientId}");
                List<VisitLog> visits = new List<VisitLog>();

                foreach (var log in existingVisitLogs)
                {
                    if (log.PatientId == patientId)
                    {
                        visits.Add(log);
                    }
                }
                for (int i = 0; i < visits.Count - 1; i++)
                {
                    for (int j = i + 1; j < visits.Count; j++)
                    {
                        if (visits[i].VisitDate < visits[j].VisitDate)
                        {
                            VisitLog temp = visits[i];
                            visits[i] = visits[j];
                            visits[j] = temp;
                        }
                    }
                }
                return visits;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientVisitHistory", ex);
                return new List<VisitLog>();
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
                _logger.LogError("Error in GetPatientPrescriptions", ex);
                return new List<Prescription>();
            }
        }
        public List<Billing> GetPatientBillingHistory(int patientId, List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug($"GetPatientBillingHistory called for PatientId: {patientId}");

                List<Billing> result = new List<Billing>();

                foreach (var bill in existingBills)
                {
                    if (bill.PatientId == patientId)
                    {
                        result.Add(bill);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].BillingDate < result[j].BillingDate)
                        {
                            Billing temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientBillingHistory", ex);
                return new List<Billing>();
            }
        }

        public string GetPatientFullName(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Name;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientFullName", ex);
                return "";
            }
        }
        public string GetPatientCNIC(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.CNIC;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientCNIC", ex);
                return "";
            }
        }
        public string GetPatientPhone(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Phone;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientPhone", ex);
                return "";
            }
        }
        public int GetPatientAge(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Age;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientAge", ex);
                return 0;
            }
        }
        public string GetPatientGender(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Gender;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientGender", ex);
                return "";
            }
        }

        public string GetPatientEmail(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Email;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientEmail", ex);
                return "";
            }
        }

        public string GetPatientMaritalStatus(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.MaritalStatus;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientMaritalStatus", ex);
                return "";
            }
        }

        public string GetPatientAddress(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.Address;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientAddress", ex);
                return "";
            }
        }

        public string GetPatientCity(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.City;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientCity", ex);
                return "";
            }
        }

        public string GetPatientBloodGroup(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.BloodGroup;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientBloodGroup", ex);
                return "";
            }
        }

        public string GetPatientChronicDiseases(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.ChronicDiseases;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientChronicDiseases", ex);
                return "";
            }
        }

        public DateTime GetPatientDateOfBirth(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.DateOfBirth;
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientDateOfBirth", ex);
                return default;
            }
        }

        public bool IsPatientHighRisk(int patientId, List<Patient> existingPatients)
        {
            try
            {
                foreach (var p in existingPatients)
                {
                    if (p.PatientId == patientId)
                        return p.RiskFlag;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in IsPatientHighRisk", ex);
                return false;
            }
        }

        public int GetTotalVisits(int patientId, List<VisitLog> existingVisitLogs)
        {
            try
            {
                int count = 0;
                foreach (var log in existingVisitLogs)
                {
                    if (log.PatientId == patientId)
                        count++;
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalVisits", ex);
                return 0;
            }
        }

        public int GetTotalPrescriptions(int patientId, List<Prescription> existingPrescriptions)
        {
            try
            {
                int count = 0;
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.PatientId == patientId)
                        count++;
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalPrescriptions", ex);
                return 0;
            }
        }

        public decimal GetTotalBilledAmount(int patientId, List<Billing> existingBills)
        {
            try
            {
                decimal total = 0;
                foreach (var bill in existingBills)
                {
                    if (bill.PatientId == patientId && bill.PaymentStatus == "Paid")
                    {
                        total += bill.TotalAmount;
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalBilledAmount", ex);
                return 0;
            }
        }
    }
}