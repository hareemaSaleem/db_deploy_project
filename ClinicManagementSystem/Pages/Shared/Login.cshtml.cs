using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Shared
{
    public class LoginModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public LoginModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        public void OnGet()
        {
            if (Request.Cookies.ContainsKey("RememberedUser"))
            {
                Username = Request.Cookies["RememberedUser"];
                RememberMe = true;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Please enter both username and password";
                return Page();
            }

            if (string.IsNullOrEmpty(Role))
            {
                TempData["ErrorMessage"] = "Please select a role";
                return Page();
            }
            string dbRole = Role.ToLower() switch
            {
                "receptionist" => "Receptionist",
                "doctor" => "Doctor",
                "owner" => "Admin",
                _ => Role
            };
            var allUsers = await _userRepository.GetAllUsersAsync();
            User loggedInUser = null;
            foreach (var user in allUsers)
            {
                if (user.Username.ToLower() == Username.ToLower() &&
                    user.Password == Password &&
                    user.Role == dbRole &&
                    user.IsActive == true)
                {
                    loggedInUser = user;
                    break;
                }
            }

            if (loggedInUser != null)
            {
                HttpContext.Session.SetString("UserId", loggedInUser.UserId.ToString());
                HttpContext.Session.SetString("Username", loggedInUser.Username);
                HttpContext.Session.SetString("UserRole", loggedInUser.Role);
                HttpContext.Session.SetString("UserEmail", loggedInUser.Email);

                if (RememberMe)
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Append("RememberedUser", Username, options);
                }
                else
                {
                    Response.Cookies.Delete("RememberedUser");
                }
                string redirectUrl = Role.ToLower() switch
                {
                    "receptionist" => "/Receptionist/ReceptionistDashboard",
                    "doctor" => "/Doctor/DoctorDashboard",
                    "owner" => "/ClinicOwner/ClinicOwnerDashboard",
                    _ => "/"
                };

                return RedirectToPage(redirectUrl);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username, password, or role selected";
                return Page();
            }
        }
    }
}