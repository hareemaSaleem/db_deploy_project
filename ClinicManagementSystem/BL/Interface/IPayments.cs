using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IPayments
    {
        List<Billing> GetAllPayments(List<Billing> existingBills);
        List<Billing> GetPaymentsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> existingBills);
        List<Billing> GetPaymentsByStatus(string status, List<Billing> existingBills);
        List<Billing> SearchPayments(string keyword, List<Billing> existingBills);
        Billing GetPaymentById(int paymentId, List<Billing> existingBills);
        List<Doctor> GetAllDoctors(List<Doctor> existingDoctors);
        List<Receptionist> GetAllStaff(List<Receptionist> existingStaff);
        bool AddPayment(Billing payment, List<Billing> existingBills, out string errorMessage);
        List<string> ValidatePayment(Billing payment);
        string GenerateReceiptNumber(int paymentId);
        Dictionary<string, decimal> GetPaymentSummary(List<Billing> existingBills);
        decimal GetTotalPaymentsByDateRange(DateTime fromDate, DateTime toDate, List<Billing> existingBills);
    }
}