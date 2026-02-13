using HospitalPatientManager.Data;
using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Services;

public class PatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> CreatePatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<MedicalRecord> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
    {
        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();
        return medicalRecord;
    }

    public async Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName)
    {
        string normalizedName = fullName.Trim().ToLower();

        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.FullName.ToLower() == normalizedName)
            .Include(p => p.MedicalRecords)
                .ThenInclude(m => m.Doctor)
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }
}

