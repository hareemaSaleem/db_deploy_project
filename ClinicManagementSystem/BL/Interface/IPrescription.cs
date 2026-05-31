using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IPrescription
    {
        List<Patient> GetAllPatients(List<Patient> existingPatients);
        List<Prescription> GetPatientPrescriptions(int patientId, List<Prescription> existingPrescriptions);
        List<Prescription> GetPatientPrescriptionsSorted(int patientId, List<Prescription> existingPrescriptions);
        int GetMedicinePrescriptionCount(int patientId, string medicineName, List<Prescription> existingPrescriptions);
        bool IsMedicineRepeated(int patientId, string medicineName, List<Prescription> existingPrescriptions, int threshold);
        List<string> ValidatePrescription(Prescription prescription, List<Medicine> existingMedicines);
        Prescription PreparePrescriptionForSave(Prescription prescription, int doctorId);
        bool SavePrescription(Prescription prescription, List<PrescribedMedicine> prescribedMedicines, out string errorMessage);
    }
}
