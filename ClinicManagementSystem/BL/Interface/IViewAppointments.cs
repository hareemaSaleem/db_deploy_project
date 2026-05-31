using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IViewAppointments
    {
        List<Appointment> GetAppointmentsByDoctor(int doctorId, List<Appointment> existingAppointments);
        List<Appointment> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date, List<Appointment> existingAppointments);
        List<Appointment> GetAppointmentsByDoctorAndStatus(int doctorId, string status, List<Appointment> existingAppointments);
        List<Appointment> GetFilteredAppointments(int doctorId, DateTime date, string status, List<Appointment> existingAppointments);
        Appointment GetAppointmentById(int appointmentId, List<Appointment> existingAppointments);
        Dictionary<string, int> GetAppointmentStatusCounts(int doctorId, DateTime date, List<Appointment> existingAppointments);
        int GetTotalAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments);
        int GetCompletedAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments);
        int GetScheduledAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments);
        int GetCancelledAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments);
        int GetNoShowAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments);
    }
}
