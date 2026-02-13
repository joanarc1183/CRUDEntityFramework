namespace HospitalPatientManager.DTOs;

public class PatientCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public required string Gender  { get; set; } = string.Empty;
    public required string Address  { get; set; } = string.Empty;
    public required string PhoneNumber { get; set; } = string.Empty;
}