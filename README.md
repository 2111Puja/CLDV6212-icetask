# Community Portal: Service Request System

A web application that lets residents report community problems (water leaks, potholes, broken streetlights) and lets municipal staff track and resolve them.

**Live app:** [ADD LIVE URL HERE]
> The free hosting tier sleeps when idle, so the first load can take up to a minute.

---

## 1. The Problem

Residents often have no simple way to report faults in their area, and no way to find out whether anyone is dealing with them. Reports get lost between phone calls, emails and walk-ins, and municipal teams struggle to see what is urgent, who is responsible, and what has been fixed.

**Who it affects**
- **Residents**, who need an easy way to report an issue and follow its progress.
- **Municipal staff**, who need one place to see, prioritise, assign and update requests.
- **Local government**, which needs accountability and a record of response times.

**Why it matters:** unresolved infrastructure problems such as burst pipes and potholes affect safety, cost more the longer they are ignored, and reduce public trust. A transparent tracking system closes the loop between the person reporting and the team fixing.

**How the app helps**
- Residents submit a report (issue type, description, location) and get a **reference number** to track it.
- Residents can look up the status of any request with that reference number.
- Staff log in to a **dashboard** showing all requests, filterable by status (Pending, In Progress, Resolved).
- Staff open a request to update its **status, priority, assigned team and notes**.

> [Add 1-2 sources or statistics from your research here to back up the problem statement.]

---

## 2. Architecture

[UPDATE this section to match the final design. The description below is the current structure.]

```
Browser
   |
   v
ASP.NET Core MVC app (Community Portal)   <-- Docker container
   |
   v
[Database: Neon / Supabase PostgreSQL]    <-- hosted free tier (live)
[PostgreSQL container]                    <-- Docker Compose (local only)
```

| Component | Technology |
|-----------|------------|
| Web app | ASP.NET Core MVC, .NET 10 |
| Database | [PostgreSQL, hosted free tier for live, container locally] |
| Containers | Docker, Docker Compose |
| CI/CD | GitHub Actions |
| Hosting | [Render, free tier] |

---

## 3. Run Locally

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) and Git.

```bash
git clone https://github.com/aminah-17/cloud-ice-task.git
cd cloud-ice-task
cp .env.example .env      # then edit the values in .env
docker compose up --build
```

Open **http://localhost:8080**.

To stop the stack: `docker compose down`.

---

## 4. Environment Variables

Copy `.env.example` to `.env` and fill in real values. Never commit `.env`.

| Variable | Purpose |
|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment (`Development` or `Production`) |
| `ASPNETCORE_HTTP_PORTS` | Port the app listens on inside the container (`8080`) |
| `ConnectionStrings__Default` | [Database connection string, once the database is set up] |

Secrets for deployment are stored in **GitHub Repository Secrets**, not in the code.

---

## 5. CI/CD

[Describe the GitHub Actions pipeline once it exists: build, tests, Docker build, security scan (CodeQL/Trivy), deploy on merge to `main`.]

---

## 6. Team and Contributions

| Member | Main contributions |
|--------|--------------------|
| Puja Mahabir |  |
| Aminah Omer |  |

All work is done on feature branches and merged through reviewed Pull Requests.
