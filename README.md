# JobTracker

An ASP.NET Core 8 MVC web app for managing a job search end to end — the
frontend of a personal pipeline: **Python scraper → SQLite → this tracker**.

Live demo: _URL added after Azure App Service deployment_

## What it does

- **Dashboard** — applications by status, response rate, follow-ups due in
  the next 7 days, and the highest-scoring saved jobs to apply to next.
- **Application tracking** — full CRUD with a status pipeline
  (Saved → Applied → Interview → Offer / Rejected), one-click status changes
  from the list, search by title/company/skill, and track filtering
  (full-stack vs embedded).
- **Match data** — each application carries a match score and matched
  skills produced by the scraper, so prioritization is data-driven.
- **Auth** — ASP.NET Core Identity (register/login), all pages private.

## Stack

- ASP.NET Core 8 MVC (Razor Views), C# 12
- Entity Framework Core + SQLite (swap to Azure SQL for deployment —
  connection string only)
- ASP.NET Core Identity
- Bootstrap 5

## Run locally

Requires the .NET 8 SDK.

```bash
dotnet ef database update   # create the local SQLite database
dotnet run
```

Then open https://localhost:5001 and register an account.

## Roadmap

- [ ] One-click import from the scraper's `jobs_latest.csv`
- [ ] Charts (applications per week) with Chart.js
- [ ] Deploy to Azure App Service + Azure SQL
- [ ] Excel export of the pipeline
