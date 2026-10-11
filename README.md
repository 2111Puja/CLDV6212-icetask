# 🏘️ Community Portal: Service Request System

A web application that allows residents to report community problems such as **water leaks, potholes, and broken streetlights**, while enabling municipal staff to track, manage, and resolve service requests.

**Live app:** [ADD LIVE URL HERE]
> The free hosting tier sleeps when idle, so the first load can take up to a minute.

---

## 📝 1. The Problem

Residents often have no simple way to report faults in their area or find out whether anyone is dealing with them. Reports can get lost between phone calls, emails, and walk-ins, while municipal teams struggle to see what is urgent, who is responsible, and what has been fixed.

### 👥 Who It Affects

* **Residents**, who need an easy way to report an issue and follow its progress.
* **Municipal staff**, who need one place to see, prioritise, assign, and update requests.
* **Local government**, which needs accountability and a record of response times.

**How the app helps**
- Residents submit a report (issue type, description, location) and get a **reference number** to track it.
- Residents can look up the status of any request with that reference number.
- Staff log in to a **dashboard** showing all requests, filterable by status (Pending, In Progress, Resolved).
- Staff open a request to update its **status, priority, assigned team and notes**.

> [Add 1-2 sources or statistics from your research here to back up the problem statement.]

A transparent tracking system helps close the loop between the person reporting the issue and the team responsible for fixing it.

## 2. Architecture

[UPDATE this section to match the final design. The description below is the current structure.]

```
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

## 3. Run Locally

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) and Git.

### 🌐 Live Application

The deployed application is accessible at:
**https://cldv6212-community-portal.onrender.com/**
<img width="1905" height="850" alt="image" src="https://github.com/user-attachments/assets/8033b761-d473-4787-b4be-43e88ad71456" />

<img width="1917" height="862" alt="image" src="https://github.com/user-attachments/assets/de7df77f-ef87-4ed0-84de-69f86f8f61c5" />

<img width="1912" height="842" alt="image" src="https://github.com/user-attachments/assets/cb16ac02-9030-4ad7-ac08-0008cf1cd3f5" />

<img width="1867" height="842" alt="image" src="https://github.com/user-attachments/assets/305c38fb-24e2-48b2-82e7-92fe9f86c56c" />

<img width="1917" height="847" alt="image" src="https://github.com/user-attachments/assets/61df1320-fae3-4c7f-9b5f-17ab2474dcd8" />


### 📖 Swagger API

The Swagger API documentation is available through the deployed application:

**https://cldv6212-community-portal.onrender.com/swagger**

### ☁️ Render Deployment

The application is deployed and running on Render.


<img width="1478" height="272" alt="Screenshot 2026-10-03 150531" src="https://github.com/user-attachments/assets/e1e89fe2-035d-41f9-aaf6-f89a4e5725dd" />

<img width="1917" height="797" alt="Screenshot 2026-10-03 151340" src="https://github.com/user-attachments/assets/da970ed9-e648-4228-9217-9093dae56ec6" />

### 🗄️ Supabase PostgreSQL

The application uses Supabase PostgreSQL as its cloud database.

<img width="1915" height="341" alt="Screenshot 2026-10-03 152517" src="https://github.com/user-attachments/assets/7d268011-47c7-4e52-8d73-6c3afb526b02" />

---

## 🚀 4. Run Locally

### Prerequisites

Before running the application locally, make sure you have:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* Git

### Clone the Repository

```bash
git clone https://github.com/aminah-17/cloud-ice-task.git
cd cloud-ice-task
cp .env.example .env      # then edit the values in .env
docker compose up --build
```

Open **http://localhost:8080**.

To stop the stack: `docker compose down`.

**📖 Swagger API Documentation:** http://localhost:8080/swagger

### Stop the Application

To stop the Docker stack:

```bash
docker compose down
```

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
