using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IPatientService
{
    Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
    Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto);
    Task<ServiceResult<List<PatientReadDto>>> GetPatientHistoryByFullNameDtoAsync(string fullName);
    Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName);
    Task<List<Patient>> GetAllPatientsAsync();
}
