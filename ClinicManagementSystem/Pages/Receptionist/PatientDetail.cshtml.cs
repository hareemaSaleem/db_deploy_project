using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class PatientDetailModel : PageModel
    {
        private readonly PatientRepository _patientRepo;

        public PatientDetailModel(PatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public Patient Patient { get; set; }

        public async Task OnGetAsync(int id)
        {
            Patient = await _patientRepo.GetPatientByIdAsync(id);
        }
    }
}