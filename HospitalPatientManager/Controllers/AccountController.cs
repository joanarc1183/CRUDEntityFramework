using System.Text.RegularExpressions;
using HospitalPatientManager.Models;
using HospitalPatientManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalPatientManager.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IPatientService patientService,
        IDoctorService doctorService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(string fullName, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(password))
        {
            TempData["FlashError"] = "Name and password are required.";
            return RedirectToAction("Index", "Home");
        }

        string normalizedName = fullName.Trim();
        string normalizedPassword = password.Trim();

        if (string.Equals(normalizedName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            var adminResult = await _signInManager.PasswordSignInAsync(
                "admin",
                normalizedPassword,
                isPersistent: false,
                lockoutOnFailure: false);

            if (adminResult.Succeeded)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            TempData["FlashError"] = "Invalid name or password.";
            return RedirectToAction("Index", "Home");
        }

        var patients = await _patientService.GetAllPatientsAsync();
        var patient = patients.FirstOrDefault(p =>
            string.Equals(p.FullName, normalizedName, StringComparison.OrdinalIgnoreCase));
        if (patient is not null)
        {
            string expectedPatientPassword = BuildPasswordFromName(patient.FullName);
            if (string.Equals(normalizedPassword, expectedPatientPassword, StringComparison.OrdinalIgnoreCase))
            {
                var appUser = await EnsureDomainUserAsync($"patient-{patient.Id}", patient.FullName, "Patient");
                await _signInManager.SignInAsync(appUser, isPersistent: false);
                return RedirectToAction("Dashboard", "Patient", new { userId = patient.Id });
            }
        }

        var doctors = await _doctorService.GetAllDoctorsAsync();
        var doctor = doctors.FirstOrDefault(d =>
            string.Equals(d.FullName, normalizedName, StringComparison.OrdinalIgnoreCase));
        if (doctor is not null)
        {
            string expectedDoctorPassword = BuildPasswordFromDoctorName(doctor.FullName);
            if (string.Equals(normalizedPassword, expectedDoctorPassword, StringComparison.OrdinalIgnoreCase))
            {
                var appUser = await EnsureDomainUserAsync($"doctor-{doctor.Id}", doctor.FullName, "Doctor");
                await _signInManager.SignInAsync(appUser, isPersistent: false);
                return RedirectToAction("Dashboard", "Doctor", new { userId = doctor.Id });
            }
        }

        TempData["FlashError"] = "Invalid name or password.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/Account/Logout")]
    public async Task<IActionResult> LogoutFromLink()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task<ApplicationUser> EnsureDomainUserAsync(string userName, string displayName, string role)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                DisplayName = displayName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, BuildDomainPassword(displayName));
            if (!createResult.Succeeded)
            {
                string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user '{displayName}': {errors}");
            }
        }
        else if (!string.Equals(user.DisplayName, displayName, StringComparison.Ordinal))
        {
            user.DisplayName = displayName;
            await _userManager.UpdateAsync(user);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    private static string BuildDomainPassword(string fullName)
    {
        return $"{Regex.Replace(fullName, "\\s+", string.Empty)}A1";
    }

    private static string BuildPasswordFromName(string fullName)
    {
        return Regex.Replace(fullName, "\\s+", string.Empty).Trim();
    }

    private static string BuildPasswordFromDoctorName(string fullName)
    {
        string withoutTitle = Regex.Replace(fullName.Trim(), "^(dr\\.?\\s+)", string.Empty, RegexOptions.IgnoreCase);
        return Regex.Replace(withoutTitle, "\\s+", string.Empty);
    }
}
