using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using System.Security.Claims;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace MediCareHub.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDoctorRepository _doctorRepository;

        public AppointmentController(AppDbContext context, IDoctorRepository doctorRepository)
        {
            _context = context;
            _doctorRepository = doctorRepository;
        }

        public async Task<IActionResult> Index() 
        {
            var userId = GetCurrentUserId();
            var doctor = await _doctorRepository.GetByUserId(userId);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointments = _context.Appointments
                .Where(a => a.DoctorId == doctor.DoctorId) 
                .Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientFullName = a.Patient.User.FullName,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status,
                    Notes = a.Notes,
                }).ToList();

            return View(appointments);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    //DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    AppointmentDate = model.AppointmentDate,
                    Status = model.Status,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var appointment = _context.Appointments
                .Where(a => a.AppointmentId == id)
                .Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status,
                    Notes = a.Notes,
                    DoctorFullName = a.Doctor.User.FullName,
                    PatientFullName = a.Patient.User.FullName,
                    MedicalRecord = _context.MedicalRecords
                        .Where(mr => mr.AppointmentId == a.AppointmentId)
                        .Select(mr => new MedicalRecordViewModel
                        {
                            RecordId = mr.RecordId,
                            AppointmentId = mr.AppointmentId,
                            Diagnosis = mr.Diagnosis,
                            Medication = mr.Medication,
                            CreatedAt = mr.CreatedAt
                        }).FirstOrDefault() // Get the latest medical record or remove if you want all records
                }).FirstOrDefault();

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient) 
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
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
                MedicalRecord = new MedicalRecordViewModel()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Update the appointment fields
            appointment.Status = model.Status;
            appointment.Notes = model.Notes;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments
                                      .Where(a => a.AppointmentId == id)
                                      .Select(a => new AppointmentViewModel
                                      {
                                          AppointmentId = a.AppointmentId,
                                          //DoctorId = a.DoctorId,
                                          PatientId = a.PatientId,
                                          AppointmentDate = a.AppointmentDate,
                                          Status = a.Status,
                                          Notes = a.Notes,
                                          //CreatedAt = a.CreatedAt
                                      }).FirstOrDefault();

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        private int GetCurrentUserId()
        {
            // Retrieve the current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }


    }

}

