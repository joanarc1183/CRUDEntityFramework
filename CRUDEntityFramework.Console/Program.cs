using CRUDEntityFramework.Data;
using CRUDEntityFramework.Models;
using CRUDEntityFramework.Services;
using Microsoft.EntityFrameworkCore;

namespace CRUDEntityFramework.Console;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var db = new HospitalDbContext();
        await db.Database.MigrateAsync();

        var patientService = new PatientService(db);
        var medicalRecordService = new MedicalRecordService(db);
        bool exit = false;

        while (!exit)
        {
            System.Console.Clear();
            System.Console.WriteLine("=== Hospital Patient Management ===");
            System.Console.WriteLine("1. Add Patient");
            System.Console.WriteLine("2. See History");
            System.Console.WriteLine("3. Update Diagnosis");
            System.Console.WriteLine("4. Delete Record");
            System.Console.WriteLine("0. Exit");
            System.Console.Write("Choose menu: ");

            string? choice = System.Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddPatientAsync(patientService);
                    break;
                case "2":
                    await SeeHistoryAsync(patientService);
                    break;
                case "3":
                    await UpdateDiagnosisAsync(medicalRecordService);
                    break;
                case "4":
                    await DeleteRecordAsync(medicalRecordService);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    System.Console.WriteLine("Invalid menu.");
                    Pause();
                    break;
            }
        }
    }

    private static async Task AddPatientAsync(PatientService patientService)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Add Patient ===");

        var patient = new Patient
        {
            FullName = ReadRequired("Full Name: "),
            DateOfBirth = ReadDate("Date of Birth (yyyy-MM-dd): "),
            Gender = ReadRequired("Gender: "),
            Address = ReadRequired("Address: "),
            PhoneNumber = ReadRequired("Phone Number: "),
            CreatedAt = DateTime.UtcNow
        };

        await patientService.CreatePatient(patient);

        System.Console.WriteLine("Patient added successfully.");
        Pause();
    }

    private static async Task SeeHistoryAsync(PatientService patientService)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Patient History ===");

        string fullName = ReadRequired("Full Name: ");
        List<Patient> patients = await patientService.GetPatientHistoryByFullNameAsync(fullName);

        if (patients.Count == 0)
        {
            System.Console.WriteLine("Patient not found.");
            Pause();
            return;
        }

        foreach (Patient patient in patients)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"ID           : {patient.Id}");
            System.Console.WriteLine($"Full Name    : {patient.FullName}");
            System.Console.WriteLine($"Date of Birth: {patient.DateOfBirth:yyyy-MM-dd}");
            System.Console.WriteLine($"Gender       : {patient.Gender}");
            System.Console.WriteLine($"Address      : {patient.Address}");
            System.Console.WriteLine($"Phone Number : {patient.PhoneNumber}");

            if (patient.MedicalRecords.Count == 0)
            {
                System.Console.WriteLine("Medical Records: none");
                continue;
            }

            System.Console.WriteLine("Medical Records:");
            foreach (MedicalRecord record in patient.MedicalRecords.OrderByDescending(m => m.VisitDate))
            {
                string doctorName = record.Doctor?.FullName ?? "Unknown Doctor";
                System.Console.WriteLine($"- {record.VisitDate:yyyy-MM-dd} | {doctorName} | {record.Diagnosis}");
            }
        }

        Pause();
    }

    private static async Task UpdateDiagnosisAsync(MedicalRecordService medicalRecordService)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Update Diagnosis ===");

        string fullName = ReadRequired("Full Name: ");
        List<MedicalRecord> records = await medicalRecordService.GetRecordsByPatientFullNameAsync(fullName);

        if (records.Count == 0)
        {
            System.Console.WriteLine("No medical record found for this patient.");
            Pause();
            return;
        }

        System.Console.WriteLine("Medical Records:");
        foreach (MedicalRecord record in records)
        {
            string doctorName = record.Doctor?.FullName ?? "Unknown Doctor";
            System.Console.WriteLine($"ID {record.Id} | {record.VisitDate:yyyy-MM-dd} | {doctorName} | {record.Diagnosis}");
        }

        int recordId = ReadPositiveInt("Choose Record ID to update: ");
        if (!records.Any(r => r.Id == recordId))
        {
            System.Console.WriteLine("Record ID is not in the list.");
            Pause();
            return;
        }

        string newDiagnosis = ReadRequired("New Diagnosis: ");
        bool updated = await medicalRecordService.UpdateDiagnosisAsync(recordId, newDiagnosis);

        System.Console.WriteLine(updated ? "Diagnosis updated successfully." : "Failed to update diagnosis.");
        Pause();
    }

    private static async Task DeleteRecordAsync(MedicalRecordService medicalRecordService)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Delete Record ===");

        List<MedicalRecord> records = await medicalRecordService.GetAllRecordAsync();

        if (records.Count == 0)
        {
            System.Console.WriteLine("No medical record found. Create a new one before deleting.");
            Pause();
            return;
        }

        System.Console.WriteLine("Medical Records:");
        foreach (MedicalRecord record in records)
        {
            System.Console.WriteLine($"ID {record.Id} | {record.VisitDate:yyyy-MM-dd} | {record.Doctor} | {record.Diagnosis}"); 
        }

        int recordId = ReadPositiveInt("Choose Record ID to delete: ");
        if (!records.Any(r => r.Id == recordId))
        {
            System.Console.WriteLine("Record ID is not in the list.");
            Pause();
            return;
        }
        
        await medicalRecordService.DeleteRecordAsync(recordId);
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            System.Console.Write(label);
            string? input = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            System.Console.WriteLine("Input cannot be empty.");
        }
    }

    private static int ReadPositiveInt(string label)
    {
        while (true)
        {
            System.Console.Write(label);
            string? input = System.Console.ReadLine();
            if (int.TryParse(input, out int value) && value > 0)
            {
                return value;
            }

            System.Console.WriteLine("Input must be a positive number.");
        }
    }

    private static DateOnly ReadDate(string label)
    {
        while (true)
        {
            System.Console.Write(label);
            string? input = System.Console.ReadLine();
            if (DateOnly.TryParse(input, out DateOnly value))
            {
                return value;
            }

            System.Console.WriteLine("Invalid date format. Example: 2001-08-17");
        }
    }

    private static void Pause()
    {
        System.Console.WriteLine();
        System.Console.Write("Press Enter to continue...");
        System.Console.ReadLine();
    }
}
