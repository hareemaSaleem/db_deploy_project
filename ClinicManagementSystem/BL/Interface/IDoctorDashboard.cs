using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IDoctorDashboard
    {
        int GetTodayAppointmentsCount(int doctorId, List<Appointment> existingAppointments);
        int GetTodayPrescriptionsCount(int doctorId, List<Prescription> existingPrescriptions);
        int GetPendingFollowUpsCount(int doctorId, List<Prescription> existingPrescriptions);
        int GetPatientsThisMonthCount(int doctorId, List<Appointment> existingAppointments, List<Patient> existingPatients);
        List<Prescription> GetRecentPrescriptions(int doctorId, List<Prescription> existingPrescriptions, int count);
        List<Prescription> GetDueFollowUps(int doctorId, List<Prescription> existingPrescriptions);
        List<Prescription> GetOverdueFollowUps(int doctorId, List<Prescription> existingPrescriptions);
    }
}