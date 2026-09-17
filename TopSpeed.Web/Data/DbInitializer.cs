using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Assign Admin Role to primary user if exists
            var adminUser = await userManager.FindByEmailAsync("ketheeswaranabivarsan@gmail.com");
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3. Seed Sample Brands if empty
            if (!await context.Brand.AnyAsync())
            {
                var ferrari = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Ferrari",
                    EstablishedYear = 1947,
                    BrandLogo = "/images/brand/ferrari.png",
                    Country = "Italy",
                    Founder = "Enzo Ferrari",
                    Headquarters = "Maranello, Italy",
                    Website = "https://www.ferrari.com",
                    Description = "Ferrari is an Italian luxury sports car manufacturer based in Maranello, Italy. Founded by Enzo Ferrari in 1939 out of Alfa Romeo's race division as Auto Avio Costruzioni, the company built its first car in 1940 and produced its first Ferrari-badged car in 1947."
                };

                var porsche = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Porsche",
                    EstablishedYear = 1931,
                    BrandLogo = "/images/brand/porsche.png",
                    Country = "Germany",
                    Founder = "Ferdinand Porsche",
                    Headquarters = "Stuttgart, Germany",
                    Website = "https://www.porsche.com",
                    Description = "Dr. Ing. h.c. F. Porsche AG, usually shortened to Porsche, is a German automobile manufacturer specializing in high-performance sports cars, SUVs and sedans, headquartered in Stuttgart, Baden-Württemberg, Germany."
                };

                var lamborghini = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Lamborghini",
                    EstablishedYear = 1963,
                    BrandLogo = "/images/brand/lamborghini.png",
                    Country = "Italy",
                    Founder = "Ferruccio Lamborghini",
                    Headquarters = "Sant'Agata Bolognese, Italy",
                    Website = "https://www.lamborghini.com",
                    Description = "Automobili Lamborghini S.p.A. is an Italian manufacturer of luxury sports cars and SUVs based in Sant'Agata Bolognese. The company is owned by the Volkswagen Group through its subsidiary Audi."
                };

                var mclaren = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "McLaren",
                    EstablishedYear = 1963,
                    BrandLogo = "/images/brand/mclaren.png",
                    Country = "United Kingdom",
                    Founder = "Bruce McLaren",
                    Headquarters = "Woking, United Kingdom",
                    Website = "https://cars.mclaren.com",
                    Description = "McLaren Automotive is a British luxury automotive manufacturer based at the McLaren Technology Centre in Woking, England. The company produces supercars, which are produced at the purpose-built McLaren Production Centre."
                };

                var nissan = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Nissan",
                    EstablishedYear = 1933,
                    BrandLogo = "/images/brand/nissan.png",
                    Country = "Japan",
                    Founder = "Yoshisuke Aikawa",
                    Headquarters = "Yokohama, Japan",
                    Website = "https://www.nissan-global.com",
                    Description = "Nissan Motor Co., Ltd. is a Japanese multinational automobile manufacturer headquartered in Nishi-ku, Yokohama, Japan. The company produces the legendary GT-R supercar and high-performance Nismo vehicles."
                };

                context.Brand.AddRange(ferrari, porsche, lamborghini, mclaren, nissan);
                await context.SaveChangesAsync();

                // 4. Seed Vehicles
                var vehicles = new List<Vehicle>
                {
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "SF90 Stradale",
                        BrandId = ferrari.Id,
                        VehicleType = "Hypercar",
                        Year = 2024,
                        Horsepower = 986,
                        Acceleration = 2.5,
                        TopSpeed = 340,
                        Price = 528000m,
                        Engine = "4.0L Twin-Turbo V8 PHEV (Tri-Motor)",
                        Transmission = "8-Speed Dual-Clutch (F1 DCT)",
                        ImageUrl = "/images/vehicles/sf90.png",
                        Description = "The SF90 Stradale is the first ever Ferrari to feature PHEV (Plug-in Hybrid Electric Vehicle) architecture, which sees the internal combustion engine integrated with three electric motors producing 986 HP."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "296 GTB",
                        BrandId = ferrari.Id,
                        VehicleType = "Supercar",
                        Year = 2023,
                        Horsepower = 819,
                        Acceleration = 2.9,
                        TopSpeed = 330,
                        Price = 338000m,
                        Engine = "3.0L Twin-Turbo 120-deg V6 Hybrid",
                        Transmission = "8-Speed Dual-Clutch",
                        ImageUrl = "/images/vehicles/296gtb.png",
                        Description = "The 296 GTB redefines the whole concept of fun behind the wheel, guaranteeing pure excitement not just when pushing the car to its limits, but in day-to-day driving situations."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "911 GT3 RS",
                        BrandId = porsche.Id,
                        VehicleType = "Supercar",
                        Year = 2024,
                        Horsepower = 518,
                        Acceleration = 3.2,
                        TopSpeed = 296,
                        Price = 223800m,
                        Engine = "4.0L Naturally Aspirated Boxer 6",
                        Transmission = "7-Speed Porsche Doppelkupplung (PDK)",
                        ImageUrl = "/images/vehicles/911gt3rs.png",
                        Description = "The Porsche 911 GT3 RS is motorsport technology brought to the road. Features active aerodynamics, DRS (Drag Reduction System), and an atmospheric flat-six revving to 9,000 RPM."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "918 Spyder",
                        BrandId = porsche.Id,
                        VehicleType = "Hypercar",
                        Year = 2015,
                        Horsepower = 875,
                        Acceleration = 2.6,
                        TopSpeed = 345,
                        Price = 845000m,
                        Engine = "4.6L Naturally Aspirated V8 Hybrid",
                        Transmission = "7-Speed PDK Dual-Clutch",
                        ImageUrl = "/images/vehicles/918.png",
                        Description = "The Holy Trinity pioneer. Mid-engine plug-in hybrid hypercar powered by a naturally aspirated 4.6-liter V8 engine developing 608 HP, with two electric motors delivering an additional 279 HP."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "Revuelto",
                        BrandId = lamborghini.Id,
                        VehicleType = "Hypercar",
                        Year = 2024,
                        Horsepower = 1001,
                        Acceleration = 2.5,
                        TopSpeed = 350,
                        Price = 608000m,
                        Engine = "6.5L Naturally Aspirated V12 + 3 Electric Motors",
                        Transmission = "8-Speed Dual-Clutch Transverse",
                        ImageUrl = "/images/vehicles/revuelto.png",
                        Description = "The first High Performance Electrified Vehicle (HPEV) hybrid super sports car from Sant'Agata Bolognese, combining an all-new 12-cylinder engine with hybrid technology."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "750S",
                        BrandId = mclaren.Id,
                        VehicleType = "Supercar",
                        Year = 2024,
                        Horsepower = 740,
                        Acceleration = 2.8,
                        TopSpeed = 332,
                        Price = 329500m,
                        Engine = "4.0L Twin-Turbo V8 (M840T)",
                        Transmission = "7-Speed Seamless Shift Gearbox (SSG)",
                        ImageUrl = "/images/vehicles/750s.png",
                        Description = "Lighter, more powerful, and with even greater agility, the McLaren 750S elevates supercar performance with a carbon fibre monocoque and active rear wing."
                    },
                    new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Name = "GT-R Nismo",
                        BrandId = nissan.Id,
                        VehicleType = "Sports Coupe",
                        Year = 2024,
                        Horsepower = 600,
                        Acceleration = 2.7,
                        TopSpeed = 330,
                        Price = 221000m,
                        Engine = "3.8L Twin-Turbo V6 (VR38DETT)",
                        Transmission = "6-Speed Dual-Clutch AWD (ATTESA E-TS)",
                        ImageUrl = "/images/vehicles/gtr.png",
                        Description = "Nicknamed 'Godzilla', the GT-R Nismo is a race-bred legend with carbon-ceramic brakes, turbochargers derived from GT3 racing, and world-beating all-wheel-drive traction."
                    }
                };

                context.Vehicles.AddRange(vehicles);
                await context.SaveChangesAsync();
            }
        }
    }
}
