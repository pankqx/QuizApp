// ============================================================
// FILE: Controllers/HomeController.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// MODULE 2: Student Dashboard — Available Tests
// DESCRIPTION: Serves the Student home page which lists all
//              published quizzes. Shows score/progress bar if
//              the student has already completed a quiz.
//              Admin users are immediately redirected to their
//              own dashboard (/Admin).
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Data;
using System.Security.Claims;

namespace QuizApp.MVC.Controllers
{
    [Authorize] // All actions require login
    public class HomeController : Controller
    {
        private readonly QuizDbContext _context;

        public HomeController(QuizDbContext context)
        {
            _context = context;
        }

        // ── GET: /Home ─────────────────────────────────────────
        // Loads all available quizzes and the current student's
        // results, then passes them to the view.
        public async Task<IActionResult> Index()
        {
            // Admins should not see the student dashboard — redirect them
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            // Get the logged-in student's ID from auth cookie claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Load all quizzes with their questions (to show question count)
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            // Load this student's results to display scores on quiz cards
            var userResults = await _context.TestResults
                .Where(r => r.UserId == userId)
                .Include(r => r.Quiz)
                .ToListAsync();

            // Pass results via ViewBag so the view can match them per quiz
            ViewBag.Results = userResults;

            return View(quizzes);
        }

        // ── GET: /Home/Error ───────────────────────────────────
        // Generic error page shown when an unhandled exception
        // occurs in production (see Program.cs UseExceptionHandler).
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
