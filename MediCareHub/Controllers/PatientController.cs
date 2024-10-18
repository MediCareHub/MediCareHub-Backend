using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediCareHub.Controllers
{
    public class PatientController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository, ILogger<PatientController> logger)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            var patient = await _patientRepository.GetByUserId(userId);

            if (patient == null)
            {
                TempData["ProfileIncomplete"] = "Please complete your profile!";
                return RedirectToAction("CompleteProfile");
            }

            // Fetch upcoming appointments for the patient
            var upcomingAppointments = await _appointmentRepository.GetUpcomingAppointmentsForPatientAsync(patient.PatientId);

            var dashboardViewModel = new PatientDashboardViewModel
            {
                UpcomingAppointments = upcomingAppointments ?? new List<Appointment>(), // Handle possible null
                PatientFullName = patient.User.FullName
            };

            return View(dashboardViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CompleteProfile()
        {
            var userId = GetCurrentUserId();
            var patient = await _patientRepository.GetByUserId(userId);

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

                return View(model); // Pre-fill with existing data
            }

            return View(new PatientProfileViewModel { UserId = userId });
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CompleteProfile(PatientProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var user = await _patientRepository.GetByUserId(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return View(model);
                }

                var existingPatient = await _patientRepository.GetByUserId(userId);

                if (existingPatient != null)
                {
                    // Update existing patient profile
                    existingPatient.MedicalHistory = model.MedicalHistory;
                    existingPatient.Allergies = model.Allergies;
                    existingPatient.CurrentMedications = model.CurrentMedications;
                    existingPatient.EmergencyContact = model.EmergencyContact;

                    _patientRepository.Update(existingPatient);
                }
                else
                {
                    // Create new patient profile
                    var patient = new Patient
                    {
                        PatientId = userId,
                        User = user.User,
                        MedicalHistory = model.MedicalHistory,
                        Allergies = model.Allergies,
                        CurrentMedications = model.CurrentMedications,
                        EmergencyContact = model.EmergencyContact
                    };

                    await _patientRepository.AddAsync(patient);
                }

                await _patientRepository.SaveAsync();
                TempData["SuccessMessage"] = "Profile completed successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model); // Return model with validation errors
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment()
        {
            // Fetch list of doctors for selection
            ViewBag.Doctors = await _doctorRepository.GetAllAsync();
            return View(new BookAppointmentViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var patient = await _patientRepository.GetByUserId(userId);

                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Patient profile not found.";
                    return RedirectToAction("CompleteProfile");
                }

                var appointment = new Appointment
                {
                    DoctorId = model.DoctorId,
                    PatientId = patient.PatientId,
                    AppointmentDate = model.AppointmentDate,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                await _appointmentRepository.AddAsync(appointment);
                await _appointmentRepository.SaveAsync();

                TempData["SuccessMessage"] = "Appointment booked successfully!";
                return RedirectToAction("Dashboard");
            }

            // Reload doctors if the form is invalid
            ViewBag.Doctors = await _doctorRepository.GetAllAsync();
            return View(model);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> ListOfAppointments()
        {
            var userId = GetCurrentUserId();
            var patient = await _patientRepository.GetByUserId(userId);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient profile not found.";
                return RedirectToAction("CompleteProfile");
            }

            var appointments = await _appointmentRepository.GetAppointmentsForPatientAsync(patient.PatientId);

            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                DoctorFullName = a.Doctor.User.FullName,
                PatientFullName = a.Patient.User.FullName,
                MedicalRecord = a.MedicalRecord != null ? new MedicalRecordViewModel
                {
                    Diagnosis = a.MedicalRecord.Diagnosis,
                    Medication = a.MedicalRecord.Medication
                } : null
            });

            return View(appointmentViewModels);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
