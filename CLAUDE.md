# XpertConnect - Project Context

> This file is automatically read by Claude Code at the start of each session.

---

## Project Overview

**XpertConnect** is a marketplace platform connecting "accomplished minds" (experts, executives, celebrities) with "seekers" (people wanting access to their expertise).

### Three Business Models

1. **Auction Model** - High-value charity auctions (like Warren Buffett lunch)
2. **Professional Fee Model** - Paid consultations ($100-$1,400/hr)
3. **Pro-Bono Model** - Free volunteer work for non-profits

---

## Tech Stack (Finalized - LTS Versions)

| Layer | Technology | LTS Until |
|-------|------------|-----------|
| **Frontend** | Next.js 14 + TypeScript + Tailwind CSS | - |
| **Backend** | ASP.NET Core 10 (LTS) + C# 13 + SignalR | Nov 2028 |
| **Database** | PostgreSQL 17 + Redis + Elasticsearch | Nov 2029 |
| **Runtime** | Node.js 24.x | - |
| **Mobile** | React Native | - |
| **Cloud** | Azure | - |
| **IDE** | VS Code (with C# Dev Kit extension) | - |

---

## Project Structure

```
xpert-connect/
├── CLAUDE.md                          # Project context (this file)
├── claude/                            # Documentation
│   ├── requirement.md                 # Original requirements
│   ├── project-understanding.md       # Layman explanation
│   ├── uml-diagrams.md               # UML diagrams
│   ├── tech-stack.md                 # Technology stack
│   └── implementation-phases.md       # 14-phase implementation plan
├── src/
│   ├── backend/                       # .NET Core Backend
│   │   ├── XpertConnect.sln
│   │   └── src/
│   │       ├── XpertConnect.Domain/           # Entities, Enums (no dependencies)
│   │       ├── XpertConnect.Application/      # Business logic, DTOs, Interfaces
│   │       ├── XpertConnect.Infrastructure/   # DbContext, Repositories, Services
│   │       └── XpertConnect.API/              # Controllers, Middleware
│   ├── frontend/                      # Next.js Frontend
│   │   ├── src/
│   │   │   └── app/                   # App Router pages
│   │   ├── package.json
│   │   └── ...
│   └── mobile/                        # React Native (future)
├── docs/                              # Additional documentation
└── tests/                             # Test projects
```

---

## Current Progress

- [x] Requirements analysis
- [x] UML diagrams created (11 diagrams)
- [x] Tech stack finalized
- [x] Project structure setup (Clean Architecture)
- [x] Domain layer (Entities, Enums)
- [x] Application layer (Interfaces, DTOs, Models)
- [x] Infrastructure layer (DbContext, Services)
- [x] API layer (Program.cs, Configuration)
- [x] Next.js frontend initialized
- [x] EF Core tools installed (v10.0.5)
- [x] Entity configurations created (14 configuration files)
- [x] Initial migration created (InitialCreate)
- [ ] Database setup (create database + apply migration)
- [ ] API endpoints implementation
- [ ] Authentication setup (Identity + JWT)
- [ ] Core modules implementation

---

## Key Entities (Domain Layer)

| Entity | Description |
|--------|-------------|
| `User` | Base user (Seeker, Expert, NonProfitOrg) |
| `Expert` | Accomplished mind with credentials |
| `Seeker` | Person seeking expert access |
| `AuctionLot` | High-value charity auction |
| `Bid` | Auction bid with proxy support |
| `Consultation` | Paid expert session |
| `ProBonoProject` | Volunteer work for charities |
| `Payment` | Financial transaction |
| `EscrowAccount` | Secure payment holding |
| `Venue` | Physical meeting location |
| `Geofence` | GPS-based meeting verification |
| `Nda/Mou` | Legal documents |

---

## Commands Reference

```bash
# Backend (.NET) - from src/backend
dotnet.exe build                          # Build solution
dotnet.exe run --project src/XpertConnect.API  # Run API

# Frontend (Next.js) - from src/frontend
npm run dev                               # Start dev server (localhost:3000)
npm run build                             # Production build

# Database migrations - from src/backend
dotnet.exe ef migrations add <name> --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
dotnet.exe ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API
```

---

## API Endpoints (Planned)

| Module | Base Route |
|--------|------------|
| Auth | `/api/auth` |
| Users | `/api/users` |
| Experts | `/api/experts` |
| Seekers | `/api/seekers` |
| Auctions | `/api/auctions` |
| Bids | `/api/bids` |
| Consultations | `/api/consultations` |
| Projects | `/api/projects` |
| Payments | `/api/payments` |

---

## Next Steps

1. **Setup PostgreSQL Database:**
   - Update password in `appsettings.Development.json` (replace `your_password_here`)
   - Create database: `psql.exe -U postgres -c "CREATE DATABASE xpertconnect_dev;"`
   - Apply migration: `dotnet.exe ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API`

2. Implement authentication (ASP.NET Identity + JWT)
3. Create API controllers for each module
4. Build frontend pages and components
5. Integrate frontend with backend API

---

## Resume Implementation

To continue implementation from any phase, say:
- **"Continue from Phase X"** - Claude will pick up where you left off
- **"What's the current phase?"** - Claude will check progress and suggest next steps

See `claude/implementation-phases.md` for the complete 14-phase plan with:
- Detailed tasks and checkboxes
- Commands for each phase
- Key files to create/modify
- Deliverables checklist

---

## Notes

- High-value transactions require escrow (Escrow.com API for $10K+)
- Real-time bidding uses SignalR
- KYC verification via Onfido/Jumio
- Geofencing for physical meeting verification
- Update PostgreSQL password in `appsettings.Development.json`
