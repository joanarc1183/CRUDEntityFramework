using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class MedicalRecordService
    : IMedicalRecordService
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

    public async Task<bool> UpdateRecordDetailsAsync(int medicalRecordId, string diagnosis, string treatment, string notes, string status)
    {
        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(medicalRecordId);
        if (record is null)
        {
            return false;
        }

        record.Diagnosis = diagnosis.Trim();
        record.Treatment = treatment.Trim();
        record.Notes = notes.Trim();
        record.Status = status.Trim();

        _medicalRecordRepository.Update(record);
        await _medicalRecordRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<MedicalRecord>> GetAllRecordAsync()
    {
        return await _medicalRecordRepository.GetAllWithRelationsAsync();
    }

    public async Task<List<MedicalRecord>> GetRecordsByDoctorIdAsync(int doctorId)
    {
        return await _medicalRecordRepository.GetRecordsByDoctorIdAsync(doctorId);
    }

    public async Task<List<MedicalRecord>> GetRecordsByDoctorAndPatientIdsAsync(int doctorId, IReadOnlyCollection<int> patientIds)
    {
        return await _medicalRecordRepository.GetRecordsByDoctorAndPatientIdsAsync(doctorId, patientIds);
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId)
    {
        return await _medicalRecordRepository.GetRecordsByPatientIdAsync(patientId);
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
