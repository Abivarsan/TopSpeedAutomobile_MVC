using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Web.Data;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Controllers
{
    public class InquiryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public InquiryController(ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Book(Guid? vehicleId)
        {
            var inquiry = new Inquiry
            {
                PreferredDate = DateTime.Today.AddDays(2)
            };

            if (vehicleId.HasValue)
            {
                var vehicle = await _dbContext.Vehicles
                    .Include(v => v.Brand)
                    .FirstOrDefaultAsync(v => v.Id == vehicleId.Value);

                if (vehicle != null)
                {
                    inquiry.VehicleId = vehicle.Id;
                    ViewBag.Vehicle = vehicle;
                }
            }

            return View(inquiry);
        }

        [HttpPost]
        public async Task<IActionResult> Book(Inquiry inquiry)
        {
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                inquiry.Id = Guid.NewGuid();
                inquiry.CreatedAt = DateTime.UtcNow;
                inquiry.Status = "Pending";

                _dbContext.Inquiries.Add(inquiry);
                await _dbContext.SaveChangesAsync();

                // Send confirmation email to customer
                try
                {
                    string vehicleName = "an exclusive vehicle";
                    if (inquiry.VehicleId.HasValue)
                    {
                        var vehicle = await _dbContext.Vehicles.Include(v => v.Brand).FirstOrDefaultAsync(v => v.Id == inquiry.VehicleId.Value);
                        if (vehicle != null)
                        {
                            vehicleName = $"{vehicle.Brand?.Name} {vehicle.Name}";
                        }
                    }

                    string subject = "TopSpeed Automobile - Test Drive Request Received";
                    string message = $@"
                        <h2>Test Drive Request Confirmed</h2>
                        <p>Dear {inquiry.Name},</p>
                        <p>Thank you for your interest in <strong>{vehicleName}</strong> at TopSpeed Automobile.</p>
                        <p>We have received your appointment request for <strong>{inquiry.PreferredDate:MMMM dd, yyyy}</strong>.</p>
                        <p>Our concierge team will contact you shortly at {inquiry.Phone ?? inquiry.Email} to finalize your personalized track experience.</p>
                        <br/>
                        <p>Best regards,<br/>The TopSpeed Automobile Concierge Team</p>";

                    await _emailSender.SendEmailAsync(inquiry.Email, subject, message);
                }
                catch
                {
                    // Fail silently for email sending in dev if network offline
                }

                TempData["Success"] = "Your test drive request has been submitted! Our concierge team will contact you shortly.";
                if (inquiry.VehicleId.HasValue)
                {
                    return RedirectToAction("Details", "Vehicle", new { id = inquiry.VehicleId.Value });
                }
                return RedirectToAction("Index", "Vehicle");
            }

            if (inquiry.VehicleId.HasValue)
            {
                ViewBag.Vehicle = await _dbContext.Vehicles.Include(v => v.Brand).FirstOrDefaultAsync(v => v.Id == inquiry.VehicleId.Value);
            }

            return View(inquiry);
        }
    }
}
