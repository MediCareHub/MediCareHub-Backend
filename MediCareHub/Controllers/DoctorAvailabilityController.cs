using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories;
using MediCareHub.DAL.Repositories.Interfaces;
using MediCareHub.Models;
using MediCareHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MediCareHub.Controllers
{
    public class DoctorAvailabilityController : Controller
    {
        private readonly IDoctorAvailabilityRepository _repository;

        public DoctorAvailabilityController(IDoctorAvailabilityRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")] // Ensure that only doctors can access

        public IActionResult SelectAvailability()
        {
            var viewModel = new DoctorAvailabilityViewModel();
            return View("~/Views/Doctor/SelectAvailability.cshtml", viewModel); // Explicitly specify the path
        }

        [HttpPost]
        public IActionResult SaveAvailability(DoctorAvailabilityViewModel viewModel)
        {
            foreach (var slot in viewModel.SelectedSlots)
            {
                var availability = new DoctorAvailability
                {
                    DoctorId = viewModel.DoctorId,
                    DayOfWeek = viewModel.DayOfWeek,
                    StartTime = slot,
                    EndTime = slot.Add(TimeSpan.FromMinutes(45)) // Each slot is 45 minutes
                };

                _repository.AddAvailability(availability);
            }

            _repository.Save();
            return RedirectToAction("SelectAvailability"); // Redirect after saving
        }
    }
}
