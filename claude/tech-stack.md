# XpertConnect - Technology Stack

## Core Stack (LTS Versions)

| Layer | Technology | Version | LTS Support Until |
|-------|------------|---------|-------------------|
| **Frontend** | Next.js | 14.x | - |
| **Backend** | ASP.NET Core | 10.x (LTS) | November 2028 |
| **Database** | PostgreSQL | 17.x | November 2029 |
| **Runtime** | Node.js | 22.x (LTS) | April 2027 |

---

## Detailed Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                         CLIENTS                                  │
├──────────────────────────────────────────────────────────────────┤
│  Web Browser          Mobile App            Admin Dashboard      │
│  (Next.js)            (React Native)        (Next.js)            │
└────────────┬─────────────────┬─────────────────┬─────────────────┘
             │                 │                 │
             └────────────────┬┴─────────────────┘
                              │ HTTPS
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│                      API GATEWAY                                 │
│                   (Azure API Management / YARP)                  │
└─────────────────────────────┬────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│                   ASP.NET CORE 8 WEB API                         │
├──────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ │
│  │    User     │ │   Expert    │ │   Auction   │ │   Booking   │ │
│  │   Module    │ │   Module    │ │   Module    │ │   Module    │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ │
│  │   Payment   │ │  Pro-Bono   │ │    KYC      │ │  Geofence   │ │
│  │   Module    │ │   Module    │ │   Module    │ │   Module    │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ │
├──────────────────────────────────────────────────────────────────┤
│                        SignalR Hub                               │
│              (Real-time: Bidding, Notifications)                 │
└───────┬──────────────────┬──────────────────┬────────────────────┘
        │                  │                  │
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│  PostgreSQL   │  │     Redis     │  │ Elasticsearch │
│  (Primary DB) │  │    (Cache)    │  │   (Search)    │
└───────────────┘  └───────────────┘  └───────────────┘
```

---

## Frontend Stack

| Category | Technology | Purpose |
|----------|------------|---------|
| **Framework** | Next.js 14 | React framework with SSR/SSG |
| **Language** | TypeScript | Type safety |
| **Styling** | Tailwind CSS | Utility-first CSS |
| **UI Components** | shadcn/ui | Accessible, customizable components |
| **State Management** | Zustand | Simple global state |
| **Server State** | TanStack Query (React Query) | API caching, sync |
| **Forms** | React Hook Form + Zod | Form handling + validation |
| **Real-time** | @microsoft/signalr | SignalR client |
| **Charts** | Recharts | Analytics dashboards |
| **Date Handling** | date-fns | Date utilities |
| **Icons** | Lucide React | Icon library |

---

## Backend Stack

| Category | Technology | Purpose |
|----------|------------|---------|
| **Framework** | ASP.NET Core 10 (LTS) | Web API |
| **Language** | C# 13 | Primary language |
| **API Style** | REST + SignalR | HTTP + Real-time |
| **ORM** | Entity Framework Core 10 | Database access |
| **Validation** | FluentValidation | Request validation |
| **Mapping** | AutoMapper | DTO mapping |
| **Auth** | ASP.NET Identity + JWT | Authentication |
| **Authorization** | Policy-based | Role & permission checks |
| **Background Jobs** | Hangfire | Scheduled tasks, queues |
| **Logging** | Serilog | Structured logging |
| **API Docs** | Swagger / OpenAPI | API documentation |
| **Health Checks** | ASP.NET Health Checks | Monitoring endpoints |

---

## Database & Storage

| Category | Technology | Purpose |
|----------|------------|---------|
| **Primary DB** | PostgreSQL 17 (LTS) | Main data store |
| **Cache** | Redis | Session, caching, pub/sub |
| **Search** | Elasticsearch | Expert search, filters |
| **File Storage** | Azure Blob / AWS S3 | Documents, images |
| **Migrations** | EF Core Migrations | Schema versioning |

---

## External Services & Integrations

| Category | Service | Purpose |
|----------|---------|---------|
| **Payments** | Stripe Connect | Marketplace payments |
| **High-Value Escrow** | Escrow.com API | Auction settlements ($10K+) |
| **KYC/Identity** | Onfido / Jumio | Identity verification |
| **Video Calls** | Twilio Video / Daily.co | Virtual consultations |
| **Calendar Sync** | Google Calendar API | Expert availability |
| **Calendar Sync** | Microsoft Graph API | Outlook integration |
| **Email** | SendGrid / AWS SES | Transactional emails |
| **SMS** | Twilio | OTP, notifications |
| **Digital Signatures** | DocuSign API | NDA, MOU signing |
| **Geolocation** | Google Maps Platform | Geofencing, venues |
| **Push Notifications** | Firebase Cloud Messaging | Mobile push |

---

## DevOps & Infrastructure

| Category | Technology | Purpose |
|----------|------------|---------|
| **Cloud Provider** | Azure (Primary) | Hosting, services |
| **Containerization** | Docker | Application packaging |
| **Orchestration** | Azure Container Apps / AKS | Container management |
| **CI/CD** | GitHub Actions | Automated pipelines |
| **IaC** | Terraform / Bicep | Infrastructure as code |
| **Secrets** | Azure Key Vault | Secret management |
| **Monitoring** | Application Insights | APM, metrics |
| **Logging** | Azure Monitor / Seq | Centralized logs |
| **Error Tracking** | Sentry | Exception monitoring |
| **CDN** | Azure CDN / Cloudflare | Static asset delivery |

---

## Mobile App

| Category | Technology | Purpose |
|----------|------------|---------|
| **Framework** | React Native | Cross-platform mobile |
| **Navigation** | React Navigation | Screen routing |
| **State** | Zustand + React Query | Same as web |
| **Push** | Firebase Cloud Messaging | Notifications |
| **Geolocation** | react-native-geolocation | Meeting verification |

---

## Security Measures

| Category | Implementation |
|----------|----------------|
| **Authentication** | JWT + Refresh Tokens |
| **Authorization** | Role-based + Policy-based |
| **Data Encryption** | TLS 1.3 in transit, AES-256 at rest |
| **API Security** | Rate limiting, CORS, HTTPS only |
| **Input Validation** | Server-side validation (FluentValidation) |
| **SQL Injection** | Parameterized queries (EF Core) |
| **XSS Prevention** | Content Security Policy, sanitization |
| **CSRF Protection** | Anti-forgery tokens |
| **Secrets** | Azure Key Vault, no hardcoded secrets |
| **Audit Logging** | All sensitive operations logged |

---

## Project Structure

### Backend (.NET Core)

```
src/
├── XpertConnect.API/                 # Web API project
│   ├── Controllers/
│   ├── Hubs/                         # SignalR hubs
│   ├── Middleware/
│   └── Program.cs
├── XpertConnect.Application/         # Business logic
│   ├── Services/
│   ├── DTOs/
│   ├── Validators/
│   └── Interfaces/
├── XpertConnect.Domain/              # Entities, enums
│   ├── Entities/
│   ├── Enums/
│   └── ValueObjects/
├── XpertConnect.Infrastructure/      # Data access, external services
│   ├── Data/
│   ├── Repositories/
│   ├── Services/
│   └── Migrations/
└── XpertConnect.Tests/               # Unit & integration tests
    ├── Unit/
    └── Integration/
```

### Frontend (Next.js)

```
src/
├── app/                              # App Router (Next.js 14)
│   ├── (auth)/                       # Auth pages group
│   │   ├── login/
│   │   └── register/
│   ├── (main)/                       # Main app group
│   │   ├── dashboard/
│   │   ├── experts/
│   │   ├── auctions/
│   │   └── bookings/
│   ├── api/                          # API routes (if needed)
│   └── layout.tsx
├── components/
│   ├── ui/                           # shadcn/ui components
│   ├── forms/
│   ├── layouts/
│   └── features/
├── lib/
│   ├── api/                          # API client
│   ├── hooks/                        # Custom hooks
│   ├── utils/                        # Utilities
│   └── validations/                  # Zod schemas
├── stores/                           # Zustand stores
└── types/                            # TypeScript types
```

---

## Development Tools

| Tool | Purpose |
|------|---------|
| **IDE** | Visual Studio 2022 / VS Code / Rider |
| **API Testing** | Postman / Thunder Client |
| **DB Client** | pgAdmin / DBeaver |
| **Git** | GitHub |
| **Package Manager** | NuGet (.NET) + npm/pnpm (JS) |

---

## Summary

```
┌─────────────────────────────────────────────────────────┐
│              XPERTCONNECT STACK (LTS)                   │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   Frontend:     Next.js 14 + TypeScript + Tailwind     │
│   Backend:      ASP.NET Core 10 (LTS) + C# 13          │
│   Database:     PostgreSQL 17 + Redis + Elasticsearch  │
│   Runtime:      Node.js 22 (LTS)                        │
│   Mobile:       React Native                            │
│   Cloud:        Azure                                   │
│                                                         │
│   LTS Support:  All versions supported until 2027+      │
│                                                         │
└─────────────────────────────────────────────────────────┘
```
