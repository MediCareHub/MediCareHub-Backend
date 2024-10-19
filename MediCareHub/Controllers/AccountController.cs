using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;

namespace MediCareHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public AccountController(IUserRepository userRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already registered.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveAsync();

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if the user is already authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to the appropriate dashboard based on the user's role
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                return RedirectToRoleBasedPage(role);
            }

            return View(new LoginViewModel());
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userRepository.GetUserByEmailAsync(model.Email);

        //        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        //        {
        //            ModelState.AddModelError("", "Invalid email or password.");
        //            return View(model);
        //        }

        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
        //            new Claim(ClaimTypes.Email, user.Email),
        //            new Claim(ClaimTypes.Role, user.Role ?? "")
        //        };

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var authProperties = new AuthenticationProperties
        //        {
        //            IsPersistent = true,
        //            ExpiresUtc = DateTime.UtcNow.AddHours(8)
        //        };

        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        //            new ClaimsPrincipal(claimsIdentity), authProperties);

        //        return RedirectToRoleBasedPage(user.Role);
        //    }

        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);

                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                var claims = new List<Claim>
{
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Add UserId claim
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                };


                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Check if the profile is completed
                if (user.Role == "Doctor")
                {
                    var doctor = await _doctorRepository.GetByUserId(user.UserId);
                    if (doctor == null)
                    {
                        return RedirectToAction("CompleteProfile", "Doctor"); // Redirect to complete doctor profile
                    }
                }
                else if (user.Role == "Patient")
                {
                    var patient = await _patientRepository.GetByUserId(user.UserId);
                    if (patient == null)
                    {
                        return RedirectToAction("CompleteProfile", "Patient"); // Redirect to complete patient profile
                    }
                }

                return RedirectToRoleBasedPage(user.Role);
            }

            return View(model);
        }
        private IActionResult RedirectToRoleBasedPage(string role)
        {
            switch (role)
            {
                case "Patient":
                    return RedirectToAction("DashBoard", "Patient");
                case "Doctor":
                    return RedirectToAction("Dashboard", "Doctor");
                default:
                    TempData["ErrorMessage"] = "Your role is not recognized.";
                    return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
