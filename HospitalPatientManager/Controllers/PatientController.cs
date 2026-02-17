using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;
using HospitalPatientManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class PatientController : Controller
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> Dashboard(int? userId)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Patient? patient = await _patientService.GetPatientByIdWithRelationsAsync(userId.Value);
        if (patient is null)
        {
            return RedirectToAction("Index", "Home");
        }

        DateTime today = DateTime.UtcNow.Date;
        var records = patient.MedicalRecords
            .OrderByDescending(r => r.VisitDate)
            .ToList();
        var completed = records.Where(r => r.VisitDate.Date <= today).ToList();
        var upcoming = records.Where(r => r.VisitDate.Date > today).OrderBy(r => r.VisitDate).ToList();

        var model = new PatientDashboardViewModel
        {
            Patient = patient,
            Initials = GetInitials(patient.FullName),
            Age = GetAge(patient.DateOfBirth),
            CompletedCount = completed.Count,
            UpcomingCount = upcoming.Count,
            PendingCount = records.Count(r => string.Equals(r.Status, "Pending", StringComparison.OrdinalIgnoreCase)),
            TotalCheckups = records.Count,
            LatestRecord = completed.FirstOrDefault(),
            UpcomingRecords = upcoming.Take(3).ToList(),
            RecentRecords = records.Take(5).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Biodata(int? userId)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Patient? patient = await _patientService.GetPatientByIdWithRelationsAsync(userId.Value);
        if (patient is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new PatientBiodataViewModel
        {
            Patient = patient,
            Initials = GetInitials(patient.FullName),
            Age = GetAge(patient.DateOfBirth)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBiodata(PatientUpdateDto dto)
    {
        var result = await _patientService.UpdatePatientAsync(dto);
        TempData[result.Success ? "FlashSuccess" : "FlashError"] = result.Message;
        return RedirectToAction(nameof(Biodata), new { userId = dto.Id });
    }

    public async Task<IActionResult> Records(int? userId, string? searchTerm)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Patient? patient = await _patientService.GetPatientByIdWithRelationsAsync(userId.Value);
        if (patient is null)
        {
            return RedirectToAction("Index", "Home");
        }

        string query = searchTerm?.Trim() ?? string.Empty;
        var filteredRecords = patient.MedicalRecords.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            filteredRecords = filteredRecords.Where(r =>
                r.Diagnosis.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (r.Doctor?.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                r.Status.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        var model = new PatientRecordsViewModel
        {
            Patient = patient,
            Initials = GetInitials(patient.FullName),
            SearchTerm = query,
            Records = filteredRecords
                .OrderByDescending(r => r.VisitDate)
                .Select(ToRecordItem)
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> SearchByNameDto(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return BadRequest("fullName is required.");
        }

        var result = await _patientService.GetPatientHistoryByFullNameDtoAsync(fullName);
        return Json(result);
    }

    private static RecordItemViewModel ToRecordItem(MedicalRecord record)
    {
        return new RecordItemViewModel
        {
            Id = record.Id,
            Diagnosis = record.Diagnosis,
            VisitDate = record.VisitDate,
            DoctorName = record.Doctor?.FullName ?? "-",
            DoctorSpecialization = record.Doctor?.Specialization ?? "-",
            PatientName = record.Patient?.FullName ?? "-",
            Status = record.Status,
            Notes = record.Notes,
            Treatment = record.Treatment
        };
    }

    private static string GetInitials(string fullName)
    {
        var tokens = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return "NA";
        }

        if (tokens.Length == 1)
        {
            return tokens[0][0].ToString().ToUpperInvariant();
        }

        return string.Concat(tokens[0][0], tokens[^1][0]).ToUpperInvariant();
    }

    private static int GetAge(DateOnly dateOfBirth)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
