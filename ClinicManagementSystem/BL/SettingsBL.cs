using ClinicManagementSystem.BL.Logs;
using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL
{
    public class SettingsBL
    {
        private readonly ILoggingService _logger;
        public SettingsBL(ILoggingService logger)
        {
            _logger = logger;
        }
        public bool ValidateClinicInfo(string clinicName, string email, string phone, string address, string city, string zipCode, out string errorMessage)
        {
            errorMessage = "";
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(clinicName))
            {
                errors.Add("Clinic name is required");
            }
            else if (clinicName.Length < 2)
            {
                errors.Add("Clinic name must be at least 2 characters");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Email is required");
            }
            else if (!IsValidEmail(email))
            {
                errors.Add("Invalid email format");
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                errors.Add("Phone number is required");
            }
            else if (!IsValidPhone(phone))
            {
                errors.Add("Phone number must be valid");
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                errors.Add("Address is required");
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                errors.Add("City is required");
            }

            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                if (!IsValidZipCode(zipCode))
                {
                    errors.Add("Zip code must be numeric");
                }
            }
            if (errors.Count > 0)
            {
                errorMessage = string.Join(", ", errors);
                return false;
            }
            return true;
        }
        public ClinicSettings GetDefaultClinicSettings()
        {
            ClinicSettings settings = new ClinicSettings();
            settings.ClinicName = "City Care Clinic";
            settings.Email = "info@citycareclinic.com";
            settings.Phone = "+92 300 1234567";
            settings.Address = "123 Main Street, Gulberg, Lahore, Pakistan";
            settings.City = "Lahore";
            settings.ZipCode = "54000";
            settings.DateFormat = "MMM DD, YYYY";
            settings.TimeFormat = "12 Hours (hh:mm AM/PM)";
            settings.Currency = "PKR (Pakistani Rupee)";
            settings.ItemsPerPage = 10;
            settings.Language = "English";
            settings.ThemeMode = "Light";
            return settings;
        }
        public bool ValidateSystemPreferences(string dateFormat, string timeFormat, string currency, int itemsPerPage, string language, out string errorMessage)
        {
            errorMessage = "";
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dateFormat))
            {
                errors.Add("Date format is required");
            }
            if (string.IsNullOrWhiteSpace(timeFormat))
            {
                errors.Add("Time format is required");
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                errors.Add("Currency is required");
            }
            if (itemsPerPage <= 0)
            {
                errors.Add("Items per page must be greater than 0");
            }
            else if (itemsPerPage > 100)
            {
                errors.Add("Items per page cannot exceed 100");
            }
            if (string.IsNullOrWhiteSpace(language))
            {
                errors.Add("Language is required");
            }
            if (errors.Count > 0)
            {
                errorMessage = string.Join(", ", errors);
                return false;
            }
            return true;
        }
        public bool ValidateThemeMode(string themeMode, out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(themeMode))
            {
                errorMessage = "Theme mode is required";
                return false;
            }

            if (themeMode != "Light" && themeMode != "Dark")
            {
                errorMessage = "Theme mode must be Light or Dark";
                return false;
            }

            return true;
        }

        public string GetThemeModeDisplayName(string themeMode)
        {
            if (themeMode == "Light")
                return "Light Mode";
            else if (themeMode == "Dark")
                return "Dark Mode";
            else
                return "Light Mode";
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            string cleanPhone = phone.Replace(" ", "").Replace("+", "");

            if (cleanPhone.StartsWith("92"))
            {
                cleanPhone = cleanPhone.Substring(2);
            }

            if (cleanPhone.Length == 10 && cleanPhone.StartsWith("3"))
            {
                return true;
            }

            if (cleanPhone.Length == 11 && cleanPhone.StartsWith("03"))
            {
                return true;
            }

            return false;
        }

        private bool IsValidZipCode(string zipCode)
        {
            foreach (char c in zipCode)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public string FormatDate(DateTime date, string dateFormat)
        {
            if (dateFormat == "MMM DD, YYYY")
            {
                return date.ToString("MMM dd, yyyy");
            }
            else if (dateFormat == "DD/MM/YYYY")
            {
                return date.ToString("dd/MM/yyyy");
            }
            else if (dateFormat == "YYYY-MM-DD")
            {
                return date.ToString("yyyy-MM-dd");
            }
            else
            {
                return date.ToString("MMM dd, yyyy");
            }
        }

        public string FormatTime(DateTime time, string timeFormat)
        {
            if (timeFormat == "12 Hours (hh:mm AM/PM)")
            {
                return time.ToString("hh:mm tt");
            }
            else if (timeFormat == "24 Hours (HH:mm)")
            {
                return time.ToString("HH:mm");
            }
            else
            {
                return time.ToString("hh:mm tt");
            }
        }

        public string FormatCurrency(decimal amount, string currency)
        {
            if (currency.StartsWith("PKR"))
            {
                return $"Rs. {amount:N0}";
            }
            else if (currency.StartsWith("USD"))
            {
                return $"${amount:N2}";
            }
            else
            {
                return $"Rs. {amount:N0}";
            }
        }

        // ============== GET AVAILABLE OPTIONS ==============

        public List<string> GetAvailableDateFormats()
        {
            List<string> formats = new List<string>();
            formats.Add("MMM DD, YYYY");
            formats.Add("DD/MM/YYYY");
            formats.Add("YYYY-MM-DD");
            return formats;
        }
        public List<string> GetAvailableTimeFormats()
        {
            List<string> formats = new List<string>();
            formats.Add("12 Hours (hh:mm AM/PM)");
            formats.Add("24 Hours (HH:mm)");
            return formats;
        }
        public List<string> GetAvailableCurrencies()
        {
            List<string> currencies = new List<string>();
            currencies.Add("PKR (Pakistani Rupee)");
            currencies.Add("USD (US Dollar)");
            currencies.Add("GBP (British Pound)");
            return currencies;
        }
        public List<string> GetAvailableLanguages()
        {
            List<string> languages = new List<string>();
            languages.Add("English");
            languages.Add("Urdu");
            languages.Add("Arabic");
            return languages;
        }
        public List<int> GetAvailableItemsPerPageOptions()
        {
            List<int> options = new List<int>();
            options.Add(5);
            options.Add(10);
            options.Add(15);
            options.Add(20);
            options.Add(25);
            options.Add(50);
            options.Add(100);
            return options;
        }
        public void LogSettingsChange(string settingName, string oldValue, string newValue, string changedBy)
        {
            try
            {
                _logger.LogAudit("Settings Changed", changedBy,
                    $"Setting: {settingName}, Old: {oldValue}, New: {newValue}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error logging settings change", ex);
            }
        }
        public List<string> ValidateAllSettings(ClinicSettings settings)
        {
            List<string> allErrors = new List<string>();
            string errorMessage;

            if (!ValidateClinicInfo(settings.ClinicName, settings.Email, settings.Phone,
                settings.Address, settings.City, settings.ZipCode, out errorMessage))
            {
                allErrors.Add($"Clinic Info: {errorMessage}");
            }

            if (!ValidateSystemPreferences(settings.DateFormat, settings.TimeFormat,
                settings.Currency, settings.ItemsPerPage, settings.Language, out errorMessage))
            {
                allErrors.Add($"System Preferences: {errorMessage}");
            }

            if (!ValidateThemeMode(settings.ThemeMode, out errorMessage))
            {
                allErrors.Add($"Theme Mode: {errorMessage}");
            }

            return allErrors;
        }
    }
}

