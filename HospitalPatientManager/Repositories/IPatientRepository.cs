using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneNumberAsync(string phoneNumber);
    Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName);
    Task<List<Patient>> GetAllPatientsAsync();
}
