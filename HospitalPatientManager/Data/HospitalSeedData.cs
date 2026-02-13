using HospitalPatientManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Data;

public static class HospitalSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                Id = 1,
                FullName = "Budi Santoso",
                DateOfBirth = new DateOnly(1990, 5, 12),
                Gender = "Male",
                Address = "Jakarta",
                PhoneNumber = "081234567890",
                CreatedAt = new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = 2,
                FullName = "Siti Rahma",
                DateOfBirth = new DateOnly(1988, 11, 3),
                Gender = "Female",
                Address = "Bandung",
                PhoneNumber = "081298765432",
                CreatedAt = new DateTime(2026, 1, 10, 8, 5, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = 3,
                FullName = "Andi Pratama",
                DateOfBirth = new DateOnly(2001, 2, 18),
                Gender = "Male",
                Address = "Surabaya",
                PhoneNumber = "081322233344",
                CreatedAt = new DateTime(2026, 1, 10, 8, 10, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                Id = 1,
                FullName = "Dr. Maya Putri",
                Specialization = "Internal Medicine",
                CreatedAt = new DateTime(2026, 1, 10, 7, 30, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = 2,
                FullName = "Dr. Raka Wijaya",
                Specialization = "Pediatrics",
                CreatedAt = new DateTime(2026, 1, 10, 7, 35, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = 3,
                FullName = "Dr. Nanda Lestari",
                Specialization = "Cardiology",
                CreatedAt = new DateTime(2026, 1, 10, 7, 40, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<MedicalRecord>().HasData(
            new MedicalRecord
            {
                Id = 1,
                PatientId = 1,
                DoctorId = 1,
                VisitDate = new DateTime(2026, 1, 11, 9, 0, 0, DateTimeKind.Utc),
                Diagnosis = "Seasonal flu"
            },
            new MedicalRecord
            {
                Id = 2,
                PatientId = 2,
                DoctorId = 3,
                VisitDate = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc),
                Diagnosis = "Hypertension"
            },
            new MedicalRecord
            {
                Id = 3,
                PatientId = 3,
                DoctorId = 2,
                VisitDate = new DateTime(2026, 1, 13, 11, 0, 0, DateTimeKind.Utc),
                Diagnosis = "Mild asthma"
            },
            new MedicalRecord
            {
                Id = 4,
                PatientId = 1,
                DoctorId = 3,
                VisitDate = new DateTime(2026, 1, 14, 13, 0, 0, DateTimeKind.Utc),
                Diagnosis = "Chest pain follow-up"
            },
            new MedicalRecord
            {
                Id = 5,
                PatientId = 2,
                DoctorId = 1,
                VisitDate = new DateTime(2026, 1, 15, 15, 0, 0, DateTimeKind.Utc),
                Diagnosis = "Gastritis"
            });
    }
}
