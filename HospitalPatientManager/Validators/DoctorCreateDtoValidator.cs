using FluentValidation;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Repositories;
using System.Text.RegularExpressions;

namespace HospitalPatientManager.Validators;

public class DoctorCreateDtoValidator : AbstractValidator<DoctorCreateDto>
{
    public DoctorCreateDtoValidator(IDoctorRepository doctorRepository)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Doctor name is required.")
            .MaximumLength(100)
            .MustAsync(async (fullName, cancellationToken) =>
            {
                string normalized = NormalizeDoctorName(fullName);
                return await doctorRepository.GetByFullNameAsync(normalized) is null;
            })
            .WithMessage("Doctor with the same name already exists.");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Specialization is required.")
            .MaximumLength(50);
    }
    private static string NormalizeDoctorName(string fullName)
    {
        string withoutTitle = Regex.Replace(fullName.Trim(), "^(dr\\.?\\s+)", string.Empty, RegexOptions.IgnoreCase);
        return Regex.Replace(withoutTitle, "\\s+", " ").Trim();
    }
}

