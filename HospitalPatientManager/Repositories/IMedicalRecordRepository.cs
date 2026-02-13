using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName);
    Task<List<MedicalRecord>> GetAllWithRelationsAsync();
    
}
