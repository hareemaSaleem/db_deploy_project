using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.BL;
using ClinicManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class ReceptionistDashboardModel : PageModel
    {
        private readonly AppointmentRepository _appointmentRepo;
        private readonly PatientRepository _patientRepo;
        private readonly BillingRepository _billingRepo;
        private readonly ReceptionistDashboardBL _dashboardBL;

        public ReceptionistDashboardModel(
            AppointmentRepository appointmentRepo,
            PatientRepository patientRepo,
            BillingRepository billingRepo,
            ReceptionistDashboardBL dashboardBL)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _billingRepo = billingRepo;
            _dashboardBL = dashboardBL;
        }
        public int TodayAppointmentsCount { get; set; }
        public int NewPatientsCount { get; set; }
        public decimal TodayBillingsTotal { get; set; }
        public decimal PendingPaymentsTotal { get; set; }
        public List<Appointment> TodayAppointments { get; set; }
        public List<Appointment> WaitingQueue { get; set; }

        public async Task OnGetAsync()
        {
            var allAppointments = await _appointmentRepo.GetAllAppointmentsAsync();
            var allPatients = await _patientRepo.GetAllPatientsAsync();
            var allBills = await _billingRepo.GetAllBillingsAsync();
            TodayAppointmentsCount = _dashboardBL.GetTodayAppointmentsCount(allAppointments);
            NewPatientsCount = _dashboardBL.GetNewPatientsCount(allPatients);
            TodayBillingsTotal = _dashboardBL.GetTodayBillingsTotal(allBills);
            PendingPaymentsTotal = _dashboardBL.GetPendingPaymentsTotal(allBills);
            TodayAppointments = _dashboardBL.GetTodayAppointments(allAppointments);
            WaitingQueue = _dashboardBL.GetWaitingQueue(allAppointments);
        }
    }
}