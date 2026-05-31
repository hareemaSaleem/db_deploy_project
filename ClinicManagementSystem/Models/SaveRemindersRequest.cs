using ClinicManagementSystem.Pages.Doctor;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.Models
{
    public class SaveRemindersRequest
    {
        public List<ReminderPatient> Patients { get; set; }
        public string Channel { get; set; }
        public bool SendNow { get; set; }
        public DateTime ScheduledDateTime { get; set; }
    }
}