# QuizApp

A full-stack quiz platform built with **ASP.NET Core MVC**. Admins create multiple-choice quizzes; students take them and get instantly scored. Built to practice the MVC pattern, EF Core, and cookie-based role authentication end to end.

> 📘 This repo tracks the code as I re-learn and extend it. See [`Roadmap`](#roadmap--things-im-relearning) below.

## Features

- **Role-based accounts** — Register as a Student or Admin; cookie authentication with `[Authorize(Roles = "Admin")]` gating admin-only routes.
- **Admin dashboard** — Create quizzes with multiple questions (4 options + correct answer each), view every quiz, and see all students' results in one table.
- **Student flow** — Browse available quizzes, take a quiz, get scored immediately, and see past attempts on the dashboard.
- **Persistent scoring** — Every attempt is saved (`TestResult`) with score, total questions, and timestamp.
- **Server-side validation** — Data annotations (`[Required]`, `[EmailAddress]`) on all forms, backed by unique-email enforcement at the DB level.

## Tech Stack

| Layer          | Technology                                   |
|----------------|-----------------------------------------------|
| Framework      | ASP.NET Core MVC (.NET 8)                     |
| ORM            | Entity Framework Core + Pomelo (MySQL)        |
| Auth           | Cookie authentication (custom, role-based)    |
| Frontend       | Razor Views + Bootstrap 5                     |
| Database       | MySQL                                         |

## Project Structure

```
QuizApp/
├── Controllers/       # Account, Admin, Home, Quiz
├── Models/            # Domain models + ViewModels
├── Data/              # EF Core DbContext
├── Views/             # Razor views (per controller)
├── wwwroot/            # Static assets (css/js)
└── Program.cs          # App startup & middleware pipeline
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL Server running locally (or update the connection string for a remote instance)

### Setup

```bash
git clone https://github.com/<your-username>/QuizApp.git
cd QuizApp/QuizApp

# Update the connection string in appsettings.json with your MySQL credentials
# "DefaultConnection": "Server=localhost;Port=3306;Database=quizapp_db;User=root;Password=YOUR_PASSWORD;"

dotnet restore
dotnet run
```

The app auto-creates the database tables on first run (`EnsureCreated()` in `Program.cs`) — no manual migration step needed for local dev.

Then open `https://localhost:5001` (or the port shown in the console), register an account, and pick **Admin** to create your first quiz.

## Roadmap / things I'm relearning

- [ ] Replace `EnsureCreated()` with proper EF Core Migrations
- [ ] Hash passwords with a real algorithm (currently a teaching-project simplification)
- [ ] Add unit tests for controllers
- [ ] Add question types beyond multiple-choice
- [ ] Move to ASP.NET Core Identity for auth

## Branches

- `main` — fully commented version (used for re-learning the codebase)
- `clean-version` — same app, no comments, for a leaner read

## License

MIT — see [LICENSE](LICENSE).
