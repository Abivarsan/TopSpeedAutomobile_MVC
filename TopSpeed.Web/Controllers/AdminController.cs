using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TopSpeed.Web.Data;

namespace TopSpeed.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var totalBrands = await _dbContext.Brand.CountAsync();
            var totalVehicles = await _dbContext.Vehicles.CountAsync();
            var totalInquiries = await _dbContext.Inquiries.CountAsync();
            var pendingInquiries = await _dbContext.Inquiries.CountAsync(i => i.Status == "Pending");

            var fastestCar = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .OrderByDescending(v => v.TopSpeed)
                .FirstOrDefaultAsync();

            var mostPowerfulCar = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .OrderByDescending(v => v.Horsepower)
                .FirstOrDefaultAsync();

            // Aggregate Chart Data: Vehicles count per brand
            var brandDistribution = await _dbContext.Brand
                .Select(b => new
                {
                    b.Name,
                    Count = b.Vehicles != null ? b.Vehicles.Count : 0
                })
                .Where(b => b.Count > 0)
                .ToListAsync();

            // Aggregate Chart Data: Top 5 most powerful vehicles
            var topHpVehicles = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .OrderByDescending(v => v.Horsepower)
                .Take(5)
                .Select(v => new
                {
                    Name = $"{v.Brand.Name} {v.Name}",
                    v.Horsepower,
                    v.TopSpeed
                })
                .ToListAsync();

            ViewBag.TotalBrands = totalBrands;
            ViewBag.TotalVehicles = totalVehicles;
            ViewBag.TotalInquiries = totalInquiries;
            ViewBag.PendingInquiries = pendingInquiries;
            ViewBag.FastestCar = fastestCar;
            ViewBag.MostPowerfulCar = mostPowerfulCar;

            ViewBag.BrandDistributionLabels = JsonSerializer.Serialize(brandDistribution.Select(b => b.Name));
            ViewBag.BrandDistributionData = JsonSerializer.Serialize(brandDistribution.Select(b => b.Count));

            ViewBag.TopHpLabels = JsonSerializer.Serialize(topHpVehicles.Select(v => v.Name));
            ViewBag.TopHpData = JsonSerializer.Serialize(topHpVehicles.Select(v => v.Horsepower));

            ViewBag.RecentInquiries = await _dbContext.Inquiries
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Inquiries()
        {
            var inquiries = await _dbContext.Inquiries
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(inquiries);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInquiryStatus(Guid id, string status)
        {
            var inquiry = await _dbContext.Inquiries.FirstOrDefaultAsync(i => i.Id == id);
            if (inquiry != null)
            {
                inquiry.Status = status;
                await _dbContext.SaveChangesAsync();
                TempData["Success"] = $"Inquiry for {inquiry.Name} marked as {status}.";
            }

            return RedirectToAction(nameof(Inquiries));
        }
    }
}
