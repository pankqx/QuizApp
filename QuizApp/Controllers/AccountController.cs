// ============================================================
// FILE: Controllers/AccountController.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// MODULE 1: Login, Registration, and Logout
// DESCRIPTION: Handles user authentication using cookie-based
//              auth. Supports two roles: Admin and Student.
// ============================================================

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Data;
using QuizApp.MVC.Models;
using System.Security.Claims;

namespace QuizApp.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuizDbContext _context;

        public AccountController(QuizDbContext context)
        {
            _context = context;
        }

        // ── GET: /Account/Login ────────────────────────────────
        // Renders the login form (Views/Account/Login.cshtml)
        [HttpGet]
        public IActionResult Login() => View();

        // ── POST: /Account/Login ───────────────────────────────
        // Validates credentials, creates an auth cookie, and
        // redirects user based on their role.
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Look up user by email and plain-text password match
                // NOTE: In production, compare using BCrypt hash instead
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email
                                           && u.PasswordHash == model.Password);

                if (user != null)
                {
                    // Build claims for the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)   // "Admin" or "Student"
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in and create the authentication cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect based on role
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "Admin");

                    return RedirectToAction("Index", "Home");
                }

                // Invalid credentials — show error on the form
                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }

        // ── GET: /Account/Register ─────────────────────────────
        // Renders the registration form (Views/Account/Register.cshtml)
        [HttpGet]
        public IActionResult Register() => View();

        // ── POST: /Account/Register ────────────────────────────
        // Creates a new user account (Student or Admin) and
        // redirects to the Login page after success.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email is already registered
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = model.Password, // NOTE: Hash this in production
                    Role = model.Role == "Admin" ? "Admin" : "Student"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Redirect to login after successful registration
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // ── POST: /Account/Logout ──────────────────────────────
        // Clears the authentication cookie and redirects to Login
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
