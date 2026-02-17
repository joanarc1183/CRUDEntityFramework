using AutoMapper;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto)
    {
        Patient? existing = await _patientRepository.GetByPhoneNumberAsync(dto.PhoneNumber.Trim());
        // Nama user bisa sama, tapi tidak dengan nomor teleponnya
        if (existing is not null)
        {
            return ServiceResult<PatientReadDto>.Fail("Tidak dapat menambahkan Pasien. Nomor telepon sudah terdaftar.");
        }

        Patient patient = _mapper.Map<Patient>(dto);
        patient.CreatedAt = DateTime.UtcNow;

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return ServiceResult<PatientReadDto>.Ok(_mapper.Map<PatientReadDto>(patient), "Patient berhasil dibuat");
    }

    public async Task<ServiceResult<PatientReadDto>> UpdatePatientAsync(PatientUpdateDto dto)
    {
        Patient? existingPatient = await _patientRepository.GetByIdAsync(dto.Id);
        if (existingPatient is null)
        {
            return ServiceResult<PatientReadDto>.Fail("Patient tidak ditemukan.");
        }

        string newPhone = dto.PhoneNumber.Trim();
        Patient? existingPhoneOwner = await _patientRepository.GetByPhoneNumberAsync(newPhone);
        if (existingPhoneOwner is not null && existingPhoneOwner.Id != dto.Id)
        {
            return ServiceResult<PatientReadDto>.Fail("Nomor telepon sudah terdaftar oleh user lain.");
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
