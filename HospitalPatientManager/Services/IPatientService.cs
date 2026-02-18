using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IPatientService
{
    Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto);
    Task<ServiceResult<PatientReadDto>> UpdatePatientAsync(PatientUpdateDto dto);
    Task<ServiceResult<List<PatientReadDto>>> GetPatientHistoryByFullNameDtoAsync(string fullName);
    Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName);
    Task<List<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdWithRelationsAsync(int patientId);
}

