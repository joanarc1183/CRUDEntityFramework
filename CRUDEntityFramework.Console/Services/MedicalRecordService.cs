using CRUDEntityFramework.Data;
using CRUDEntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUDEntityFramework.Services;

public class MedicalRecordService
{
    private readonly HospitalDbContext _context;

    public MedicalRecordService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName)
    {
        string normalizedName = fullName.Trim().ToLower();

        return await _context.MedicalRecords
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .Where(m => m.Patient != null && m.Patient.FullName.ToLower() == normalizedName)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateDiagnosisAsync(int medicalRecordId, string diagnosis)
    {
        MedicalRecord? record = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.Id == medicalRecordId);
        if (record is null)
        {
            return false;
        }

        record.Diagnosis = diagnosis.Trim();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MedicalRecord>> GetAllRecordAsync()
    {
        return await _context.MedicalRecords
            .AsNoTracking()
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task<bool> DeleteRecordAsync(int id)
    {
        var idDelete = await _context.MedicalRecords.FindAsync(id);

        if (idDelete == null)
            return false;
        
        _context.MedicalRecords.Remove(idDelete);
        await _context.SaveChangesAsync();

        return true;
    }

}
