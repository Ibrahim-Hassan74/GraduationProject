# SmartMicrobus API

A real-time microbus queue management and trip tracking system built with ASP.NET Core 8.

## Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **Database:** SQL Server 2022 (with NetTopologySuite for spatial data)
- **Cache:** Redis 7
- **Real-time:** SignalR
- **Background Jobs:** Hangfire
- **Auth:** ASP.NET Identity + JWT
- **Containerization:** Docker & Docker Compose

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- EF Core CLI tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 1. Clone the Repository

```bash
git clone https://github.com/Ibrahim-Hassan74/GraduationProject.git
cd GraduationProject
```

### 2. Start the Services

```bash
docker-compose up --build
```

This will start:

| Service         | Container Name       | Port  |
| --------------- | -------------------- | ----- |
| **API**         | smartmicrobus-api    | 5000  |
| **SQL Server**  | smartmicrobus-db     | 1433  |
| **Redis**       | smartmicrobus-redis  | 6379  |

> Wait until you see the API container logs showing `Now listening on: http://[::]:8080` before proceeding.

### 3. Apply Database Migrations

Open a **new terminal** in the project root and run:

**Windows (CMD / PowerShell):**
```bash
dotnet ef database update --project SmartMicrobus.Infrastructure --startup-project SmartMicrobus.API --connection "Server=localhost,1433;Database=SmartMicrobusDb;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

**Linux / macOS:**
```bash
dotnet ef database update \
  --project SmartMicrobus.Infrastructure \
  --startup-project SmartMicrobus.API \
  --connection "Server=localhost,1433;Database=SmartMicrobusDb;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

### 4. Open the API

Navigate to **http://localhost:5000/swagger** to explore the API.

---

## Stop the Services

```bash
docker-compose down
```

To also remove the database volume (full reset):

```bash
docker-compose down -v
```

---

## Project Structure

```
GraduationProject/
├── SmartMicrobus.API/              # Web API layer (Controllers, Hubs, Filters)
├── SmartMicrobus.Core/             # Domain entities, DTOs, Services, Interfaces
├── SmartMicrobus.Infrastructure/   # EF Core DbContext, Repositories, Migrations
├── Dockerfile                      # Multi-stage build
├── docker-compose.yml              # Full stack orchestration
└── .dockerignore
```

---

## Environment Variables

The following environment variables are configured in `docker-compose.yml`:

| Variable                             | Default Value            |
| ------------------------------------ | ------------------------ |
| `ASPNETCORE_ENVIRONMENT`             | `Production`             |
| `ConnectionStrings__DefaultConnection` | Docker SQL Server      |
| `ConnectionStrings__RedisConnection` | `redis:6379`             |
| `MSSQL_SA_PASSWORD`                  | `YourStrong@Pass123`     |

To customize, edit the `environment` section in `docker-compose.yml`.

---

## Default Credentials

| Service     | Username | Password            |
| ----------- | -------- | ------------------- |
| SQL Server  | `sa`     | `YourStrong@Pass123` |
