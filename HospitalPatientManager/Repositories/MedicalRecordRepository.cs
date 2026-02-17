using HospitalPatientManager.Data;
using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Repositories;

public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(HospitalDbContext context) : base(context)
    {
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName)
    {
        string normalizedName = fullName.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .Where(m => m.Patient != null && m.Patient.FullName.ToLower() == normalizedName)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetAllWithRelationsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetRecordsByDoctorIdAsync(int doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .Where(m => m.DoctorId == doctorId)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetRecordsByDoctorAndPatientIdsAsync(int doctorId, IReadOnlyCollection<int> patientIds)
    {
        if (patientIds.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .Where(m => m.DoctorId == doctorId && patientIds.Contains(m.PatientId))
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }
}
