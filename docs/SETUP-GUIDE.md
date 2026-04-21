# XpertConnect - Setup & Run Guide

This guide provides step-by-step instructions to run the XpertConnect application locally.

---

## Prerequisites

Ensure you have the following installed:

| Software | Version | Download |
|----------|---------|----------|
| PostgreSQL | 16 or 17 | https://www.postgresql.org/download/ |
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| Node.js | 24.x (LTS) | https://nodejs.org/ |
| Git | Latest | https://git-scm.com/ |

### Verify Installation

```bash
# Check PostgreSQL
psql --version

# Check .NET
dotnet --version

# Check Node.js
node --version
npm --version
```

---

## Step 1: PostgreSQL Database Setup

### Option A: Using psql Command Line

```bash
# Connect to PostgreSQL as postgres user
psql -U postgres

# Create the database
CREATE DATABASE xpertconnect_dev;

# Verify database was created
\l

# Exit psql
\q
```

### Option B: Using pgAdmin

1. Open pgAdmin
2. Right-click on "Databases" → "Create" → "Database"
3. Name: `xpertconnect_dev`
4. Owner: `postgres`
5. Click "Save"

### Option C: Using Windows Command Prompt

```cmd
:: Navigate to PostgreSQL bin directory (adjust path as needed)
cd "C:\Program Files\PostgreSQL\16\bin"

:: Create database
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"
```

### Verify Database Connection

```bash
psql -U postgres -d xpertconnect_dev -c "SELECT 1;"
```

---

## Step 2: Configure Backend Connection String

The connection string is already configured in `appsettings.Development.json`:

**File:** `src/backend/src/XpertConnect.API/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=xpertconnect_dev;Username=postgres;Password=PGUser@001"
  }
}
```

### If Your PostgreSQL Password is Different:

Edit the `Password` value to match your PostgreSQL password:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=xpertconnect_dev;Username=postgres;Password=YOUR_PASSWORD_HERE"
```

---

## Step 3: Run Database Migrations

Navigate to the backend directory and apply Entity Framework migrations:

```bash
# Navigate to backend directory
cd src/backend

# Apply migrations to create all tables
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Verify Tables Created

```bash
# Connect to database and list tables
psql -U postgres -d xpertconnect_dev -c "\dt"
```

You should see 25+ tables including:
- `Users`, `Experts`, `Seekers`, `NonProfitOrgs`
- `AuctionLots`, `Bids`, `Consultations`, `ProBonoProjects`
- `Payments`, `EscrowAccounts`, `Milestones`
- And more...

---

## Step 4: Run the Backend API

### Terminal 1: Start the API Server

```bash
# Navigate to backend directory
cd src/backend

# Build the solution
dotnet build

# Run the API
dotnet run --project src/XpertConnect.API
```

### Expected Output

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5200
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### API Endpoints

| URL | Description |
|-----|-------------|
| `http://localhost:5200` | API Base URL |
| `http://localhost:5200/swagger` | Swagger UI (if enabled) |
| `http://localhost:5200/hubs/auction` | SignalR Auction Hub |
| `http://localhost:5200/hubs/notifications` | SignalR Notification Hub |

### Test API is Running

```bash
# Test the API (should return 401 Unauthorized for protected endpoint)
curl http://localhost:5200/api/auth/me

# Or open in browser
# http://localhost:5200/api/experts
```

---

## Step 5: Run the Frontend UI

### Terminal 2: Start the Frontend Development Server

```bash
# Navigate to frontend directory
cd src/frontend

# Install dependencies (first time only)
npm install

# Run development server
npm run dev
```

### Expected Output

```
   ▲ Next.js 16.x
   - Local:        http://localhost:3000
   - Environments: .env.local

 ✓ Starting...
 ✓ Ready in Xs
```

### Frontend URLs

| URL | Description |
|-----|-------------|
| `http://localhost:3000` | Frontend Application |

---

## Quick Start Commands Summary

Open **3 terminals** and run these commands:

### Terminal 1: PostgreSQL (if not running as service)
```bash
# Start PostgreSQL service (Windows)
net start postgresql-x64-16

# Or on Linux/Mac
sudo service postgresql start
```

### Terminal 2: Backend API
```bash
cd /mnt/c/Users/mural/source/repos/xpert-connect/src/backend
dotnet run --project src/XpertConnect.API
```

### Terminal 3: Frontend
```bash
cd /mnt/c/Users/mural/source/repos/xpert-connect/src/frontend
npm run dev
```

---

## Application URLs Summary

| Component | URL | Port |
|-----------|-----|------|
| **Frontend** | http://localhost:3000 | 3000 |
| **Backend API** | http://localhost:5200 | 5200 |
| **Backend HTTPS** | https://localhost:7200 | 7200 |
| **PostgreSQL** | localhost | 5432 |
| **SignalR Auction Hub** | ws://localhost:5200/hubs/auction | 5200 |
| **SignalR Notification Hub** | ws://localhost:5200/hubs/notifications | 5200 |

---

## API Testing with Postman/cURL

### Register a New User

```bash
curl -X POST http://localhost:5200/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123!",
    "firstName": "John",
    "lastName": "Doe",
    "userType": "Seeker"
  }'
```

### Login

```bash
curl -X POST http://localhost:5200/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123!"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "...",
  "expiresAt": "2026-04-21T12:00:00Z"
}
```

### Access Protected Endpoint

```bash
curl http://localhost:5200/api/auth/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Browse Experts (Public)

```bash
curl http://localhost:5200/api/experts
```

---

## Troubleshooting

### Database Connection Failed

**Error:** `Connection refused` or `password authentication failed`

**Solution:**
1. Ensure PostgreSQL is running:
   ```bash
   # Windows
   net start postgresql-x64-16

   # Linux/Mac
   sudo service postgresql status
   ```
2. Verify password in `appsettings.Development.json`
3. Check PostgreSQL is listening on port 5432:
   ```bash
   netstat -an | grep 5432
   ```

### Migration Failed

**Error:** `The entity type 'X' requires a primary key`

**Solution:**
```bash
# Drop and recreate database
psql -U postgres -c "DROP DATABASE xpertconnect_dev;"
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"

# Re-run migrations
cd src/backend
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

### Port Already in Use

**Error:** `Address already in use` on port 5200 or 3000

**Solution:**
```bash
# Find process using the port (Windows)
netstat -ano | findstr :5200

# Kill the process
taskkill /PID <PID> /F

# Or use different port for API
dotnet run --project src/XpertConnect.API --urls "http://localhost:5201"
```

### Frontend Build Errors

**Error:** `Module not found` or dependency issues

**Solution:**
```bash
cd src/frontend

# Clear node_modules and reinstall
rm -rf node_modules
rm package-lock.json
npm install
```

### CORS Issues

If frontend can't connect to backend API, ensure CORS is configured in the API.
Check `Program.cs` has CORS policy allowing `http://localhost:3000`.

---

## Development Tools

### Recommended VS Code Extensions

- **C# Dev Kit** - .NET development
- **PostgreSQL** - Database explorer
- **REST Client** - API testing
- **Mermaid Preview** - View flow diagrams
- **ESLint** - JavaScript/TypeScript linting

### Database GUI Tools

- **pgAdmin 4** - PostgreSQL admin (comes with PostgreSQL)
- **DBeaver** - Universal database tool
- **Azure Data Studio** - Microsoft's database tool

### API Testing Tools

- **Postman** - API testing
- **Insomnia** - REST client
- **Thunder Client** - VS Code extension

---

## Environment-Specific Configurations

### Development (Default)
- Database: `xpertconnect_dev`
- API: `http://localhost:5200`
- Frontend: `http://localhost:3000`
- Logging: Debug level

### Production (Phase 14 - Pending)
- Database: Azure PostgreSQL
- API: Azure App Service
- Frontend: Azure Static Web Apps
- Logging: Warning level

---

## Next Steps After Setup

1. **Test Registration/Login** - Create test users
2. **Create Expert Profile** - Test expert features
3. **Create Seeker Profile** - Test booking flow
4. **Test Auction** - Create and bid on auctions
5. **Test Real-Time** - Open multiple browsers to test SignalR

---

*Last Updated: April 2026*
