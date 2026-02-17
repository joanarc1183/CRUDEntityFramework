using System.ComponentModel.DataAnnotations;

namespace HospitalPatientManager.Models;

public class MedicalRecord
{
    public int Id { get; set; }

    public DateTime VisitDate { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Diagnosis { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Treatment { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Completed";

    public virtual Patient? Patient { get; set; }

    public virtual Doctor? Doctor { get; set; }
}

