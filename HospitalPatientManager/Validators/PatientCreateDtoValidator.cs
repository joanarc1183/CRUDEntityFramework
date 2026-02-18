using FluentValidation;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Repositories;

namespace HospitalPatientManager.Validators;

public class PatientCreateDtoValidator : AbstractValidator<PatientCreateDto>
{
    public PatientCreateDtoValidator(IPatientRepository patientRepository)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth cannot be in the future.");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.")
            .Must(g => g == "Male" || g == "Female")
            .WithMessage("Gender must be Male or Female.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .MaximumLength(20)
            .Matches(@"^[0-9+\-\s]{8,20}$")
            .WithMessage("Phone number format is invalid.")
            .MustAsync(async (phone, cancellationToken) =>
            {
                string normalizedPhone = phone.Trim();
                return await patientRepository.GetByPhoneNumberAsync(normalizedPhone) is null;
            })
            .WithMessage("Phone number is already registered.");
    }
}
