using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class GenerateBillBL : IGenerateBill
    {
        private readonly ILoggingService _logger;
        private static int _billCounter = 9; 

        public GenerateBillBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool GenerateBill(Billing bill, List<Billing> existingBills, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"GenerateBill called for PatientId: {bill.PatientId}");
                var validationErrors = ValidateBill(bill);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }
                if (bill.PatientId <= 0)
                {
                    errorMessage = "Valid patient is required";
                    _logger.LogWarning("Invalid patient selected");
                    return false;
                }
                bill.TotalAmount = CalculateTotalAmount(bill.ConsultationFee, bill.MedicineCharges, bill.OtherCharges, bill.Discount);
                if (bill.TotalAmount <= 0)
                {
                    errorMessage = "Total amount must be greater than 0";
                    _logger.LogWarning($"Invalid total amount: {bill.TotalAmount}");
                    return false;
                }
                bill.BillNumber = GenerateBillNumber();
                bill.BillingDate = DateTime.Now;
                bill.PaymentStatus = "Paid";
                _logger.LogAudit("Bill Generated", "System",
                    $"BillNumber: {bill.BillNumber}, PatientId: {bill.PatientId}, Amount: {bill.TotalAmount}");
                _logger.LogInfo($"Bill generated successfully. BillNumber: {bill.BillNumber}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GenerateBill", ex);
                errorMessage = "An unexpected error occurred while generating bill";
                return false;
            }
        }
        public bool UpdateBill(int billId, Billing updatedBill, List<Billing> existingBills, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"UpdateBill called for ID: {billId}");
                Billing existingBill = null;
                foreach (var bill in existingBills)
                {
                    if (bill.BillingId == billId)
                    {
                        existingBill = bill;
                        break;
                    }
                }

                if (existingBill == null)
                {
                    errorMessage = "Bill not found";
                    _logger.LogWarning($"Bill not found. ID: {billId}");
                    return false;
                }
                var validationErrors = ValidateBill(updatedBill);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }
                decimal newTotal = CalculateTotalAmount(updatedBill.ConsultationFee, updatedBill.MedicineCharges, updatedBill.OtherCharges, updatedBill.Discount);
                _logger.LogAudit("Bill Updated", "System", $"BillId: {billId}, BillNumber: {existingBill.BillNumber}");
                _logger.LogInfo($"Bill updated successfully. ID: {billId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateBill", ex);
                errorMessage = "An unexpected error occurred while updating bill";
                return false;
            }
        }
        public bool DeleteBill(int billId, List<Billing> existingBills, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"DeleteBill called for ID: {billId}");
                bool billExists = false;
                foreach (var bill in existingBills)
                {
                    if (bill.BillingId == billId)
                    {
                        billExists = true;
                        break;
                    }
                }
                if (!billExists)
                {
                    errorMessage = "Bill not found";
                    _logger.LogWarning($"Bill not found. ID: {billId}");
                    return false;
                }
                _logger.LogAudit("Bill Deleted", "System", $"BillId: {billId}");
                _logger.LogInfo($"Bill deleted successfully. ID: {billId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteBill", ex);
                errorMessage = "An unexpected error occurred while deleting bill";
                return false;
            }
        }
        public List<Billing> FilterBillsByPatient(int patientId, List<Billing> allBills)
        {
            try
            {
                List<Billing> result = new List<Billing>();
                foreach (var bill in allBills)
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
                _logger.LogError("Error in FilterBillsByPatient", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> FilterBillsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> allBills)
        {
            try
            {
                List<Billing> result = new List<Billing>();

                foreach (var bill in allBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date)
                    {
                        result.Add(bill);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in FilterBillsByDateRange", ex);
                return new List<Billing>();
            }
        }
        public List<Billing> SearchBills(string keyword, List<Billing> allBills)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return allBills;

                string searchTerm = keyword.ToLower();
                List<Billing> results = new List<Billing>();

                foreach (var bill in allBills)
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
                _logger.LogError("Error in SearchBills", ex);
                return new List<Billing>();
            }
        }
        public decimal CalculateTotalAmount(decimal consultationFee, decimal medicineCharges, decimal otherCharges, decimal discount)
        {
            return consultationFee + medicineCharges + otherCharges - discount;
        }
        public decimal GetTotalRevenueByDateRange(DateTime fromDate, DateTime toDate, List<Billing> allBills)
        {
            try
            {
                decimal total = 0;

                foreach (var bill in allBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date)
                    {
                        if (bill.PaymentStatus == "Paid")
                        {
                            total += bill.TotalAmount;
                        }
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalRevenueByDateRange", ex);
                return 0;
            }
        }
        public Dictionary<string, decimal> GetPaymentMethodSummary(DateTime fromDate, DateTime toDate, List<Billing> allBills)
        {
            try
            {
                Dictionary<string, decimal> summary = new Dictionary<string, decimal>();
                summary["Cash"] = 0;
                summary["UPI"] = 0;
                summary["Card"] = 0;
                foreach (var bill in allBills)
                {
                    if (bill.BillingDate.Date >= fromDate.Date && bill.BillingDate.Date <= toDate.Date)
                    {
                        if (bill.PaymentStatus == "Paid")
                        {
                            if (summary.ContainsKey(bill.PaymentMethod))
                            {
                                summary[bill.PaymentMethod] += bill.TotalAmount;
                            }
                        }
                    }
                }
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPaymentMethodSummary", ex);
                return new Dictionary<string, decimal>();
            }
        }
        public List<string> ValidateBill(Billing bill)
        {
            List<string> errors = new List<string>();

            if (bill.PatientId <= 0)
            {
                errors.Add("Valid patient is required");
            }

            if (bill.ConsultationFee < 0)
            {
                errors.Add("Consultation fee cannot be negative");
            }
            if (bill.MedicineCharges < 0)
            {
                errors.Add("Medicine charges cannot be negative");
            }
            if (bill.OtherCharges < 0)
            {
                errors.Add("Other charges cannot be negative");
            }
            if (bill.Discount < 0)
            {
                errors.Add("Discount cannot be negative");
            }
            if (bill.Discount > (bill.ConsultationFee + bill.MedicineCharges + bill.OtherCharges))
            {
                errors.Add("Discount cannot exceed total charges");
            }
            if (string.IsNullOrWhiteSpace(bill.PaymentMethod))
            {
                errors.Add("Payment method is required");
            }
            else if (bill.PaymentMethod != "Cash" && bill.PaymentMethod != "UPI" && bill.PaymentMethod != "Card")
            {
                errors.Add("Payment method must be Cash, UPI, or Card");
            }
            return errors;
        }
        public string GenerateBillNumber()
        {
            string billNumber = $"BILL{_billCounter.ToString().PadLeft(4, '0')}";
            _billCounter++;
            return billNumber;
        }
        public bool UpdatePaymentStatus(int billId, string status, string paymentMethod, List<Billing> existingBills, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"UpdatePaymentStatus called for BillId: {billId}, Status: {status}");
                bool billExists = false;
                foreach (var b in existingBills)
                {
                    if (b.BillingId == billId)
                    {
                        billExists = true;
                        break;
                    }
                }
                if (!billExists)
                {
                    errorMessage = "Bill not found";
                    _logger.LogWarning($"Bill not found. ID: {billId}");
                    return false;
                }
                if (status != "Paid" && status != "Pending" && status != "Cancelled")
                {
                    errorMessage = "Invalid payment status";
                    return false;
                }
                _logger.LogAudit("Payment Status Updated", "System", $"BillId: {billId}, New Status: {status}");
                _logger.LogInfo($"Payment status updated successfully. BillId: {billId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdatePaymentStatus", ex);
                errorMessage = "An unexpected error occurred while updating payment status";
                return false;
            }
        }
    }
}