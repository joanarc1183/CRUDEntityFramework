using FluentValidation;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Services;
using HospitalPatientManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class HomeController : Controller
{
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;
    private readonly IValidator<SignUpDto> _signUpValidator;

    public HomeController(IPatientService patientService, IDoctorService doctorService, IValidator<SignUpDto> signUpValidator)
    {
        _patientService = patientService;
        _doctorService = doctorService;
        _signUpValidator = signUpValidator;
    }

    public async Task<IActionResult> Index()
    {
        var allPatients = await _patientService.GetAllPatientsAsync();
        var doctors = await _doctorService.GetAllDoctorsAsync();

        var model = new HomeIndexViewModel
        {
            Patients = allPatients
                .OrderBy(p => p.FullName)
                .Select(p => new LoginOptionViewModel
                {
                    Id = p.Id,
                    Name = p.FullName,
                    Subtitle = p.PhoneNumber
                })
                .ToList(),
            Doctors = doctors
                .OrderBy(d => d.FullName)
                .Select(d => new LoginOptionViewModel
                {
                    Id = d.Id,
                    Name = d.FullName,
                    Subtitle = d.Specialization
                })
                .ToList()
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    // After press button "Sign Up Patient" in index.cshtml
    public async Task<IActionResult> CreatePatient(SignUpDto dto)
    {
        var validationResult = await _signUpValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            TempData["FlashError"] = validationResult.Errors[0].ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var patientCreateDto = new PatientCreateDto
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber
        };

        // Call patient service to create new patient
        var result = await _patientService.CreatePatientAsync(patientCreateDto);

        if (!result.Success)
        {
            TempData["FlashError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["FlashSuccess"] = $"Patient '{result.Data?.FullName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

}
