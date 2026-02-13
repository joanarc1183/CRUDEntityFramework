using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;

namespace HospitalPatientManager.Controllers;

public class PatientsController
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<ServiceResult<PatientReadDto>> CreateAsync(PatientCreateDto dto)
    {
        return await _patientService.CreatePatientAsync(dto);
    }

    public async Task<List<Patient>> GetHistoryAsync(string fullName)
    {
        return await _patientService.GetPatientHistoryByFullNameAsync(fullName);
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _patientService.GetAllPatientsAsync();
    }
}
