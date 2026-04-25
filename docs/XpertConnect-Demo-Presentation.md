---
marp: true
theme: default
paginate: true
backgroundColor: #fff
style: |
  section {
    font-family: 'Segoe UI', Arial, sans-serif;
  }
  h1 {
    color: #1e40af;
  }
  h2 {
    color: #3b82f6;
  }
  table {
    font-size: 0.8em;
  }
  .columns {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
  }
---

<!-- _class: lead -->
<!-- _backgroundColor: #1e40af -->
<!-- _color: white -->

# XpertConnect

## Connecting Accomplished Minds with Seekers

**A Marketplace for Elite Expertise**

---

# The Problem

## Access to Elite Expertise is Broken

- **Opaque Barriers** - No clear path to reach industry leaders
- **No Trust Layer** - How do you verify credentials?
- **No Structure** - Ad-hoc networking doesn't scale
- **High Risk** - Large transactions without protection
- **Wasted Potential** - Experts want to give back but lack channels

---

# The Solution

## XpertConnect: "Airbnb for Expertise"

A structured, secure, multi-modal platform that connects:

| Accomplished Minds | Seekers |
|-------------------|---------|
| Industry Titans | Entrepreneurs |
| Retired CEOs | Startup Founders |
| Subject Matter Experts | Business Leaders |
| Celebrities & Philanthropists | High-Net-Worth Individuals |
| Non-Profit Volunteers | Charities & NGOs |

---

# Three Business Models

![bg right:40% 80%](https://via.placeholder.com/400x600/3b82f6/ffffff?text=3+Models)

## 1. Auction Model
High-value charity auctions (Warren Buffett style)

## 2. Professional Fee Model
Paid consultations ($100 - $1,400/hr)

## 3. Pro-Bono Model
Free volunteer work for non-profits

---

# Model 1: Charity Auctions

## "Win a Lunch with a Legend"

**Real Example:** Warren Buffett's annual lunch auction raised $19M in 2022

### How It Works:
1. Expert offers exclusive experience (lunch, meeting)
2. Seekers bid competitively
3. Highest bidder wins
4. Proceeds go to charity
5. Platform handles all logistics & security

**Target:** $25,000 - $19,000,000+ transactions

---

# Model 2: Professional Consultations

## "Expert Advice on Demand"

### How It Works:
1. Seeker browses expert profiles
2. Views hourly rates ($100 - $1,400/hr)
3. Books available time slot
4. Signs mutual NDA automatically
5. Conducts video/in-person meeting
6. Payment released after completion

**Target:** Quick, transactional expert access

---

# Model 3: Pro-Bono Projects

## "Skills for Good"

### How It Works:
1. Non-profit posts project need
2. Expert volunteers apply
3. Non-profit selects best match
4. MOU signed for accountability
5. Expert delivers work (tracked hours)
6. CSR reports generated for tax deductions

**Target:** Social impact & corporate giving

---

<!-- _class: lead -->
<!-- _backgroundColor: #059669 -->
<!-- _color: white -->

# Technical Architecture

---

# Tech Stack Overview

| Layer | Technology | Why |
|-------|------------|-----|
| **Frontend** | Next.js 14 + TypeScript + Tailwind | Modern React, SSR, type safety |
| **Backend** | ASP.NET Core 10 + C# 13 | Enterprise-grade, LTS until 2028 |
| **Real-Time** | SignalR | Live bidding, notifications |
| **Database** | PostgreSQL 17 | Robust, ACID-compliant |
| **Cache** | Redis | Session, real-time data |
| **Search** | Elasticsearch | Expert discovery |
| **Cloud** | Azure | Enterprise hosting |

---

# Clean Architecture

```
XpertConnect/
├── Domain/           # Entities, Enums (no dependencies)
├── Application/      # Business logic, DTOs, Interfaces
├── Infrastructure/   # DbContext, Repositories, Services
└── API/              # Controllers, Middleware, Hubs
```

### Benefits:
- **Separation of Concerns** - Each layer has single responsibility
- **Testability** - Easy to mock dependencies
- **Maintainability** - Changes isolated to specific layers
- **Scalability** - Can swap implementations easily

---

# Domain Entities

| Entity | Purpose |
|--------|---------|
| `User` | Base user (Seeker, Expert, NonProfit) |
| `Expert` | Accomplished mind with credentials |
| `Seeker` | Person seeking expert access |
| `AuctionLot` | High-value charity auction |
| `Bid` | Auction bid with proxy support |
| `Consultation` | Paid expert session |
| `ProBonoProject` | Volunteer work for charities |
| `Payment` | Financial transaction |
| `EscrowAccount` | Secure payment holding |

---

# API Architecture

## RESTful Endpoints

| Module | Base Route | Status |
|--------|------------|--------|
| Authentication | `/api/auth` | ✅ Complete |
| Users | `/api/users` | ✅ Complete |
| Experts | `/api/experts` | ✅ Complete |
| Consultations | `/api/consultations` | ✅ Complete |
| Auctions | `/api/auctions` | ✅ Complete |
| Bids | `/api/bids` | ✅ Complete |
| Projects | `/api/projects` | ✅ Complete |
| Payments | `/api/payments` | ✅ Complete |

---

# Real-Time Features (SignalR)

## Live Auction Bidding

```
/hubs/auction
├── JoinAuction(auctionId)    → Subscribe to updates
├── NewBid                     → Real-time bid notification
├── Outbid                     → Notify previous high bidder
└── AuctionClosed              → Winner announcement
```

## User Notifications

```
/hubs/notifications
├── ConsultationReminder       → Upcoming meeting alerts
├── PaymentReceived            → Payment confirmations
└── ProjectApplication         → Expert applied to project
```

---

<!-- _class: lead -->
<!-- _backgroundColor: #7c3aed -->
<!-- _color: white -->

# Key Features

---

# Security & Verification

## Multi-Layer Trust System

| Layer | What We Verify |
|-------|----------------|
| **Identity (KYC)** | Biometric liveness, document auth |
| **Financial** | Bank references, credit validation |
| **Credentials** | LinkedIn, company records, publications |
| **Background** | Sanctions lists, PEP screening |
| **Reputation** | Social media monitoring |

**Providers:** Onfido/Jumio for KYC, Escrow.com for high-value transactions

---

# Payment & Escrow System

## Secure Transaction Flow

```
1. Seeker wins auction ($100,000)
         ↓
2. Funds transferred to Escrow
         ↓
3. Meeting occurs (GPS verified)
         ↓
4. Escrow releases to Charity/Expert
```

### Payment Methods:
- **Stripe Connect** - Standard payments ($100 - $5,000)
- **Milestone Escrow** - Long-term projects
- **High-Value Escrow** - Auction wins ($10K+)

---

# Physical Meeting Logistics

## White-Glove Service

| Security Layer | Implementation |
|----------------|----------------|
| **Venue Vetting** | Pre-approved restaurants, private rooms |
| **Geofencing** | GPS verification both parties present |
| **Device Policy** | Phone-free meetings (Yondr pouches) |
| **Executive Protection** | Bodyguards if required |
| **TSCM** | Sweep for recording devices |

**Goal:** "Visible but not imposing" security

---

# Consultation Booking Flow

## User Journey

```
Browse Experts → View Profile → Check Availability
        ↓
Select Date/Time → Choose Duration (30min - 2hrs)
        ↓
Select Meeting Type (Virtual/In-Person)
        ↓
Add Notes → Auto-generate NDA → Pay
        ↓
Meeting Happens → Rate & Review
```

---

<!-- _class: lead -->
<!-- _backgroundColor: #dc2626 -->
<!-- _color: white -->

# Demo Walkthrough

---

# Demo: Seeker Books a Consultation

## Step 1: Browse Experts

- Filter by category, rate, availability
- View verified credentials
- Check ratings and reviews

## Step 2: View Expert Profile

- See headline, bio, experience
- Check hourly rate
- View available time slots

---

# Demo: Booking Page

## `/book/[expertId]`

**Features shown:**
- Calendar date picker
- Available time slots (from expert's schedule)
- Duration selection (30min, 1hr, 1.5hrs, 2hrs)
- Meeting type toggle (Virtual/In-Person)
- Notes field for discussion topics
- Real-time price calculation
- Booking summary sidebar

---

# Demo: Live Auction

## Real-Time Bidding

**Features shown:**
- Live bid updates (SignalR)
- Current high bid display
- Countdown timer
- Proxy bidding support
- Buy Now option
- Bid history (anonymized)
- Outbid notifications

---

# Demo: Pro-Bono Projects

## Volunteer Matching

**Features shown:**
- Project listing with skill requirements
- Expert application flow
- Non-profit approval process
- Time tracking dashboard
- CSR hours report generation

---

<!-- _class: lead -->
<!-- _backgroundColor: #0891b2 -->
<!-- _color: white -->

# Implementation Progress

---

# Backend Progress

| Phase | Module | Status |
|-------|--------|--------|
| 1 | Database Setup (PostgreSQL) | ✅ Complete |
| 2 | Authentication (JWT + Identity) | ✅ Complete |
| 3 | User Management APIs | ✅ Complete |
| 4 | Expert & Seeker Profiles | ✅ Complete |
| 5 | Consultation Module | ✅ Complete |
| 6 | Auction Module | ✅ Complete |
| 7 | Pro-Bono Module | ✅ Complete |
| 8 | Payment & Escrow | ✅ Complete |
| 9 | Real-Time (SignalR) | ✅ Complete |

---

# Frontend Progress

| Phase | Module | Status |
|-------|--------|--------|
| 10 | Core Pages (Layout, Auth, Dashboard) | ✅ Complete |
| 11 | Business Flows (Booking, Auctions) | ✅ Complete |
| 12 | Third-Party Integrations | ✅ Complete |
| 13 | Testing & QA | ✅ Complete |
| 14 | Deployment Setup | ⬜ Pending |

**Overall: 13/14 phases complete (93%)**

---

# What's Built

## Backend (100% Core Complete)

- 25+ database tables with EF Core
- 50+ API endpoints
- JWT authentication with refresh tokens
- Role-based authorization (Admin, Expert, Seeker, NonProfit)
- SignalR hubs for real-time features
- Mock payment service (Stripe-ready)

## Frontend

- Next.js 14 with App Router
- Responsive UI components
- Expert browse & booking flows
- Real-time auction interface

---

<!-- _class: lead -->
<!-- _backgroundColor: #f59e0b -->
<!-- _color: white -->

# Roadmap & Next Steps

---

# Phase 14: Deployment

## Remaining Tasks

| Task | Technology |
|------|------------|
| Docker containers | API + Frontend Dockerfiles |
| Azure setup | App Service, PostgreSQL, Redis |
| CI/CD Pipeline | GitHub Actions |
| Monitoring | Application Insights |
| Security | Key Vault, WAF, SSL |

---

# Future Enhancements

## Planned Features

- **Mobile App** - React Native for iOS/Android
- **AI Matching** - Smart expert recommendations
- **Video Integration** - Built-in meeting platform
- **Advanced Analytics** - Expert earnings dashboard
- **Multi-Language** - Internationalization
- **Blockchain** - Smart contracts for escrow

---

# Revenue Model

| Revenue Stream | Rate |
|----------------|------|
| Consultation Commission | 10-20% of transaction |
| Auction Success Fee | 5-15% of hammer price |
| Premium Expert Profiles | Monthly subscription |
| Featured Listings | Pay-per-impression |
| Enterprise Plans | Custom pricing |

---

<!-- _class: lead -->
<!-- _backgroundColor: #1e40af -->
<!-- _color: white -->

# Thank You

## Questions?

**XpertConnect**
*Connecting Accomplished Minds with Seekers*

---

# Appendix: Commands Reference

```bash
# Backend
cd src/backend
dotnet build                          # Build solution
dotnet run --project src/XpertConnect.API  # Run API

# Frontend
cd src/frontend
npm run dev                           # Start dev server

# Database Migrations
dotnet ef migrations add <name> \
  --project src/XpertConnect.Infrastructure \
  --startup-project src/XpertConnect.API
```

---

# Appendix: Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                      FRONTEND                            │
│              Next.js 14 + TypeScript                     │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    API LAYER                             │
│         ASP.NET Core 10 + SignalR Hubs                   │
├─────────────────────────────────────────────────────────┤
│                 APPLICATION LAYER                        │
│          Business Logic, DTOs, Validators                │
├─────────────────────────────────────────────────────────┤
│               INFRASTRUCTURE LAYER                       │
│     Repositories, EF Core, External Services             │
├─────────────────────────────────────────────────────────┤
│                   DOMAIN LAYER                           │
│              Entities, Enums, Value Objects              │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              PostgreSQL + Redis + Elasticsearch          │
└─────────────────────────────────────────────────────────┘
```
