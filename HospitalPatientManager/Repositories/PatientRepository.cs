using HospitalPatientManager.Data;
using HospitalPatientManager.Models.Entities;
using HospitalPatientManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(HospitalDbContext context) : base(context) { }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }
}

