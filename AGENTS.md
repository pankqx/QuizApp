# AGENTS.md

Instructions for AI coding agents (Claude, Copilot, Cursor, Gemini, etc.) working in this repo.
Humans: read [GUIDE.md](GUIDE.md) instead. It explains everything from zero.

## What this project is

QuizApp is a server-rendered quiz platform built with **ASP.NET Core MVC (.NET 8)**,
**Entity Framework Core** and **MySQL** (Pomelo provider). Admins create multiple-choice
quizzes. Students take them and get a score that is saved to the database.

## Layout

```
QuizApp/                     the one .NET project (QuizApp.MVC.csproj)
  Program.cs                 startup: services, middleware pipeline, EnsureCreated()
  Controllers/               Account (login/register/logout), Admin, Home, Quiz
  Models/Models.cs           EF entities (User, Quiz, Question, TestResult) + view models
  Data/QuizDbContext.cs      DbContext, unique index on User.Email
  Views/<Controller>/*.cshtml Razor views, Bootstrap 5 from a CDN
  wwwroot/css/site.css       custom styles
  appsettings.json           connection string (placeholder password, never commit a real one)
```

## Commands

```bash
cd QuizApp
dotnet restore
dotnet build
dotnet run            # needs MySQL running and a valid connection string
```

There are no tests yet. If you add some, put them in a separate `QuizApp.Tests` xUnit project.

## Conventions

- One controller per feature area. Actions are `async` and use `_context` (injected `QuizDbContext`).
- Admin-only controllers carry `[Authorize(Roles = "Admin")]`. Logged-in-only use `[Authorize]`.
- The current user id comes from the `ClaimTypes.NameIdentifier` claim.
- Entities live in `Models/Models.cs`. Keep view models (form shapes) separate from entities.
- Every file starts with a banner comment (FILE / PROJECT / DESCRIPTION). Keep that style.
- The schema is created with `EnsureCreated()`. If you change an entity, the dev database
  must be dropped and recreated, or you must switch to EF Core Migrations first.

## Known weak spots (fix only when asked)

- Passwords are stored and compared as plain text (`PasswordHash` holds the raw password).
- POST actions do not have `[ValidateAntiForgeryToken]`.
- Anyone can register as Admin from the public Register page.
- No edit/delete for quizzes, no tests, no migrations.

## Rules for agents

- Commit as `pankqx <pankqx@gmail.com>`. Do not add `Co-Authored-By` or any AI trailer.
- Never commit a real database password. Use user-secrets or environment variables.
- Keep changes small and explain them in plain English in the PR body.
- If you change behaviour, update GUIDE.md in the same PR so the owner can still explain it.
