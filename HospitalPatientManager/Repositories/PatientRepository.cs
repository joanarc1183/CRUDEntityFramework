using HospitalPatientManager.Data;
using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(HospitalDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }
}
