# CitizenPanel

A multi-tenant web platform that allows organisations to manage citizen panels — groups of residents that are recruited, drawn by lottery, and consulted on local policy topics.

Built with ASP.NET Core 8 MVC, PostgreSQL, and a Vite/TypeScript/Tailwind CSS front-end.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Test Accounts](#test-accounts)
- [Seeded Test Data](#seeded-test-data)
- [Architecture Overview](#architecture-overview)

---

## Project Structure

```
CitizenPanel/
├── Domain/          # Domain models and validation attributes
├── DAL/             # Data access layer (EF Core repositories + DbContext)
├── BL/              # Business logic layer (managers)
├── UI_MVC/          # ASP.NET Core MVC web application
│   ├── ClientApp/   # Vite + TypeScript + Tailwind CSS front-end
│   ├── Controllers/ # MVC and API controllers
│   ├── Views/       # Razor views
│   ├── Areas/       # Identity pages (login, register, …)
│   └── Middleware/  # Multi-tenancy middleware
└── CitizenPanel.sln
```

---

## Prerequisites

Make sure the following are installed before you begin:

| Tool | Version      |
|------|--------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0.x        |
| [Node.js](https://nodejs.org/) | 18 or higher |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | any recent version |

If you prefer not to use Docker, PostgreSQL 13+ installed locally works too.

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/jurgen-potter/ip1.git
cd ip1
```

### 2. Set up the database

The easiest way is with Docker:
```powershell
docker run --name citizenpanel-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=Student_1234 -e POSTGRES_DB=PanelManager -p 5432:5432 -d postgres
```

Alternatively, if you have PostgreSQL installed locally, create the database manually:
```sql
CREATE DATABASE "PanelManager";
```

The default connection string expects host `localhost`, username `postgres`, and password `Student_1234`. The application automatically creates all tables and seeds initial data on first start.

### 3. Configure the application

Open `UI_MVC/appsettings.json` and update the connection string and SMTP settings if needed (see [Configuration](#configuration)).

### 4. Build the front-end

```bash
cd UI_MVC/ClientApp
npm install
npm run build
cd ../..
```

> **Note:** The .NET build also runs `npm install` and `npm run build` automatically, so this step is only needed if you want to pre-build the front-end separately.

### 5. Run the application

```bash
cd UI_MVC
dotnet run --launch-profile https
```

The application will be available at `https://localhost:7145`.

---

## Configuration

All settings live in `UI_MVC/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PanelManager;Username=postgres;Password=Student_1234"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com"
  }
}
```

Email confirmation is required for new accounts. If SMTP is not configured or the credentials are invalid, email sending will silently fail and log a warning — the app will still work and registration can be completed via the confirmation link shown on screen.

---

## Test Accounts

The database is seeded with the following test users on first run. All passwords are `Test1!`.

| Email | Password | Role | Notes |
|-------|----------|------|-------|
| `admin@example.com` | `Test1!` | Admin | Super admin |
| `admin2@example.com` | `Test1!` | Admin | |
| `antwerpen@example.com` | `Test1!` | Organisation | Tenant: `antwerpen`, super |
| `brussel@example.com` | `Test1!` | Organisation | Tenant: `brussel` |
| `paul@example.com` | `Test1!` | Member | Tenant: `antwerpen`, born 1980 |

---

## Seeded Test Data

The seed data includes two tenants (`antwerpen` and `brussel`) with pre-configured panels, registrations and invitations so you can explore the full flow without manual setup.

**Antwerpen panels:**
- Panel 1 is already active. `paul@example.com` is a member of this panel.
- Panel 2 has existing registrations and seeded invitations.

**Ready-to-use invitation codes for Panel 2 (activate by going to "code registreren"):**

| Code | Status |
|------|--------|
| `HEMf-Xu0L-1ETh-urZJ-26s9` | Can register for panel 2 immediately |
| `blLy-Tvxl-1TXG-nYg3-CBtD` | Needs to register first, then be drawn by lottery |

---

## Architecture Overview

The project follows a layered architecture:

- **Domain** — Plain C# classes representing the core entities (panels, meetings, draws, questionnaires, users).
- **DAL** — Entity Framework Core repositories backed by PostgreSQL. Contains `PanelDbContext`, migrations, and data seeding.
- **BL** — Business logic managers that sit between the UI and the DAL. Each domain area has a matching manager and interface.
- **UI_MVC** — ASP.NET Core MVC application with Razor views, API controllers, ASP.NET Identity, and multi-tenancy middleware. The front-end (TypeScript + Tailwind CSS) is bundled with Vite and served as static files.

Multi-tenancy is implemented via a URL prefix (`/{tenantId}/...`) and resolved through `TenantMiddleware` at request time.
