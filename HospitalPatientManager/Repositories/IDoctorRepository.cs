using HospitalPatientManager.Models;

namespace HospitalPatientManager.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<List<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetByIdReadOnlyAsync(int id);
    Task<Doctor?> GetByFullNameAsync(string fullName);
}
