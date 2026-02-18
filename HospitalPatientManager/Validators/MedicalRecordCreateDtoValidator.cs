using FluentValidation;
using HospitalPatientManager.DTOs.MedicalRecord;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Validators;

public class MedicalRecordCreateDtoValidator : AbstractValidator<MedicalRecordCreateDto>
{
    public MedicalRecordCreateDtoValidator(IPatientRepository patientRepository, IDoctorRepository doctorRepository)
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient id is invalid.")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await patientRepository.GetByIdAsync(id) is not null;
            })
            .WithMessage("Patient not found.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0)
            .WithMessage("Doctor id is invalid.")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await doctorRepository.GetByIdAsync(id) is not null;
            })
            .WithMessage("Doctor not found.");

        RuleFor(x => x.VisitDate)
            .NotEmpty()
            .WithMessage("Visit date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Visit date is not valid.");

        RuleFor(x => x.Diagnosis)
            .NotEmpty()
            .WithMessage("Diagnosis is required.")
            .MaximumLength(500);

        RuleFor(x => x.Treatment)
            .NotEmpty()
            .WithMessage("Treatment is required.")
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .NotEmpty()
            .WithMessage("Notes is required.")
            .MaximumLength(1000);

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(status => status.Trim() is "Completed" or "Pending")
            .WithMessage("Status must be Completed or Pending.");
    }
}
