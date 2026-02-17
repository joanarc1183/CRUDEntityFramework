using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneNumberAsync(string phoneNumber);
    Task<Patient?> GetByIdWithRelationsAsync(int patientId);
    Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName);
    Task<List<Patient>> GetAllPatientsAsync();
}
