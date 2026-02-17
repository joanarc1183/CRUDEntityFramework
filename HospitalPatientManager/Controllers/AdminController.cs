using HospitalPatientManager.DTOs;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;
using HospitalPatientManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class AdminController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;
    private readonly IMedicalRecordService _medicalRecordService;

    public AdminController(
        IDoctorService doctorService,
        IPatientService patientService,
        IMedicalRecordService medicalRecordService)
    {
        _doctorService = doctorService;
        _patientService = patientService;
        _medicalRecordService = medicalRecordService;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Dashboard()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        var doctors = await _doctorService.GetAllDoctorsAsync();
        var records = await _medicalRecordService.GetAllRecordAsync();

        var model = new AdminDashboardViewModel
        {
            TotalCheckups = records.Count,
            TotalPatients = patients.Count,
            TotalDoctors = doctors.Count
        };

        return View(model);
    }

    public async Task<IActionResult> Patients(string? searchTerm)
    {
        var patients = await _patientService.GetAllPatientsAsync();
        string query = searchTerm?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(query))
        {
            patients = patients
                .Where(p => p.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var model = new AdminPatientsViewModel
        {
            SearchTerm = query,
            Patients = patients
                .OrderBy(p => p.FullName)
                .Select(p =>
                {
                    var lastRecord = p.MedicalRecords.OrderByDescending(m => m.VisitDate).FirstOrDefault();
                    return new DoctorPatientCardViewModel
                    {
                        PatientId = p.Id,
                        FullName = p.FullName,
                        Initials = GetInitials(p.FullName),
                        Age = GetAge(p.DateOfBirth),
                        Gender = p.Gender,
                        RecordCount = p.MedicalRecords.Count,
                        LastDiagnosis = lastRecord?.Diagnosis,
                        LastVisitDate = lastRecord?.VisitDate
                    };
                })
                .ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Doctors(string? searchTerm)
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        var records = await _medicalRecordService.GetAllRecordAsync();
        string query = searchTerm?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(query))
        {
            doctors = doctors
                .Where(d => d.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            d.Specialization.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var model = new AdminDoctorsViewModel
        {
            SearchTerm = query,
            Doctors = doctors
                .OrderBy(d => d.FullName)
                .Select(d => new DoctorListItemViewModel
                {
                    DoctorId = d.Id,
                    FullName = d.FullName,
                    Initials = GetInitials(d.FullName),
                    Specialization = d.Specialization,
                    RecordCount = records.Count(r => r.DoctorId == d.Id)
                })
                .ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> PatientDetail(int patientId)
    {
        Patient? patient = await _patientService.GetPatientByIdWithRelationsAsync(patientId);
        if (patient is null)
        {
            TempData["FlashError"] = "Patient not found.";
            return RedirectToAction(nameof(Patients));
        }

        var records = await _medicalRecordService.GetRecordsByPatientIdAsync(patientId);
        var model = new AdminPatientDetailViewModel
        {
            Patient = patient,
            Initials = GetInitials(patient.FullName),
            Age = GetAge(patient.DateOfBirth),
            Records = records.Select(ToRecordItem).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDoctor(DoctorCreateDto dto)
    {
        var result = await _doctorService.CreateDoctorAsync(dto);
        TempData[result.Success ? "FlashSuccess" : "FlashError"] = result.Message;
        return RedirectToAction(nameof(Doctors));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRecord(int recordId)
    {
        var result = await _medicalRecordService.DeleteRecordAsync(recordId);
        TempData[result.Success ? "FlashSuccess" : "FlashError"] = result.Message;
        return RedirectToAction(nameof(Records));
    }

    public async Task<IActionResult> Records(string? searchTerm)
    {
        var records = await _medicalRecordService.GetAllRecordAsync();
        string query = searchTerm?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(query))
        {
            records = records
                .Where(r =>
                    r.Diagnosis.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (r.Patient?.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Doctor?.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    r.Status.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var model = new AdminRecordsViewModel
        {
            SearchTerm = query,
            Records = records
                .OrderByDescending(r => r.VisitDate)
                .Select(ToRecordItem)
                .ToList()
        };

        return View(model);
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
