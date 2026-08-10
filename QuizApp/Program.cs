// ============================================================
// FILE: Program.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// DESCRIPTION: Application entry point.
//              Configures services (DB, Auth, MVC) and the
//              HTTP middleware pipeline. Also auto-creates
//              database tables on startup.
// ============================================================

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Data;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Register MVC with Razor Views ──────────────────────
builder.Services.AddControllersWithViews();

// ── 2. Configure MySQL Database using Pomelo EF Core ──────
//       Connection string is loaded from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── 3. Configure Cookie-based Authentication ───────────────
//       Login/Logout/Access-denied paths are defined here.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Redirect if not logged in
        options.LogoutPath = "/Account/Logout";       // Logout endpoint
        options.AccessDeniedPath = "/Account/AccessDenied"; // If role is wrong
    });

var app = builder.Build();

// ── 4. Configure HTTP Request Pipeline ────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();      // Serves wwwroot (CSS, JS, images)

app.UseRouting();

app.UseAuthentication();   // Must come before UseAuthorization
app.UseAuthorization();    // Enforces [Authorize] attributes on controllers

// ── 5. Default Route: /{Controller}/{Action}/{id?} ─────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── 6. Auto-create DB tables on startup ────────────────────
//       EnsureCreated() creates tables if they don't exist.
//       Suitable for academic/demo use. Use Migrations in production.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
