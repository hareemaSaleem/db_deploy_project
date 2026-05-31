using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IReceptionistDashboard
    {
        int GetTodayAppointmentsCount(List<Appointment> existingAppointments);
        int GetNewPatientsCount(List<Patient> existingPatients);
        decimal GetTodayBillingsTotal(List<Billing> existingBills);
        decimal GetPendingPaymentsTotal(List<Billing> existingBills);
        List<Appointment> GetTodayAppointments(List<Appointment> existingAppointments);
        List<Appointment> GetWaitingQueue(List<Appointment> existingAppointments);
        List<Appointment> GetAppointmentsByPriority(List<Appointment> existingAppointments);
    }
}