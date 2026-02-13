using HospitalPatientManager.Data;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Services;

public interface IPatientService
{
    Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto);
    Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
    Task<ServiceResult<List<PatientReadDto>>> GetPatientHistoryByFullNameAsync(string fullName);
}

