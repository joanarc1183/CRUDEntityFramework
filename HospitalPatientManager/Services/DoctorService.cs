using AutoMapper;
using FluentValidation;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;
using System.Text.RegularExpressions;

namespace HospitalPatientManager.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<DoctorCreateDto> _doctorCreateValidator;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IMapper mapper,
        IValidator<DoctorCreateDto> doctorCreateValidator)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _doctorCreateValidator = doctorCreateValidator;
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _doctorRepository.GetAllDoctorsAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int doctorId)
    {
        return await _doctorRepository.GetByIdReadOnlyAsync(doctorId);
    }

    public async Task<ServiceResult<Doctor>> CreateDoctorAsync(DoctorCreateDto dto)
    {
        var validation = await _doctorCreateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ServiceResult<Doctor>.Fail(validation.Errors[0].ErrorMessage);
        }

        string fullName = NormalizeDoctorName(dto.FullName);
        string specialization = dto.Specialization.Trim();

        var doctor = _mapper.Map<Doctor>(dto);
        doctor.FullName = fullName;
        doctor.Specialization = specialization;
        doctor.CreatedAt = DateTime.UtcNow;

        await _doctorRepository.AddAsync(doctor);
        await _doctorRepository.SaveChangesAsync();

        return ServiceResult<Doctor>.Ok(doctor, "Doctor created successfully.");
    }

    private static string NormalizeDoctorName(string fullName)
    {
        string withoutTitle = Regex.Replace(fullName.Trim(), "^(dr\\.?\\s+)", string.Empty, RegexOptions.IgnoreCase);
        return Regex.Replace(withoutTitle, "\\s+", " ").Trim();
    }
}
