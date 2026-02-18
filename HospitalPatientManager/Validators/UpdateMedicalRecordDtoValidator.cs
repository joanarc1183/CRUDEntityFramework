using FluentValidation;
using HospitalPatientManager.DTOs.MedicalRecord;

namespace HospitalPatientManager.Validators;

public class UpdateMedicalRecordDtoValidator : AbstractValidator<UpdateMedicalRecordDto>
{
    public UpdateMedicalRecordDtoValidator()
    {
        RuleFor(x => x.RecordId)
            .GreaterThan(0)
            .WithMessage("Record id is invalid.");

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

