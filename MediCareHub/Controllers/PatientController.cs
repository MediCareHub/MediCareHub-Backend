using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;
using MediCareHub.DAL.Models;
using System.Security.Claims;

namespace MediCareHub.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;

        public PatientController(IPatientRepository patientRepository, IUserRepository userRepository)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Patient")] // Only Patients can access this
        public IActionResult Dashboard()
        {
            // Logic to fetch patient-specific data can be added here
            return View(); // Ensure you have a corresponding Dashboard view
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public IActionResult CompleteProfile()
        {
            var userId = GetCurrentUserId(); // Assume this method fetches the logged-in user's ID
            var patient = _patientRepository.GetByUserId(userId).Result; // Fetch patient details

            if (patient != null)
            {
                var model = new PatientProfileViewModel
                {
                    UserId = userId,
                    MedicalHistory = patient.MedicalHistory,
                    Allergies = patient.Allergies,
                    CurrentMedications = patient.CurrentMedications,
                    EmergencyContact = patient.EmergencyContact
                };

                return View(model);
            }

            return View(new PatientProfileViewModel { UserId = userId }); // Return empty model for new patients
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CompleteProfile(PatientProfileViewModel model)
        {
            var userId = GetCurrentUserId(); // Assume this method fetches the logged-in user's ID

            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    PatientId = userId, // Ensure you set this correctly
                    MedicalHistory = model.MedicalHistory,
                    Allergies = model.Allergies,
                    CurrentMedications = model.CurrentMedications,
                    EmergencyContact = model.EmergencyContact
                };

                await _patientRepository.AddAsync(patient);
                await _patientRepository.SaveAsync(); // Ensure the changes are saved to the database

                // Profile completed successfully
                TempData["SuccessMessage"] = "Profile completed successfully!";
                return RedirectToAction("Dashboard"); // Redirect to Dashboard after completion
            }

            return View(model); // Return model with validation errors if any
        }

        private int GetCurrentUserId()
        {
            // Retrieve the current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
