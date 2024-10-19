using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediCareHub.DAL.Models;

namespace MediCareHub.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository, ILogger<AppointmentController> logger)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var doctor = await _doctorRepository.GetByUserId(userId);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction("Error");
            }

            var appointments = await _appointmentRepository.GetAppointmentsForDoctorAsync(doctor.DoctorId);

            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientFullName = a.Patient.User.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            }).ToList();

            return View(appointmentViewModels);
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    AppointmentDate = model.AppointmentDate,
                    Status = model.Status,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now
                };

                await _appointmentRepository.AddAsync(appointment);
                await _appointmentRepository.SaveAsync();

                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }

            LogValidationErrors();
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepository.GetallAsync(id);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Error");
            }

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorFullName = appointment.Doctor.User.FullName,
                PatientFullName = appointment.Patient.User.FullName,

            };

            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepository.GetallAsync(id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorFullName = appointment.Doctor.User.FullName,
                PatientFullName = appointment.Patient.User.FullName,

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return View(model);
            }

            var appointment = await _appointmentRepository.GetallAsync(id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            appointment.Status = model.Status;
            appointment.Notes = model.Notes;

            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveAsync();

            TempData["SuccessMessage"] = "Appointment updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepository.GetallAsync(id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentRepository.GetallAsync(id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            _appointmentRepository.Delete(appointment);
            await _appointmentRepository.SaveAsync();

            TempData["SuccessMessage"] = "Appointment deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private void LogValidationErrors()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogError("Validation Errors: " + string.Join(", ", errors));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
