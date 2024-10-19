using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;
using MediCareHub.DAL.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var user = await _userRepository.GetByIdAsync(userId);

            // Populate department list for dropdown'


            ViewBag.Departments = await _departmentRepository.GetAllAsync();

            if (doctor != null)
            {
                var model = new DoctorProfileViewModel
                {
                    UserId = userId,
                    DepartmentId = doctor.DepartmentId, // Set to existing doctor's department
                    ExperienceYears = doctor.ExperienceYears,
                    Qualification = doctor.Qualification,
                    Description = doctor.Description,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender

                };

                return View(model); // Pre-fill with existing data
            }

            return View(new CompleteDoctorProfileViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteProfile(CompleteDoctorProfileViewModel model)
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
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogError("Validation Errors: " + string.Join(", ", errors));

                ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
                return View(model); // Return the form with validation errors
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




        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId(); // Fetch the logged-in user's ID
            var doctor = await _doctorRepository.GetByUserId(userId); // Fetch the doctor details

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("Dashboard"); // Redirect if no profile is found
            }

            var model = new DoctorProfileViewModel
            {
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                Phone = doctor.User.Phone,
                ExperienceYears = doctor.ExperienceYears,
                Qualification = doctor.Qualification,
                DepartmentName = doctor.Department.DepartmentName, // Assuming Department navigation property is loaded
                Description = doctor.Description,
                DateOfBirth = doctor.User.DateOfBirth,
                Gender = doctor.User.Gender
            };

            return View(model); // Pass the profile model to the view
        }


        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = GetCurrentUserId();
            var doctor = await _doctorRepository.GetByUserId(userId);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("Dashboard");
            }

            var model = new EditDoctorProfileViewModel
            {
                UserId = doctor.DoctorId,
                FullName = doctor.User.FullName,
                Phone = doctor.User.Phone,
                Email = doctor.User.Email,
                ExperienceYears = doctor.ExperienceYears,
                Qualification = doctor.Qualification,
                DepartmentId = doctor.DepartmentId,
                Description = doctor.Description,
                DateOfBirth = doctor.User.DateOfBirth,
                Gender = doctor.User.Gender
            };

            // Populate departments for the dropdown
            ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "DepartmentId", "DepartmentName");

            return View(model);
        }




        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> EditProfile(EditDoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var doctor = await _doctorRepository.GetByUserId(userId);

                if (doctor == null)
                {
                    TempData["ErrorMessage"] = "Profile not found.";
                    return RedirectToAction("Dashboard");
                }

                // Update the doctor and related user profile
                doctor.User.FullName = model.FullName;
                doctor.User.Phone = model.Phone;
                doctor.User.Email = model.Email;  // Update email
                doctor.ExperienceYears = model.ExperienceYears;
                doctor.Qualification = model.Qualification;
                doctor.DepartmentId = model.DepartmentId;
                doctor.Description = model.Description;
                doctor.User.DateOfBirth = model.DateOfBirth; // Update Date of Birth
                doctor.User.Gender = model.Gender; // Update Gender

                // Update both User and Doctor entities
                _doctorRepository.Update(doctor);  // Includes the related User entity
                await _doctorRepository.SaveAsync();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogError("Validation Errors: " + string.Join(", ", errors));

                ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);
                return View(model); // Return the form with validation errors
            }


            ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "DepartmentId", "DepartmentName", model.DepartmentId);

            return View(model);
        }





        private int GetCurrentUserId()
        {
            // Retrieve the current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
