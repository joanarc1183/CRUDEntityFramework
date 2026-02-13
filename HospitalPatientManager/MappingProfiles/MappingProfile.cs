using System.Runtime;
using AutoMapper;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;

namespace HospitalPatientManager.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientReadDto>();
        CreateMap<PatientCreateDto, Patient>();
        CreateMap<PatientUpdateDto, Patient>();
    }
}