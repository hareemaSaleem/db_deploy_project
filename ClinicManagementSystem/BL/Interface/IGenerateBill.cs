using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IGenerateBill
    {
        bool GenerateBill(Billing bill, List<Billing> existingBills, out string errorMessage);
        bool UpdateBill(int billId, Billing updatedBill, List<Billing> existingBills, out string errorMessage);
        bool DeleteBill(int billId, List<Billing> existingBills, out string errorMessage);
        List<Billing> FilterBillsByPatient(int patientId, List<Billing> allBills);
        List<Billing> FilterBillsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> allBills);
        List<Billing> SearchBills(string keyword, List<Billing> allBills);
        decimal CalculateTotalAmount(decimal consultationFee, decimal medicineCharges, decimal otherCharges, decimal discount);
        decimal GetTotalRevenueByDateRange(DateTime fromDate, DateTime toDate, List<Billing> allBills);
        Dictionary<string, decimal> GetPaymentMethodSummary(DateTime fromDate, DateTime toDate, List<Billing> allBills);
        List<string> ValidateBill(Billing bill);
        string GenerateBillNumber();
        bool UpdatePaymentStatus(int billId, string status, string paymentMethod, List<Billing> existingBills, out string errorMessage);
    }
}