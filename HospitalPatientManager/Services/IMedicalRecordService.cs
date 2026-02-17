using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IMedicalRecordService
{
    Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName);
    Task<List<MedicalRecord>> GetAllRecordAsync();
    Task<List<MedicalRecord>> GetRecordsByDoctorIdAsync(int doctorId);
    Task<List<MedicalRecord>> GetRecordsByDoctorAndPatientIdsAsync(int doctorId, IReadOnlyCollection<int> patientIds);
    Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId);
    Task<bool> UpdateDiagnosisAsync(int medicalRecordId, string diagnosis);
    Task<bool> UpdateRecordDetailsAsync(int medicalRecordId, string diagnosis, string treatment, string notes, string status);
    Task<bool> DeleteRecordAsync(int id);
}
