using FluentValidation;
using HospitalPatientManager.DTOs.MedicalRecord;

namespace HospitalPatientManager.Validators;

public class UpdateDiagnosisDtoValidator : AbstractValidator<UpdateDiagnosisDto>
{
    public UpdateDiagnosisDtoValidator()
    {
        RuleFor(x => x.Diagnosis)
            .NotEmpty()
            .WithMessage("Diagnosis is required.")
            .MaximumLength(500);
    }
}
