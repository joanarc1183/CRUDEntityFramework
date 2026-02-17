using HospitalPatientManager.DTOs.MedicalRecord;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IMedicalRecordService
{
    Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName);
    Task<List<MedicalRecord>> GetAllRecordAsync();
    Task<List<MedicalRecord>> GetRecordsByDoctorIdAsync(int doctorId);
    Task<List<MedicalRecord>> GetRecordsByDoctorAndPatientIdsAsync(int doctorId, IReadOnlyCollection<int> patientIds);
    Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId);
    Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecordCreateDto dto);
    Task<ServiceResult<MedicalRecord>> UpdateDiagnosisAsync(int medicalRecordId, UpdateDiagnosisDto dto);
    Task<ServiceResult<MedicalRecord>> UpdateRecordDetailsAsync(UpdateMedicalRecordDto dto);
    Task<ServiceResult<bool>> DeleteRecordAsync(int id);
}
