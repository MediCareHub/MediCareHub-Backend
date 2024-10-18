using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
