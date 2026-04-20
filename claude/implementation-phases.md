# XpertConnect - Phased Implementation Plan

> Reference this document to resume implementation from any phase.
> Update the checkboxes as you complete each task.

---

## Phase Overview

| Phase | Name | Status | Dependencies |
|-------|------|--------|--------------|
| 1 | Database Setup | ✅ Complete | None |
| 2 | Authentication & Authorization | ✅ Complete | Phase 1 |
| 3 | User Management APIs | ✅ Complete | Phase 2 |
| 4 | Expert & Seeker Profiles | ✅ Complete | Phase 3 |
| 5 | Consultation Module | ✅ Complete | Phase 4 |
| 6 | Auction Module | ✅ Complete | Phase 4 |
| 7 | Pro-Bono Module | ✅ Complete | Phase 4 |
| 8 | Payment & Escrow | ✅ Complete | Phase 5, 6 |
| 9 | Real-Time Features (SignalR) | ✅ Complete | Phase 6 |
| 10 | Frontend - Core Pages | ✅ Complete | Phase 3 |
| 11 | Frontend - Business Flows | ✅ Complete | Phase 5, 6, 7 |
| 12 | Integrations (KYC, Payments) | ✅ Complete | Phase 8 |
| 13 | Testing & QA | ✅ Complete | All Phases |
| 14 | Deployment Setup | ⬜ Not Started | Phase 13 |

---

## Phase 1: Database Setup

**Goal:** Get PostgreSQL database running with initial schema

### Tasks
- [x] 1.1 Install PostgreSQL 17 (if not installed) - PostgreSQL 16 installed
- [x] 1.2 Update connection string password in `appsettings.Development.json`
- [x] 1.3 Create database `xpertconnect_dev`
- [x] 1.4 Apply EF Core migration (InitialCreate)
- [x] 1.5 Verify tables created successfully (25 tables + migrations history)

### Commands
```bash
# Navigate to backend
cd src/backend

# Update password in appsettings.Development.json first, then:

# Create database
psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"

# Apply migration
dotnet ef database update --project src/XpertConnect.Infrastructure --startup-project src/XpertConnect.API

# Verify (list tables)
psql -U postgres -d xpertconnect_dev -c "\dt"
```

### Key Files
- `src/backend/src/XpertConnect.API/appsettings.Development.json`
- `src/backend/src/XpertConnect.Infrastructure/Data/Migrations/`

### Deliverables
- PostgreSQL database with all 21 tables created
- Connection verified from API project

---

## Phase 2: Authentication & Authorization

**Goal:** Implement user authentication with JWT tokens

### Tasks
- [x] 2.1 Add ASP.NET Identity packages
- [x] 2.2 Configure Identity in `ApplicationDbContext`
- [x] 2.3 Create Identity migration (AddIdentity)
- [x] 2.4 Implement JWT token service
- [x] 2.5 Create `AuthController` with endpoints:
  - [x] POST `/api/auth/register`
  - [x] POST `/api/auth/login`
  - [x] POST `/api/auth/refresh-token`
  - [x] POST `/api/auth/revoke-token`
  - [x] GET `/api/auth/me` (protected)
- [x] 2.6 Configure JWT in `Program.cs`
- [x] 2.7 Add role-based authorization policies (Admin, Expert, Seeker, NonProfit)
- [x] 2.8 Test authentication flow - All endpoints working

### Packages to Add
```bash
cd src/backend/src/XpertConnect.Infrastructure
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

cd ../XpertConnect.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Key Files to Create/Modify
- `src/backend/src/XpertConnect.Application/Common/Interfaces/ITokenService.cs`
- `src/backend/src/XpertConnect.Application/Features/Auth/DTOs/`
- `src/backend/src/XpertConnect.Infrastructure/Services/TokenService.cs`
- `src/backend/src/XpertConnect.API/Controllers/AuthController.cs`

### Deliverables
- User registration working
- Login returns JWT token
- Protected endpoints require valid token
- Role-based access control configured

---

## Phase 3: User Management APIs

**Goal:** CRUD operations for base user management

### Tasks
- [x] 3.1 Create User DTOs (Request/Response)
- [x] 3.2 Create `IUserRepository` interface
- [x] 3.3 Implement `UserRepository`
- [x] 3.4 Create `UsersController` with endpoints:
  - [x] GET `/api/users` (Admin only)
  - [x] GET `/api/users/{id}`
  - [x] PUT `/api/users/{id}` and PUT `/api/users/me`
  - [x] DELETE `/api/users/{id}`
  - [x] GET `/api/users/me` (Current user profile)
  - [x] POST `/api/users/me/change-password`
  - [x] POST `/api/users/{id}/activate` and `/deactivate` (Admin)
- [x] 3.5 Add validation using FluentValidation
- [x] 3.6 Implement AutoMapper profiles

### Packages to Add
```bash
cd src/backend/src/XpertConnect.Application
dotnet add package FluentValidation
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### Key Files to Create
- `src/backend/src/XpertConnect.Application/Features/Users/DTOs/`
- `src/backend/src/XpertConnect.Application/Features/Users/Validators/`
- `src/backend/src/XpertConnect.Application/Common/Mappings/UserProfile.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/UserRepository.cs`
- `src/backend/src/XpertConnect.API/Controllers/UsersController.cs`

### Deliverables
- User CRUD operations working
- Input validation in place
- Proper error responses

---

## Phase 4: Expert & Seeker Profiles

**Goal:** Profile management for experts and seekers

### Tasks
- [x] 4.1 Create Expert DTOs (ExpertResponse, ExpertListResponse, UpdateExpertProfileRequest, etc.)
- [x] 4.2 Create Seeker DTOs (SeekerResponse, CreateSeekerProfileRequest, UpdateSeekerProfileRequest)
- [ ] 4.3 Create NonProfitOrg DTOs (deferred to future phase)
- [x] 4.4 Implement `ExpertsController`:
  - [x] GET `/api/experts` (Browse/Search with filters)
  - [x] GET `/api/experts/{id}` (Public profile view)
  - [x] GET `/api/experts/me` (Own profile)
  - [x] POST `/api/experts/me` (Create expert profile)
  - [x] PUT `/api/experts/me` (Update profile)
  - [x] POST `/api/experts/me/availability` (Add availability)
  - [x] DELETE `/api/experts/me/availability/{id}` (Remove availability)
  - [x] POST `/api/experts/me/credentials` (Add credential)
  - [x] DELETE `/api/experts/me/credentials/{id}` (Remove credential)
- [x] 4.5 Implement `SeekersController`:
  - [x] GET `/api/seekers/{id}` (Admin/self only)
  - [x] GET `/api/seekers/me` (Own profile)
  - [x] POST `/api/seekers/me` (Create seeker profile)
  - [x] PUT `/api/seekers/me` (Update profile)
  - [x] DELETE `/api/seekers/{id}` (Admin only)
- [ ] 4.6 Implement `NonProfitsController` (deferred to future phase)
- [x] 4.7 Implement expert search with filters (category, rate, availability, verification status)
- [ ] 4.8 Add credential verification workflow (deferred - needs admin panel)

### Key Files Created
- `src/backend/src/XpertConnect.Application/Features/Experts/DTOs/` (7 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Experts/Validators/` (3 validators)
- `src/backend/src/XpertConnect.Application/Features/Seekers/DTOs/` (3 DTO files)
- `src/backend/src/XpertConnect.Application/Common/Mappings/ExpertMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Mappings/SeekerMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IExpertRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/ISeekerRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/ExpertRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/SeekerRepository.cs`
- `src/backend/src/XpertConnect.API/Controllers/ExpertsController.cs`
- `src/backend/src/XpertConnect.API/Controllers/SeekersController.cs`

### Deliverables
- ✅ Expert profiles with credentials and availability
- ✅ Seeker profiles with company/job info
- ✅ Expert search/browse with filters
- ✅ Role-based authorization (Expert role for expert endpoints, Seeker role for seeker endpoints)

---

## Phase 5: Consultation Module

**Goal:** Implement professional fee consultation booking

### Tasks
- [x] 5.1 Create Consultation DTOs (ConsultationResponse, CreateConsultationRequest, etc.)
- [x] 5.2 Create Feedback DTOs (FeedbackResponse, CreateFeedbackRequest, ExpertFeedbackSummary)
- [x] 5.3 Implement `ConsultationsController`:
  - [x] POST `/api/consultations` (Book consultation)
  - [x] GET `/api/consultations/{id}`
  - [x] GET `/api/consultations/my` (Seeker's consultations)
  - [x] GET `/api/consultations/expert` (Expert's consultations)
  - [x] PUT `/api/consultations/{id}/status`
  - [x] POST `/api/consultations/{id}/reschedule`
  - [x] POST `/api/consultations/{id}/cancel`
  - [x] POST `/api/consultations/{id}/complete`
- [x] 5.4 Implement `FeedbackController`:
  - [x] POST `/api/consultations/{id}/feedback`
  - [x] GET `/api/consultations/{id}/feedback`
  - [x] GET `/api/experts/{id}/feedback` (with rating summary)
- [x] 5.5 Implement booking status workflow (16 states with validation)
- [ ] 5.6 Add NDA generation/signing flow (deferred - needs document service)
- [ ] 5.7 Implement email notifications (deferred - needs email service)

### Key Files Created
- `src/backend/src/XpertConnect.Application/Features/Consultations/DTOs/` (6 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Consultations/Validators/` (2 validators)
- `src/backend/src/XpertConnect.Application/Features/Feedback/DTOs/` (3 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Feedback/Validators/` (1 validator)
- `src/backend/src/XpertConnect.Application/Common/Mappings/ConsultationMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Mappings/FeedbackMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IConsultationRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IFeedbackRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/ConsultationRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/FeedbackRepository.cs`
- `src/backend/src/XpertConnect.API/Controllers/ConsultationsController.cs`
- `src/backend/src/XpertConnect.API/Controllers/FeedbackController.cs`

### Deliverables
- ✅ Consultation booking with overlap detection
- ✅ Status transitions with role-based validation
- ✅ Virtual meeting link generation
- ✅ Feedback system with rating summary

---

## Phase 6: Auction Module

**Goal:** Implement charity auction system

### Tasks
- [x] 6.1 Create AuctionLot DTOs (AuctionResponse, AuctionListResponse, CreateAuctionRequest, UpdateAuctionRequest, AuctionQueryParams)
- [x] 6.2 Create Bid DTOs (BidResponse, PlaceBidRequest, BidHistoryResponse)
- [ ] 6.3 Create Guest DTOs (deferred - needs separate guest workflow)
- [x] 6.4 Implement `AuctionsController`:
  - [x] GET `/api/auctions` (Browse auctions with filters)
  - [x] GET `/api/auctions/{id}`
  - [x] GET `/api/auctions/my` (Expert's auctions)
  - [x] POST `/api/auctions` (Create auction - Expert only)
  - [x] PUT `/api/auctions/{id}`
  - [x] POST `/api/auctions/{id}/publish`
  - [x] POST `/api/auctions/{id}/open`
  - [x] POST `/api/auctions/{id}/close`
  - [x] POST `/api/auctions/{id}/cancel`
  - [x] GET `/api/auctions/{id}/bids` (Bid history with anonymized bidders)
- [x] 6.5 Implement `BidsController`:
  - [x] POST `/api/auctions/{id}/bids` (Place bid)
  - [x] GET `/api/bids/my` (Seeker's bids)
  - [x] GET `/api/bids/{id}` (Get bid by ID)
- [ ] 6.6 Implement `GuestsController` (deferred - needs guest vetting workflow)
- [x] 6.7 Implement auction status workflow (Draft → Scheduled → Open → WinnerSelected/Closed/Cancelled)
- [x] 6.8 Implement proxy bidding support (IsProxyBid, MaxProxyAmount fields)
- [x] 6.9 Add auction scheduling (StartTime, EndTime validation)
- [x] 6.10 Implement winner selection (close sets winning bid)
- [x] 6.11 Implement Buy Now functionality

### Key Files Created
- `src/backend/src/XpertConnect.Application/Features/Auctions/DTOs/` (5 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Auctions/Validators/` (1 validator)
- `src/backend/src/XpertConnect.Application/Features/Bids/DTOs/` (3 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Bids/Validators/` (1 validator)
- `src/backend/src/XpertConnect.Application/Common/Mappings/AuctionMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Mappings/BidMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IAuctionRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IBidRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/AuctionRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/BidRepository.cs`
- `src/backend/src/XpertConnect.API/Controllers/AuctionsController.cs`
- `src/backend/src/XpertConnect.API/Controllers/BidsController.cs`

### Deliverables
- ✅ Auction creation and management
- ✅ Bidding system with proxy bid support
- ✅ Buy Now functionality
- ✅ Auction lifecycle management (publish, open, close, cancel)
- ✅ Anonymized bid history (shows initials only)
- ✅ Winner selection on auction close

---

## Phase 7: Pro-Bono Module

**Goal:** Implement volunteer project management

### Tasks
- [x] 7.1 Create ProBonoProject DTOs (ProjectResponse, ProjectListResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryParams, ApplyToProjectRequest)
- [x] 7.2 Create TimeEntry DTOs (TimeEntryResponse, CreateTimeEntryRequest, ProjectHoursSummary, ExpertHoursSummary)
- [x] 7.3 Create MOU DTOs (MouResponse, CreateMouRequest, SignMouRequest)
- [x] 7.4 Implement `ProjectsController`:
  - [x] GET `/api/projects` (Browse projects with filters)
  - [x] GET `/api/projects/{id}`
  - [x] GET `/api/projects/my` (Expert's projects)
  - [x] GET `/api/projects/my-org` (NonProfit's projects)
  - [x] POST `/api/projects` (Create - NonProfit only)
  - [x] PUT `/api/projects/{id}`
  - [x] POST `/api/projects/{id}/publish`
  - [x] POST `/api/projects/{id}/apply` (Expert applies)
  - [x] POST `/api/projects/{id}/accept` (Accept expert)
  - [x] POST `/api/projects/{id}/reject` (Reject expert)
  - [x] POST `/api/projects/{id}/start`
  - [x] POST `/api/projects/{id}/complete`
  - [x] POST `/api/projects/{id}/cancel`
- [x] 7.5 Implement `TimeEntriesController`:
  - [x] POST `/api/time-entries/project/{projectId}` (Log time)
  - [x] GET `/api/time-entries/project/{projectId}`
  - [x] GET `/api/time-entries/my`
  - [x] GET `/api/time-entries/{id}`
  - [x] PUT `/api/time-entries/{id}`
  - [x] DELETE `/api/time-entries/{id}`
  - [x] GET `/api/time-entries/summary` (CSR report)
  - [x] GET `/api/time-entries/project/{projectId}/total`
- [x] 7.6 Implement project matching (Expert applies → NonProfit accepts/rejects)
- [ ] 7.7 Add MOU generation (deferred - needs document service)
- [x] 7.8 Implement CSR reporting (ExpertHoursSummary with project breakdown)

### Key Files Created
- `src/backend/src/XpertConnect.Application/Features/Projects/DTOs/` (6 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Projects/Validators/` (1 validator)
- `src/backend/src/XpertConnect.Application/Features/TimeEntries/DTOs/` (3 DTO files)
- `src/backend/src/XpertConnect.Application/Features/TimeEntries/Validators/` (1 validator)
- `src/backend/src/XpertConnect.Application/Common/Mappings/ProjectMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Mappings/TimeEntryMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IProBonoProjectRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/ITimeEntryRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/INonProfitOrgRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/ProBonoProjectRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/TimeEntryRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/NonProfitOrgRepository.cs`
- `src/backend/src/XpertConnect.API/Controllers/ProjectsController.cs`
- `src/backend/src/XpertConnect.API/Controllers/TimeEntriesController.cs`

### Deliverables
- ✅ Project creation by non-profits
- ✅ Expert application/matching workflow
- ✅ Time tracking with CRUD operations
- ✅ CSR hours reporting (per expert, per project)
- ✅ Project lifecycle (Draft → Open → Matching → InProgress → Completed)

---

## Phase 8: Payment & Escrow

**Goal:** Implement payment processing and escrow

### Tasks
- [x] 8.1 Create Payment DTOs (PaymentResponse, CreatePaymentRequest, CapturePaymentRequest, RefundPaymentRequest)
- [x] 8.2 Create EscrowAccount DTOs (EscrowAccountResponse, MilestoneResponse, CreateEscrowRequest, CreateMilestoneRequest)
- [x] 8.3 Create Milestone DTOs (included in Escrow DTOs)
- [x] 8.4 Implement IPaymentService interface with MockPaymentService (Stripe integration deferred to Phase 12)
- [x] 8.5 Implement `PaymentsController`:
  - [x] POST `/api/payments/authorize`
  - [x] POST `/api/payments/{id}/capture`
  - [x] POST `/api/payments/{id}/refund`
  - [x] POST `/api/payments/{id}/cancel`
  - [x] GET `/api/payments/{id}`
  - [x] GET `/api/payments/my`
- [x] 8.6 Implement `EscrowController`:
  - [x] POST `/api/escrow` (Create escrow)
  - [x] GET `/api/escrow/{id}`
  - [x] POST `/api/escrow/{id}/fund`
  - [x] POST `/api/escrow/{id}/release`
  - [x] POST `/api/escrow/{id}/dispute`
  - [x] POST `/api/escrow/{id}/milestones` (Add milestone)
  - [x] POST `/api/escrow/milestones/{id}/approve` (Approve milestone)
- [x] 8.7 Implement milestone-based releases
- [ ] 8.8 Add Stripe webhook handlers (deferred to Phase 12)
- [ ] 8.9 Implement high-value escrow (Escrow.com API for $10K+) (deferred to Phase 12)

### Key Files Created
- `src/backend/src/XpertConnect.Application/Features/Payments/DTOs/` (4 DTO files)
- `src/backend/src/XpertConnect.Application/Features/Payments/Validators/` (2 validators)
- `src/backend/src/XpertConnect.Application/Features/Escrow/DTOs/` (7 DTO files)
- `src/backend/src/XpertConnect.Application/Common/Mappings/PaymentMappingProfile.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IPaymentRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IEscrowRepository.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/IPaymentService.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/PaymentRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Repositories/EscrowRepository.cs`
- `src/backend/src/XpertConnect.Infrastructure/Services/MockPaymentService.cs`
- `src/backend/src/XpertConnect.API/Controllers/PaymentsController.cs`
- `src/backend/src/XpertConnect.API/Controllers/EscrowController.cs`

### Deliverables
- ✅ Payment authorization, capture, refund, cancel
- ✅ Escrow account creation and management
- ✅ Milestone-based releases
- ✅ Mock payment service for development (Stripe integration in Phase 12)

---

## Phase 9: Real-Time Features (SignalR)

**Goal:** Add real-time bidding and notifications

### Tasks
- [x] 9.1 Add SignalR package
- [x] 9.2 Create `AuctionHub` for real-time bidding
  - [x] JoinAuction - Subscribe to auction updates
  - [x] LeaveAuction - Unsubscribe from auction
  - [x] GetAuctionState - Get current auction status
  - [x] Ping - Keep connection alive
- [x] 9.3 Create `NotificationHub` for user notifications
  - [x] Subscribe/Unsubscribe to channels
  - [x] MarkAsRead - Mark notifications as read
- [x] 9.4 Implement real-time bid updates
  - [x] NewBid notification to auction watchers
  - [x] Outbid notification to previous high bidder
- [x] 9.5 Implement auction countdown (timeRemaining in AuctionState)
- [x] 9.6 Add bid outbid notifications (NotifyOutbidAsync)
- [x] 9.7 Implement INotificationService for all notification types:
  - [x] Consultation reminders (ConsultationReminderNotification)
  - [x] Consultation status changes (ConsultationStatusNotification)
  - [x] Payment notifications (PaymentNotification)
  - [x] Project application notifications (ProjectApplicationNotification)
- [x] 9.8 Add connection management (auto user group join on connect)
- [x] 9.9 Configure JWT authentication for SignalR (query string token)
- [x] 9.10 Update AuctionsController with real-time notifications

### Key Files Created
- `src/backend/src/XpertConnect.API/Hubs/AuctionHub.cs`
- `src/backend/src/XpertConnect.API/Hubs/NotificationHub.cs`
- `src/backend/src/XpertConnect.API/Services/SignalRNotificationService.cs`
- `src/backend/src/XpertConnect.Application/Common/Interfaces/INotificationService.cs`
- `src/backend/src/XpertConnect.Application/Features/Notifications/DTOs/NotificationMessage.cs`
  - BidNotification, OutbidNotification, AuctionStatusNotification
  - ConsultationReminderNotification, ConsultationStatusNotification
  - PaymentNotification, ProjectApplicationNotification

### Hub Endpoints
- `/hubs/auction` - Real-time auction bidding
- `/hubs/notifications` - User notifications

### Deliverables
- ✅ Real-time bid updates (NewBid, Outbid events)
- ✅ Live auction countdown (timeRemaining)
- ✅ Push notifications infrastructure
- ✅ Connection state management (user groups)
- ✅ JWT authentication for SignalR via query string

---

## Phase 10: Frontend - Core Pages

**Goal:** Build essential frontend pages

### Tasks
- [ ] 10.1 Setup frontend architecture
  - [ ] API client (axios/fetch wrapper)
  - [ ] State management (Zustand/Context)
  - [ ] Auth context
  - [ ] Protected routes
- [ ] 10.2 Create layout components
  - [ ] Navbar
  - [ ] Footer
  - [ ] Sidebar
- [ ] 10.3 Create auth pages
  - [ ] `/login`
  - [ ] `/register`
  - [ ] `/forgot-password`
  - [ ] `/reset-password`
- [ ] 10.4 Create user pages
  - [ ] `/dashboard`
  - [ ] `/profile`
  - [ ] `/settings`
- [ ] 10.5 Create expert pages
  - [ ] `/experts` (Browse)
  - [ ] `/experts/[id]` (Profile)
  - [ ] `/become-expert` (Registration)
- [ ] 10.6 Create common components
  - [ ] Button, Input, Modal
  - [ ] Card, Avatar
  - [ ] Loading, Error states

### Key Files to Create
```
src/frontend/src/
├── components/
│   ├── layout/
│   ├── ui/
│   └── forms/
├── lib/
│   ├── api.ts
│   └── auth.ts
├── hooks/
├── stores/
└── app/
    ├── (auth)/
    ├── (dashboard)/
    └── experts/
```

### Deliverables
- Authentication flow working
- User dashboard
- Expert browse page
- Responsive design

---

## Phase 11: Frontend - Business Flows

**Goal:** Implement business module UIs

### Tasks
- [ ] 11.1 Consultation booking flow
  - [ ] `/book/[expertId]`
  - [ ] `/consultations`
  - [ ] `/consultations/[id]`
- [ ] 11.2 Auction pages
  - [ ] `/auctions` (Browse)
  - [ ] `/auctions/[id]` (Detail + bidding)
  - [ ] `/auctions/create`
  - [ ] Real-time bid updates (SignalR client)
- [ ] 11.3 Pro-bono project pages
  - [ ] `/projects` (Browse)
  - [ ] `/projects/[id]`
  - [ ] `/projects/create`
- [ ] 11.4 Payment pages
  - [ ] `/checkout`
  - [ ] `/payment-success`
  - [ ] `/payment-history`
- [ ] 11.5 Admin dashboard
  - [ ] `/admin/users`
  - [ ] `/admin/verifications`
  - [ ] `/admin/reports`

### Packages to Add
```bash
cd src/frontend
npm install @microsoft/signalr
npm install @stripe/stripe-js @stripe/react-stripe-js
```

### Deliverables
- Complete booking flow
- Live auction bidding
- Project management UI
- Payment checkout

---

## Phase 12: Integrations

**Goal:** Connect third-party services

### Tasks
- [ ] 12.1 KYC Integration (Onfido/Jumio)
  - [ ] Implement IKycService
  - [ ] Verification flow
  - [ ] Webhook handlers
- [ ] 12.2 Payment Integration (Stripe)
  - [ ] Connect accounts for experts
  - [ ] Payment intents
  - [ ] Subscription support
- [ ] 12.3 High-Value Escrow (Escrow.com)
  - [ ] API integration for $10K+ transactions
  - [ ] Milestone management
- [ ] 12.4 Email Service
  - [ ] SendGrid/Mailgun setup
  - [ ] Email templates
  - [ ] Transactional emails
- [ ] 12.5 Geofencing
  - [ ] GPS verification service
  - [ ] Geofence event processing
  - [ ] Meeting verification

### Key Files to Create
- `src/backend/src/XpertConnect.Infrastructure/Services/OnfidoKycService.cs`
- `src/backend/src/XpertConnect.Infrastructure/Services/StripePaymentService.cs`
- `src/backend/src/XpertConnect.Infrastructure/Services/EscrowComService.cs`
- `src/backend/src/XpertConnect.Infrastructure/Services/SendGridEmailService.cs`
- `src/backend/src/XpertConnect.Infrastructure/Services/GeofenceService.cs`

### Deliverables
- KYC verification working
- Payments processing
- Escrow for high-value
- Email notifications
- GPS meeting verification

---

## Phase 13: Testing & QA

**Goal:** Comprehensive test coverage

### Tasks
- [ ] 13.1 Setup test projects
  - [ ] Unit tests (XpertConnect.Domain.Tests)
  - [ ] Integration tests (XpertConnect.API.Tests)
  - [ ] Frontend tests (Jest + React Testing Library)
- [ ] 13.2 Write unit tests
  - [ ] Entity tests
  - [ ] Service tests
  - [ ] Validation tests
- [ ] 13.3 Write integration tests
  - [ ] API endpoint tests
  - [ ] Database tests
  - [ ] Auth flow tests
- [ ] 13.4 Write E2E tests
  - [ ] Playwright/Cypress setup
  - [ ] Critical path tests
- [ ] 13.5 Performance testing
  - [ ] Load testing
  - [ ] SignalR stress testing
- [ ] 13.6 Security testing
  - [ ] OWASP checklist
  - [ ] Penetration testing

### Commands
```bash
# Backend tests
cd src/backend
dotnet test

# Frontend tests
cd src/frontend
npm test

# E2E tests
npm run test:e2e
```

### Deliverables
- 80%+ code coverage
- All critical paths tested
- Performance benchmarks
- Security audit passed

---

## Phase 14: Deployment Setup

**Goal:** Production-ready deployment

### Tasks
- [ ] 14.1 Docker setup
  - [ ] API Dockerfile
  - [ ] Frontend Dockerfile
  - [ ] docker-compose.yml
- [ ] 14.2 Azure setup
  - [ ] Azure App Service (API)
  - [ ] Azure Static Web Apps (Frontend)
  - [ ] Azure Database for PostgreSQL
  - [ ] Azure Redis Cache
  - [ ] Azure SignalR Service
- [ ] 14.3 CI/CD Pipeline
  - [ ] GitHub Actions workflows
  - [ ] Build and test
  - [ ] Deploy to staging
  - [ ] Deploy to production
- [ ] 14.4 Monitoring
  - [ ] Application Insights
  - [ ] Log aggregation
  - [ ] Alerting
- [ ] 14.5 Security
  - [ ] SSL certificates
  - [ ] Key Vault for secrets
  - [ ] WAF configuration

### Key Files to Create
- `Dockerfile` (API)
- `src/frontend/Dockerfile`
- `docker-compose.yml`
- `.github/workflows/ci.yml`
- `.github/workflows/deploy.yml`

### Deliverables
- Containerized applications
- CI/CD pipeline working
- Staging environment
- Production environment
- Monitoring in place

---

## Quick Reference: Resume Points

| To Resume From | Start With |
|----------------|------------|
| **Phase 1** | `psql -U postgres -c "CREATE DATABASE xpertconnect_dev;"` |
| **Phase 2** | Add Identity packages, create AuthController |
| **Phase 3** | Create UserRepository and UsersController |
| **Phase 4** | Create Expert/Seeker DTOs and controllers |
| **Phase 5** | Create ConsultationsController |
| **Phase 6** | Create AuctionsController and BidsController |
| **Phase 7** | Create ProjectsController |
| **Phase 8** | Add Stripe.net, create PaymentsController |
| **Phase 9** | Add SignalR, create AuctionHub |
| **Phase 10** | Setup frontend API client and auth context |
| **Phase 11** | Build consultation booking pages |
| **Phase 12** | Implement IKycService with Onfido |
| **Phase 13** | Create test projects, write unit tests |
| **Phase 14** | Create Dockerfiles, setup Azure |

---

## How to Use This Document

1. **Starting Fresh:** Begin with Phase 1
2. **Resuming Work:** Find your current phase, check incomplete tasks
3. **Tracking Progress:** Update checkboxes as you complete tasks
4. **Ask Claude:** Say "Continue from Phase X" to resume implementation

---

*Last Updated: 2026-04-19*
