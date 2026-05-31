using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.DL.Repositories
{
    public class PrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all prescriptions
        public async Task<List<Prescription>> GetAllPrescriptionsAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .OrderByDescending(p => p.VisitDate)
                .ToListAsync();
        }

        // ✅ Get disease trends - Returns List<Disease> from your existing model
        public async Task<List<Disease>> GetDiseaseTrendsAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);

            // Get all prescriptions in the last X days
            var prescriptions = await _context.Prescriptions
                .Where(p => p.VisitDate >= cutoffDate && p.Diagnosis != null)
                .ToListAsync();

            if (!prescriptions.Any())
                return new List<Disease>();

            // Group by diagnosis and create/update Disease objects
            var diseaseGroups = prescriptions
                .GroupBy(p => p.Diagnosis)
                .Select(g => new
                {
                    DiseaseName = g.Key,
                    PatientCount = g.Count(),
                    LastOccurrence = g.Max(p => p.VisitDate),
                    AllSymptoms = string.Join("; ", g.Select(p => p.Symptoms).Where(s => !string.IsNullOrEmpty(s)))
                })
                .OrderByDescending(d => d.PatientCount)
                .Take(10)
                .ToList();

            var diseases = new List<Disease>();

            foreach (var group in diseaseGroups)
            {
                // Get common symptoms
                string commonSymptoms = "No symptoms recorded";
                if (!string.IsNullOrEmpty(group.AllSymptoms))
                {
                    var symptoms = group.AllSymptoms
                        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .GroupBy(s => s)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key);

                    commonSymptoms = string.Join(", ", symptoms);
                }

                // Calculate trend
                var weeklyData = prescriptions
                    .Where(p => p.Diagnosis == group.DiseaseName)
                    .GroupBy(p => GetWeekNumber(p.VisitDate))
                    .Select(g => new { Week = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Week)
                    .ToList();

                string trend = "Stable";
                if (weeklyData.Count >= 2)
                {
                    var firstHalf = weeklyData.Take(weeklyData.Count / 2).Sum(x => x.Count);
                    var secondHalf = weeklyData.Skip(weeklyData.Count / 2).Sum(x => x.Count);

                    if (secondHalf > firstHalf * 1.2)
                        trend = "Increasing";
                    else if (secondHalf < firstHalf * 0.8)
                        trend = "Decreasing";
                }

                // Create Disease object (or fetch from database if exists)
                var disease = new Disease
                {
                    DiseaseName = group.DiseaseName ?? "Unknown",
                    OccurrenceCount = group.PatientCount,
                    CommonSymptoms = commonSymptoms,
                    LastOccurrence = group.LastOccurrence,
                    Trend = trend,
                    CreatedAt = DateTime.Now
                };

                diseases.Add(disease);
            }

            return diseases;
        }

        // Update or create disease in database when prescription is added
        public async Task UpdateOrCreateDiseaseAsync(string diagnosis, string symptoms)
        {
            var existingDisease = await _context.Diseases
                .FirstOrDefaultAsync(d => d.DiseaseName == diagnosis);

            if (existingDisease != null)
            {
                // Update existing disease
                existingDisease.OccurrenceCount++;
                existingDisease.LastOccurrence = DateTime.Now;

                // Update symptoms if new ones provided
                if (!string.IsNullOrEmpty(symptoms) && !string.IsNullOrEmpty(existingDisease.CommonSymptoms))
                {
                    var existingSymptoms = existingDisease.CommonSymptoms.Split(',');
                    var newSymptoms = symptoms.Split(',');
                    var allSymptoms = existingSymptoms.Union(newSymptoms).Distinct().Take(5);
                    existingDisease.CommonSymptoms = string.Join(", ", allSymptoms);
                }
                else if (!string.IsNullOrEmpty(symptoms))
                {
                    existingDisease.CommonSymptoms = symptoms;
                }

                _context.Diseases.Update(existingDisease);
            }
            else
            {
                // Create new disease
                var newDisease = new Disease
                {
                    DiseaseName = diagnosis,
                    CommonSymptoms = symptoms,
                    OccurrenceCount = 1,
                    LastOccurrence = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                await _context.Diseases.AddAsync(newDisease);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<Dictionary<string, int>> GetDiseaseDistributionAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);

            var distribution = await _context.Diseases
                .Where(d => d.LastOccurrence >= cutoffDate)
                .OrderByDescending(d => d.OccurrenceCount)
                .Take(5)
                .ToDictionaryAsync(d => d.DiseaseName, d => d.OccurrenceCount);

            return distribution;
        }
        public async Task<List<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.PrescribedMedicines)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.VisitDate)
                .ToListAsync();
        }
        public async Task<int> AddPrescriptionAsync(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return prescription.PrescriptionId;
        }
        public async Task<int> GetMedicinePrescriptionCountAsync(int patientId, string medicineName)
        {
            var prescriptions = await GetPrescriptionsByPatientAsync(patientId);

            int count = 0;
            foreach (var pres in prescriptions)
            {
                if (pres.PrescribedMedicines != null)
                {
                    foreach (var med in pres.PrescribedMedicines)
                    {
                        if (med.MedicineName.ToLower() == medicineName.ToLower())
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private int GetWeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}