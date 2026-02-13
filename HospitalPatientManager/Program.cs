using AutoMapper;
using HospitalPatientManager.Controllers;
using HospitalPatientManager.Data;
using HospitalPatientManager.DTOs;
using HospitalPatientManager.Mappings;
using HospitalPatientManager.Models;
using HospitalPatientManager.Repositories;
using HospitalPatientManager.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalPatientManager.Console;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var db = new HospitalDbContext();
        await db.Database.MigrateAsync();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        IMapper mapper = mapperConfig.CreateMapper();

        IPatientRepository patientRepository = new PatientRepository(db);
        IMedicalRecordRepository medicalRecordRepository = new MedicalRecordRepository(db);

        IPatientService patientService = new PatientService(patientRepository, medicalRecordRepository, mapper);
        var medicalRecordService = new MedicalRecordService(medicalRecordRepository);

        var patientsController = new PatientsController(patientService);
        var medicalRecordsController = new MedicalRecordsController(medicalRecordService);

        bool exit = false;

        while (!exit)
        {
            System.Console.Clear();
            System.Console.WriteLine("=== Hospital Patient Management (MVC) ===");
            System.Console.WriteLine("1. Add Patient");
            System.Console.WriteLine("2. See History");
            System.Console.WriteLine("3. Update Diagnosis");
            System.Console.WriteLine("4. Delete Record");
            System.Console.WriteLine("9. See All Patient with history");
            System.Console.WriteLine("0. Exit");
            System.Console.Write("Choose menu: ");

            string? choice = System.Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddPatientAsync(patientsController);
                    break;
                case "2":
                    await SeeHistoryAsync(patientsController);
                    break;
                case "3":
                    await UpdateDiagnosisAsync(medicalRecordsController);
                    break;
                case "4":
                    await DeleteRecordAsync(medicalRecordsController);
                    break;
                case "9":
                    await SeeListPatient(patientsController);
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

    private static async Task AddPatientAsync(PatientsController patientsController)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Add Patient ===");

        var dto = new PatientCreateDto
        {
            FullName = ReadRequired("Full Name: "),
            DateOfBirth = ReadDate("Date of Birth (yyyy-MM-dd): "),
            Gender = ReadRequired("Gender: "),
            Address = ReadRequired("Address: "),
            PhoneNumber = ReadRequired("Phone Number: ")
        };

        var result = await patientsController.CreateAsync(dto);
        System.Console.WriteLine(result.Success ? result.Message : $"Failed: {result.Message}");
        Pause();
    }

    private static async Task SeeHistoryAsync(PatientsController patientsController)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Patient History ===");

        string fullName = ReadRequired("Full Name: ");
        List<Patient> patients = await patientsController.GetHistoryAsync(fullName);

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

    private static async Task UpdateDiagnosisAsync(MedicalRecordsController medicalRecordsController)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Update Diagnosis ===");

        string fullName = ReadRequired("Full Name: ");
        List<MedicalRecord> records = await medicalRecordsController.GetAsync(fullName);

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
        bool updated = await medicalRecordsController.UpdateDiagnosisAsync(recordId, newDiagnosis);

        System.Console.WriteLine(updated ? "Diagnosis updated successfully." : "Failed to update diagnosis.");
        Pause();
    }

    private static async Task DeleteRecordAsync(MedicalRecordsController medicalRecordsController)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Delete Record ===");

        List<MedicalRecord> records = await medicalRecordsController.GetAsync();

        if (records.Count == 0)
        {
            System.Console.WriteLine("No medical record found. Create a new one before deleting.");
            Pause();
            return;
        }

        System.Console.WriteLine("Medical Records:");
        foreach (MedicalRecord record in records)
        {
            string doctorName = record.Doctor?.FullName ?? "Unknown Doctor";
            System.Console.WriteLine($"ID {record.Id} | {record.VisitDate:yyyy-MM-dd} | {doctorName} | {record.Diagnosis}");
        }

        int recordId = ReadPositiveInt("Choose Record ID to delete: ");
        if (!records.Any(r => r.Id == recordId))
        {
            System.Console.WriteLine("Record ID is not in the list.");
            Pause();
            return;
        }

        bool deleted = await medicalRecordsController.DeleteAsync(recordId);
        System.Console.WriteLine(deleted ? "Record deleted successfully." : "Failed to delete record.");
        Pause();
    }

    private static async Task SeeListPatient(PatientsController patientsController)
    {
        System.Console.Clear();
        System.Console.WriteLine("=== All Patient ===");

        List<Patient> patients = await patientsController.GetAllPatients();

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

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

// // Add Entity Framework
// builder.Services.AddDbContext<HospitalDbContext>(options =>
//     options.UseSqlite("Data Source=hospital.db"));

// builder.Services.AddAutoMapper(typeof(MappingProfile));

// // Add Repositories
// builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// builder.Services.AddScoped<IPatientRepository, PatientRepository>();
// builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();

// // Add Services
// builder.Services.AddScoped<IPatientService, PatientService>();
// builder.Services.AddScoped<MedicalRecordService>();

// var app = builder.Build();

// // Seed roles and admin user
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
//     await db.Database.MigrateAsync();
// }

// app.UseHttpsRedirection();
// app.MapControllers();

// app.Run();
