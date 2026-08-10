// ============================================================
// FILE: Models/Models.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// DESCRIPTION: Contains all Domain Models and View Models
//              used across the application.
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.MVC.Models
{
    // ─────────────────────────────────────────────────────────
    // DOMAIN MODELS  (mapped to database tables via EF Core)
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a registered user of the application.
    /// Role is either "Admin" (teacher/host) or "Student".
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // "Admin" or "Student"
        public string Role { get; set; } = "Student";
    }

    /// <summary>
    /// Represents a Quiz (Test) created by an Admin.
    /// Each Quiz contains one or more Questions.
    /// </summary>
    public class Quiz
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Automatically set to current UTC time when created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key — links to the Admin who created this quiz
        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        // Navigation property — all questions belonging to this quiz
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    /// <summary>
    /// Represents a single multiple-choice question inside a Quiz.
    /// Has 4 options (A, B, C, D) and a correct answer stored as a letter.
    /// </summary>
    public class Question
    {
        public int Id { get; set; }

        // Foreign key — links this question to its parent Quiz
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required] public string OptionA { get; set; } = string.Empty;
        [Required] public string OptionB { get; set; } = string.Empty;
        [Required] public string OptionC { get; set; } = string.Empty;
        [Required] public string OptionD { get; set; } = string.Empty;

        // Stores the correct answer as: "A", "B", "C", or "D"
        [Required]
        public string CorrectOption { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stores the result after a Student completes a Quiz.
    /// Records score, total questions, and date/time taken.
    /// </summary>
    public class TestResult
    {
        public int Id { get; set; }

        // Foreign key — the student who took the test
        public int UserId { get; set; }
        public User? User { get; set; }

        // Foreign key — which quiz was taken
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public int Score { get; set; }           // Number of correct answers
        public int TotalQuestions { get; set; }  // Total questions in the quiz
        public DateTime DateTaken { get; set; } = DateTime.UtcNow;
    }


    // ─────────────────────────────────────────────────────────
    // VIEW MODELS  (used to transfer data between views/forms)
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// ViewModel for the Login form.
    /// Validates email format and required password.
    /// </summary>
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel for the Registration form.
    /// Allows selecting role: "Student" or "Admin".
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Defaults to "Student"; Admin can be selected on register page
        public string Role { get; set; } = "Student";
    }

    /// <summary>
    /// ViewModel for creating a new Quiz.
    /// Carries the title, description, and list of questions from the form.
    /// </summary>
    public class CreateQuizViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Dynamically populated via JavaScript on the Create Quiz page
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    /// <summary>
    /// Represents a single question entry within the CreateQuizViewModel.
    /// Each question carries its 4 options and the correct answer letter.
    /// </summary>
    public class QuestionViewModel
    {
        [Required] public string Text { get; set; } = string.Empty;
        [Required] public string OptionA { get; set; } = string.Empty;
        [Required] public string OptionB { get; set; } = string.Empty;
        [Required] public string OptionC { get; set; } = string.Empty;
        [Required] public string OptionD { get; set; } = string.Empty;

        // Default correct answer is "A" unless changed by the Admin
        [Required] public string CorrectOption { get; set; } = "A";
    }
}
