using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using TopSpeed.Web.Data;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehicleController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? brandId, string? type, string? sort)
        {
            var query = _dbContext.Vehicles.Include(v => v.Brand).AsQueryable();

            if (!string.IsNullOrEmpty(brandId) && Guid.TryParse(brandId, out Guid parsedBrandId))
            {
                query = query.Where(v => v.BrandId == parsedBrandId);
                ViewBag.SelectedBrandId = parsedBrandId;
            }

            if (!string.IsNullOrEmpty(type) && type != "All")
            {
                query = query.Where(v => v.VehicleType == type);
                ViewBag.SelectedType = type;
            }

            query = sort switch
            {
                "speed_desc" => query.OrderByDescending(v => v.TopSpeed),
                "hp_desc" => query.OrderByDescending(v => v.Horsepower),
                "acc_asc" => query.OrderBy(v => v.Acceleration),
                "price_asc" => query.OrderBy(v => v.Price),
                "price_desc" => query.OrderByDescending(v => v.Price),
                "year_desc" => query.OrderByDescending(v => v.Year),
                _ => query.OrderByDescending(v => v.Horsepower)
            };

            var vehicles = await query.ToListAsync();
            ViewBag.Brands = await _dbContext.Brand.OrderBy(b => b.Name).ToListAsync();
            ViewBag.CurrentSort = sort;

            // Load user favorites if signed in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteIds = await _dbContext.UserGarageItems
                    .Where(g => g.UserId == userId)
                    .Select(g => g.VehicleId)
                    .ToListAsync();
                ViewBag.FavoriteIds = new HashSet<Guid>(favoriteIds);
            }
            else
            {
                ViewBag.FavoriteIds = new HashSet<Guid>();
            }

            return View(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var vehicle = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.IsFavorited = !string.IsNullOrEmpty(userId) &&
                await _dbContext.UserGarageItems.AnyAsync(g => g.UserId == userId && g.VehicleId == id);

            // Fetch similar vehicles by brand or type
            ViewBag.SimilarVehicles = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .Where(v => v.Id != id && (v.BrandId == vehicle.BrandId || v.VehicleType == vehicle.VehicleType))
                .Take(3)
                .ToListAsync();

            return View(vehicle);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.BrandList = new SelectList(await _dbContext.Brand.OrderBy(b => b.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(webRootPath, @"images\vehicles");
                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

                var extension = Path.GetExtension(file[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    await file[0].CopyToAsync(fileStream);
                }

                vehicle.ImageUrl = @"/images/vehicles/" + newFileName + extension;
            }

            ModelState.Remove("Brand");
            ModelState.Remove("GarageBookmarks");

            if (ModelState.IsValid)
            {
                _dbContext.Vehicles.Add(vehicle);
                await _dbContext.SaveChangesAsync();

                TempData["Success"] = $"Vehicle '{vehicle.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BrandList = new SelectList(await _dbContext.Brand.OrderBy(b => b.Name).ToListAsync(), "Id", "Name", vehicle.BrandId);
            return View(vehicle);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            ViewBag.BrandList = new SelectList(await _dbContext.Brand.OrderBy(b => b.Name).ToListAsync(), "Id", "Name", vehicle.BrandId);
            return View(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Vehicle vehicle)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;

            var objFromDb = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicle.Id);
            if (objFromDb == null)
            {
                return NotFound();
            }

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(webRootPath, @"images\vehicles");
                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

                var extension = Path.GetExtension(file[0].FileName);
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl) && !objFromDb.ImageUrl.StartsWith("http"))
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('/', '\\').Replace('/', '\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try { System.IO.File.Delete(oldImagePath); } catch { }
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    await file[0].CopyToAsync(fileStream);
                }

                objFromDb.ImageUrl = @"/images/vehicles/" + newFileName + extension;
            }

            ModelState.Remove("Brand");
            ModelState.Remove("GarageBookmarks");

            if (ModelState.IsValid)
            {
                objFromDb.Name = vehicle.Name;
                objFromDb.BrandId = vehicle.BrandId;
                objFromDb.VehicleType = vehicle.VehicleType;
                objFromDb.Year = vehicle.Year;
                objFromDb.Horsepower = vehicle.Horsepower;
                objFromDb.Acceleration = vehicle.Acceleration;
                objFromDb.TopSpeed = vehicle.TopSpeed;
                objFromDb.Price = vehicle.Price;
                objFromDb.Engine = vehicle.Engine;
                objFromDb.Transmission = vehicle.Transmission;
                objFromDb.Description = vehicle.Description;

                _dbContext.Vehicles.Update(objFromDb);
                await _dbContext.SaveChangesAsync();

                TempData["Warning"] = $"Vehicle '{vehicle.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BrandList = new SelectList(await _dbContext.Brand.OrderBy(b => b.Name).ToListAsync(), "Id", "Name", vehicle.BrandId);
            return View(vehicle);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vehicle = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Vehicle vehicle)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var objFromDb = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicle.Id);

            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl) && !objFromDb.ImageUrl.StartsWith("http"))
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('/', '\\').Replace('/', '\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try { System.IO.File.Delete(oldImagePath); } catch { }
                    }
                }

                _dbContext.Vehicles.Remove(objFromDb);
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "Vehicle deleted from catalog.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Compare(Guid? v1, Guid? v2)
        {
            var allVehicles = await _dbContext.Vehicles
                .Include(v => v.Brand)
                .OrderBy(v => v.Brand.Name)
                .ThenBy(v => v.Name)
                .ToListAsync();

            ViewBag.VehicleSelectList = allVehicles.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.Brand?.Name} {v.Name} ({v.Horsepower} HP)"
            }).ToList();

            Vehicle? vehicle1 = null;
            Vehicle? vehicle2 = null;

            if (v1.HasValue)
            {
                vehicle1 = allVehicles.FirstOrDefault(v => v.Id == v1.Value);
            }
            else if (allVehicles.Count > 0)
            {
                vehicle1 = allVehicles[0];
            }

            if (v2.HasValue)
            {
                vehicle2 = allVehicles.FirstOrDefault(v => v.Id == v2.Value);
            }
            else if (allVehicles.Count > 1)
            {
                vehicle2 = allVehicles[1];
            }

            ViewBag.Vehicle1 = vehicle1;
            ViewBag.Vehicle2 = vehicle2;

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(Guid vehicleId, string? returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var existing = await _dbContext.UserGarageItems
                .FirstOrDefaultAsync(g => g.UserId == userId && g.VehicleId == vehicleId);

            if (existing != null)
            {
                _dbContext.UserGarageItems.Remove(existing);
                await _dbContext.SaveChangesAsync();
                TempData["Warning"] = "Removed from your Virtual Garage.";
            }
            else
            {
                _dbContext.UserGarageItems.Add(new UserGarageItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    VehicleId = vehicleId,
                    AddedAt = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();
                TempData["Success"] = "Added to your Virtual Garage!";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var vehicles = await _dbContext.Vehicles.Include(v => v.Brand).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Id,Manufacturer,Model,Category,Year,Horsepower,Acceleration_0_100_Sec,TopSpeed_KMH,Price_USD,Engine,Transmission");

            foreach (var v in vehicles)
            {
                string escape(string? val) => $"\"{val?.Replace("\"", "\"\"") ?? ""}\"";
                sb.AppendLine($"{v.Id},{escape(v.Brand?.Name)},{escape(v.Name)},{escape(v.VehicleType)},{v.Year},{v.Horsepower},{v.Acceleration},{v.TopSpeed},{v.Price},{escape(v.Engine)},{escape(v.Transmission)}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", $"TopSpeed_Vehicles_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}
