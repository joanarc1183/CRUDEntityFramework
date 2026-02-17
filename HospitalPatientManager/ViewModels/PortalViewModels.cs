using HospitalPatientManager.Models;

namespace HospitalPatientManager.ViewModels;

public class HomeIndexViewModel
{
    public List<LoginOptionViewModel> Patients { get; init; } = [];
    public List<LoginOptionViewModel> Doctors { get; init; } = [];
}

public class LoginOptionViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
}

public class PatientDashboardViewModel
{
    public required Patient Patient { get; init; }
    public string Initials { get; init; } = string.Empty;
    public int Age { get; init; }
    public int CompletedCount { get; init; }
    public int UpcomingCount { get; init; }
    public int PendingCount { get; init; }
    public int TotalCheckups { get; init; }
    public MedicalRecord? LatestRecord { get; init; }
    public List<MedicalRecord> UpcomingRecords { get; init; } = [];
    public List<MedicalRecord> RecentRecords { get; init; } = [];
}

public class PatientBiodataViewModel
{
    public required Patient Patient { get; init; }
    public string Initials { get; init; } = string.Empty;
    public int Age { get; init; }
}

public class PatientRecordsViewModel
{
    public required Patient Patient { get; init; }
    public string Initials { get; init; } = string.Empty;
    public string SearchTerm { get; init; } = string.Empty;
    public List<RecordItemViewModel> Records { get; init; } = [];
}

public class DoctorDashboardViewModel
{
    public required Doctor Doctor { get; init; }
    public string Initials { get; init; } = string.Empty;
    public int MyPatientsCount { get; init; }
    public int CompletedCount { get; init; }
    public int UpcomingCount { get; init; }
    public int PendingCount { get; init; }
    public List<DoctorPatientCardViewModel> MyPatients { get; init; } = [];
    public List<RecordItemViewModel> UpcomingRecords { get; init; } = [];
    public List<RecordItemViewModel> PendingRecords { get; init; } = [];
    public List<RecordItemViewModel> RecentRecords { get; init; } = [];
}

public class DoctorPatientsViewModel
{
    public required Doctor Doctor { get; init; }
    public string Initials { get; init; } = string.Empty;
    public string SearchFullName { get; init; } = string.Empty;
    public bool IsSearchMode { get; init; }
    public List<DoctorPatientCardViewModel> Patients { get; init; } = [];
}

public class DoctorPatientDetailViewModel
{
    public required Doctor Doctor { get; init; }
    public required Patient Patient { get; init; }
    public string DoctorInitials { get; init; } = string.Empty;
    public string PatientInitials { get; init; } = string.Empty;
    public int Age { get; init; }
    public List<RecordItemViewModel> Records { get; init; } = [];
}

public class DoctorRecordsViewModel
{
    public required Doctor Doctor { get; init; }
    public string Initials { get; init; } = string.Empty;
    public string SearchTerm { get; init; } = string.Empty;
    public List<LoginOptionViewModel> Patients { get; init; } = [];
    public List<RecordItemViewModel> Records { get; init; } = [];
}

public class AdminDashboardViewModel
{
    public int TotalCheckups { get; init; }
    public int TotalPatients { get; init; }
    public int TotalDoctors { get; init; }
}

public class AdminPatientsViewModel
{
    public string SearchTerm { get; init; } = string.Empty;
    public List<DoctorPatientCardViewModel> Patients { get; init; } = [];
}

public class AdminDoctorsViewModel
{
    public string SearchTerm { get; init; } = string.Empty;
    public List<DoctorListItemViewModel> Doctors { get; init; } = [];
}

public class AdminRecordsViewModel
{
    public string SearchTerm { get; init; } = string.Empty;
    public List<RecordItemViewModel> Records { get; init; } = [];
}

public class AdminPatientDetailViewModel
{
    public required Patient Patient { get; init; }
    public string Initials { get; init; } = string.Empty;
    public int Age { get; init; }
    public List<RecordItemViewModel> Records { get; init; } = [];
}

public class DoctorListItemViewModel
{
    public int DoctorId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public int RecordCount { get; init; }
}

public class DoctorPatientCardViewModel
{
    public int PatientId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Gender { get; init; } = string.Empty;
    public int RecordCount { get; init; }
    public string? LastDiagnosis { get; init; }
    public DateTime? LastVisitDate { get; init; }
}

public class RecordItemViewModel
{
    public int Id { get; init; }
    public string Diagnosis { get; init; } = string.Empty;
    public DateTime VisitDate { get; init; }
    public string DoctorName { get; init; } = string.Empty;
    public string DoctorSpecialization { get; init; } = string.Empty;
    public string PatientName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string Treatment { get; init; } = string.Empty;
}
