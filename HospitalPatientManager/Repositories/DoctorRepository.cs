using HospitalPatientManager.Data;
using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(HospitalDbContext context) : base(context)
    {
    }
    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(d => d.FullName)
            .ToListAsync();
    }
    public async Task<Doctor?> GetByIdReadOnlyAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    public async Task<Doctor?> GetByFullNameAsync(string fullName)
    {
        string normalized = fullName.Trim().ToLower();
        return await _dbSet
            .FirstOrDefaultAsync(d => d.FullName.ToLower() == normalized);
    }
}

