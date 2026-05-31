using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManagementSystem.DL.Repositories;
using ClinicManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Pages.Receptionist
{
    public class PatientListModel : PageModel
    {
        private readonly PatientRepository _patientRepo;

        public PatientListModel(PatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public List<Patient> Patients { get; set; }

        public async Task OnGetAsync()
        {
            Patients = await _patientRepo.GetAllPatientsAsync();
        }
    }
}