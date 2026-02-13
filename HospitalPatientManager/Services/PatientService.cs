using AutoMapper;
using HospitalPatientManager.Data;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Mappings;
using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;
    private readonly IMapper _mapper;

    public PatientService(HospitalDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public PatientService(HospitalDbContext context)
    {
        _context = context;
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();
    }

    public async Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
    {
        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();
        return ServiceResult<MedicalRecord>.Ok(medicalRecord, "Medical record berhasil dibuat");
    }

    public async Task<ServiceResult<PatientReadDto>> CreatePatientAsync(PatientCreateDto dto)
    {
        bool exists = await _context.Patients.AnyAsync(p =>
            p.FullName.ToLower() == dto.FullName.Trim().ToLower() &&
            p.PhoneNumber == dto.PhoneNumber.Trim());

        if (exists)
        {
            return ServiceResult<PatientReadDto>.Fail("Patient sudah terdaftar");
        }

        Patient patient = _mapper.Map<Patient>(dto);
        patient.CreatedAt = DateTime.UtcNow;

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return ServiceResult<PatientReadDto>.Ok(_mapper.Map<PatientReadDto>(patient), "Patient berhasil dibuat");
    }

    public async Task<ServiceResult<List<PatientReadDto>>> GetPatientHistoryByFullNameDtoAsync(string fullName)
    {
        string normalizedName = fullName.Trim().ToLower();

        List<Patient> patients = await _context.Patients
            .AsNoTracking()
            .Where(p => p.FullName.ToLower() == normalizedName)
            .Include(p => p.MedicalRecords)
                .ThenInclude(m => m.Doctor)
            .OrderBy(p => p.FullName)
            .ToListAsync();

        List<PatientReadDto> result = _mapper.Map<List<PatientReadDto>>(patients);
        return ServiceResult<List<PatientReadDto>>.Ok(result);
    }

    public async Task<Patient> CreatePatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
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
