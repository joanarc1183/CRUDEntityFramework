using CRUDEntityFramework.ConsoleApp.Data;
using CRUDEntityFramework.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

using var db = new HospitalDbContext();
db.Database.EnsureCreated();

bool exit = false;

while (!exit)
{
    Console.Clear();
    Console.WriteLine("=== Hospital Patient Management (EF Core) ===");
    Console.WriteLine("1. Add Patient");
    Console.WriteLine("2. View All Patients");
    Console.WriteLine("3. View Patient Detail");
    Console.WriteLine("4. Update Patient");
    Console.WriteLine("5. Delete Patient");
    Console.WriteLine("0. Exit");
    Console.Write("Choose menu: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddPatient(db);
            break;
        case "2":
            ListPatients(db);
            break;
        case "3":
            ViewPatientDetail(db);
            break;
        case "4":
            UpdatePatient(db);
            break;
        case "5":
            DeletePatient(db);
            break;
        case "0":
            exit = true;
            break;
        default:
            Console.WriteLine("Invalid menu.");
            Pause();
            break;
    }
}

static void AddPatient(HospitalDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== Add Patient ===");

    string mrn = ReadRequired("Medical Record Number: ");

    bool exists = db.Patients.Any(p => p.MedicalRecordNumber == mrn);
    if (exists)
    {
        Console.WriteLine("MRN already exists.");
        Pause();
        return;
    }

    string fullName = ReadRequired("Full Name: ");
    DateOnly dob = ReadDate("Date of Birth (yyyy-MM-dd): ");
    string gender = ReadRequired("Gender: ");
    string address = ReadRequired("Address: ");
    string phone = ReadRequired("Phone Number: ");

    var patient = new Patient
    {
        MedicalRecordNumber = mrn,
        FullName = fullName,
        DateOfBirth = dob,
        Gender = gender,
        Address = address,
        PhoneNumber = phone,
        CreatedAt = DateTime.UtcNow
    };

    db.Patients.Add(patient);
    db.SaveChanges();

    Console.WriteLine("Patient added successfully.");
    Pause();
}

static void ListPatients(HospitalDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== All Patients ===");

    var patients = db.Patients
        .AsNoTracking()
        .OrderBy(p => p.FullName)
        .ToList();

    if (patients.Count == 0)
    {
        Console.WriteLine("No patient data.");
        Pause();
        return;
    }

    Console.WriteLine("ID | MRN | Name | DOB | Phone");
    Console.WriteLine(new string('-', 70));

    foreach (Patient patient in patients)
    {
        Console.WriteLine(
            $"{patient.Id} | {patient.MedicalRecordNumber} | {patient.FullName} | {patient.DateOfBirth:yyyy-MM-dd} | {patient.PhoneNumber}");
    }

    Pause();
}

static void ViewPatientDetail(HospitalDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== Patient Detail ===");

    int id = ReadInt("Patient ID: ");

    Patient? patient = db.Patients.AsNoTracking().FirstOrDefault(p => p.Id == id);
    if (patient is null)
    {
        Console.WriteLine("Patient not found.");
        Pause();
        return;
    }

    Console.WriteLine($"ID: {patient.Id}");
    Console.WriteLine($"MRN: {patient.MedicalRecordNumber}");
    Console.WriteLine($"Name: {patient.FullName}");
    Console.WriteLine($"DOB: {patient.DateOfBirth:yyyy-MM-dd}");
    Console.WriteLine($"Gender: {patient.Gender}");
    Console.WriteLine($"Address: {patient.Address}");
    Console.WriteLine($"Phone: {patient.PhoneNumber}");
    Console.WriteLine($"Created (UTC): {patient.CreatedAt:yyyy-MM-dd HH:mm:ss}");

    Pause();
}

static void UpdatePatient(HospitalDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== Update Patient ===");

    int id = ReadInt("Patient ID: ");
    Patient? patient = db.Patients.FirstOrDefault(p => p.Id == id);

    if (patient is null)
    {
        Console.WriteLine("Patient not found.");
        Pause();
        return;
    }

    Console.WriteLine("Press Enter to keep current value.");

    string? fullName = ReadOptional($"Full Name ({patient.FullName}): ");
    string? gender = ReadOptional($"Gender ({patient.Gender}): ");
    string? address = ReadOptional($"Address ({patient.Address}): ");
    string? phone = ReadOptional($"Phone Number ({patient.PhoneNumber}): ");

    Console.Write($"Date of Birth ({patient.DateOfBirth:yyyy-MM-dd}) [yyyy-MM-dd]: ");
    string? dobInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(fullName))
    {
        patient.FullName = fullName.Trim();
    }

    if (!string.IsNullOrWhiteSpace(gender))
    {
        patient.Gender = gender.Trim();
    }

    if (!string.IsNullOrWhiteSpace(address))
    {
        patient.Address = address.Trim();
    }

    if (!string.IsNullOrWhiteSpace(phone))
    {
        patient.PhoneNumber = phone.Trim();
    }

    if (!string.IsNullOrWhiteSpace(dobInput))
    {
        while (!DateOnly.TryParse(dobInput, out DateOnly dob))
        {
            Console.Write("Invalid format. Re-enter DOB (yyyy-MM-dd): ");
            dobInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dobInput))
            {
                dob = patient.DateOfBirth;
                break;
            }
        }

        if (DateOnly.TryParse(dobInput, out DateOnly validDob))
        {
            patient.DateOfBirth = validDob;
        }
    }

    db.SaveChanges();
    Console.WriteLine("Patient updated successfully.");
    Pause();
}

static void DeletePatient(HospitalDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== Delete Patient ===");

    int id = ReadInt("Patient ID: ");
    Patient? patient = db.Patients.FirstOrDefault(p => p.Id == id);

    if (patient is null)
    {
        Console.WriteLine("Patient not found.");
        Pause();
        return;
    }

    Console.WriteLine($"Patient: {patient.FullName} ({patient.MedicalRecordNumber})");
    Console.Write("Delete this patient? (y/n): ");
    string? confirmation = Console.ReadLine();

    if (confirmation?.Trim().ToLowerInvariant() == "y")
    {
        db.Patients.Remove(patient);
        db.SaveChanges();
        Console.WriteLine("Patient deleted successfully.");
    }
    else
    {
        Console.WriteLine("Delete canceled.");
    }

    Pause();
}

static string ReadRequired(string label)
{
    while (true)
    {
        Console.Write(label);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine("Input cannot be empty.");
    }
}

static string? ReadOptional(string label)
{
    Console.Write(label);
    return Console.ReadLine();
}

static int ReadInt(string label)
{
    while (true)
    {
        Console.Write(label);
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int number) && number > 0)
        {
            return number;
        }

        Console.WriteLine("Input must be a positive number.");
    }
}

static DateOnly ReadDate(string label)
{
    while (true)
    {
        Console.Write(label);
        string? input = Console.ReadLine();

        if (DateOnly.TryParse(input, out DateOnly date))
        {
            return date;
        }

        Console.WriteLine("Invalid date format. Example: 2001-08-17");
    }
}

static void Pause()
{
    Console.WriteLine();
    Console.Write("Press Enter to continue...");
    Console.ReadLine();
}
