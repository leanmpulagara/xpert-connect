# XpertConnect - Requirements & Functional Specification

**Document Version:** 1.0
**Last Updated:** April 2026
**Status:** Demo Ready (Phases 1-13 Complete)

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Project Overview](#2-project-overview)
3. [Business Models](#3-business-models)
4. [Functional Requirements](#4-functional-requirements)
5. [Technical Architecture](#5-technical-architecture)
6. [API Specification](#6-api-specification)
7. [Database Schema](#7-database-schema)
8. [Security & Compliance](#8-security--compliance)
9. [Implementation Status](#9-implementation-status)

---

## 1. Executive Summary

### 1.1 Purpose

XpertConnect is a marketplace platform that connects **"Accomplished Minds"** (experts, executives, celebrities) with **"Seekers"** (individuals and organizations seeking access to their expertise). The platform facilitates three distinct business models: high-value charity auctions, professional fee-based consultations, and pro-bono social impact projects.

### 1.2 Vision Statement

*"Democratizing access to elite intellectual capital through a secure, multi-modal platform that enables meaningful connections between accomplished minds and those who seek their guidance."*

### 1.3 Key Value Propositions

| Stakeholder | Value Delivered |
|-------------|-----------------|
| **Experts** | Monetize expertise, give back through pro-bono work, secure & verified interactions |
| **Seekers** | Access to world-class mentors, verified experts, secure transactions |
| **Non-Profits** | Free expert services, CSR reporting, volunteer matching |
| **Platform** | Commission revenue, marketplace fees, premium subscriptions |

---

## 2. Project Overview

### 2.1 System Context

```
+------------------+     +--------------------+     +------------------+
|     Seekers      |<--->|    XpertConnect    |<--->|     Experts      |
| (Individuals/Orgs)|     |      Platform      |     | (Accomplished    |
+------------------+     +--------------------+     |     Minds)       |
                               ^    ^              +------------------+
                               |    |
        +----------------------+    +----------------------+
        |                                                  |
+------------------+                              +------------------+
|   Non-Profit     |                              | Payment/Escrow   |
|  Organizations   |                              |    Providers     |
+------------------+                              +------------------+
```

### 2.2 User Personas

#### Expert Categories

| Category | Description | Example | Verification Level |
|----------|-------------|---------|-------------------|
| **Category A** | Subject Matter Experts (SMEs) | PhD researchers, Certified professionals | Academic credentials, certifications |
| **Category B** | C-Suite & Executive Leaders | CEOs, CFOs, Board members | SEC filings, company registries |
| **Category C** | High-Profile Figures | Celebrities, Billionaires, Public figures | Official representatives, security assessment |

#### Seeker Types

| Type | Description | Access Level |
|------|-------------|--------------|
| **Standard Seeker** | Basic verified user | Professional consultations |
| **Premium Seeker** | Financially pre-qualified | High-value auctions, VIP access |
| **Enterprise Seeker** | Corporate accounts | Bulk bookings, team access |

### 2.3 Technology Stack

| Layer | Technology | Version |
|-------|------------|---------|
| **Frontend** | Next.js + TypeScript + Tailwind CSS | 14.x |
| **Backend** | ASP.NET Core + C# | 10.0 LTS |
| **Database** | PostgreSQL | 17.x |
| **Cache** | Redis | Latest |
| **Search** | Elasticsearch | Latest |
| **Real-Time** | SignalR | Integrated |
| **Cloud** | Microsoft Azure | - |

---

## 3. Business Models

### 3.1 Auction Model

**Purpose:** High-value charity auctions for exclusive access to celebrities/executives

#### Auction Lifecycle

```
Draft → Scheduled → Open → Closed → Winner Selected → Funded → Meeting → Disbursed
```

#### Key Features

- **Proxy Bidding:** Automatic bidding up to a maximum amount
- **Buy Now Option:** Instant purchase at fixed price
- **Guest Management:** Winner can bring verified guests (up to limit)
- **Escrow Protection:** Funds held securely until meeting verified
- **Geofencing:** GPS verification of physical meeting attendance

#### Typical Use Case

> Warren Buffett-style charity lunch where a high-net-worth individual bids $250,000 for a 2-hour lunch, proceeds go to charity, and the meeting is coordinated by platform concierge.

### 3.2 Professional Fee Model

**Purpose:** Paid consultations with verified experts

#### Consultation Lifecycle

```
Discovery → Booking → NDA Signed → Payment Authorized → Meeting → Feedback → Settlement
```

#### Key Features

- **Expert Discovery:** Search/filter by industry, skills, rate, availability
- **Calendar Integration:** Google/Outlook calendar sync
- **Automated NDAs:** Digital contract generation and signing
- **Virtual Hub:** Secure video conferencing
- **Feedback System:** Ratings and reviews

#### Pricing Tiers

| Expert Level | Typical Rate | Duration Options |
|--------------|--------------|------------------|
| SME | $100-$300/hr | 30min, 1hr, 2hr |
| Executive | $400-$800/hr | 1hr, 2hr |
| C-Suite | $800-$1,400/hr | 1hr, 2hr |

### 3.3 Pro-Bono Model

**Purpose:** Volunteer expert services for non-profit organizations

#### Project Lifecycle

```
Need Posted → Expert Applies → Matched → MOU Signed → Execution → Hours Tracked → CSR Report
```

#### Key Features

- **Project Scoping:** Non-profits define specific needs and deliverables
- **Expert Matching:** Skills-based matching with application process
- **Time Tracking:** Log volunteer hours with descriptions
- **CSR Reporting:** Automated reports for corporate social responsibility
- **Certificates:** Volunteer recognition for tax/resume purposes

---

## 4. Functional Requirements

### 4.1 Authentication & Authorization (FR-AUTH)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-AUTH-01 | User registration with email verification | High | Complete |
| FR-AUTH-02 | JWT-based authentication with refresh tokens | High | Complete |
| FR-AUTH-03 | Role-based access control (Admin, Expert, Seeker, NonProfit) | High | Complete |
| FR-AUTH-04 | Password reset functionality | Medium | Complete |
| FR-AUTH-05 | Token revocation capability | Medium | Complete |

### 4.2 User Management (FR-USER)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-USER-01 | User profile CRUD operations | High | Complete |
| FR-USER-02 | Profile photo upload | Medium | Complete |
| FR-USER-03 | Account activation/deactivation (Admin) | High | Complete |
| FR-USER-04 | Change password functionality | Medium | Complete |
| FR-USER-05 | User search and listing (Admin) | Medium | Complete |

### 4.3 Expert Management (FR-EXPERT)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-EXPERT-01 | Expert profile creation with bio, headline, photo | High | Complete |
| FR-EXPERT-02 | Hourly rate configuration | High | Complete |
| FR-EXPERT-03 | Availability management (time slots) | High | Complete |
| FR-EXPERT-04 | Credential management (add/remove/verify) | High | Complete |
| FR-EXPERT-05 | Expert search with filters (category, rate, skills) | High | Complete |
| FR-EXPERT-06 | Expert verification workflow | Medium | Partial |

### 4.4 Seeker Management (FR-SEEKER)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-SEEKER-01 | Seeker profile creation | High | Complete |
| FR-SEEKER-02 | Company/job information capture | Medium | Complete |
| FR-SEEKER-03 | KYC verification integration | High | Complete |
| FR-SEEKER-04 | Premium seeker upgrade path | Medium | Complete |

### 4.5 Consultation Module (FR-CONSULT)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-CONSULT-01 | Consultation booking with availability check | High | Complete |
| FR-CONSULT-02 | Booking overlap detection | High | Complete |
| FR-CONSULT-03 | 16-state status workflow | High | Complete |
| FR-CONSULT-04 | Reschedule functionality | Medium | Complete |
| FR-CONSULT-05 | Cancellation with policy enforcement | Medium | Complete |
| FR-CONSULT-06 | Virtual meeting link generation | High | Complete |
| FR-CONSULT-07 | Consultation completion confirmation | High | Complete |

### 4.6 Feedback System (FR-FEEDBACK)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-FEEDBACK-01 | Post-consultation feedback submission | High | Complete |
| FR-FEEDBACK-02 | 5-star rating system | High | Complete |
| FR-FEEDBACK-03 | Expert rating summary (average, count) | High | Complete |
| FR-FEEDBACK-04 | Feedback retrieval by consultation | Medium | Complete |

### 4.7 Auction Module (FR-AUCTION)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-AUCTION-01 | Auction lot creation with parameters | High | Complete |
| FR-AUCTION-02 | Auction lifecycle management (publish, open, close, cancel) | High | Complete |
| FR-AUCTION-03 | Bid placement with validation | High | Complete |
| FR-AUCTION-04 | Proxy bidding support | Medium | Complete |
| FR-AUCTION-05 | Buy Now functionality | Medium | Complete |
| FR-AUCTION-06 | Winner selection on close | High | Complete |
| FR-AUCTION-07 | Anonymized bid history (initials only) | Medium | Complete |
| FR-AUCTION-08 | Auction search with filters | High | Complete |

### 4.8 Pro-Bono Module (FR-PROBONO)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-PROBONO-01 | Project creation by non-profits | High | Complete |
| FR-PROBONO-02 | Project lifecycle management | High | Complete |
| FR-PROBONO-03 | Expert application workflow | High | Complete |
| FR-PROBONO-04 | Accept/reject expert applications | High | Complete |
| FR-PROBONO-05 | Time entry logging | High | Complete |
| FR-PROBONO-06 | CSR hours reporting | High | Complete |
| FR-PROBONO-07 | Project search with filters | Medium | Complete |

### 4.9 Payment & Escrow (FR-PAYMENT)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-PAYMENT-01 | Payment authorization | High | Complete |
| FR-PAYMENT-02 | Payment capture | High | Complete |
| FR-PAYMENT-03 | Refund processing | High | Complete |
| FR-PAYMENT-04 | Payment cancellation | Medium | Complete |
| FR-PAYMENT-05 | Escrow account creation | High | Complete |
| FR-PAYMENT-06 | Escrow funding | High | Complete |
| FR-PAYMENT-07 | Milestone-based releases | High | Complete |
| FR-PAYMENT-08 | Dispute handling | Medium | Complete |

### 4.10 Real-Time Features (FR-REALTIME)

| ID | Requirement | Priority | Status |
|----|-------------|----------|--------|
| FR-REALTIME-01 | Real-time bid updates via SignalR | High | Complete |
| FR-REALTIME-02 | Outbid notifications | High | Complete |
| FR-REALTIME-03 | Auction countdown timer | Medium | Complete |
| FR-REALTIME-04 | User notifications hub | High | Complete |
| FR-REALTIME-05 | Connection state management | Medium | Complete |

---

## 5. Technical Architecture

### 5.1 Clean Architecture Layers

```
+------------------------------------------------------------------+
|                        API Layer                                   |
|  Controllers, Middleware, Hubs (SignalR), Authentication          |
+------------------------------------------------------------------+
                              |
                              v
+------------------------------------------------------------------+
|                    Application Layer                               |
|  DTOs, Interfaces, Validators, Mapping Profiles, Business Logic   |
+------------------------------------------------------------------+
                              |
                              v
+------------------------------------------------------------------+
|                   Infrastructure Layer                             |
|  DbContext, Repositories, External Services, Migrations           |
+------------------------------------------------------------------+
                              |
                              v
+------------------------------------------------------------------+
|                      Domain Layer                                  |
|  Entities, Enums, Value Objects (No dependencies)                 |
+------------------------------------------------------------------+
```

### 5.2 Project Structure

```
src/backend/
├── XpertConnect.slnx                    # Solution file
└── src/
    ├── XpertConnect.Domain/             # Entities, Enums
    │   ├── Entities/
    │   │   ├── User.cs
    │   │   ├── Expert.cs
    │   │   ├── Seeker.cs
    │   │   ├── AuctionLot.cs
    │   │   ├── Bid.cs
    │   │   ├── Consultation.cs
    │   │   ├── ProBonoProject.cs
    │   │   ├── Payment.cs
    │   │   ├── EscrowAccount.cs
    │   │   └── ... (21 total entities)
    │   └── Enums/
    │       ├── UserType.cs
    │       ├── AuctionStatus.cs
    │       ├── BookingStatus.cs
    │       └── ... (12 total enums)
    │
    ├── XpertConnect.Application/        # Business Logic
    │   ├── Common/
    │   │   ├── Interfaces/              # Repository interfaces
    │   │   └── Mappings/                # AutoMapper profiles
    │   └── Features/
    │       ├── Auth/DTOs/
    │       ├── Users/DTOs/
    │       ├── Experts/DTOs/
    │       ├── Seekers/DTOs/
    │       ├── Consultations/DTOs/
    │       ├── Feedback/DTOs/
    │       ├── Auctions/DTOs/
    │       ├── Bids/DTOs/
    │       ├── Projects/DTOs/
    │       ├── TimeEntries/DTOs/
    │       ├── Payments/DTOs/
    │       ├── Escrow/DTOs/
    │       └── Notifications/DTOs/
    │
    ├── XpertConnect.Infrastructure/     # Data Access
    │   ├── Data/
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── Configurations/          # Entity configurations
    │   │   └── Migrations/
    │   ├── Repositories/
    │   │   ├── UserRepository.cs
    │   │   ├── ExpertRepository.cs
    │   │   ├── ConsultationRepository.cs
    │   │   └── ... (12 total repositories)
    │   └── Services/
    │       ├── TokenService.cs
    │       ├── MockPaymentService.cs
    │       └── ...
    │
    └── XpertConnect.API/                # Web API
        ├── Controllers/
        │   ├── AuthController.cs
        │   ├── UsersController.cs
        │   ├── ExpertsController.cs
        │   ├── SeekersController.cs
        │   ├── ConsultationsController.cs
        │   ├── FeedbackController.cs
        │   ├── AuctionsController.cs
        │   ├── BidsController.cs
        │   ├── ProjectsController.cs
        │   ├── TimeEntriesController.cs
        │   ├── PaymentsController.cs
        │   └── EscrowController.cs
        ├── Hubs/
        │   ├── AuctionHub.cs
        │   └── NotificationHub.cs
        └── Program.cs
```

### 5.3 Key Design Patterns

| Pattern | Usage |
|---------|-------|
| **Repository Pattern** | Data access abstraction |
| **Unit of Work** | Transaction management via DbContext |
| **DTO Pattern** | Data transfer between layers |
| **Dependency Injection** | Service registration and resolution |
| **CQRS (Light)** | Separate read/write DTOs |

---

## 6. API Specification

### 6.1 Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login and get tokens | No |
| POST | `/api/auth/refresh-token` | Refresh access token | No |
| POST | `/api/auth/revoke-token` | Revoke refresh token | Yes |
| GET | `/api/auth/me` | Get current user info | Yes |

### 6.2 User Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | List all users | Admin |
| GET | `/api/users/{id}` | Get user by ID | Yes |
| GET | `/api/users/me` | Get current user | Yes |
| PUT | `/api/users/{id}` | Update user | Admin/Self |
| PUT | `/api/users/me` | Update current user | Yes |
| DELETE | `/api/users/{id}` | Delete user | Admin |
| POST | `/api/users/me/change-password` | Change password | Yes |
| POST | `/api/users/{id}/activate` | Activate user | Admin |
| POST | `/api/users/{id}/deactivate` | Deactivate user | Admin |

### 6.3 Expert Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/experts` | Search/browse experts | No |
| GET | `/api/experts/{id}` | Get expert public profile | No |
| GET | `/api/experts/me` | Get own expert profile | Expert |
| POST | `/api/experts/me` | Create expert profile | Yes |
| PUT | `/api/experts/me` | Update expert profile | Expert |
| POST | `/api/experts/me/availability` | Add availability slot | Expert |
| DELETE | `/api/experts/me/availability/{id}` | Remove availability | Expert |
| POST | `/api/experts/me/credentials` | Add credential | Expert |
| DELETE | `/api/experts/me/credentials/{id}` | Remove credential | Expert |

### 6.4 Consultation Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/consultations` | Book consultation | Seeker |
| GET | `/api/consultations/{id}` | Get consultation | Yes |
| GET | `/api/consultations/my` | Get seeker's consultations | Seeker |
| GET | `/api/consultations/expert` | Get expert's consultations | Expert |
| PUT | `/api/consultations/{id}/status` | Update status | Yes |
| POST | `/api/consultations/{id}/reschedule` | Reschedule | Yes |
| POST | `/api/consultations/{id}/cancel` | Cancel | Yes |
| POST | `/api/consultations/{id}/complete` | Mark complete | Yes |
| POST | `/api/consultations/{id}/feedback` | Submit feedback | Seeker |
| GET | `/api/consultations/{id}/feedback` | Get feedback | Yes |
| GET | `/api/experts/{id}/feedback` | Get expert's feedback | No |

### 6.5 Auction Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/auctions` | Browse auctions | No |
| GET | `/api/auctions/{id}` | Get auction details | No |
| GET | `/api/auctions/my` | Get expert's auctions | Expert |
| POST | `/api/auctions` | Create auction | Expert |
| PUT | `/api/auctions/{id}` | Update auction | Expert |
| POST | `/api/auctions/{id}/publish` | Publish auction | Expert |
| POST | `/api/auctions/{id}/open` | Open bidding | Expert |
| POST | `/api/auctions/{id}/close` | Close auction | Expert |
| POST | `/api/auctions/{id}/cancel` | Cancel auction | Expert |
| GET | `/api/auctions/{id}/bids` | Get bid history | Yes |
| POST | `/api/auctions/{id}/bids` | Place bid | Seeker |
| GET | `/api/bids/my` | Get seeker's bids | Seeker |
| GET | `/api/bids/{id}` | Get bid details | Yes |

### 6.6 Pro-Bono Project Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/projects` | Browse projects | No |
| GET | `/api/projects/{id}` | Get project details | No |
| GET | `/api/projects/my` | Get expert's projects | Expert |
| GET | `/api/projects/my-org` | Get org's projects | NonProfit |
| POST | `/api/projects` | Create project | NonProfit |
| PUT | `/api/projects/{id}` | Update project | NonProfit |
| POST | `/api/projects/{id}/publish` | Publish project | NonProfit |
| POST | `/api/projects/{id}/apply` | Apply to project | Expert |
| POST | `/api/projects/{id}/accept` | Accept expert | NonProfit |
| POST | `/api/projects/{id}/reject` | Reject expert | NonProfit |
| POST | `/api/projects/{id}/start` | Start project | NonProfit |
| POST | `/api/projects/{id}/complete` | Complete project | NonProfit |
| POST | `/api/projects/{id}/cancel` | Cancel project | NonProfit |

### 6.7 Time Entry Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/time-entries/project/{projectId}` | Log time | Expert |
| GET | `/api/time-entries/project/{projectId}` | Get project entries | Yes |
| GET | `/api/time-entries/my` | Get expert's entries | Expert |
| GET | `/api/time-entries/{id}` | Get entry details | Yes |
| PUT | `/api/time-entries/{id}` | Update entry | Expert |
| DELETE | `/api/time-entries/{id}` | Delete entry | Expert |
| GET | `/api/time-entries/summary` | Get CSR summary | Expert |
| GET | `/api/time-entries/project/{projectId}/total` | Get project total | Yes |

### 6.8 Payment Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/payments/authorize` | Authorize payment | Yes |
| POST | `/api/payments/{id}/capture` | Capture payment | Yes |
| POST | `/api/payments/{id}/refund` | Refund payment | Yes |
| POST | `/api/payments/{id}/cancel` | Cancel payment | Yes |
| GET | `/api/payments/{id}` | Get payment details | Yes |
| GET | `/api/payments/my` | Get user's payments | Yes |

### 6.9 Escrow Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/escrow` | Create escrow | Yes |
| GET | `/api/escrow/{id}` | Get escrow details | Yes |
| POST | `/api/escrow/{id}/fund` | Fund escrow | Yes |
| POST | `/api/escrow/{id}/release` | Release funds | Yes |
| POST | `/api/escrow/{id}/dispute` | Initiate dispute | Yes |
| POST | `/api/escrow/{id}/milestones` | Add milestone | Yes |
| POST | `/api/escrow/milestones/{id}/approve` | Approve milestone | Yes |

### 6.10 SignalR Hubs

| Hub | Endpoint | Methods |
|-----|----------|---------|
| **AuctionHub** | `/hubs/auction` | JoinAuction, LeaveAuction, GetAuctionState, Ping |
| **NotificationHub** | `/hubs/notifications` | Subscribe, Unsubscribe, MarkAsRead |

---

## 7. Database Schema

### 7.1 Core Tables

| Table | Description | Key Fields |
|-------|-------------|------------|
| `Users` | Base user information | Id, Email, UserType, VerificationStatus |
| `Experts` | Expert profiles | UserId, Category, HourlyRate, Bio |
| `Seekers` | Seeker profiles | UserId, KycStatus, BidEligible |
| `NonProfitOrgs` | Non-profit organizations | UserId, OrgName, TaxId, Mission |

### 7.2 Business Tables

| Table | Description | Key Fields |
|-------|-------------|------------|
| `AuctionLots` | Charity auctions | ExpertId, Title, StartingBid, BuyNowPrice, Status |
| `Bids` | Auction bids | AuctionId, SeekerId, Amount, IsProxyBid |
| `Consultations` | Paid sessions | ExpertId, SeekerId, ScheduledAt, Rate, Status |
| `ProBonoProjects` | Volunteer projects | OrgId, ExpertId, Title, Status |
| `TimeEntries` | Volunteer hours | ProjectId, ExpertId, Hours, Description |
| `Feedback` | Session ratings | ConsultationId, SeekerId, Rating, Comments |

### 7.3 Financial Tables

| Table | Description | Key Fields |
|-------|-------------|------------|
| `Payments` | Payment records | Amount, Currency, Status, StripePaymentId |
| `EscrowAccounts` | Escrow holding | PaymentId, Amount, Status |
| `Milestones` | Payment milestones | EscrowId, Amount, DueDate, Approved |

### 7.4 Supporting Tables

| Table | Description | Key Fields |
|-------|-------------|------------|
| `Credentials` | Expert credentials | ExpertId, Type, IssuingBody, VerifiedAt |
| `ExpertAvailabilities` | Time slots | ExpertId, DayOfWeek, StartTime, EndTime |
| `NDAs` | Legal agreements | PartyAId, PartyBId, SignedAt |
| `MOUs` | Project agreements | ProjectId, Scope, Timeline |
| `Venues` | Meeting locations | Name, Address, Latitude, Longitude |
| `Geofences` | GPS boundaries | VenueId, Radius, DwellDuration |

---

## 8. Security & Compliance

### 8.1 Authentication Security

| Feature | Implementation |
|---------|----------------|
| Password Hashing | ASP.NET Identity (PBKDF2) |
| Token Type | JWT (JSON Web Tokens) |
| Token Expiry | Access: 15 min, Refresh: 7 days |
| Token Storage | HttpOnly cookies recommended |

### 8.2 Authorization Model

| Role | Permissions |
|------|-------------|
| **Admin** | Full system access, user management, dispute resolution |
| **Expert** | Manage own profile, auctions, consultations, projects |
| **Seeker** | Book consultations, place bids, submit feedback |
| **NonProfit** | Create projects, manage volunteers, generate reports |

### 8.3 Data Protection

| Measure | Description |
|---------|-------------|
| HTTPS | All API communication encrypted |
| Input Validation | FluentValidation on all inputs |
| SQL Injection | Prevented via Entity Framework parameterization |
| XSS Protection | Input sanitization, Content-Security-Policy |

### 8.4 KYC/Verification

| Level | Requirements |
|-------|--------------|
| **Basic** | Email verification, phone verification |
| **Standard** | Identity document, biometric liveness check |
| **Premium** | Financial references, bank statement validation |

---

## 9. Implementation Status

### 9.1 Completed Phases (1-13)

| Phase | Name | Key Deliverables |
|-------|------|------------------|
| 1 | Database Setup | PostgreSQL, EF Core migrations, 25 tables |
| 2 | Authentication | JWT tokens, refresh tokens, role-based auth |
| 3 | User Management | CRUD operations, password management |
| 4 | Expert & Seeker Profiles | Profile management, search, credentials |
| 5 | Consultation Module | Booking, 16-state workflow, virtual meetings |
| 6 | Auction Module | Bidding, proxy bids, Buy Now, lifecycle |
| 7 | Pro-Bono Module | Projects, applications, time tracking |
| 8 | Payment & Escrow | Authorize/capture/refund, milestones |
| 9 | Real-Time Features | SignalR hubs, bid notifications |
| 10 | Frontend - Core Pages | Auth, dashboard, profiles |
| 11 | Frontend - Business Flows | Booking, auctions, projects |
| 12 | Integrations | KYC, payment services |
| 13 | Testing & QA | Unit tests, integration tests |

### 9.2 Pending Phase

| Phase | Name | Status |
|-------|------|--------|
| 14 | Deployment Setup | Not Started |

**Phase 14 includes:**
- Docker containerization
- Azure infrastructure setup
- CI/CD pipelines (GitHub Actions)
- Monitoring and alerting
- SSL/security configuration

### 9.3 Feature Completion Summary

| Module | Endpoints | Status |
|--------|-----------|--------|
| Authentication | 5 | 100% |
| Users | 10 | 100% |
| Experts | 9 | 100% |
| Seekers | 5 | 100% |
| Consultations | 10 | 100% |
| Feedback | 3 | 100% |
| Auctions | 10 | 100% |
| Bids | 3 | 100% |
| Projects | 13 | 100% |
| Time Entries | 8 | 100% |
| Payments | 6 | 100% |
| Escrow | 7 | 100% |
| SignalR Hubs | 2 | 100% |

**Total: 91 API endpoints implemented**

---

## Appendix A: Glossary

| Term | Definition |
|------|------------|
| **Accomplished Mind** | Expert, executive, or celebrity offering their time/expertise |
| **Seeker** | Individual or organization seeking access to experts |
| **Auction Lot** | A listing for a high-value charity auction |
| **Proxy Bid** | Automatic bidding up to a specified maximum |
| **Escrow** | Secure holding of funds until conditions are met |
| **Geofencing** | GPS-based verification of physical meeting attendance |
| **CSR** | Corporate Social Responsibility |
| **MOU** | Memorandum of Understanding |
| **NDA** | Non-Disclosure Agreement |
| **KYC** | Know Your Customer (identity verification) |

---

## Appendix B: Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | April 2026 | XpertConnect Team | Initial release for demo |

---

*This document is intended for demo purposes and represents the current state of the XpertConnect platform implementation.*
