using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Web.Data;
using TopSpeed.Web.Models;
using TopSpeed.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add the DbContext and Identity services.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity services with ApplicationUser and IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;  // Optional: Force email confirmation for account sign-in
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();  // Required for features like password reset tokens and Identity UI

// Register the EmailSender service
builder.Services.AddSingleton<IEmailSender, EmailSender>(); // Add EmailSender service

// Make sure you're not calling AddAuthentication more than once
// and avoid specifying the scheme here if it's already added by AddDefaultIdentity.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

//Password Validation
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Enable 30-day client-side caching for images and static assets to eliminate reload lag
        const int durationInSeconds = 60 * 60 * 24 * 30;
        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
            $"public,max-age={durationInSeconds}";
    }
});

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();  // Ensure this is added
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();  // Required if using Identity UI or Razor Pages for authentication

// Initialize and seed database roles and catalog data
await DbInitializer.SeedAsync(app.Services);

app.Run();
