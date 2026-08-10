// ============================================================
// FILE: Data/QuizDbContext.cs
// PROJECT: QuizApp — ASP.NET Core MVC
// DESCRIPTION: Entity Framework Core database context.
//              Registers all domain models as DB tables and
//              applies custom constraints (e.g. unique email).
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuizApp.MVC.Models;

namespace QuizApp.MVC.Data
{
    /// <summary>
    /// The main database context for QuizApp.
    /// EF Core uses this to create and manage the MySQL database tables.
    /// </summary>
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

        // ── DB Tables ──────────────────────────────────────────
        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestResult> TestResults { get; set; }

        /// <summary>
        /// Fluent API configuration for model constraints.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enforce unique email across all users to prevent duplicate accounts
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
