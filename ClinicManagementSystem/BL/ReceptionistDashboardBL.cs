using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class ReceptionistDashboardBL : IReceptionistDashboard
    {
        private readonly ILoggingService _logger;

        public ReceptionistDashboardBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public int GetTodayAppointmentsCount(List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug("GetTodayAppointmentsCount called");

                int count = 0;
                DateTime today = DateTime.Today;

                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date == today.Date)
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
        public int GetNewPatientsCount(List<Patient> existingPatients)
        {
            try
            {
                _logger.LogDebug("GetNewPatientsCount called");

                int count = 0;
                DateTime today = DateTime.Today;

                foreach (var patient in existingPatients)
                {
                    if (patient.CreatedAt.Date == today.Date)
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetNewPatientsCount", ex);
                return 0;
            }
        }
        public decimal GetTodayBillingsTotal(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetTodayBillingsTotal called");

                decimal total = 0;
                DateTime today = DateTime.Today;

                foreach (var bill in existingBills)
                {
                    if (bill.BillingDate.Date == today.Date && bill.PaymentStatus == "Paid")
                    {
                        total += bill.TotalAmount;
                    }
                }

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTodayBillingsTotal", ex);
                return 0;
            }
        }
        public decimal GetPendingPaymentsTotal(List<Billing> existingBills)
        {
            try
            {
                _logger.LogDebug("GetPendingPaymentsTotal called");

                decimal total = 0;

                foreach (var bill in existingBills)
                {
                    if (bill.PaymentStatus == "Pending")
                    {
                        total += bill.TotalAmount;
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingPaymentsTotal", ex);
                return 0;
            }
        }
        public List<Appointment> GetTodayAppointments(List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug("GetTodayAppointments called");
                List<Appointment> todayAppointments = new List<Appointment>();
                DateTime today = DateTime.Today;

                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date == today.Date)
                    {
                        todayAppointments.Add(apt);
                    }
                }
                for (int i = 0; i < todayAppointments.Count - 1; i++)
                {
                    for (int j = i + 1; j < todayAppointments.Count; j++)
                    {
                        if (todayAppointments[i].AppointmentTime > todayAppointments[j].AppointmentTime)
                        {
                            Appointment temp = todayAppointments[i];
                            todayAppointments[i] = todayAppointments[j];
                            todayAppointments[j] = temp;
                        }
                    }
                }
                return todayAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetTodayAppointments", ex);
                return new List<Appointment>();
            }
        }
        public List<Appointment> GetWaitingQueue(List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug("GetWaitingQueue called");
                List<Appointment> waitingQueue = new List<Appointment>();
                DateTime today = DateTime.Today;

                foreach (var apt in existingAppointments)
                {
                    if (apt.AppointmentDate.Date == today.Date && apt.Status == "Scheduled")
                    {
                        waitingQueue.Add(apt);
                    }
                }
                for (int i = 0; i < waitingQueue.Count - 1; i++)
                {
                    for (int j = i + 1; j < waitingQueue.Count; j++)
                    {
                        int priorityI = GetPriorityValue(waitingQueue[i].PriorityLevel);
                        int priorityJ = GetPriorityValue(waitingQueue[j].PriorityLevel);

                        if (priorityI > priorityJ)
                        {
                            Appointment temp = waitingQueue[i];
                            waitingQueue[i] = waitingQueue[j];
                            waitingQueue[j] = temp;
                        }
                        else if (priorityI == priorityJ)
                        {
                            if (waitingQueue[i].AppointmentTime > waitingQueue[j].AppointmentTime)
                            {
                                Appointment temp = waitingQueue[i];
                                waitingQueue[i] = waitingQueue[j];
                                waitingQueue[j] = temp;
                            }
                        }
                    }
                }
                return waitingQueue;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetWaitingQueue", ex);
                return new List<Appointment>();
            }
        }
        public List<Appointment> GetAppointmentsByPriority(List<Appointment> existingAppointments)
        {
            try
            {
                _logger.LogDebug("GetAppointmentsByPriority called");

                List<Appointment> sortedAppointments = new List<Appointment>();
                foreach (var apt in existingAppointments)
                {
                    sortedAppointments.Add(apt);
                }

                for (int i = 0; i < sortedAppointments.Count - 1; i++)
                {
                    for (int j = i + 1; j < sortedAppointments.Count; j++)
                    {
                        int priorityI = GetPriorityValue(sortedAppointments[i].PriorityLevel);
                        int priorityJ = GetPriorityValue(sortedAppointments[j].PriorityLevel);

                        if (priorityI > priorityJ)
                        {
                            Appointment temp = sortedAppointments[i];
                            sortedAppointments[i] = sortedAppointments[j];
                            sortedAppointments[j] = temp;
                        }
                    }
                }

                return sortedAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentsByPriority", ex);
                return new List<Appointment>();
            }
        }

        private int GetPriorityValue(string priority)
        {
            if (priority == "Emergency")
                return 1;
            else if (priority == "Normal")
                return 2;
            else if (priority == "Routine")
                return 3;
            return 2;
        }
    }
}