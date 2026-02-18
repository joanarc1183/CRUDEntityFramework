using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName);
    Task<List<MedicalRecord>> GetAllWithRelationsAsync();
    Task<List<MedicalRecord>> GetRecordsByDoctorIdAsync(int doctorId);
    Task<List<MedicalRecord>> GetRecordsByDoctorAndPatientIdsAsync(int doctorId, IReadOnlyCollection<int> patientIds);
    Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId);
}

