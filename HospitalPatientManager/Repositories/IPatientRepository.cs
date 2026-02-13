using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneNumberAsync(string phoneNumber);
}
