using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.ClinicOwner
{
    public class ClinicOwnerDashboardModel : PageModel
    {
        private readonly DoctorRepository _doctorRepo;
        private readonly ReceptionistRepository _receptionistRepo;
        private readonly BillingRepository _billingRepo;

        public ClinicOwnerDashboardModel(
            DoctorRepository doctorRepo,
            ReceptionistRepository receptionistRepo,
            BillingRepository billingRepo)
        {
            _doctorRepo = doctorRepo;
            _receptionistRepo = receptionistRepo;
            _billingRepo = billingRepo;
        }

        // Statistics
        public int TotalDoctorsCount { get; set; }
        public int ActiveDoctorsCount { get; set; }
        public int TotalStaffCount { get; set; }
        public int ActiveStaffCount { get; set; }
        public decimal TotalPaymentsThisMonth { get; set; }
        public int PendingPaymentsCount { get; set; }

        // Payment Summary
        public decimal TotalPayableThisMonth { get; set; }
        public decimal TotalPaidThisMonth { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public decimal TotalOverdueAmount { get; set; }

        // Chart Data - Last 7 months
        public List<string> ChartLabels { get; set; }
        public List<decimal> ChartData { get; set; }

        // Pending Payments List
        public List<Billing> PendingPaymentsList { get; set; }

        // Recent Payments List
        public List<Billing> RecentPayments { get; set; }

        // Payment Percentages
        public decimal PaidPercentage { get; set; }
        public decimal PendingPercentage { get; set; }
        public decimal OverduePercentage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get all doctors
            var allDoctors = await _doctorRepo.GetAllDoctorsAsync();
            TotalDoctorsCount = allDoctors.Count;
            ActiveDoctorsCount = allDoctors.Count(d => d.Status == "Active");

            // Get all staff (receptionists)
            var allStaff = await _receptionistRepo.GetAllReceptionistsAsync();
            TotalStaffCount = allStaff.Count;
            ActiveStaffCount = allStaff.Count(s => s.Status == "Active");

            // Get all billings
            var allBillings = await _billingRepo.GetAllBillingsAsync();

            // Calculate current month (this month)
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Total payments this month (paid)
            TotalPaymentsThisMonth = allBillings
                .Where(b => b.BillingDate.Date >= firstDayOfMonth.Date
                         && b.BillingDate.Date <= lastDayOfMonth.Date
                         && b.PaymentStatus == "Paid")
                .Sum(b => b.TotalAmount);

            // Pending payments count (all time)
            PendingPaymentsCount = allBillings.Count(b => b.PaymentStatus == "Pending");

            // Payment Summary for current month
            TotalPayableThisMonth = allBillings
                .Where(b => b.BillingDate.Date >= firstDayOfMonth.Date
                         && b.BillingDate.Date <= lastDayOfMonth.Date)
                .Sum(b => b.TotalAmount);

            TotalPaidThisMonth = allBillings
                .Where(b => b.BillingDate.Date >= firstDayOfMonth.Date
                         && b.BillingDate.Date <= lastDayOfMonth.Date
                         && b.PaymentStatus == "Paid")
                .Sum(b => b.TotalAmount);

            TotalPendingAmount = allBillings
                .Where(b => b.BillingDate.Date >= firstDayOfMonth.Date
                         && b.BillingDate.Date <= lastDayOfMonth.Date
                         && b.PaymentStatus == "Pending")
                .Sum(b => b.TotalAmount);

            TotalOverdueAmount = allBillings
                .Where(b => b.PaymentStatus == "Pending" && b.BillingDate.Date < today.Date)
                .Sum(b => b.TotalAmount);

            // Calculate percentages
            if (TotalPayableThisMonth > 0)
            {
                PaidPercentage = (TotalPaidThisMonth / TotalPayableThisMonth) * 100;
                PendingPercentage = (TotalPendingAmount / TotalPayableThisMonth) * 100;
                OverduePercentage = (TotalOverdueAmount / TotalPayableThisMonth) * 100;
            }

            // Monthly payment trend for chart (last 7 months)
            ChartLabels = new List<string>();
            ChartData = new List<decimal>();

            for (int i = 6; i >= 0; i--)
            {
                var monthDate = today.AddMonths(-i);
                var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthName = monthDate.ToString("MMM yyyy");

                var monthlyTotal = allBillings
                    .Where(b => b.BillingDate.Date >= monthStart.Date
                             && b.BillingDate.Date <= monthEnd.Date
                             && b.PaymentStatus == "Paid")
                    .Sum(b => b.TotalAmount);

                ChartLabels.Add(monthName);
                ChartData.Add(monthlyTotal);
            }
            PendingPaymentsList = allBillings
                .Where(b => b.PaymentStatus == "Pending")
                .OrderBy(b => b.BillingDate)
                .Take(10)
                .ToList();
            RecentPayments = allBillings
                .Where(b => b.PaymentStatus == "Paid")
                .OrderByDescending(b => b.BillingDate)
                .Take(5)
                .ToList();

            return Page();
        }
    }
}