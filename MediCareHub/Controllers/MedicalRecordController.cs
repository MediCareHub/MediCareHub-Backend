using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediCareHub.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDoctorRepository _doctorRepository;

        public MedicalRecordController(AppDbContext context, IDoctorRepository doctorRepository)
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
                return NotFound(); // Handle case when doctor is not found
            }

            // Fetch medical records for appointments related to the current doctor
            var medicalRecords = await _context.MedicalRecords
                .Where(m => m.Appointment.DoctorId == doctor.DoctorId) // Ensure this relationship exists
                .Select(m => new MedicalRecordViewModel
                {
                    RecordId = m.RecordId,
                    AppointmentId = m.AppointmentId,
                    Diagnosis = m.Diagnosis,
                    Medication = m.Medication,
                    CreatedAt = m.CreatedAt,
                    PatientFullName = m.Appointment.Patient.User.FullName
                }).ToListAsync();

            return View(medicalRecords);
        }

        private int GetCurrentUserId()
        {
            // Retrieve the current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
        public IActionResult Create(int appointmentId)
        {
            var model = new MedicalRecordViewModel
            {
                AppointmentId = appointmentId
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalRecordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new MedicalRecord
                {
                    AppointmentId = model.AppointmentId,
                    Diagnosis = model.Diagnosis,
                    Medication = model.Medication,
                    CreatedAt = DateTime.Now
                };

                _context.MedicalRecords.Add(record);
                _context.SaveChanges();
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var record = _context.MedicalRecords
                .Where(r => r.RecordId == id)
                .Select(r => new MedicalRecordViewModel
                {
                    RecordId = r.RecordId,
                    AppointmentId = r.AppointmentId,
                    Diagnosis = r.Diagnosis,
                    Medication = r.Medication,
                    CreatedAt = r.CreatedAt
                }).FirstOrDefault();

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        public IActionResult Edit(int id)
        {
            var record = _context.MedicalRecords
                .Where(r => r.RecordId == id)
                .Select(r => new MedicalRecordViewModel
                {
                    RecordId = r.RecordId,
                    AppointmentId = r.AppointmentId,
                    Diagnosis = r.Diagnosis,
                    Medication = r.Medication,
                    CreatedAt = r.CreatedAt
                }).FirstOrDefault();

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalRecordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = _context.MedicalRecords.FirstOrDefault(r => r.RecordId == id);
                if (record == null)
                {
                    return NotFound();
                }

                // Update the medical record fields
                record.AppointmentId = model.AppointmentId;
                record.Diagnosis = model.Diagnosis;
                record.Medication = model.Medication;

                _context.SaveChanges();
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var record = _context.MedicalRecords
                .Where(r => r.RecordId == id)
                .Select(r => new MedicalRecordViewModel
                {
                    RecordId = r.RecordId,
                    AppointmentId = r.AppointmentId,
                    Diagnosis = r.Diagnosis,
                    Medication = r.Medication,
                    CreatedAt = r.CreatedAt
                }).FirstOrDefault();

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var record = _context.MedicalRecords.FirstOrDefault(r => r.RecordId == id);
            if (record == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(record);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
