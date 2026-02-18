using HospitalPatientManager.DTOs.MedicalRecord;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;
using HospitalPatientManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class DoctorController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;
    private readonly IMedicalRecordService _medicalRecordService;

    public DoctorController(
        IDoctorService doctorService,
        IPatientService patientService,
        IMedicalRecordService medicalRecordService)
    {
        _doctorService = doctorService;
        _patientService = patientService;
        _medicalRecordService = medicalRecordService;
    }
    public async Task<IActionResult> Dashboard(int? userId)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Doctor? doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
        if (doctor is null)
        {
            return RedirectToAction("Index", "Home");
        }

        List<MedicalRecord> records = await _medicalRecordService.GetRecordsByDoctorIdAsync(doctor.Id);
        DateTime today = DateTime.UtcNow.Date;
        var completed = records.Where(r => r.VisitDate.Date <= today).ToList();
        var upcoming = records.Where(r => r.VisitDate.Date > today).OrderBy(r => r.VisitDate).ToList();

        var patientCards = BuildPatientCards(records);

        var model = new DoctorDashboardViewModel
        {
            Doctor = doctor,
            Initials = GetInitials(doctor.FullName),
            MyPatientsCount = patientCards.Count,
            CompletedCount = completed.Count,
            UpcomingCount = upcoming.Count,
            PendingCount = records.Count(r => string.Equals(r.Status, "Pending", StringComparison.OrdinalIgnoreCase)),
            MyPatients = patientCards,
            UpcomingRecords = upcoming.Select(ToRecordItem).ToList(),
            PendingRecords = records
                .Where(r => string.Equals(r.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                .Select(ToRecordItem)
                .ToList(),
            RecentRecords = records.OrderByDescending(r => r.VisitDate).Take(8).Select(ToRecordItem).ToList()
        };

        return View(model);
    }
    public async Task<IActionResult> Patients(int? userId, string? fullName)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Doctor? doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
        if (doctor is null)
        {
            return RedirectToAction("Index", "Home");
        }

        List<MedicalRecord> records;
        bool isSearchMode = !string.IsNullOrWhiteSpace(fullName);
        if (isSearchMode)
        {
            var patientsByName = await _patientService.GetPatientHistoryByFullNameAsync(fullName!);
            var patientIds = patientsByName.Select(p => p.Id).ToList();
            records = await _medicalRecordService.GetRecordsByDoctorAndPatientIdsAsync(doctor.Id, patientIds);
        }
        else
        {
            records = await _medicalRecordService.GetRecordsByDoctorIdAsync(doctor.Id);
        }

        var model = new DoctorPatientsViewModel
        {
            Doctor = doctor,
            Initials = GetInitials(doctor.FullName),
            SearchFullName = fullName ?? string.Empty,
            IsSearchMode = isSearchMode,
            Patients = BuildPatientCards(records)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedicalRecord(
        int userId,
        int patientId,
        DateTime visitDate,
        string diagnosis,
        string treatment,
        string notes,
        string status,
        string? from)
    {
        if (string.IsNullOrWhiteSpace(diagnosis) ||
            string.IsNullOrWhiteSpace(treatment) ||
            string.IsNullOrWhiteSpace(notes) ||
            string.IsNullOrWhiteSpace(status))
        {
            TempData["FlashError"] = "Diagnosis, treatment, notes, and status are required.";
            if (string.Equals(from, "Records", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Records), new { userId });
            }

            return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
        }

        status = status.Trim();
        if (!IsValidStatus(status))
        {
            TempData["FlashError"] = "Status must be Completed or Pending.";
            if (string.Equals(from, "Records", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Records), new { userId });
            }

            return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
        }

        bool doctorExists = await _doctorService.GetDoctorByIdAsync(userId) is not null;
        bool patientExists = await _patientService.GetPatientByIdWithRelationsAsync(patientId) is not null;
        if (!doctorExists || !patientExists)
        {
            TempData["FlashError"] = "Doctor or patient not found.";
            if (string.Equals(from, "Records", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Records), new { userId });
            }

            return RedirectToAction(nameof(Patients), new { userId });
        }

        var dto = new MedicalRecordCreateDto
        {
            DoctorId = userId,
            PatientId = patientId,
            VisitDate = visitDate,
            Diagnosis = diagnosis.Trim(),
            Treatment = treatment.Trim(),
            Notes = notes.Trim(),
            Status = status
        };

        var result = await _medicalRecordService.CreateMedicalRecordAsync(dto);
        TempData[result.Success ? "FlashSuccess" : "FlashError"] = result.Message;

        if (string.Equals(from, "Records", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Records), new { userId });
        }

        return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRecord(
        int userId,
        int recordId,
        string diagnosis,
        string treatment,
        string notes,
        string status,
        int? patientId,
        string? from)
    {
        if (string.IsNullOrWhiteSpace(diagnosis) ||
            string.IsNullOrWhiteSpace(treatment) ||
            string.IsNullOrWhiteSpace(notes) ||
            string.IsNullOrWhiteSpace(status))
        {
            TempData["FlashError"] = "Diagnosis, treatment, notes, and status are required.";
            if (string.Equals(from, "PatientDetail", StringComparison.OrdinalIgnoreCase) && patientId is not null)
            {
                return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
            }

            return RedirectToAction(nameof(Records), new { userId });
        }

        status = status.Trim();
        if (!IsValidStatus(status))
        {
            TempData["FlashError"] = "Status must be Completed or Pending.";
            if (string.Equals(from, "PatientDetail", StringComparison.OrdinalIgnoreCase) && patientId is not null)
            {
                return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
            }

            return RedirectToAction(nameof(Records), new { userId });
        }

        var result = await _medicalRecordService.UpdateRecordDetailsAsync(new UpdateMedicalRecordDto
        {
            RecordId = recordId,
            Diagnosis = diagnosis,
            Treatment = treatment,
            Notes = notes,
            Status = status
        });
        TempData[result.Success ? "FlashSuccess" : "FlashError"] = result.Message;

        if (string.Equals(from, "PatientDetail", StringComparison.OrdinalIgnoreCase) && patientId is not null)
        {
            return RedirectToAction(nameof(PatientDetail), new { userId, patientId });
        }

        return RedirectToAction(nameof(Records), new { userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRecord(
        int userId,
        int recordId,
        int? patientId,
        string? from)
    {
        TempData["FlashError"] = "Doctor is not allowed to delete records. Please use admin account.";
        return RedirectToAction(nameof(Records), new { userId });
    }
    public async Task<IActionResult> PatientDetail(int? userId, int? patientId)
    {
        if (userId is null || patientId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Doctor? doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
        if (doctor is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Patient? patient = await _patientService.GetPatientByIdWithRelationsAsync(patientId.Value);
        if (patient is null)
        {
            return RedirectToAction("Patients", new { userId = userId.Value });
        }

        var records = await _medicalRecordService.GetRecordsByPatientIdAsync(patient.Id);

        var model = new DoctorPatientDetailViewModel
        {
            Doctor = doctor,
            Patient = patient,
            DoctorInitials = GetInitials(doctor.FullName),
            PatientInitials = GetInitials(patient.FullName),
            Age = GetAge(patient.DateOfBirth),
            Records = records.Select(ToRecordItem).ToList()
        };

        return View(model);
    }
    public async Task<IActionResult> Records(int? userId, string? searchTerm)
    {
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        Doctor? doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
        if (doctor is null)
        {
            return RedirectToAction("Index", "Home");
        }

        List<MedicalRecord> records = await _medicalRecordService.GetRecordsByDoctorIdAsync(doctor.Id);
        string query = searchTerm?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(query))
        {
            records = records
                .Where(r =>
                    r.Diagnosis.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (r.Patient?.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    r.Status.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var patients = (await _patientService.GetAllPatientsAsync())
            .OrderBy(p => p.FullName)
            .Select(p => new LoginOptionViewModel
            {
                Id = p.Id,
                Name = p.FullName,
                Subtitle = p.Gender
            })
            .ToList();

        var model = new DoctorRecordsViewModel
        {
            Doctor = doctor,
            Initials = GetInitials(doctor.FullName),
            SearchTerm = query,
            Patients = patients,
            Records = records.Select(ToRecordItem).ToList()
        };

        return View(model);
    }
    private static List<DoctorPatientCardViewModel> BuildPatientCards(IEnumerable<MedicalRecord> records)
    {
        return records
            .Where(r => r.Patient is not null)
            .GroupBy(r => r.PatientId)
            .Select(group =>
            {
                Patient patient = group.First().Patient!;
                var lastRecord = group.OrderByDescending(r => r.VisitDate).First();

                return new DoctorPatientCardViewModel
                {
                    PatientId = patient.Id,
                    FullName = patient.FullName,
                    Initials = GetInitials(patient.FullName),
                    Age = GetAge(patient.DateOfBirth),
                    Gender = patient.Gender,
                    RecordCount = group.Count(),
                    LastDiagnosis = lastRecord.Diagnosis,
                    LastVisitDate = lastRecord.VisitDate
                };
            })
            .OrderBy(c => c.FullName)
            .ToList();
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
    private static bool IsValidStatus(string status)
    {
        return status is "Completed" or "Pending";
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

