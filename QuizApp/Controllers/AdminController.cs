// ============================================================
// FILE: Controllers/AdminController.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// MODULE 3: Admin Dashboard — Manage Tests & View Results
// DESCRIPTION: Restricted to users with Role = "Admin".
//              Allows Admins to:
//                1. View all published quizzes (Index)
//                2. Create a new quiz with questions (CreateQuiz)
//                3. View all student results in a table (Results)
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Data;
using QuizApp.MVC.Models;
using System.Security.Claims;

namespace QuizApp.MVC.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admins can access any action here
    public class AdminController : Controller
    {
        private readonly QuizDbContext _context;

        public AdminController(QuizDbContext context)
        {
            _context = context;
        }

        // ── GET: /Admin ────────────────────────────────────────
        // Loads all quizzes with their questions and creator info.
        // Displayed as cards on the Manage Tests page.
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)   // Needed to show question count on card
                .Include(q => q.CreatedBy)   // Needed to show creator name on card
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return View(quizzes);
        }

        // ── GET: /Admin/CreateQuiz ─────────────────────────────
        // Renders the blank Create Quiz form with one default question block.
        [HttpGet]
        public IActionResult CreateQuiz()
        {
            return View(new CreateQuizViewModel());
        }

        // ── POST: /Admin/CreateQuiz ────────────────────────────
        // Receives the form data (title, description, questions list),
        // saves the quiz and all its questions to the database,
        // then redirects back to the Manage Tests page.
        [HttpPost]
        public async Task<IActionResult> CreateQuiz(CreateQuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in Admin's ID
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // Build the Quiz entity from the ViewModel
                var quiz = new Quiz
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedById = userId,

                    // Map each QuestionViewModel to a Question entity
                    Questions = model.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        OptionA = q.OptionA,
                        OptionB = q.OptionB,
                        OptionC = q.OptionC,
                        OptionD = q.OptionD,
                        CorrectOption = q.CorrectOption   // "A", "B", "C", or "D"
                    }).ToList()
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // If validation fails, return the form with error messages
            return View(model);
        }

        // ── GET: /Admin/Results ────────────────────────────────
        // Loads all student test results with their user and quiz info.
        // Displayed in a sortable table on the All Results page.
        public async Task<IActionResult> Results()
        {
            var results = await _context.TestResults
                .Include(r => r.User)   // Needed to show student name and email
                .Include(r => r.Quiz)   // Needed to show quiz title
                .OrderByDescending(r => r.DateTaken)
                .ToListAsync();

            return View(results);
        }
    }
}
