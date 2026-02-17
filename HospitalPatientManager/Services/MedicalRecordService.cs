using AutoMapper;
using HospitalPatientManager.DTOs.MedicalRecord;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientFullNameAsync(string fullName)
    {
        return await _medicalRecordRepository.GetRecordsByPatientFullNameAsync(fullName);
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

    public async Task<ServiceResult<MedicalRecord>> CreateMedicalRecordAsync(MedicalRecordCreateDto dto)
    {
        if (!IsValidStatus(dto.Status))
        {
            return ServiceResult<MedicalRecord>.Fail("Status must be Completed, Pending, or Scheduled.");
        }

        var medicalRecord = _mapper.Map<MedicalRecord>(dto);
        medicalRecord.VisitDate = DateTime.SpecifyKind(dto.VisitDate.Date, DateTimeKind.Utc);
        medicalRecord.Diagnosis = dto.Diagnosis.Trim();
        medicalRecord.Treatment = dto.Treatment.Trim();
        medicalRecord.Notes = dto.Notes.Trim();
        medicalRecord.Status = dto.Status.Trim();

        await _medicalRecordRepository.AddAsync(medicalRecord);
        await _medicalRecordRepository.SaveChangesAsync();

        return ServiceResult<MedicalRecord>.Ok(medicalRecord, "Medical record berhasil dibuat");
    }

    public async Task<ServiceResult<MedicalRecord>> UpdateDiagnosisAsync(int medicalRecordId, UpdateDiagnosisDto dto)
    {
        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(medicalRecordId);
        if (record is null)
        {
            return ServiceResult<MedicalRecord>.Fail("Medical record tidak ditemukan.");
        }

        _mapper.Map(dto, record);
        record.Diagnosis = dto.Diagnosis.Trim();

        _medicalRecordRepository.Update(record);
        await _medicalRecordRepository.SaveChangesAsync();

        return ServiceResult<MedicalRecord>.Ok(record, "Diagnosis berhasil diperbarui.");
    }

    public async Task<ServiceResult<MedicalRecord>> UpdateRecordDetailsAsync(UpdateMedicalRecordDto dto)
    {
        if (!IsValidStatus(dto.Status))
        {
            return ServiceResult<MedicalRecord>.Fail("Status must be Completed, Pending, or Scheduled.");
        }

        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(dto.RecordId);
        if (record is null)
        {
            return ServiceResult<MedicalRecord>.Fail("Medical record tidak ditemukan.");
        }

        _mapper.Map(dto, record);
        record.Diagnosis = dto.Diagnosis.Trim();
        record.Treatment = dto.Treatment.Trim();
        record.Notes = dto.Notes.Trim();
        record.Status = dto.Status.Trim();

        _medicalRecordRepository.Update(record);
        await _medicalRecordRepository.SaveChangesAsync();

        return ServiceResult<MedicalRecord>.Ok(record, "Record updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteRecordAsync(int id)
    {
        MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(id);
        if (record is null)
        {
            return ServiceResult<bool>.Fail("Medical record tidak ditemukan.");
        }

        _medicalRecordRepository.Delete(record);
        await _medicalRecordRepository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Record deleted successfully.");
    }

    private static bool IsValidStatus(string status)
    {
        return status.Trim() is "Completed" or "Pending" or "Scheduled";
    }
}
