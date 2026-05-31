using System;
using System.Collections.Generic;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IMedicalRep
    {
        bool AddMedicalRepVisit(MedicalRep medicalRep, List<MedicalRep> existingVisits, out string errorMessage);
        bool UpdateMedicalRepVisit(int visitId, MedicalRep updatedRep, List<MedicalRep> existingVisits, out string errorMessage);
        bool DeleteMedicalRepVisit(int visitId, List<MedicalRep> existingVisits, out string errorMessage);
        MedicalRep GetMedicalRepVisitById(int visitId, List<MedicalRep> existingVisits);
        List<MedicalRep> GetAllMedicalRepVisits(List<MedicalRep> existingVisits);
        List<MedicalRep> GetVisitsByCompany(string companyName, List<MedicalRep> existingVisits);
        List<MedicalRep> GetVisitsByDateRange(DateTime fromDate, DateTime toDate, List<MedicalRep> existingVisits);
        bool AddMedicineSample(int visitId, MedicineSample sample, List<MedicalRep> existingVisits, out string errorMessage);
        List<MedicineSample> GetSamplesByVisit(int visitId, List<MedicalRep> existingVisits);
        List<string> ValidateMedicalRep(MedicalRep medicalRep);
        List<string> ValidateMedicineSample(MedicineSample sample);
        int CalculateTotalSamplesCount(int visitId, List<MedicalRep> existingVisits);
        List<string> GetAllMedicineNamesFromSamples(List<MedicalRep> existingVisits);
    }
}