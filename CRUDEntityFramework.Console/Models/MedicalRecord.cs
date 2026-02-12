using System.ComponentModel.DataAnnotations;

namespace CRUDEntityFramework.Console.Models;

public class MedicalRecord
{
    public int Id { get; set; }

    public DateTime VisitDate { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Diagnosis { get; set; } = string.Empty;

    public virtual Patient? Patient { get; set; }

    public virtual Doctor? Doctor { get; set; }
}
