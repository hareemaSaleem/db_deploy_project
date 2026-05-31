using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class FollowUpBL
    {
        private readonly ILoggingService _logger;
        public FollowUpBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public int GetRemindersSentTodayCount(List<VisitLog> existingVisitLogs)
        {
            try
            {
                _logger.LogDebug("GetRemindersSentTodayCount called");
                int count = 0;
                DateTime today = DateTime.Today;
                foreach (var log in existingVisitLogs)
                {
                    if (log.VisitDate.Date == today.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetRemindersSentTodayCount", ex);
                return 0;
            }
        }
        public int GetScheduledForTomorrowCount(List<Prescription> existingPrescriptions)
        {
            try
            {
                _logger.LogDebug("GetScheduledForTomorrowCount called");

                int count = 0;
                DateTime tomorrow = DateTime.Today.AddDays(1);

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.FollowUpDate != null && pres.FollowUpDate.Value.Date == tomorrow.Date)
                    {
                        count++;
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetScheduledForTomorrowCount", ex);
                return 0;
            }
        }

        public int GetUpcomingFollowUpsCount(List<Prescription> existingPrescriptions, int days)
        {
            try
            {
                _logger.LogDebug("GetUpcomingFollowUpsCount called");

                int count = 0;
                DateTime today = DateTime.Today;
                DateTime endDate = today.AddDays(days);

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.FollowUpDate != null)
                    {
                        if (pres.FollowUpDate.Value.Date >= today.Date && pres.FollowUpDate.Value.Date <= endDate.Date)
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetUpcomingFollowUpsCount", ex);
                return 0;
            }
        }

        public int GetTotalSentThisMonthCount(List<VisitLog> existingVisitLogs)
        {
            try
            {
                _logger.LogDebug("GetTotalSentThisMonthCount called");

                int count = 0;
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                foreach (var log in existingVisitLogs)
                {
                    if (log.VisitDate.Date >= firstDayOfMonth.Date)
                    {
                        count++;
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTotalSentThisMonthCount", ex);
                return 0;
            }
        }

        public List<Patient> GetPatientsWithDueFollowUps(List<Prescription> existingPrescriptions, List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug("GetPatientsWithDueFollowUps called");

                List<int> patientIds = new List<int>();
                DateTime today = DateTime.Today;

                foreach (var pres in existingPrescriptions)
                {
                    if (pres.FollowUpDate != null && pres.FollowUpDate.Value.Date >= today.Date)
                    {
                        bool exists = false;
                        foreach (int id in patientIds)
                        {
                            if (id == pres.PatientId)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            patientIds.Add(pres.PatientId);
                        }
                    }
                }
                List<Patient> duePatients = new List<Patient>();
                foreach (int patientId in patientIds)
                {
                    foreach (var patient in existingPatients)
                    {
                        if (patient.PatientId == patientId)
                        {
                            duePatients.Add(patient);
                            break;
                        }
                    }
                }
                for (int i = 0; i < duePatients.Count - 1; i++)
                {
                    for (int j = i + 1; j < duePatients.Count; j++)
                    {
                        if (string.Compare(duePatients[i].Name, duePatients[j].Name) > 0)
                        {
                            Patient temp = duePatients[i];
                            duePatients[i] = duePatients[j];
                            duePatients[j] = temp;
                        }
                    }
                }
                return duePatients;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientsWithDueFollowUps", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> GetFilteredPatientsForReminders(List<Prescription> existingPrescriptions, List<Patient> existingPatients, DateTime followUpDate, string searchTerm)
        {
            try
            {
                _logger.LogDebug("GetFilteredPatientsForReminders called");
                List<int> patientIds = new List<int>();
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.FollowUpDate != null && pres.FollowUpDate.Value.Date == followUpDate.Date)
                    {
                        bool exists = false;
                        foreach (int id in patientIds)
                        {
                            if (id == pres.PatientId)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            patientIds.Add(pres.PatientId);
                        }
                    }
                }
                List<Patient> patientsForReminder = new List<Patient>();
                foreach (int patientId in patientIds)
                {
                    foreach (var patient in existingPatients)
                    {
                        if (patient.PatientId == patientId)
                        {
                            patientsForReminder.Add(patient);
                            break;
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string term = searchTerm.ToLower();
                    List<Patient> filteredPatients = new List<Patient>();
                    foreach (var patient in patientsForReminder)
                    {
                        if (patient.Name.ToLower().Contains(term) ||
                            (patient.Email != null && patient.Email.ToLower().Contains(term)) ||
                            patient.Phone.Contains(term))
                        {
                            filteredPatients.Add(patient);
                        }
                    }
                    patientsForReminder = filteredPatients;
                }

                for (int i = 0; i < patientsForReminder.Count - 1; i++)
                {
                    for (int j = i + 1; j < patientsForReminder.Count; j++)
                    {
                        if (string.Compare(patientsForReminder[i].Name, patientsForReminder[j].Name) > 0)
                        {
                            Patient temp = patientsForReminder[i];
                            patientsForReminder[i] = patientsForReminder[j];
                            patientsForReminder[j] = temp;
                        }
                    }
                }
                return patientsForReminder;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetFilteredPatientsForReminders", ex);
                return new List<Patient>();
            }
        }
        public List<Patient> GetPatientsByFollowUpDate(DateTime followUpDate, List<Prescription> existingPrescriptions, List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug("GetPatientsByFollowUpDate called");
                List<int> patientIds = new List<int>();
                foreach (var pres in existingPrescriptions)
                {
                    if (pres.FollowUpDate != null && pres.FollowUpDate.Value.Date == followUpDate.Date)
                    {
                        bool exists = false;
                        foreach (int id in patientIds)
                        {
                            if (id == pres.PatientId)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            patientIds.Add(pres.PatientId);
                        }
                    }
                }

                List<Patient> result = new List<Patient>();

                foreach (int patientId in patientIds)
                {
                    foreach (var patient in existingPatients)
                    {
                        if (patient.PatientId == patientId)
                        {
                            result.Add(patient);
                            break;
                        }
                    }
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (string.Compare(result[i].Name, result[j].Name) > 0)
                        {
                            Patient temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientsByFollowUpDate", ex);
                return new List<Patient>();
            }
        }
        public string GetPatientContactMethod(Patient patient)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(patient.Email))
                {
                    return "Email";
                }
                else if (!string.IsNullOrWhiteSpace(patient.Phone))
                {
                    return "SMS";
                }
                else
                {
                    return "No Contact";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPatientContactMethod", ex);
                return "Unknown";
            }
        }
    }
}
