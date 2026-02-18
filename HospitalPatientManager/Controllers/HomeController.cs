using HospitalPatientManager.DTOs;
using HospitalPatientManager.Services;
using HospitalPatientManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class HomeController : Controller
{
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;

    public HomeController(IPatientService patientService, IDoctorService doctorService)
    {
        _patientService = patientService;
        _doctorService = doctorService;
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
    public async Task<IActionResult> CreatePatient(PatientCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName) ||
            string.IsNullOrWhiteSpace(dto.DateOfBirth.ToString()) ||
            string.IsNullOrWhiteSpace(dto.Gender) ||
            string.IsNullOrWhiteSpace(dto.Address) ||
            string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            TempData["FlashError"] = "All patient fields are required.";
            return RedirectToAction(nameof(Index));
        }

        // Call patient service to create new patient
        var result = await _patientService.CreatePatientAsync(dto);

        if (!result.Success)
        {
            TempData["FlashError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["FlashSuccess"] = $"Patient '{result.Data?.FullName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

}
