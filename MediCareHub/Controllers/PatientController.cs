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
        private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository, IDoctorAvailabilityRepository doctorAvailabilityRepository, ILogger<PatientController> logger)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _doctorAvailabilityRepository = doctorAvailabilityRepository;
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
                UpcomingAppointments = upcomingAppointments ?? new List<Appointment>(),
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
                var userId = GetCurrentUserId(); // Assuming you have this method to get the current user's ID.
                var patient = await _patientRepository.GetByUserId(userId);

                if (patient != null)
                {
                    // Update existing patient profile
                    patient.MedicalHistory = model.MedicalHistory;
                    patient.Allergies = model.Allergies;
                    patient.CurrentMedications = model.CurrentMedications;
                    patient.EmergencyContact = model.EmergencyContact;

                    _patientRepository.Update(patient);
                }
                else
                {
                    // Create new patient profile
                    patient = new Patient
                    {
                        PatientId = userId,
                        MedicalHistory = model.MedicalHistory,
                        Allergies = model.Allergies,
                        CurrentMedications = model.CurrentMedications,
                        EmergencyContact = model.EmergencyContact,
                        CreatedAt = DateTime.Now
                    };

                    await _patientRepository.AddAsync(patient);
                }

                await _patientRepository.SaveAsync();
                TempData["SuccessMessage"] = "Profile completed successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model); // In case of invalid model
        }


        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment(int doctorId)
        {
            var doctor = await _doctorRepository.GetByUserId(doctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            // Fetch the doctor's availability
            var availableTimeSlots = await _doctorAvailabilityRepository.GetAvailableSlotsForDoctor(doctorId);

            var currentDate = DateTime.Today;
            var availableDates = new List<DateTime>();
            var availableSlots = new Dictionary<string, List<string>>(); // Change to string for proper JSON conversion

            // Loop over each availability day of the week and calculate available dates and times within the current month
            foreach (var availability in availableTimeSlots)
            {
                if (Enum.TryParse<DayOfWeek>(availability.DayOfWeek, true, out var dayOfWeekEnum))
                {
                    // Find all dates for this specific DayOfWeek in the current month
                    for (int day = 1; day <= DateTime.DaysInMonth(currentDate.Year, currentDate.Month); day++)
                    {
                        var potentialDate = new DateTime(currentDate.Year, currentDate.Month, day);
                        if (potentialDate.DayOfWeek == dayOfWeekEnum)
                        {
                            availableDates.Add(potentialDate); // Add the available date to the list

                            // Generate time slots for that day
                            var startTime = availability.StartTime;
                            var endTime = availability.EndTime;
                            var slotsForDay = new List<string>();

                            while (startTime.Add(TimeSpan.FromMinutes(45)) <= endTime)
                            {
                                slotsForDay.Add(startTime.ToString(@"hh\:mm")); // Format time as string for easy display
                                startTime = startTime.Add(TimeSpan.FromMinutes(45)); // Increment by 45 minutes
                            }

                            availableSlots[potentialDate.ToString("yyyy-MM-dd")] = slotsForDay; // Map date to time slots
                        }
                    }
                }
            }

            var model = new BookAppointmentViewModel
            {
                DoctorId = doctor.DoctorId,
                DoctorFullName = doctor.User.FullName,
                AvailableDates = availableDates,
                AvailableSlots = availableSlots // Pass the time slots to the view
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> ConfirmAppointment(int doctorId, string appointmentDate, string appointmentTime)
        {
            var patientId = GetCurrentUserId(); // Get the logged-in patient's ID

            // Try to parse the full date and time
            if (!DateTime.TryParseExact($"{appointmentDate} {appointmentTime}", "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime appointmentDateTime))
            {
                return BadRequest("Invalid date or time format.");
            }

            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentDate = appointmentDateTime, // Use the parsed DateTime
                Status = "Booked",
                CreatedAt = DateTime.Now,
                Notes = "" // You can add a notes field in the form if needed
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveAsync();

            TempData["SuccessMessage"] = "Appointment successfully booked!";
            return RedirectToAction("Dashboard");
        }





        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> ViewDoctors()
        {
            var doctors = await _doctorRepository.GetAllAsync(); // Fetch all doctors

            var viewModel = new ViewDoctorsViewModel
            {
                Doctors = new List<DoctorWithAppointmentsViewModel>()
            };

            foreach (var doctor in doctors)
            {
                // Fetch available time slots for the doctor
                var availableTimeSlots = await _doctorAvailabilityRepository.GetAvailableSlotsForDoctor(doctor.DoctorId);

                // Fetch available appointments for the doctor
                var availableAppointments = await _appointmentRepository.GetAvailableAppointmentsForDoctor(doctor.DoctorId)
                                            ?? new List<Appointment>();

                var doctorWithAppointments = new DoctorWithAppointmentsViewModel
                {
                    DoctorId = doctor.DoctorId,
                    DoctorFullName = doctor.User.FullName,
                    Specialty = doctor.Department?.DepartmentName ?? "General", // Assuming Department represents specialty
                    AvailableTimeSlots = availableTimeSlots.Select(a => new DoctorAvailabilityViewModel
                    {
                        DayOfWeek = a.DayOfWeek,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime
                    }).ToList(),
                    AvailableAppointments = availableAppointments.Select(a => new AppointmentViewModel
                    {
                        AppointmentId = a.AppointmentId,
                        AppointmentDate = a.AppointmentDate,
                        Status = a.Status
                    }).ToList()
                };

                viewModel.Doctors.Add(doctorWithAppointments);
            }

            return View(viewModel);
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
                // This is now nullable
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                DoctorFullName = a.Doctor.User.FullName,
                PatientFullName = a.Patient != null ? a.Patient.User.FullName : "Not Assigned",  // Handle null Patient
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
