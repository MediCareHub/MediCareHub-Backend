using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;
using MediCareHub.DAL.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MediCareHub.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAppointmentRepository _appointmentRepository;  // Added appointment repository
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorRepository doctorRepository, IUserRepository userRepository, IDepartmentRepository departmentRepository, IAppointmentRepository appointmentRepository, ILogger<DoctorController> logger)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _appointmentRepository = appointmentRepository;  // Inject appointment repository
            _logger = logger;
        }

        [Authorize(Roles = "Doctor")] // Only Doctors can access this
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            var doctor = await _doctorRepository.GetByUserId(userId);
            if (doctor == null)
            {
                TempData["ProfileIncomplete"] = "Please complete your profile!";
                return RedirectToAction("CompleteProfile");
            }

            // Fetch today's appointments for this doctor
            var todayAppointments = await _appointmentRepository.GetTodayAppointmentsForDoctorAsync(doctor.DoctorId);
            var pendingAppointments = await _appointmentRepository.GetPendingAppointmentsForDoctorAsync(doctor.DoctorId);


            var dashboardViewModel = new DoctorDashboardViewModel
            {
                TodayAppointments = todayAppointments ?? new List<Appointment>(), // Handle possible null
                PendingAppointments = pendingAppointments ?? new List<Appointment>(),
                DoctorFullName = doctor.User.FullName // Pass the doctor's full name
            };

            return View(dashboardViewModel); // Return the dashboard view with appointments data
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteProfile()
        {
            var userId = GetCurrentUserId();
            var doctor = await _doctorRepository.GetByUserId(userId);

            // Populate department list for dropdown
            ViewBag.Departments = await _departmentRepository.GetAllAsync();

            if (doctor != null)
            {
                var model = new DoctorProfileViewModel
                {
                    UserId = userId,
                    DepartmentId = doctor.DepartmentId, // Set to existing doctor's department
                    ExperienceYears = doctor.ExperienceYears,
                    Qualification = doctor.Qualification,
                    Description = doctor.Description
                };

                return View(model); // Pre-fill with existing data
            }

            return View(new DoctorProfileViewModel { UserId = userId });
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteProfile(DoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return View(model);
                }

                var existingDoctor = await _doctorRepository.GetByUserId(userId);

                if (existingDoctor != null)
                {
                    // Update existing doctor profile
                    existingDoctor.DepartmentId = model.DepartmentId;
                    existingDoctor.ExperienceYears = model.ExperienceYears;
                    existingDoctor.Qualification = model.Qualification;
                    existingDoctor.Description = model.Description;

                    _doctorRepository.Update(existingDoctor);
                }
                else
                {
                    // Create a new doctor profile
                    var doctor = new Doctor
                    {
                        DoctorId = userId, // Set UserId as DoctorId
                        User = user, // Associate the User entity
                        DepartmentId = model.DepartmentId,
                        ExperienceYears = model.ExperienceYears,
                        Qualification = model.Qualification,
                        Description = model.Description
                    };

                    await _doctorRepository.AddAsync(doctor);
                }

                await _doctorRepository.SaveAsync();
                TempData["SuccessMessage"] = "Profile completed successfully!";
                return RedirectToAction("Dashboard");
            }

            // Reload departments if the form is invalid
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(model); // Return model with validation errors
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ListOfAppointments()
        {
            var doctorId = GetCurrentUserId(); // Get the current logged-in doctor's ID

            // Fetch all appointments for this doctor
            var appointments = await _appointmentRepository.GetAppointmentsForDoctorAsync(doctorId);

            // Map appointments to the view model
            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                DoctorFullName = a.Doctor.User.FullName, // Fetch the doctor's full name
                PatientFullName = a.Patient.User.FullName, // Fetch the patient's full name
                MedicalRecord = a.MedicalRecord != null ? new MedicalRecordViewModel
                {
                    Diagnosis = a.MedicalRecord.Diagnosis,
                    Medication = a.MedicalRecord.Medication
                } : null
            });

            return View(appointmentViewModels); // Pass the list of appointments to the view
        }


        private int GetCurrentUserId()
        {
            // Retrieve the current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
