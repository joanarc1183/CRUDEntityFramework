using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Services;

public interface IDoctorService
{
    Task<List<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(int doctorId);
    Task<ServiceResult<Doctor>> CreateDoctorAsync(DoctorCreateDto dto);
}
