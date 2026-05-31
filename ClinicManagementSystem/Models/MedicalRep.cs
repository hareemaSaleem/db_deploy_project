namespace ClinicManagementSystem.Models
{
    public class MedicalRep
    {
        public int MedicalRepId { get; set; }
        public string RepName { get; set; }
        public string CompanyName { get; set; }
        public DateTime VisitDate { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<MedicineSample> MedicineSamples { get; set; }

        public MedicalRep()
        {
            VisitDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            MedicineSamples = new List<MedicineSample>();
        }

        public MedicalRep(string repName, string companyName)
        {
            RepName = repName;
            CompanyName = companyName;
            VisitDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            MedicineSamples = new List<MedicineSample>();
        }
    }
}
