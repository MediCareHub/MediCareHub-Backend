using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MediCareHub.DAL.Data.Configurations;

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
                                          DoctorId = a.DoctorId,
                                          PatientId = a.PatientId,
                                          AppointmentDate = a.AppointmentDate,
                                          Status = a.Status,
                                          Notes = a.Notes,
                                          CreatedAt = a.CreatedAt
                                      }).ToList();

            return View(appointments);
        }

        public IActionResult Create()
        {
            return View();
        }


    }
}
