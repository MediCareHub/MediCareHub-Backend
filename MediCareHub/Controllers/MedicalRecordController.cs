using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCareHub.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var records = _context.MedicalRecords
                .Select(r => new MedicalRecordViewModel
                {
                    RecordId = r.RecordId,
                    AppointmentId = r.AppointmentId,
                    Diagnosis = r.Diagnosis,
                    Medication = r.Medication,
                    CreatedAt = r.CreatedAt
                }).ToList();

            return View(records);
        }
        public IActionResult Create(int appointmentId)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient) 
                .ThenInclude(p => p.User)
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = new MedicalRecordViewModel
            {
                AppointmentId = appointmentId,
                PatientFullName = appointment.Patient.User.FullName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

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
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
                .ThenInclude(p => p.User) 
                .FirstOrDefault(r => r.RecordId == id);

            if (record == null)
            {
                return NotFound();
            }

            var model = new MedicalRecordViewModel
            {
                RecordId = record.RecordId,
                AppointmentId = record.AppointmentId,
                Diagnosis = record.Diagnosis,
                Medication = record.Medication,
                CreatedAt = record.CreatedAt,
                PatientFullName = record.Appointment.Patient.User.FullName
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Output any validation errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

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