namespace ClinicManagementSystem.Models
{
    public class ReminderStats
    {
        public int SentToday { get; set; }
        public int ScheduledForTomorrow { get; set; }
        public int UpcomingNext7Days { get; set; }
        public int TotalSentThisMonth { get; set; }
    }
}
