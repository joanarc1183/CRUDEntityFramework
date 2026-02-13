using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class MedicalRecordService
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository)
    {
        _medicalRecordRepository = medicalRecordRepository;
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName)
    {
        return await _medicalRecordRepository.GetRecordsByPatientFullNameAsync(fullName);
    }

    public async Task<bool> UpdateDiagnosisAsync(int medicalRecordId, string diagnosis)
    {
        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(medicalRecordId);
        if (record is null)
        {
            return false;
        }

        record.Diagnosis = diagnosis.Trim();
        _medicalRecordRepository.Update(record);
        await _medicalRecordRepository.SaveChangesAsync();

        return true;
    }

    public async Task<List<MedicalRecord>> GetAllRecordAsync()
    {
        return await _medicalRecordRepository.GetAllWithRelationsAsync();
    }

    public async Task<bool> DeleteRecordAsync(int id)
    {
        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(id);
        if (record is null)
        {
            return false;
        }

        _medicalRecordRepository.Delete(record);
        await _medicalRecordRepository.SaveChangesAsync();

        return true;
    }
}
