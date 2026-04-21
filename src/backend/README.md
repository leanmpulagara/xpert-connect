# XpertConnect Backend

ASP.NET Core 10 Web API with Clean Architecture

## Quick Start for New Developers

### Prerequisites

| Software | Version | Download |
|----------|---------|----------|
| PostgreSQL | 16 or 17 | https://www.postgresql.org/download/ |
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |

### 1. Create Database

```bash
# Option A: Command line
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"

# Option B: Using pgAdmin
# Right-click Databases → Create → Database → Name: xpertconnect_dev
```

### 2. Configure Connection String

Edit `src/XpertConnect.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=xpertconnect_dev;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

> **Note:** Replace `YOUR_PASSWORD` with your PostgreSQL password

### 3. Run Database Migrations

```bash
# From src/backend directory
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### 4. Run the API

```bash
# From src/backend directory
dotnet run --project src/XpertConnect.API
```

API will be available at: **http://localhost:5200**

---

## Common Commands

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Add New Migration
```bash
dotnet ef migrations add <MigrationName> --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Update Database
```bash
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Rollback Migration
```bash
# Rollback to specific migration
dotnet ef database update <PreviousMigrationName> --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Reset Database (Development Only)
```bash
# Drop and recreate database
psql -U postgres -c "DROP DATABASE IF EXISTS xpertconnect_dev;"
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"

# Re-apply all migrations
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

---

## Project Structure

```
src/backend/
├── src/
│   ├── XpertConnect.Domain/           # Entities, Enums (no dependencies)
│   ├── XpertConnect.Application/      # DTOs, Interfaces, Business Logic
│   ├── XpertConnect.Infrastructure/   # DbContext, Repositories, Services
│   └── XpertConnect.API/              # Controllers, Hubs, Middleware
└── tests/
    ├── XpertConnect.Domain.Tests/
    ├── XpertConnect.Application.Tests/
    └── XpertConnect.API.Tests/
```

---

## API Endpoints

| Module | Base Route | Auth Required |
|--------|------------|---------------|
| Auth | `/api/auth` | No (except /me) |
| Users | `/api/users` | Yes |
| Experts | `/api/experts` | Partial |
| Seekers | `/api/seekers` | Yes |
| Consultations | `/api/consultations` | Yes |
| Auctions | `/api/auctions` | Partial |
| Bids | `/api/bids` | Yes |
| Projects | `/api/projects` | Partial |
| Payments | `/api/payments` | Yes |
| Escrow | `/api/escrow` | Yes |

### SignalR Hubs

| Hub | Endpoint |
|-----|----------|
| Auction (Real-time bidding) | `/hubs/auction` |
| Notifications | `/hubs/notifications` |

---

## Troubleshooting

### "Connection refused" error
- Ensure PostgreSQL is running
- Check port 5432 is not blocked
- Verify password in connection string

### Migration fails
```bash
# Check current migrations
dotnet ef migrations list --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API

# If corrupted, reset database (dev only)
psql -U postgres -c "DROP DATABASE xpertconnect_dev;"
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Port 5200 already in use
```bash
# Find process using port (Windows)
netstat -ano | findstr :5200

# Kill process
taskkill /PID <PID> /F
```
