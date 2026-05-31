using ClinicManagementSystem.BL.Interface;
using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.BL
{
    public class AppointmentsBL : IAppointments
    {
        private readonly ILoggingService _logger;
        private readonly TimeSpan WORK_START = new TimeSpan(8, 0, 0);
        private readonly TimeSpan WORK_END = new TimeSpan(17, 0, 0);
        private readonly int SLOT_DURATION_MINUTES = 20;
        public AppointmentsBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool BookAppointment(Appointment appointment, List<Appointment> existingAppointments, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                _logger.LogInfo($"BookAppointment called for Patient: {appointment.PatientId}, Doctor: {appointment.DoctorId}");

                var validationErrors = ValidateAppointment(appointment);
                if (validationErrors.Count > 0)
                {
                    errorMessage = string.Join(", ", validationErrors);
                    _logger.LogWarning($"Validation failed: {errorMessage}");
                    return false;
                }

                bool isAvailable = IsTimeSlotAvailable(appointment.DoctorId, appointment.AppointmentDate, appointment.AppointmentTime, existingAppointments);
                if (!isAvailable)
                {
                    errorMessage = "Selected time slot is already booked";
                    _logger.LogWarning($"Time slot not available: {appointment.AppointmentDate} {appointment.AppointmentTime}");
                    return false;
                }

                if (appointment.AppointmentDate < DateTime.Today)
                {
                    errorMessage = "Cannot book appointment for a past date";
                    _logger.LogWarning($"Past date booking attempted: {appointment.AppointmentDate}");
                    return false;
                }

                if (appointment.AppointmentTime < WORK_START || appointment.AppointmentTime > WORK_END)
                {
                    errorMessage = "Appointment time must be between 8:00 AM and 5:00 PM";
                    _logger.LogWarning($"Outside working hours: {appointment.AppointmentTime}");
                    return false;
                }

                var todaysAppointments = GetAppointmentsByDoctor(appointment.DoctorId, appointment.AppointmentDate, existingAppointments);
                appointment.QueuePosition = CalculateQueuePosition(appointment.PriorityLevel, todaysAppointments);
                appointment.Duration = SLOT_DURATION_MINUTES;
                appointment.Status = "Scheduled";
                appointment.CreatedAt = DateTime.Now;

                _logger.LogAudit("Appointment Booked", "System", $"DoctorId: {appointment.DoctorId}, PatientId: {appointment.PatientId}, Time: {appointment.AppointmentTime}");
                _logger.LogInfo($"Appointment booked successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in BookAppointment", ex);
                errorMessage = "An unexpected error occurred while booking appointment";
                return false;
            }
        }

        public bool CancelAppointment(int appointmentId, List<Appointment> existingAppointments, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"CancelAppointment called for ID: {appointmentId}");

                Appointment appointmentToCancel = null;
                foreach (var app in existingAppointments)
                {
                    if (app.AppointmentId == appointmentId)
                    {
                        appointmentToCancel = app;
                        break;
                    }
                }

                if (appointmentToCancel == null)
                {
                    errorMessage = "Appointment not found";
                    _logger.LogWarning($"Appointment not found. ID: {appointmentId}");
                    return false;
                }

                if (appointmentToCancel.Status == "Completed")
                {
                    errorMessage = "Cannot cancel a completed appointment";
                    _logger.LogWarning($"Cannot cancel completed appointment. ID: {appointmentId}");
                    return false;
                }

                if (appointmentToCancel.Status == "Cancelled")
                {
                    errorMessage = "Appointment is already cancelled";
                    _logger.LogWarning($"Appointment already cancelled. ID: {appointmentId}");
                    return false;
                }

                DateTime appointmentDateTime = appointmentToCancel.AppointmentDate.Add(appointmentToCancel.AppointmentTime);
                if (appointmentDateTime < DateTime.Now.AddHours(1))
                {
                    errorMessage = "Cannot cancel appointment less than 1 hour before scheduled time";
                    _logger.LogWarning($"Late cancellation attempt. ID: {appointmentId}");
                    return false;
                }

                _logger.LogAudit("Appointment Cancelled", "System", $"AppointmentId: {appointmentId}, PatientId: {appointmentToCancel.PatientId}");
                _logger.LogInfo($"Appointment cancelled successfully. ID: {appointmentId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CancelAppointment", ex);
                errorMessage = "An unexpected error occurred while cancelling appointment";
                return false;
            }
        }

        public bool RescheduleAppointment(int appointmentId, DateTime newDate, TimeSpan newTime, List<Appointment> existingAppointments, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                _logger.LogInfo($"RescheduleAppointment called for ID: {appointmentId}");

                Appointment appointmentToReschedule = null;
                foreach (var app in existingAppointments)
                {
                    if (app.AppointmentId == appointmentId)
                    {
                        appointmentToReschedule = app;
                        break;
                    }
                }

                if (appointmentToReschedule == null)
                {
                    errorMessage = "Appointment not found";
                    _logger.LogWarning($"Appointment not found. ID: {appointmentId}");
                    return false;
                }

                bool isAvailable = IsTimeSlotAvailable(appointmentToReschedule.DoctorId, newDate, newTime, existingAppointments);
                if (!isAvailable)
                {
                    errorMessage = "Selected time slot is already booked";
                    _logger.LogWarning($"New time slot not available: {newDate} {newTime}");
                    return false;
                }

                if (newDate < DateTime.Today)
                {
                    errorMessage = "Cannot reschedule to a past date";
                    _logger.LogWarning($"Past date reschedule attempted: {newDate}");
                    return false;
                }

                if (newTime < WORK_START || newTime > WORK_END)
                {
                    errorMessage = "Appointment time must be between 8:00 AM and 5:00 PM";
                    _logger.LogWarning($"Outside working hours: {newTime}");
                    return false;
                }

                _logger.LogAudit("Appointment Rescheduled", "System", $"AppointmentId: {appointmentId}, New Time: {newDate} {newTime}");
                _logger.LogInfo($"Appointment rescheduled successfully. ID: {appointmentId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RescheduleAppointment", ex);
                errorMessage = "An unexpected error occurred while rescheduling appointment";
                return false;
            }
        }

        public List<Appointment> GetAppointmentsByDoctor(int doctorId, DateTime date, List<Appointment> allAppointments)
        {
            try
            {
                List<Appointment> result = new List<Appointment>();

                foreach (var app in allAppointments)
                {
                    if (app.DoctorId == doctorId && app.AppointmentDate.Date == date.Date)
                    {
                        result.Add(app);
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
                _logger.LogError("Error in GetAppointmentsByDoctor", ex);
                return new List<Appointment>();
            }
        }

        public List<Appointment> GetAppointmentsByPatient(int patientId, List<Appointment> allAppointments)
        {
            try
            {
                List<Appointment> result = new List<Appointment>();

                foreach (var app in allAppointments)
                {
                    if (app.PatientId == patientId)
                    {
                        result.Add(app);
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
                _logger.LogError("Error in GetAppointmentsByPatient", ex);
                return new List<Appointment>();
            }
        }

        public List<Appointment> GetTodaysAppointments(int doctorId, List<Appointment> allAppointments)
        {
            return GetAppointmentsByDoctor(doctorId, DateTime.Today, allAppointments);
        }
        public Appointment GetAppointmentById(int appointmentId, List<Appointment> allAppointments)
        {
            try
            {
                foreach (var app in allAppointments)
                {
                    if (app.AppointmentId == appointmentId)
                        return app;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAppointmentById", ex);
                return null;
            }
        }

        public bool IsTimeSlotAvailable(int doctorId, DateTime date, TimeSpan time, List<Appointment> existingAppointments)
        {
            try
            {
                foreach (var app in existingAppointments)
                {
                    if (app.DoctorId == doctorId &&
                        app.AppointmentDate.Date == date.Date &&
                        app.AppointmentTime == time &&
                        app.Status != "Cancelled")
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in IsTimeSlotAvailable", ex);
                return false;
            }
        }

        public List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date, List<Appointment> existingAppointments)
        {
            List<TimeSpan> availableSlots = new List<TimeSpan>();
            try
            {
                TimeSpan current = WORK_START;
                while (current <= WORK_END)
                {
                    availableSlots.Add(current);
                    current = current.Add(TimeSpan.FromMinutes(SLOT_DURATION_MINUTES));
                }
                List<TimeSpan> slotsToRemove = new List<TimeSpan>();
                foreach (var slot in availableSlots)
                {
                    if (!IsTimeSlotAvailable(doctorId, date, slot, existingAppointments))
                    {
                        slotsToRemove.Add(slot);
                    }
                }

                foreach (var slot in slotsToRemove)
                {
                    availableSlots.Remove(slot);
                }
                return availableSlots;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAvailableTimeSlots", ex);
                return new List<TimeSpan>();
            }
        }
        public List<string> ValidateAppointment(Appointment appointment)
        {
            List<string> errors = new List<string>();
            string errorMessage;
            if (appointment.PatientId <= 0)
            {
                errors.Add("Valid Patient is required");
            }

            if (appointment.DoctorId <= 0)
            {
                errors.Add("Valid Doctor is required");
            }
            if (!Validators.IsDateNotInFuture(appointment.AppointmentDate, out errorMessage))
            {
                errors.Add(errorMessage);
            }
            if (appointment.AppointmentTime < WORK_START || appointment.AppointmentTime > WORK_END)
            {
                errors.Add("Appointment time must be between 8:00 AM and 5:00 PM");
            }
            if (string.IsNullOrWhiteSpace(appointment.PriorityLevel))
            {
                errors.Add("Priority level is required");
            }
            else if (appointment.PriorityLevel != "Emergency" && appointment.PriorityLevel != "Normal" && appointment.PriorityLevel != "Routine")
            {
                errors.Add("Priority must be Emergency, Normal, or Routine");
            }
            return errors;
        }
        public int GetBookedSlotsCount(int doctorId, DateTime date, List<Appointment> allAppointments)
        {
            int count = 0;
            foreach (var app in allAppointments)
            {
                if (app.DoctorId == doctorId &&
                    app.AppointmentDate.Date == date.Date &&
                    app.Status != "Cancelled")
                {
                    count++;
                }
            }
            return count;
        }
        public int GetAvailableSlotsCount(int doctorId, DateTime date, List<Appointment> allAppointments)
        {
            int totalSlots = GetTotalSlotsCount();
            int bookedSlots = GetBookedSlotsCount(doctorId, date, allAppointments);
            return totalSlots - bookedSlots;
        }
        public int GetTotalSlotsCount()
        {
            return 27;
        }
        public int CalculateQueuePosition(string priority, List<Appointment> todaysAppointments)
        {
            int priorityValue = 0;
            if (priority == "Emergency") priorityValue = 1;
            else if (priority == "Normal") priorityValue = 2;
            else if (priority == "Routine") priorityValue = 3;
            int position = 1;
            foreach (var app in todaysAppointments)
            {
                int existingPriorityValue = 0;
                if (app.PriorityLevel == "Emergency") existingPriorityValue = 1;
                else if (app.PriorityLevel == "Normal") existingPriorityValue = 2;
                else if (app.PriorityLevel == "Routine") existingPriorityValue = 3;
                if (existingPriorityValue < priorityValue)
                {
                    position++;
                }
                else if (existingPriorityValue == priorityValue)
                {
                    if (app.AppointmentTime < DateTime.Now.TimeOfDay)
                    {
                        position++;
                    }
                }
            }
            return position;
        }
        public string FormatTimeSlot(TimeSpan time)
        {
            DateTime tempDate = DateTime.Today.Add(time);
            return tempDate.ToString("hh:mm tt");
        }
        public List<string> GetAllTimeSlots()
        {
            List<string> slots = new List<string>();
            TimeSpan current = WORK_START;
            while (current <= WORK_END)
            {
                slots.Add(FormatTimeSlot(current));
                current = current.Add(TimeSpan.FromMinutes(SLOT_DURATION_MINUTES));
            }
            return slots;
        }
    }
}