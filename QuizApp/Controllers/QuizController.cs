// ============================================================
// FILE: Controllers/QuizController.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// MODULE 4: Take Quiz and View Result
// DESCRIPTION: Handles the quiz-taking flow for Students:
//                1. Load a quiz with all its questions (Take)
//                2. Evaluate submitted answers and save score (Submit)
//                3. Display the result summary after submission (Result)
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Data;
using QuizApp.MVC.Models;
using System.Security.Claims;

namespace QuizApp.MVC.Controllers
{
    [Authorize] // All actions require login
    public class QuizController : Controller
    {
        private readonly QuizDbContext _context;

        public QuizController(QuizDbContext context)
        {
            _context = context;
        }

        // ── GET: /Quiz/Take/{id} ───────────────────────────────
        // Loads the quiz with all its questions.
        // Renders the quiz form where the student selects answers.
        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions) // Load all questions for this quiz
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

        // ── POST: /Quiz/Submit ─────────────────────────────────
        // Receives the student's answers from the form via IFormCollection.
        // Each answer is posted as: question_{questionId} = "A" | "B" | "C" | "D"
        // Calculates the score, saves the result, then redirects to Result page.
        [HttpPost]
        public async Task<IActionResult> Submit(int quizId, IFormCollection form)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            // Compare each submitted answer with the stored correct answer
            int score = 0;
            foreach (var question in quiz.Questions)
            {
                // The form field name for each question is "question_{id}"
                var answer = form[$"question_{question.Id}"].ToString();
                if (answer == question.CorrectOption)
                    score++;
            }

            // Get the logged-in student's ID from the auth cookie
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Save the result to the database
            var result = new TestResult
            {
                UserId = userId,
                QuizId = quizId,
                Score = score,
                TotalQuestions = quiz.Questions.Count
            };

            _context.TestResults.Add(result);
            await _context.SaveChangesAsync();

            // Redirect to the result summary page
            return RedirectToAction("Result", new { id = result.Id });
        }

        // ── GET: /Quiz/Result/{id} ─────────────────────────────
        // Loads a specific TestResult by its ID.
        // Students can only view their own results.
        // Admins can view any result.
        public async Task<IActionResult> Result(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _context.TestResults
                .Include(r => r.Quiz)  // Needed to display the quiz title
                .FirstOrDefaultAsync(r => r.Id == id &&
                    (r.UserId == userId || User.IsInRole("Admin")));

            if (result == null)
                return NotFound();

            return View(result);
        }
    }
}
