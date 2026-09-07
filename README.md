# JobTracker

An ASP.NET Core 8 MVC web app for managing a job search end to end — the
frontend of a personal pipeline: **Python scraper → SQLite → this tracker**.

**Live demo:** https://jobtracker-test-ewcgexgdebcdb0cd.westus3-01.azurewebsites.net

Deployed on Azure App Service (Linux, .NET 8) with GitHub Actions CI/CD —
every push to `main` builds and deploys automatically. Register a free
account to explore; data is stored in SQLite under App Service persistent
storage.

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
dotnet run
```

Then open http://localhost:5000 and register an account. Migrations are
applied automatically at startup, so the database is created on first run.

## Run with Docker

Requires Docker (Desktop). The SQLite database is stored in a named volume
so data survives container restarts.

```bash
docker compose up --build
```

Then open http://localhost:8080. To stop: `Ctrl+C`. To reset the data:
`docker compose down -v`.

## Roadmap

- [ ] One-click import from the scraper's `jobs_latest.csv`
- [ ] Charts (applications per week) with Chart.js
- [ ] Deploy to Azure App Service + Azure SQL
- [ ] Excel export of the pipeline
