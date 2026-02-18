namespace HospitalPatientManager.DTOs.MedicalRecord;

public class UpdateMedicalRecordDto
{
    public int RecordId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

