using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;
using System.Text.RegularExpressions;

namespace HospitalPatientManager.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
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
        string fullName = NormalizeDoctorName(dto.FullName);
        string specialization = dto.Specialization.Trim();
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(specialization))
        {
            return ServiceResult<Doctor>.Fail("Doctor name and specialization are required.");
        }

        var doctors = await _doctorRepository.GetAllDoctorsAsync();
        bool exists = doctors.Any(d =>
            string.Equals(NormalizeDoctorName(d.FullName), fullName, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            return ServiceResult<Doctor>.Fail("Doctor with the same name already exists.");
        }

        var doctor = new Doctor
        {
            FullName = fullName,
            Specialization = specialization,
            CreatedAt = DateTime.UtcNow
        };

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
