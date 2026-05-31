using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ClinicSettings
    {
        [Key]
        public int Id { get; set; }
        public string ClinicName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string Currency { get; set; }
        public int ItemsPerPage { get; set; }
        public string Language { get; set; }
        public string ThemeMode { get; set; }
    }
}
