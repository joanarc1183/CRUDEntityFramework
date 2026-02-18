using System.Text.RegularExpressions;
using FluentValidation;
using HospitalPatientManager.DTOs;
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
    private readonly IValidator<LoginDto> _loginValidator;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IPatientService patientService,
        IDoctorService doctorService,
        IValidator<LoginDto> loginValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _patientService = patientService;
        _doctorService = doctorService;
        _loginValidator = loginValidator;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(LoginDto dto)
    {
        var validationResult = await _loginValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            TempData["FlashError"] = validationResult.Errors[0].ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        string normalizedName = dto.FullName.Trim();
        string normalizedPassword = dto.Password.Trim();

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
            string generatedPassword = GeneratePasswordFromFullName(patient.FullName);
            var appUser = await EnsureDomainUserAsync(
                $"patient-{patient.Id}",
                patient.FullName,
                "Patient",
                generatedPassword);
            var patientSignIn = await _signInManager.PasswordSignInAsync(
                appUser.UserName!,
                normalizedPassword,
                isPersistent: false,
                lockoutOnFailure: false);
            if (patientSignIn.Succeeded)
            {
                return RedirectToAction("Dashboard", "Patient", new { userId = patient.Id });
            }
        }

        var doctors = await _doctorService.GetAllDoctorsAsync();
        var doctor = doctors.FirstOrDefault(d =>
            string.Equals(d.FullName, normalizedName, StringComparison.OrdinalIgnoreCase));
        if (doctor is not null)
        {
            string generatedPassword = GeneratePasswordFromFullName(doctor.FullName);
            var appUser = await EnsureDomainUserAsync(
                $"doctor-{doctor.Id}",
                doctor.FullName,
                "Doctor",
                generatedPassword);
            var doctorSignIn = await _signInManager.PasswordSignInAsync(
                appUser.UserName!,
                normalizedPassword,
                isPersistent: false,
                lockoutOnFailure: false);
            if (doctorSignIn.Succeeded)
            {
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

    private async Task<ApplicationUser> EnsureDomainUserAsync(
        string userName,
        string displayName,
        string role,
        string generatedPassword)
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

            var createResult = await _userManager.CreateAsync(user, generatedPassword);
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

        if (!await _userManager.CheckPasswordAsync(user, generatedPassword))
        {
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                string errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to reset password for '{displayName}': {errors}");
            }

            var addResult = await _userManager.AddPasswordAsync(user, generatedPassword);
            if (!addResult.Succeeded)
            {
                string errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to set password for '{displayName}': {errors}");
            }
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
    private static string GeneratePasswordFromFullName(string fullName)
    {
        string normalized = Regex.Replace(fullName.Trim(), "\\s+", string.Empty).ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized))
        {
            normalized = "user";
        }

        if (normalized.Length < 4)
        {
            normalized = normalized.PadRight(4, 'x');
        }

        return $"{normalized}a1";
    }
}

