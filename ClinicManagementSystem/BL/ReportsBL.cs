using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class ReportsBL
    {
        private readonly ILoggingService _logger;

        public ReportsBL(ILoggingService logger)
        {
            _logger = logger;
        }

        // ============== PATIENT REPORTS ==============

        public List<Patient> GetPatientReport(DateTime fromDate, DateTime toDate, string gender, string ageRange, List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug($"GetPatientReport called from {fromDate} to {toDate}");

                List<Patient> result = new List<Patient>();

                foreach (var patient in existingPatients)
                {
                    bool matches = true;

                    if (patient.CreatedAt.Date < fromDate.Date || patient.CreatedAt.Date > toDate.Date)
                    {
                        matches = false;
                    }

                    if (matches && !string.IsNullOrWhiteSpace(gender) && gender != "All")
                    {
                        if (patient.Gender != gender)
                        {
                            matches = false;
                        }
                    }

                    if (matches && !string.IsNullOrWhiteSpace(ageRange) && ageRange != "All")
                    {
                        if (!IsAgeInRange(patient.Age, ageRange))
                        {
                            matches = false;
                        }
                    }

                    if (matches)
                    {
                        result.Add(patient);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (string.Compare(result[i].Name, result[j].Name) > 0)
                        {
                            Patient temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientReport", ex);
                return new List<Patient>();
            }
        }

        public int GetTotalPatientsCount(DateTime fromDate, DateTime toDate, List<Patient> existingPatients)
        {
            try
            {
                int count = 0;
                foreach (var patient in existingPatients)
                {
                    if (patient.CreatedAt.Date >= fromDate.Date && patient.CreatedAt.Date <= toDate.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalPatientsCount", ex);
                return 0;
            }
        }
        public int GetNewPatientsCount(DateTime fromDate, DateTime toDate, List<Patient> existingPatients)
        {
            try
            {
                int count = 0;
                foreach (var patient in existingPatients)
                {
                    if (patient.CreatedAt.Date >= fromDate.Date && patient.CreatedAt.Date <= toDate.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetNewPatientsCount", ex);
                return 0;
            }
        }
        public List<Appointment> GetAppointmentReport(DateTime fromDate, DateTime toDate, int doctorId, string status, bool includeCancelled, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetAppointmentReport called from {fromDate} to {toDate}");

                List<Appointment> result = new List<Appointment>();

                foreach (var apt in existingAppointments)
                {
                    bool matches = true;

                    if (apt.AppointmentDate.Date < fromDate.Date || apt.AppointmentDate.Date > toDate.Date)
                    {
                        matches = false;
                    }

                    if (matches && doctorId > 0)
                    {
                        if (apt.DoctorId != doctorId)
                        {
                            matches = false;
                        }
                    }

                    if (matches && !string.IsNullOrWhiteSpace(status) && status != "All Status")
                    {
                        string statusLower = status.ToLower();
                        string aptStatusLower = apt.Status.ToLower();

                        if (statusLower == "completed" && aptStatusLower != "completed")
                        {
                            matches = false;
                        }
                        else if (statusLower == "pending" && aptStatusLower != "scheduled")
                        {
                            matches = false;
                        }
                        else if (statusLower == "cancelled" && aptStatusLower != "cancelled")
                        {
                            matches = false;
                        }
                    }
                    if (matches && !includeCancelled)
                    {
                        if (apt.Status == "Cancelled")
                        {
                            matches = false;
                        }
                    }

                    if (matches)
                    {
                        result.Add(apt);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].AppointmentDate < result[j].AppointmentDate)
                        {
                            Appointment temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentReport", ex);
                return new List<Appointment>();
            }
        }

        public int GetTotalAppointmentsCount(DateTime fromDate, DateTime toDate, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date >= fromDate.Date && apt.AppointmentDate.Date <= toDate.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalAppointmentsCount", ex);
                return 0;
            }
        }

        public int GetCompletedAppointmentsCount(DateTime fromDate, DateTime toDate, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date >= fromDate.Date && apt.AppointmentDate.Date <= toDate.Date && apt.Status == "Completed")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCompletedAppointmentsCount", ex);
                return 0;
            }
        }

        public int GetCancelledAppointmentsCount(DateTime fromDate, DateTime toDate, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date >= fromDate.Date && apt.AppointmentDate.Date <= toDate.Date && apt.Status == "Cancelled")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCancelledAppointmentsCount", ex);
                return 0;
            }
        }

        // ============== FINANCIAL REPORTS ==============

        public List<Billing> GetFinancialReport(DateTime fromDate, DateTime toDate, string paymentType, string status, List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug($"GetFinancialReport called from {fromDate} to {toDate}");

                List<Billing> result = new List<Billing>();

                foreach (var bill in existingBills)
                {
                    bool matches = true;

                    if (bill.BillingDate.Date < fromDate.Date || bill.BillingDate.Date > toDate.Date)
                    {
                        matches = false;
                    }

                    if (matches && !string.IsNullOrWhiteSpace(paymentType) && paymentType != "All Payment Types")
                    {
                        if (bill.PaymentMethod != paymentType)
                        {
                            matches = false;
                        }
                    }

                    if (matches && !string.IsNullOrWhiteSpace(status) && status != "All Status")
                    {
                        if (bill.PaymentStatus != status)
                        {
                            matches = false;
                        }
                    }

                    if (matches)
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
                _logger.LogError("Error in GetFinancialReport", ex);
                return new List<Billing>();
            }
        }

        public decimal GetTotalRevenue(DateTime fromDate, DateTime toDate, List<Billing> existingBills)
        {
            try
            {
                decimal total = 0;
                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date && bill.PaymentStatus == "Paid")
                    {
                        total += bill.TotalAmount;
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalRevenue", ex);
                return 0;
            }
        }

        public decimal GetTotalPendingAmount(DateTime fromDate, DateTime toDate, List<Billing> existingBills)
        {
            try
            {
                decimal total = 0;
                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date && bill.PaymentStatus == "Pending")
                    {
                        total += bill.TotalAmount;
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalPendingAmount", ex);
                return 0;
            }
        }

        public Dictionary<string, decimal> GetPaymentMethodBreakdown(DateTime fromDate, DateTime toDate, List<Billing> existingBills)
        {
            try
            {
                Dictionary<string, decimal> breakdown = new Dictionary<string, decimal>();
                breakdown["Cash"] = 0;
                breakdown["UPI"] = 0;
                breakdown["Card"] = 0;
                breakdown["Bank Transfer"] = 0;
                breakdown["Easypaisa"] = 0;
                breakdown["JazzCash"] = 0;

                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date && bill.PaymentStatus == "Paid")
                    {
                        if (breakdown.ContainsKey(bill.PaymentMethod))
                        {
                            breakdown[bill.PaymentMethod] += bill.TotalAmount;
                        }
                    }
                }

                return breakdown;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentMethodBreakdown", ex);
                return new Dictionary<string, decimal>();
            }
        }

        // ============== DOCTOR PERFORMANCE REPORTS ==============

        public Dictionary<string, int> GetDoctorAppointmentCount(DateTime fromDate, DateTime toDate, List<Appointment> existingAppointments, List<Doctor> existingDoctors)
        {
            try
            {
                Dictionary<string, int> doctorStats = new Dictionary<string, int>();

                foreach (var doctor in existingDoctors)
                {
                    doctorStats[doctor.Name] = 0;
                }

                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date >= fromDate.Date && apt.AppointmentDate.Date <= toDate.Date)
                    {
                        string doctorName = GetDoctorNameById(apt.DoctorId, existingDoctors);
                        if (doctorStats.ContainsKey(doctorName))
                        {
                            doctorStats[doctorName]++;
                        }
                    }
                }

                return doctorStats;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetDoctorAppointmentCount", ex);
                return new Dictionary<string, int>();
            }
        }

        public Dictionary<string, decimal> GetDoctorRevenue(DateTime fromDate, DateTime toDate, List<Appointment> existingAppointments, List<Billing> existingBills, List<Doctor> existingDoctors)
        {
            try
            {
                Dictionary<string, decimal> doctorRevenue = new Dictionary<string, decimal>();

                foreach (var doctor in existingDoctors)
                {
                    doctorRevenue[doctor.Name] = 0;
                }

                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date >= fromDate.Date && apt.AppointmentDate.Date <= toDate.Date)
                    {
                        string doctorName = GetDoctorNameById(apt.DoctorId, existingDoctors);
                        decimal consultationFee = GetDoctorConsultationFee(apt.DoctorId, existingDoctors);

                        if (doctorRevenue.ContainsKey(doctorName))
                        {
                            doctorRevenue[doctorName] += consultationFee;
                        }
                    }
                }

                return doctorRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetDoctorRevenue", ex);
                return new Dictionary<string, decimal>();
            }
        }
        public Dictionary<string, int> GetStaffByDesignation(List<Receptionist> existingStaff)
        {
            try
            {
                Dictionary<string, int> staffCount = new Dictionary<string, int>();

                foreach (var staff in existingStaff)
                {
                    if (staffCount.ContainsKey(staff.Designation))
                    {
                        staffCount[staff.Designation]++;
                    }
                    else
                    {
                        staffCount[staff.Designation] = 1;
                    }
                }

                return staffCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStaffByDesignation", ex);
                return new Dictionary<string, int>();
            }
        }

        public List<Receptionist> GetStaffReport(DateTime fromDate, DateTime toDate, List<Receptionist> existingStaff)
        {
            try
            {
                List<Receptionist> result = new List<Receptionist>();

                foreach (var staff in existingStaff)
                {
                    if (staff.DateOfJoining.Date >= fromDate.Date && staff.DateOfJoining.Date <= toDate.Date)
                    {
                        result.Add(staff);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (string.Compare(result[i].FirstName, result[j].FirstName) > 0)
                        {
                            Receptionist temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStaffReport", ex);
                return new List<Receptionist>();
            }
        }

        // ============== MEDICINE REPORTS ==============

        public List<Medicine> GetLowStockMedicinesReport(int threshold, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();

                foreach (var med in existingMedicines)
                {
                    if (med.StockQuantity < threshold)
                    {
                        result.Add(med);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (string.Compare(result[i].Name, result[j].Name) > 0)
                        {
                            Medicine temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetLowStockMedicinesReport", ex);
                return new List<Medicine>();
            }
        }

        public List<Medicine> GetExpiringMedicinesReport(int daysThreshold, List<Medicine> existingMedicines)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();
                DateTime expiryThreshold = DateTime.Now.AddDays(daysThreshold);

                foreach (var med in existingMedicines)
                {
                    if (med.ExpiryDate <= expiryThreshold)
                    {
                        result.Add(med);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].ExpiryDate < result[j].ExpiryDate)
                        {
                            Medicine temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetExpiringMedicinesReport", ex);
                return new List<Medicine>();
            }
        }

        // ============== HELPER METHODS ==============

        private bool IsAgeInRange(int age, string range)
        {
            if (range == "0-18") return age >= 0 && age <= 18;
            if (range == "19-35") return age >= 19 && age <= 35;
            if (range == "36-50") return age >= 36 && age <= 50;
            if (range == "51+") return age >= 51;
            return true;
        }

        private string GetDoctorNameById(int doctorId, List<Doctor> existingDoctors)
        {
            foreach (var doctor in existingDoctors)
            {
                if (doctor.DoctorId == doctorId)
                {
                    return doctor.Name;
                }
            }
            return "Unknown";
        }

        private decimal GetDoctorConsultationFee(int doctorId, List<Doctor> existingDoctors)
        {
            foreach (var doctor in existingDoctors)
            {
                if (doctor.DoctorId == doctorId)
                {
                    return doctor.ConsultationFee;
                }
            }
            return 0;
        }
        public List<string> ValidateReportDates(DateTime fromDate, DateTime toDate)
        {
            List<string> errors = new List<string>();

            if (fromDate > toDate)
            {
                errors.Add("From date cannot be greater than To date");
            }

            if (toDate > DateTime.Now)
            {
                errors.Add("To date cannot be in the future");
            }

            return errors;
        }
        public string GetReportFileName(string reportType, string format)
        {
            string dateStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileExtension = format.ToLower() == "pdf" ? "pdf" : "xlsx";
            return $"{reportType.Replace(" ", "_")}_{dateStamp}.{fileExtension}";
        }
    }
}
