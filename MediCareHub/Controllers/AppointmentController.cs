using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;

namespace MediCareHub.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var appointments = _context.Appointments
                                      .Select(a => new AppointmentViewModel
                                      {
                                          AppointmentId = a.AppointmentId,
                                          //DoctorId = a.DoctorId,
                                          PatientId = a.PatientId,
                                          AppointmentDate = a.AppointmentDate,
                                          Status = a.Status,
                                          Notes = a.Notes,
                                          //CreatedAt = a.CreatedAt
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
                                          //DoctorId = a.DoctorId,
                                          PatientId = a.PatientId,
                                          AppointmentDate = a.AppointmentDate,
                                          Status = a.Status,
                                          Notes = a.Notes,
                                          //CreatedAt = a.CreatedAt,
                                          DoctorFullName = a.Doctor.User.FullName,
                                          PatientFullName = a.Patient.User.FullName
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
                                          DoctorFullName = a.Doctor.User.FullName,
                                          PatientFullName = a.Patient.User.FullName
                                      }).FirstOrDefault();

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentViewModel model)
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
            if (ModelState.IsValid)
            {
                var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                if (appointment == null)
                {
                    return NotFound();
                }

                //appointment.DoctorId = model.DoctorId;
                appointment.PatientId = model.PatientId;
                //appointment.AppointmentDate = model.AppointmentDate;
                appointment.Status = model.Status;
                appointment.Notes = model.Notes;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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





    }
}
