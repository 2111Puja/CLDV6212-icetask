# 🏘️ Community Portal: Service Request System

A web application that allows residents to report community problems such as **water leaks, potholes, and broken streetlights**, while enabling municipal staff to track, manage, and resolve service requests.

**🌐 Live App:** https://cldv6212-community-portal.onrender.com/

**📖 API Documentation (Swagger):** https://cldv6212-community-portal.onrender.com/swagger
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

## 📸 3. Deployment Screenshots

The Community Portal was successfully deployed using **Docker and Render**, with **Supabase PostgreSQL** used as the cloud database.

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

## 🔐 5. Environment Variables

Copy `.env.example` to `.env` and fill in the required values.

> ⚠️ **Never commit `.env` to the repository.**

| Variable                               | Purpose                                                       |
| -------------------------------------- | ------------------------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT`               | Runtime environment (`Development` or `Production`)           |
| `ASPNETCORE_HTTP_PORTS`                | Port the application listens on inside the container (`8080`) |
| `ConnectionStrings__DefaultConnection` | Supabase Session Pooler connection string                     |
| `PORT`                                 | Set automatically by Render                                   |
| `Staff__Username` / `Staff__Password`  | Municipality login credentials                                |

Deployment secrets are stored securely in **GitHub Repository Secrets** and are not included in the source code.

---

## 🔄 6. CI/CD

The project uses an automated **GitHub Actions** pipeline configured under:

```text
.github/workflows/
```

On every push or pull request to the `main` branch, the pipeline automatically:

1. 📦 Restores dependencies and builds the ASP.NET Core solution.
2. 🐳 Builds and validates the Docker container configuration.
3. 🚀 Triggers deployment updates to Render after successful verification.

---

## 👩‍💻 7. Team and Contributions

| Member | Main Contributions |
| :--- | :--- |
| **Puja Mahabir** | Full-stack .NET 10 MVC architecture, Entity Framework Core, Supabase PostgreSQL database integration, Docker containerisation, root docker-compose orchestration, Swashbuckle Swagger API documentation, GitHub Actions CI/CD pipeline configuration, and Render cloud deployment. |
| **Aminah Omer** | Problem statement research, target user analysis, documentation and presentation support. |

### 🤝 Contribution Workflow

All work is completed on **feature branches** and merged through reviewed **Pull Requests**.

---

## 📌 Project Links

* 🌐 **[Live Application](https://cldv6212-community-portal.onrender.com/)**
* 📖 **[Swagger API Documentation](https://cldv6212-community-portal.onrender.com/swagger)**
* 💻 **[GitHub Repository](https://github.com/2111Puja/CLDV6212-icetask)**

---

## 🔑 8. Deploying on Render + Supabase

> ⚠️ **Read this section if you get HTTP 500 errors during deployment.**

### 1. Configure Supabase

In **Supabase**, navigate to:

**Project Settings → Database → Connection String → Session Pooler**

Use the **Session Pooler** connection details for the deployed application.

> ⚠️ **Do not use the Direct Connection host.** The Direct Connection uses IPv6, which can prevent Render from reaching the database.

### 2. Configure Render Environment Variables

In **Render → Your Service → Environment**, configure:

| Key                                    | Value                                          |
| -------------------------------------- | ---------------------------------------------- |
| `ConnectionStrings__DefaultConnection` | Your Supabase Session Pooler connection string |
| `Staff__Username`                      | Your staff login username                      |
| `Staff__Password`                      | Your staff login password                      |

A PostgreSQL connection URI can also be used.

> 🔐 **Never commit database passwords or other secrets to the repository.**

### 3. Deploy

Deploy the application to Render.

On startup, the application creates or upgrades the `ServiceRequests` table using idempotent SQL through `DatabaseInitializer`.

No manual SQL or `EnsureCreated()` is required, even if the Supabase project already contains other tables.

### 4. Verify

Open the health endpoint:

```text
https://cldv6212-community-portal.onrender.com/health
```

A successful database connection returns:

```json
{"status":"healthy"}
```

If there is a database connection problem, the JSON response provides information about the issue. Full details are also available in the **Render logs**.

### 🔑 Staff Login — Demo Defaults

| Username                    | Password         |
| --------------------------- | ---------------- |
| `staff@municipality.gov.za` | `Municipal@2026` |

These values can be overridden using the `Staff__Username` and `Staff__Password` environment variables on Render.

### 🔌 REST API

The following endpoints are documented in Swagger:

```text
GET  /api/service-requests/{referenceNumber}
POST /api/service-requests
```

Swagger is available at:

**https://cldv6212-community-portal.onrender.com/swagger**

---
