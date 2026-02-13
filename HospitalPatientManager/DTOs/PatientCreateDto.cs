namespace HospitalPatientManager.DTOs;

public class PatientCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
