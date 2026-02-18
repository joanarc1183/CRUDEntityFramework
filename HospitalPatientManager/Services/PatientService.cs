using AutoMapper;
using FluentValidation;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<PatientCreateDto> _patientCreateValidator;
    private readonly IValidator<PatientUpdateDto> _patientUpdateValidator;

    public PatientService(
        IPatientRepository patientRepository,
        IMapper mapper,
        IValidator<PatientCreateDto> patientCreateValidator,
        IValidator<PatientUpdateDto> patientUpdateValidator)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
        _patientCreateValidator = patientCreateValidator;
        _patientUpdateValidator = patientUpdateValidator;
    }

    // From PatientController
    public async Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto)
    {
        var validation = await _patientCreateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ServiceResult<PatientReadDto>.Fail(validation.Errors[0].ErrorMessage);
        }

        Patient patient = _mapper.Map<Patient>(dto);
        patient.CreatedAt = DateTime.UtcNow;

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return ServiceResult<PatientReadDto>.Ok(_mapper.Map<PatientReadDto>(patient), "Patient berhasil dibuat");
    }

    public async Task<ServiceResult<PatientReadDto>> UpdatePatientAsync(PatientUpdateDto dto)
    {
        var validation = await _patientUpdateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ServiceResult<PatientReadDto>.Fail(validation.Errors[0].ErrorMessage);
        }

        Patient? existingPatient = await _patientRepository.GetByIdAsync(dto.Id);
        if (existingPatient is null)
        {
            return ServiceResult<PatientReadDto>.Fail("Patient tidak ditemukan.");
        }

        _mapper.Map(dto, existingPatient);
        _patientRepository.Update(existingPatient);
        await _patientRepository.SaveChangesAsync();

        return ServiceResult<PatientReadDto>.Ok(_mapper.Map<PatientReadDto>(existingPatient), "Biodata patient berhasil diperbarui.");
    }

    public async Task<List<Patient>> GetPatientHistoryByFullNameAsync(string fullName)
    {
        return await _patientRepository.GetPatientHistoryByFullNameAsync(fullName);
    }

    public async Task<ServiceResult<List<PatientReadDto>>> GetPatientHistoryByFullNameDtoAsync(string fullName)
    {
        List<Patient> patients = await _patientRepository.GetPatientHistoryByFullNameAsync(fullName);
        List<PatientReadDto> result = _mapper.Map<List<PatientReadDto>>(patients);

        return ServiceResult<List<PatientReadDto>>.Ok(result);
    }

    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        return await _patientRepository.GetAllPatientsAsync();
    }

    public async Task<Patient?> GetPatientByIdWithRelationsAsync(int patientId)
    {
        return await _patientRepository.GetByIdWithRelationsAsync(patientId);
    }
}
