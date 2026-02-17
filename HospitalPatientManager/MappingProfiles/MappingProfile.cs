using AutoMapper;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.DTOs.MedicalRecord;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientReadDto>();
        CreateMap<PatientCreateDto, Patient>();
        CreateMap<PatientUpdateDto, Patient>();
        CreateMap<DoctorCreateDto, Doctor>();
        CreateMap<MedicalRecordCreateDto, MedicalRecord>();
        CreateMap<UpdateDiagnosisDto, MedicalRecord>();
        CreateMap<UpdateMedicalRecordDto, MedicalRecord>();
    }
}
