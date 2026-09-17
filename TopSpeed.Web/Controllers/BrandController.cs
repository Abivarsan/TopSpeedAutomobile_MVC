using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TopSpeed.Web.Data;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Brand> brands = await _dbContext.Brand.Include(b => b.Vehicles).ToListAsync();
            return View(brands);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(webRootPath, @"images\brand");
                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

                var extension = Path.GetExtension(file[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    await file[0].CopyToAsync(fileStream);
                }

                brand.BrandLogo = @"/images/brand/" + newFileName + extension;
            }

            ModelState.Remove("BrandLogo");
            ModelState.Remove("Vehicles");

            if (ModelState.IsValid)
            {
                _dbContext.Brand.Add(brand);
                await _dbContext.SaveChangesAsync();

                TempData["Success"] = "Brand added successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Brand? brand = await _dbContext.Brand
                .Include(b => b.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            Brand? brand = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;

            var objFromDb = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == brand.Id);
            if (objFromDb == null)
            {
                return NotFound();
            }

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(webRootPath, @"images\brand");
                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

                var extension = Path.GetExtension(file[0].FileName);

                // Delete old image if custom
                if (!string.IsNullOrEmpty(objFromDb.BrandLogo) && !objFromDb.BrandLogo.StartsWith("http"))
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.TrimStart('/', '\\').Replace('/', '\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try { System.IO.File.Delete(oldImagePath); } catch { }
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    await file[0].CopyToAsync(fileStream);
                }

                objFromDb.BrandLogo = @"/images/brand/" + newFileName + extension;
            }

            ModelState.Remove("BrandLogo");
            ModelState.Remove("Vehicles");

            if (ModelState.IsValid)
            {
                objFromDb.Name = brand.Name;
                objFromDb.EstablishedYear = brand.EstablishedYear;
                objFromDb.Country = brand.Country;
                objFromDb.Founder = brand.Founder;
                objFromDb.Headquarters = brand.Headquarters;
                objFromDb.Website = brand.Website;
                objFromDb.Description = brand.Description;

                _dbContext.Brand.Update(objFromDb);
                await _dbContext.SaveChangesAsync();

                TempData["Warning"] = "Brand updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Brand? brand = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var objFromDb = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == brand.Id);

            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(objFromDb.BrandLogo) && !objFromDb.BrandLogo.StartsWith("http"))
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.TrimStart('/', '\\').Replace('/', '\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try { System.IO.File.Delete(oldImagePath); } catch { }
                    }
                }

                _dbContext.Brand.Remove(objFromDb);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "Brand deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var brands = await _dbContext.Brand.Include(b => b.Vehicles).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,EstablishedYear,Country,Founder,Headquarters,Website,TotalVehicles");

            foreach (var b in brands)
            {
                string escape(string? val) => $"\"{val?.Replace("\"", "\"\"") ?? ""}\"";
                sb.AppendLine($"{b.Id},{escape(b.Name)},{b.EstablishedYear},{escape(b.Country)},{escape(b.Founder)},{escape(b.Headquarters)},{escape(b.Website)},{b.Vehicles?.Count ?? 0}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", $"TopSpeed_Brands_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}
