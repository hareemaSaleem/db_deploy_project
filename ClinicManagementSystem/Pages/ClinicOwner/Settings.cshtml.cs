using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.ClinicOwner
{
    public class SettingsModel : PageModel
    {
        private readonly SettingsRepository _settingsRepo;

        public SettingsModel(SettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        [BindProperty]
        public ClinicSettings Settings { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Settings = await _settingsRepo.GetSettingsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveSettingsAsync()
        {
            if (string.IsNullOrWhiteSpace(Settings.ClinicName))
            {
                ErrorMessage = "Clinic name is required";
                await OnGetAsync();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Settings.Email))
            {
                ErrorMessage = "Email is required";
                await OnGetAsync();
                return Page();
            }

            bool updated = await _settingsRepo.UpdateSettingsAsync(Settings);

            if (updated)
            {
                SuccessMessage = "Settings saved successfully!";

                // Set cookie for theme preference
                Response.Cookies.Append("ThemeMode", Settings.ThemeMode, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1),
                    HttpOnly = false
                });
            }
            else
            {
                ErrorMessage = "Failed to save settings";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSetThemeAsync(string theme)
        {
            var settings = await _settingsRepo.GetSettingsAsync();
            settings.ThemeMode = theme;
            await _settingsRepo.UpdateSettingsAsync(settings);

            Response.Cookies.Append("ThemeMode", theme, new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                HttpOnly = false
            });

            return new JsonResult(new { success = true });
        }
    }
}