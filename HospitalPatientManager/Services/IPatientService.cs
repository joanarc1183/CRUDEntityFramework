using HospitalPatientManager.Data;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IPatientService
{
    Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
    Task<Patient> CreatePatient(Patient patient);
    Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName);
}

