using ClinicManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.BL.Interface
{
    public interface IRegisterDoctor
    {
        List<Doctor> GetAllDoctors(List<Doctor> existingDoctors);
        List<Doctor> GetActiveDoctors(List<Doctor> existingDoctors);
        List<Doctor> GetInactiveDoctors(List<Doctor> existingDoctors);
        List<Doctor> SearchDoctors(string keyword, List<Doctor> existingDoctors);
        Doctor GetDoctorById(int doctorId, List<Doctor> existingDoctors);
        Doctor GetDoctorByEmail(string email, List<Doctor> existingDoctors);
        bool AddDoctor(Doctor doctor, List<Doctor> existingDoctors, out string errorMessage);
        bool UpdateDoctor(int doctorId, Doctor updatedDoctor, List<Doctor> existingDoctors, out string errorMessage);
        bool DeleteDoctor(int doctorId, List<Doctor> existingDoctors, out string errorMessage);
        List<string> ValidateDoctor(Doctor doctor);
        bool IsDuplicateEmail(string email, List<Doctor> existingDoctors);
        bool IsDuplicatePhone(string phone, List<Doctor> existingDoctors);
        int GetTotalDoctorsCount(List<Doctor> existingDoctors);
        int GetActiveDoctorsCount(List<Doctor> existingDoctors);
        int GetInactiveDoctorsCount(List<Doctor> existingDoctors);
        int GetTotalSpecializationsCount(List<Doctor> existingDoctors);
    }
}