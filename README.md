# 🏘️ Community Portal: Service Request System

A web application that allows residents to report community problems such as **water leaks, potholes, and broken streetlights**, while enabling municipal staff to track, manage, and resolve service requests.

**🌐 Live App:** [https://cldv6212-community-portal.onrender.com/](https://cldv6212-community-portal.onrender.com/)

**📖 API Documentation (Swagger):** [https://cldv6212-community-portal.onrender.com/swagger](https://cldv6212-community-portal.onrender.com/swagger)
*Locally: `http://localhost:8080/swagger`*

> ⚠️ **Note:** The free hosting tier sleeps when idle, so the first load can take up to a minute.

---

## 📝 1. The Problem

Residents often have no simple way to report faults in their area or find out whether anyone is dealing with them. Reports can get lost between phone calls, emails, and walk-ins, while municipal teams struggle to see what is urgent, who is responsible, and what has been fixed.

### 👥 Who It Affects

* **Residents**, who need an easy way to report an issue and follow its progress.
* **Municipal staff**, who need one place to see, prioritise, assign, and update requests.
* **Local government**, which needs accountability and a record of response times.

### 💡 Why It Matters

Unresolved infrastructure problems such as burst pipes and potholes can affect safety, cost more the longer they are ignored, and reduce public trust.

A transparent tracking system helps close the loop between the person reporting the issue and the team responsible for fixing it.

### ✅ How the App Helps

* Residents submit a report containing the **issue type, description, and location** and receive a **reference number** to track it.
* Residents can look up the status of any request using its **reference number**.
* Staff log in to a **dashboard** displaying all requests, which can be filtered by status:

  * 🟡 Pending
  * 🔵 In Progress
  * 🟢 Resolved
* Staff can open a request and update its:

  * **Status**
  * **Priority**
  * **Assigned team**
  * **Notes**

---

## 🏗️ 2. Architecture

```text
Browser
   |
   v
ASP.NET Core MVC App (Community Portal)
   <-- Docker Container (Render / Local)
   |
   v
Database: Supabase PostgreSQL
   <-- Hosted Cloud Instance
```

| Component     | Technology                                       |
| ------------- | ------------------------------------------------ |
| 🌐 Web App    | ASP.NET Core MVC, .NET 10, Swashbuckle (Swagger) |
| 🗄️ Database  | Supabase PostgreSQL                              |
| 🐳 Containers | Docker, Docker Compose                           |
| ⚙️ CI/CD      | GitHub Actions                                   |
| ☁️ Hosting    | Render (Free Tier)                               |

---

## 🚀 3. Run Locally

### Prerequisites

Before running the application locally, make sure you have:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* Git

### Clone the Repository

```bash
git clone https://github.com/2111Puja/CLDV6212-icetask.git
cd CLDV6212-icetask
```

### Configure Environment Variables

Copy the example environment file:

```bash
cp .env.example .env
```

Then edit `.env` and enter the required values.

### Start the Application

```bash
docker compose up --build
```

Once the containers have started, open:

**🌐 Application:** http://localhost:8080

**📖 Swagger API Documentation:** http://localhost:8080/swagger

### Stop the Application

To stop the Docker stack:

```bash
docker compose down
```

---

## 🔐 4. Environment Variables

Copy `.env.example` to `.env` and fill in the required values.

> ⚠️ **Never commit `.env` to the repository.**

| Variable                     | Purpose                                                       |
| ---------------------------- | ------------------------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT`     | Runtime environment (`Development` or `Production`)           |
| `ASPNETCORE_HTTP_PORTS`      | Port the application listens on inside the container (`8080`) |
| `ConnectionStrings__DefaultConnection` | Supabase **Session pooler** connection string (see section 7) |
| `PORT`                       | Set automatically by Render                                   |
| `Staff__Username` / `Staff__Password` | Municipality login (defaults below - change on Render) |

Deployment secrets are stored securely in **GitHub Repository Secrets** and are not included in the source code.

---

## 🔄 5. CI/CD

The project uses an automated **GitHub Actions** pipeline configured under:

```text
.github/workflows/
```

On every push or pull request to the `main` branch, the pipeline automatically:

1. 📦 Restores dependencies and builds the ASP.NET Core solution.
2. 🐳 Builds and validates the Docker container configuration.
3. 🚀 Triggers deployment updates to Render after successful verification.

---

## 👩‍💻 6. Team and Contributions

| Member           | Main Contributions                                                                                                                                                             |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Puja Mahabir** | Full-stack MVC architecture, Supabase PostgreSQL database integration, Docker containerisation, Swashbuckle Swagger API documentation setup, and CI/CD workflow configuration. |
| **Aminah Omer**  | Problem statement research, frontend views design, Docker containerisation, system structure documentation, and user workflow implementation.                                                           |

### 🤝 Contribution Workflow

All work is completed on **feature branches** and merged through reviewed **Pull Requests**.

---

## 📌 Project Links

* 🌐 **[Live Application](https://cldv6212-community-portal.onrender.com/)**
* 📖 **[Swagger API Documentation](https://cldv6212-community-portal.onrender.com/swagger)**
* 💻 **[GitHub Repository](https://github.com/aminah-17/cloud-ice-task)**


---

## 🔑 7. Deploying on Render + Supabase (read this if you get HTTP 500)

1. **Supabase → Project Settings → Database → Connection string → _Session pooler_.** Copy the host
   (`aws-0-<region>.pooler.supabase.com`), port `5432` and user `postgres.<project-ref>`.
   > Do **not** use the "Direct connection" host (`db.<ref>.supabase.co`). It is IPv6-only and Render cannot reach it,
   > which produces 500 errors on every database call.
2. **Render → your service → Environment**, add:

   | Key | Value |
   | --- | ----- |
   | `ConnectionStrings__DefaultConnection` | `Host=aws-0-<region>.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<project-ref>;Password=<db-password>;SSL Mode=Require;Trust Server Certificate=true` |
   | `Staff__Username` | your staff login |
   | `Staff__Password` | a strong password |

   (A `postgresql://user:password@host:5432/postgres` URI also works. URL-encode special characters in the password, e.g. `@` → `%40`.)
3. **Deploy.** On start-up the app now creates/upgrades the `ServiceRequests` table itself using idempotent SQL
   (`DatabaseInitializer`), so no manual SQL or `EnsureCreated()` is needed - even if the Supabase project already has other tables.
4. **Verify:** open `https://<your-app>.onrender.com/health`. `{"status":"healthy"}` means the database is connected;
   otherwise the JSON tells you what is wrong (wrong password, unreachable host, ...). Full details are in the Render logs.

### Staff login (demo defaults)

| Username | Password |
| -------- | -------- |
| `staff@municipality.gov.za` | `Municipal@2026` |

Override with `Staff__Username` / `Staff__Password` on Render.

### REST API

`GET /api/service-requests/{referenceNumber}` and `POST /api/service-requests` are documented in Swagger at `/swagger`.
