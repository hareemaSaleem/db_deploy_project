using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class ClinicOwnerDashboardBL
    {
        private readonly ILoggingService _logger;

        public ClinicOwnerDashboardBL(ILoggingService logger)
        {
            _logger = logger;
        }

        public int GetTotalDoctorsCount(List<Doctor> existingDoctors)
        {
            try
            {
                _logger.LogDebug("GetTotalDoctorsCount called");
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
                _logger.LogDebug("GetActiveDoctorsCount called");

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

        public int GetTotalStaffCount(List<Receptionist> existingStaff)
        {
            try
            {
                _logger.LogDebug("GetTotalStaffCount called");
                return existingStaff.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalStaffCount", ex);
                return 0;
            }
        }

        public int GetActiveStaffCount(List<Receptionist> existingStaff)
        {
            try
            {
                _logger.LogDebug("GetActiveStaffCount called");

                int count = 0;
                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Active")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetActiveStaffCount", ex);
                return 0;
            }
        }

        public decimal GetTotalPaymentsThisMonth(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetTotalPaymentsThisMonth called");

                decimal total = 0;
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= firstDayOfMonth.Date && bill.PaymentStatus == "Paid")
                    {
                        total += bill.TotalAmount;
                    }
                }

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalPaymentsThisMonth", ex);
                return 0;
            }
        }

        public int GetPendingPaymentsCount(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPendingPaymentsCount called");

                int count = 0;
                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == "Pending")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingPaymentsCount", ex);
                return 0;
            }
        }

        public decimal GetTotalPayableThisMonth(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetTotalPayableThisMonth called");

                decimal total = 0;
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= firstDayOfMonth.Date)
                    {
                        total += bill.TotalAmount;
                    }
                }

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalPayableThisMonth", ex);
                return 0;
            }
        }

        public decimal GetPaidAmountThisMonth(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPaidAmountThisMonth called");

                decimal total = 0;
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= firstDayOfMonth.Date && bill.PaymentStatus == "Paid")
                    {
                        total += bill.TotalAmount;
                    }
                }

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaidAmountThisMonth", ex);
                return 0;
            }
        }

        public decimal GetPendingAmountThisMonth(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPendingAmountThisMonth called");
                decimal total = 0;
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= firstDayOfMonth.Date && bill.PaymentStatus == "Pending")
                    {
                        total += bill.TotalAmount;
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingAmountThisMonth", ex);
                return 0;
            }
        }
        public Dictionary<string, decimal> GetMonthlyPaymentTrend(List<Billing> existingBills, int months)
        {
            try
            {
                _logger.LogDebug("GetMonthlyPaymentTrend called");
                Dictionary<string, decimal> trend = new Dictionary<string, decimal>();
                DateTime today = DateTime.Today;
                for (int i = months - 1; i >= 0; i--)
                {
                    DateTime monthDate = today.AddMonths(-i);
                    string monthName = monthDate.ToString("MMM yyyy");
                    trend[monthName] = 0;
                    DateTime firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                    DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    foreach (var bill in existingBills)
                    {
                        if (bill.BillingDate.Date >= firstDayOfMonth.Date &&
                            bill.BillingDate.Date <= lastDayOfMonth.Date &&
                            bill.PaymentStatus == "Paid")
                        {
                            trend[monthName] += bill.TotalAmount;
                        }
                    }
                }
                return trend;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMonthlyPaymentTrend", ex);
                return new Dictionary<string, decimal>();
            }
        }
        public List<Billing> GetPendingPaymentsList(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPendingPaymentsList called");
                List<Billing> pendingPayments = new List<Billing>();
                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == "Pending")
                    {
                        pendingPayments.Add(bill);
                    }
                }
                for (int i = 0; i < pendingPayments.Count - 1; i++)
                {
                    for (int j = i + 1; j < pendingPayments.Count; j++)
                    {
                        if (pendingPayments[i].BillingDate < pendingPayments[j].BillingDate)
                        {
                            Billing temp = pendingPayments[i];
                            pendingPayments[i] = pendingPayments[j];
                            pendingPayments[j] = temp;
                        }
                    }
                }
                return pendingPayments;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingPaymentsList", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> GetRecentPayments(List<Billing> existingBills, int count)
        {
            try
            {
                _logger.LogDebug("GetRecentPayments called");
                List<Billing> allPaidBills = new List<Billing>();

                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == "Paid")
                    {
                        allPaidBills.Add(bill);
                    }
                }
                for (int i = 0; i < allPaidBills.Count - 1; i++)
                {
                    for (int j = i + 1; j < allPaidBills.Count; j++)
                    {
                        if (allPaidBills[i].BillingDate < allPaidBills[j].BillingDate)
                        {
                            Billing temp = allPaidBills[i];
                            allPaidBills[i] = allPaidBills[j];
                            allPaidBills[j] = temp;
                        }
                    }
                }
                List<Billing> recentPayments = new List<Billing>();
                int maxCount = count;
                if (allPaidBills.Count < maxCount)
                {
                    maxCount = allPaidBills.Count;
                }

                for (int i = 0; i < maxCount; i++)
                {
                    recentPayments.Add(allPaidBills[i]);
                }
                return recentPayments;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetRecentPayments", ex);
                return new List<Billing>();
            }
        }
        public Dictionary<string, decimal> GetPaymentSummary(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPaymentSummary called");
                Dictionary<string, decimal> summary = new Dictionary<string, decimal>();
                decimal totalPayable = 0;
                decimal totalPaid = 0;
                decimal totalPending = 0;
                decimal totalOverdue = 0;
                DateTime today = DateTime.Today;
                foreach (var bill in existingBills)
                {
                    totalPayable += bill.TotalAmount;

                    if (bill.PaymentStatus == "Paid")
                    {
                        totalPaid += bill.TotalAmount;
                    }
                    else if (bill.PaymentStatus == "Pending")
                    {
                        if (bill.BillingDate < today)
                        {
                            totalOverdue += bill.TotalAmount;
                        }
                        else
                        {
                            totalPending += bill.TotalAmount;
                        }
                    }
                }
                summary["TotalPayable"] = totalPayable;
                summary["TotalPaid"] = totalPaid;
                summary["TotalPending"] = totalPending;
                summary["TotalOverdue"] = totalOverdue;
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentSummary", ex);
                return new Dictionary<string, decimal>();
            }
        }
    }
}
