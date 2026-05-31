using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class PaymentsBL : IPayments
    {
        private readonly ILoggingService _logger;
        private static int _receiptCounter = 100;
        public PaymentsBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public List<Billing> GetAllPayments(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetAllPayments called");
                List<Billing> sortedBills = new List<Billing>();
                foreach (var bill in existingBills)
                {
                    sortedBills.Add(bill);
                }
                for (int i = 0; i < sortedBills.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedBills.Count; j++)
                    {
                        if (sortedBills[i].BillingDate < sortedBills[j].BillingDate)
                        {
                            Billing temp = sortedBills[i];
                            sortedBills[i] = sortedBills[j];
                            sortedBills[j] = temp;
                        }
                    }
                }
                return sortedBills;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllPayments", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> GetPaymentsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug($"GetPaymentsByDateRange called from {fromDate} to {toDate}");
                List<Billing> result = new List<Billing>();
                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date)
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
                _logger.LogError("Error in GetPaymentsByDateRange", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> GetPaymentsByStatus(string status, List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug($"GetPaymentsByStatus called for status: {status}");

                List<Billing> result = new List<Billing>();

                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == status)
                    {
                        result.Add(bill);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentsByStatus", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> SearchPayments(string keyword, List<Billing> existingBills)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllPayments(existingBills);

                string searchTerm = keyword.ToLower();
                List<Billing> results = new List<Billing>();

                foreach (var bill in existingBills)
                {
                    if (bill.BillNumber.ToLower().Contains(searchTerm) ||
                        (bill.Patient != null && bill.Patient.Name != null && bill.Patient.Name.ToLower().Contains(searchTerm)))
                    {
                        results.Add(bill);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SearchPayments", ex);
                return new List<Billing>();
            }
        }

        public Billing GetPaymentById(int paymentId, List<Billing> existingBills)
        {
            try
            {
                foreach (var bill in existingBills)
                {
                    if (bill.BillingId == paymentId)
                    {
                        return bill;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentById", ex);
                return null;
            }
        }

        public List<Doctor> GetAllDoctors(List<Doctor> existingDoctors)
        {
            try
            {
                List<Doctor> sortedDoctors = new List<Doctor>();
                foreach (var doctor in existingDoctors)
                {
                    if (doctor.Status == "Active")
                    {
                        sortedDoctors.Add(doctor);
                    }
                }
                for (int i = 0; i < sortedDoctors.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedDoctors.Count; j++)
                    {
                        if (string.Compare(sortedDoctors[i].Name, sortedDoctors[j].Name) > 0)
                        {
                            Doctor temp = sortedDoctors[i];
                            sortedDoctors[i] = sortedDoctors[j];
                            sortedDoctors[j] = temp;
                        }
                    }
                }

                return sortedDoctors;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllDoctors", ex);
                return new List<Doctor>();
            }
        }

        public List<Receptionist> GetAllStaff(List<Receptionist> existingStaff)
        {
            try
            {
                List<Receptionist> sortedStaff = new List<Receptionist>();
                foreach (var staff in existingStaff)
                {
                    if (staff.Status == "Active")
                    {
                        sortedStaff.Add(staff);
                    }
                }

                for (int i = 0; i < sortedStaff.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedStaff.Count; j++)
                    {
                        if (string.Compare(sortedStaff[i].FirstName, sortedStaff[j].FirstName) > 0)
                        {
                            Receptionist temp = sortedStaff[i];
                            sortedStaff[i] = sortedStaff[j];
                            sortedStaff[j] = temp;
                        }
                    }
                }

                return sortedStaff;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllStaff", ex);
                return new List<Receptionist>();
            }
        }

        public bool AddPayment(Billing payment, List<Billing> existingBills, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"AddPayment called for amount: {payment.TotalAmount}");

                var validationErrors = ValidatePayment(payment);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                if (payment.TotalAmount <= 0)
                {
                    errorMessage = "Amount must be greater than 0";
                    _logger.LogWarning($"Invalid amount: {payment.TotalAmount}");
                    return false;
                }

                payment.BillingDate = DateTime.Now;
                payment.PaymentStatus = "Paid";
                payment.BillNumber = GenerateReceiptNumber(payment.BillingId);

                _logger.LogAudit("Payment Added", "System",
                    $"Amount: {payment.TotalAmount}, Method: {payment.PaymentMethod}");
                _logger.LogInfo($"Payment validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in AddPayment", ex);
                errorMessage = "An unexpected error occurred while adding payment";
                return false;
            }
        }
        public List<string> ValidatePayment(Billing payment)
        {
            List<string> errors = new List<string>();
            if (payment.TotalAmount <= 0)
            {
                errors.Add("Amount must be greater than 0");
            }

            if (payment.TotalAmount > 10000000)
            {
                errors.Add("Amount cannot exceed 10,000,000");
            }

            if (payment.Bonus < 0)
            {
                errors.Add("Bonus cannot be negative");
            }
            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
            {
                errors.Add("Payment method is required");
            }
            else
            {
                if (payment.PaymentMethod != "Cash" &&
                    payment.PaymentMethod != "Bank Transfer" &&
                    payment.PaymentMethod != "Credit Card" &&
                    payment.PaymentMethod != "Easypaisa" &&
                    payment.PaymentMethod != "JazzCash")
                {
                    errors.Add("Invalid payment method");
                }
            }
            return errors;
        }

        public string GenerateReceiptNumber(int paymentId)
        {
            string receiptNumber = $"RCPT{_receiptCounter.ToString().PadLeft(4, '0')}";
            _receiptCounter++;
            return receiptNumber;
        }
        public Dictionary<string, decimal> GetPaymentSummary(List<Billing> existingBills)
        {
            try
            {
                Dictionary<string, decimal> summary = new Dictionary<string, decimal>();
                decimal totalPaid = 0;
                decimal totalPending = 0;
                decimal totalBonus = 0;
                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == "Paid")
                    {
                        totalPaid += bill.TotalAmount;
                        totalBonus += bill.Discount;
                    }
                    else if (bill.PaymentStatus == "Pending")
                    {
                        totalPending += bill.TotalAmount;
                    }
                }
                summary["TotalPaid"] = totalPaid;
                summary["TotalPending"] = totalPending;
                summary["TotalBonus"] = totalBonus;
                summary["GrandTotal"] = totalPaid + totalPending;
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentSummary", ex);
                return new Dictionary<string, decimal>();
            }
        }

        public decimal GetTotalPaymentsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> existingBills)
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
                _logger.LogError("Error in GetTotalPaymentsByDateRange", ex);
                return 0;
            }
        }
    }
}