using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class DoctorDashboardBL:IDoctorDashboard
    {
        private readonly ILoggingService _logger;
        public DoctorDashboardBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public int GetTodayAppointmentsCount(int doctorId, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug("GetTodayAppointmentsCount called");
                int count = 0;
                DateTime today = DateTime.Today;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == today.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTodayAppointmentsCount", ex);
                return 0;
            }
        }
        public int GetTodayPrescriptionsCount(int doctorId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug("GetTodayPrescriptionsCount called");
                int count = 0;
                DateTime today = DateTime.Today;
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.DoctorId == doctorId && pres.VisitDate.Date == today.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTodayPrescriptionsCount", ex);
                return 0;
            }
        }
        public int GetPendingFollowUpsCount(int doctorId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug("GetPendingFollowUpsCount called");
                int count = 0;
                DateTime today = DateTime.Today;
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.DoctorId == doctorId && pres.FollowUpDate != null)
                    {
                        if (pres.FollowUpDate.Value.Date >= today.Date)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingFollowUpsCount", ex);
                return 0;
            }
        }
        public int GetPatientsThisMonthCount(int doctorId, List<Appointment> existingAppointments, List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug("GetPatientsThisMonthCount called");
                List<int> uniquePatientIds = new List<int>();
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date >= firstDayOfMonth.Date)
                    {
                        bool exists = false;
                        foreach (int id in uniquePatientIds)
                        {
                            if (id == apt.PatientId)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            uniquePatientIds.Add(apt.PatientId);
                        }
                    }
                }

                return uniquePatientIds.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientsThisMonthCount", ex);
                return 0;
            }
        }
        public List<Prescription> GetRecentPrescriptions(int doctorId, List<Prescription> existingPrescriptions, int count)
        {
            try
            {
                _logger.LogDebug("GetRecentPrescriptions called");
                List<Prescription> doctorPrescriptions = new List<Prescription>();
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.DoctorId == doctorId)
                    {
                        doctorPrescriptions.Add(pres);
                    }
                }
                for (int i = 0; i < doctorPrescriptions.Count - 1; i++)
                {
                    for (int j = i + 1; j < doctorPrescriptions.Count; j++)
                    {
                        if (doctorPrescriptions[i].VisitDate < doctorPrescriptions[j].VisitDate)
                        {
                            Prescription temp = doctorPrescriptions[i];
                            doctorPrescriptions[i] = doctorPrescriptions[j];
                            doctorPrescriptions[j] = temp;
                        }
                    }
                }
                List<Prescription> recentPrescriptions = new List<Prescription>();
                int maxCount = count;
                if (doctorPrescriptions.Count < maxCount)
                {
                    maxCount = doctorPrescriptions.Count;
                }
                for (int i = 0; i < maxCount; i++)
                {
                    recentPrescriptions.Add(doctorPrescriptions[i]);
                }
                return recentPrescriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetRecentPrescriptions", ex);
                return new List<Prescription>();
            }
        }
        public List<Prescription> GetDueFollowUps(int doctorId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug("GetDueFollowUps called");

                List<Prescription> dueFollowUps = new List<Prescription>();
                DateTime today = DateTime.Today;

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.DoctorId == doctorId && pres.FollowUpDate != null)
                    {
                        if (pres.FollowUpDate.Value.Date >= today.Date)
                        {
                            dueFollowUps.Add(pres);
                        }
                    }
                }
                for (int i = 0; i < dueFollowUps.Count - 1; i++)
                {
                    for (int j = i + 1; j < dueFollowUps.Count; j++)
                    {
                        if (dueFollowUps[i].FollowUpDate > dueFollowUps[j].FollowUpDate)
                        {
                            Prescription temp = dueFollowUps[i];
                            dueFollowUps[i] = dueFollowUps[j];
                            dueFollowUps[j] = temp;
                        }
                    }
                }

                return dueFollowUps;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetDueFollowUps", ex);
                return new List<Prescription>();
            }
        }

        public List<Prescription> GetOverdueFollowUps(int doctorId, List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug("GetOverdueFollowUps called");
                List<Prescription> overdueFollowUps = new List<Prescription>();
                DateTime today = DateTime.Today;

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.DoctorId == doctorId && pres.FollowUpDate != null)
                    {
                        if (pres.FollowUpDate.Value.Date < today.Date)
                        {
                            overdueFollowUps.Add(pres);
                        }
                    }
                }

                for (int i = 0; i < overdueFollowUps.Count - 1; i++)
                {
                    for (int j = i + 1; j < overdueFollowUps.Count; j++)
                    {
                        if (overdueFollowUps[i].FollowUpDate < overdueFollowUps[j].FollowUpDate)
                        {
                            Prescription temp = overdueFollowUps[i];
                            overdueFollowUps[i] = overdueFollowUps[j];
                            overdueFollowUps[j] = temp;
                        }
                    }
                }

                return overdueFollowUps;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetOverdueFollowUps", ex);
                return new List<Prescription>();
            }
        }
    }
}