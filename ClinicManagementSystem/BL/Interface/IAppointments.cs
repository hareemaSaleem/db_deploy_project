using System;
using System.Collections.Generic;
using ClinicManagementSystem.Models;
namespace ClinicManagementSystem.BL.Interface
{
    public interface IAppointments
    {
        bool BookAppointment(Appointment appointment, List<Appointment> existingAppointments, out string errorMessage);
        bool CancelAppointment(int appointmentId, List<Appointment> existingAppointments, out string errorMessage);
        bool RescheduleAppointment(int appointmentId, DateTime newDate, TimeSpan newTime, List<Appointment> existingAppointments, out string errorMessage);
        List<Appointment> GetAppointmentsByDoctor(int doctorId, DateTime date, List<Appointment> allAppointments);
        List<Appointment> GetAppointmentsByPatient(int patientId, List<Appointment> allAppointments);
        List<Appointment> GetTodaysAppointments(int doctorId, List<Appointment> allAppointments);
        Appointment GetAppointmentById(int appointmentId, List<Appointment> allAppointments);
        bool IsTimeSlotAvailable(int doctorId, DateTime date, TimeSpan time, List<Appointment> existingAppointments);
        List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date, List<Appointment> existingAppointments);
        List<string> ValidateAppointment(Appointment appointment);
        int GetBookedSlotsCount(int doctorId, DateTime date, List<Appointment> allAppointments);
        int GetAvailableSlotsCount(int doctorId, DateTime date, List<Appointment> allAppointments);
        int GetTotalSlotsCount();
        int CalculateQueuePosition(string priority, List<Appointment> todaysAppointments);
        string FormatTimeSlot(TimeSpan time);
        List<string> GetAllTimeSlots();
    }
}
