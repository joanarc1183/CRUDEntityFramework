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

    public async Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName)
    {
        string normalizedName = fullName.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(p => p.FullName.ToLower() == normalizedName)
            .Include(p => p.MedicalRecords)
                .ThenInclude(m => m.Doctor)
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        return await _dbSet
            .Include(p => p.MedicalRecords)
                .ThenInclude(m => m.Doctor)
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }
}
