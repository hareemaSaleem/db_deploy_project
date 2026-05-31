using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class ViewAppointmentsBL : IViewAppointments
    {
        private readonly ILoggingService _logger;
        public ViewAppointmentsBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public List<Appointment> GetAppointmentsByDoctor(int doctorId, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetAppointmentsByDoctor called for DoctorId: {doctorId}");
                List<Appointment> result = new List<Appointment>();
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId)
                    {
                        result.Add(apt);
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].AppointmentDate < result[j].AppointmentDate)
                        {
                            Appointment temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                        else if (result[i].AppointmentDate == result[j].AppointmentDate)
                        {
                            if (result[i].AppointmentTime > result[j].AppointmentTime)
                            {
                                Appointment temp = result[i];
                                result[i] = result[j];
                                result[j] = temp;
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentsByDoctor", ex);
                return new List<Appointment>();
            }
        }
        public List<Appointment> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetAppointmentsByDoctorAndDate called for DoctorId: {doctorId}, Date: {date}");
                List<Appointment> result = new List<Appointment>();
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date)
                    {
                        result.Add(apt);
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].AppointmentTime > result[j].AppointmentTime)
                        {
                            Appointment temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentsByDoctorAndDate", ex);
                return new List<Appointment>();
            }
        }
        public List<Appointment> GetAppointmentsByDoctorAndStatus(int doctorId, string status, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetAppointmentsByDoctorAndStatus called for DoctorId: {doctorId}, Status: {status}");
                List<Appointment> result = new List<Appointment>();
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.Status == status)
                    {
                        result.Add(apt);
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].AppointmentDate < result[j].AppointmentDate)
                        {
                            Appointment temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentsByDoctorAndStatus", ex);
                return new List<Appointment>();
            }
        }

        public List<Appointment> GetFilteredAppointments(int doctorId, DateTime date, string status, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetFilteredAppointments called for DoctorId: {doctorId}, Date: {date}, Status: {status}");

                List<Appointment> result = new List<Appointment>();

                foreach (var apt in existingAppointments)
                {
                    bool matches = true;

                    if (apt.DoctorId != doctorId)
                    {
                        matches = false;
                    }

                    if (matches && date != default)
                    {
                        if (apt.AppointmentDate.Date != date.Date)
                        {
                            matches = false;
                        }
                    }

                    if (matches && !string.IsNullOrWhiteSpace(status) && status != "all")
                    {
                        if (apt.Status != status)
                        {
                            matches = false;
                        }
                    }

                    if (matches)
                    {
                        result.Add(apt);
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (result[i].AppointmentTime > result[j].AppointmentTime)
                        {
                            Appointment temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetFilteredAppointments", ex);
                return new List<Appointment>();
            }
        }
        public Appointment GetAppointmentById(int appointmentId, List<Appointment> existingAppointments)
        {
            try
            {
                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentId == appointmentId)
                    {
                        return apt;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentById", ex);
                return null;
            }
        }
        public Dictionary<string, int> GetAppointmentStatusCounts(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug($"GetAppointmentStatusCounts called for DoctorId: {doctorId}, Date: {date}");
                Dictionary<string, int> counts = new Dictionary<string, int>();
                counts["all"] = 0;
                counts["completed"] = 0;
                counts["scheduled"] = 0;
                counts["cancelled"] = 0;
                counts["no-show"] = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date)
                    {
                        counts["all"]++;
                        if (apt.Status == "completed")
                        {
                            counts["completed"]++;
                        }
                        else if (apt.Status == "scheduled")
                        {
                            counts["scheduled"]++;
                        }
                        else if (apt.Status == "cancelled")
                        {
                            counts["cancelled"]++;
                        }
                        else if (apt.Status == "no-show")
                        {
                            counts["no-show"]++;
                        }
                    }
                }
                return counts;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentStatusCounts", ex);
                return new Dictionary<string, int>();
            }
        }
        public int GetTotalAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalAppointmentsCount", ex);
                return 0;
            }
        }
        public int GetCompletedAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date && apt.Status == "completed")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCompletedAppointmentsCount", ex);
                return 0;
            }
        }
        public int GetScheduledAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date && apt.Status == "scheduled")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetScheduledAppointmentsCount", ex);
                return 0;
            }
        }
        public int GetCancelledAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date && apt.Status == "cancelled")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCancelledAppointmentsCount", ex);
                return 0;
            }
        }
        public int GetNoShowAppointmentsCount(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            try
            {
                int count = 0;
                foreach (var apt in existingAppointments)
                {
                    if (apt.DoctorId == doctorId && apt.AppointmentDate.Date == date.Date && apt.Status == "no-show")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetNoShowAppointmentsCount", ex);
                return 0;
            }
        }
    }
}