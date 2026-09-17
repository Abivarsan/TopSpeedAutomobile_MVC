using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TopSpeed.Web.Data;

namespace TopSpeed.Web.Controllers
{
    [Authorize]
    public class GarageController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public GarageController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var garageItems = await _dbContext.UserGarageItems
                .Include(g => g.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.AddedAt)
                .ToListAsync();

            ViewBag.TotalHorsepower = garageItems.Sum(g => g.Vehicle?.Horsepower ?? 0);
            ViewBag.TotalValue = garageItems.Sum(g => g.Vehicle?.Price ?? 0m);
            ViewBag.MaxSpeed = garageItems.Any() ? garageItems.Max(g => g.Vehicle?.TopSpeed ?? 0) : 0;
            ViewBag.Avg0To100 = garageItems.Any() ? garageItems.Average(g => g.Vehicle?.Acceleration ?? 0) : 0;

            return View(garageItems);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _dbContext.UserGarageItems
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (item != null)
            {
                _dbContext.UserGarageItems.Remove(item);
                await _dbContext.SaveChangesAsync();
                TempData["Warning"] = "Vehicle removed from your Virtual Garage.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
