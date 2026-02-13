using HospitalPatientManager.Models;
using HospitalPatientManager.Services;

namespace HospitalPatientManager.Controllers;

public class MedicalRecordsController
{
    private readonly MedicalRecordService _medicalRecordService;

    public MedicalRecordsController(MedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    public async Task<List<MedicalRecord>> GetAsync(string? fullName = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return await _medicalRecordService.GetRecordsByPatientFullNameAsync(fullName);
        }

        return await _medicalRecordService.GetAllRecordAsync();
    }

    public async Task<bool> UpdateDiagnosisAsync(int id, string diagnosis)
    {
        return await _medicalRecordService.UpdateDiagnosisAsync(id, diagnosis);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _medicalRecordService.DeleteRecordAsync(id);
    }
}
