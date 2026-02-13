using AutoMapper;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository, IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _medicalRecordRepository = medicalRecordRepository;
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

    public async Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
    {
        await _medicalRecordRepository.AddAsync(medicalRecord);
        await _medicalRecordRepository.SaveChangesAsync();

        return ServiceResult<MedicalRecord>.Ok(medicalRecord, "Medical record berhasil dibuat");
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
}
