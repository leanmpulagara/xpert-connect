# XpertConnect UML Diagrams

---

## Table of Contents

| # | Diagram | Type | Description |
|---|---------|------|-------------|
| 1 | [Use Case Diagram](#1-use-case-diagram) | Behavioral | All actors and system interactions |
| 2 | [Class Diagram](#2-class-diagram) | Structural | Entity model with attributes and relationships |
| 3 | [Sequence - Auction Flow](#3-sequence-diagram---auction-flow) | Behavioral | End-to-end auction process |
| 4 | [Sequence - Professional Fee](#4-sequence-diagram---professional-fee-flow) | Behavioral | Consultation booking flow |
| 5 | [Sequence - Pro-Bono](#5-sequence-diagram---pro-bono-flow) | Behavioral | Social impact project flow |
| 6 | [Activity - Verification](#6-activity-diagram---verification-process) | Behavioral | User verification process |
| 7 | [Activity - Geofence](#7-activity-diagram---geofence-meeting-verification) | Behavioral | Meeting verification via GPS |
| 8 | [Component Diagram](#8-component-diagram) | Structural | System architecture and services |
| 9 | [State - Auction Lifecycle](#9-state-diagram---auction-lot-lifecycle) | Behavioral | Auction lot state transitions |
| 10 | [State - Consultation Lifecycle](#10-state-diagram---consultation-booking-lifecycle) | Behavioral | Booking state transitions |
| 11 | [ERD (Database Schema)](#11-entity-relationship-diagram-erd) | Structural | Database tables and relationships |

---

### Quick Reference by Category

**Structural Diagrams** (What the system IS)
- [Class Diagram](#2-class-diagram) - Domain model
- [Component Diagram](#8-component-diagram) - Architecture
- [ERD](#11-entity-relationship-diagram-erd) - Database schema

**Behavioral Diagrams** (How the system WORKS)
- [Use Case Diagram](#1-use-case-diagram) - User interactions
- [Sequence Diagrams](#3-sequence-diagram---auction-flow) - Process flows
- [Activity Diagrams](#6-activity-diagram---verification-process) - Workflows
- [State Diagrams](#9-state-diagram---auction-lot-lifecycle) - Lifecycles

---

## 1. Use Case Diagram

```plantuml
@startuml UseCaseDiagram
left to right direction
skinparam packageStyle rectangle

actor "Seeker" as seeker
actor "Expert\n(Accomplished Mind)" as expert
actor "Platform Admin" as admin
actor "Concierge" as concierge
actor "Non-Profit Org" as nonprofit
actor "Escrow Agent" as escrow

rectangle "XpertConnect Platform" {
    ' Authentication & Registration
    package "Registration & Verification" {
        usecase "Register Account" as UC1
        usecase "Complete KYC Verification" as UC2
        usecase "Submit Financial References" as UC3
        usecase "Verify Expert Credentials" as UC4
        usecase "Categorize Expert (A/B/C)" as UC5
    }

    ' Auction Model
    package "Auction Model" {
        usecase "Create Auction Lot" as UC6
        usecase "Define Meeting Parameters" as UC7
        usecase "Pre-Register for Auction" as UC8
        usecase "Approve Bidder" as UC9
        usecase "Place Bid" as UC10
        usecase "Set Proxy Bid" as UC11
        usecase "Buy Now" as UC12
        usecase "Close Auction" as UC13
        usecase "Fund Escrow" as UC14
    }

    ' Professional Fee Model
    package "Professional Fee Model" {
        usecase "Create Expert Profile" as UC15
        usecase "Set Availability & Rates" as UC16
        usecase "Browse Experts" as UC17
        usecase "Book Consultation" as UC18
        usecase "Execute Digital NDA" as UC19
        usecase "Process Payment" as UC20
        usecase "Conduct Session" as UC21
        usecase "Provide Feedback" as UC22
    }

    ' Pro-Bono Model
    package "Pro-Bono Model" {
        usecase "Define Project Need" as UC23
        usecase "Browse Social Causes" as UC24
        usecase "Apply for Project" as UC25
        usecase "Match Expert to Project" as UC26
        usecase "Sign MOU" as UC27
        usecase "Track Volunteer Hours" as UC28
        usecase "Generate CSR Report" as UC29
    }

    ' Physical Meeting Logistics
    package "Physical Meeting Logistics" {
        usecase "Coordinate Meeting Logistics" as UC30
        usecase "Vet Venue Security" as UC31
        usecase "Book Executive Protection" as UC32
        usecase "Verify Meeting via Geofence" as UC33
        usecase "Release Escrow Funds" as UC34
    }

    ' Compliance & Admin
    package "Compliance & Administration" {
        usecase "Screen Against Sanctions" as UC35
        usecase "Monitor Compliance" as UC36
        usecase "Handle Disputes" as UC37
    }
}

' Seeker relationships
seeker --> UC1
seeker --> UC2
seeker --> UC3
seeker --> UC8
seeker --> UC10
seeker --> UC11
seeker --> UC12
seeker --> UC14
seeker --> UC17
seeker --> UC18
seeker --> UC19
seeker --> UC21
seeker --> UC22
seeker --> UC33

' Expert relationships
expert --> UC1
expert --> UC4
expert --> UC6
expert --> UC7
expert --> UC15
expert --> UC16
expert --> UC19
expert --> UC21
expert --> UC24
expert --> UC25
expert --> UC27
expert --> UC33

' Admin relationships
admin --> UC5
admin --> UC9
admin --> UC13
admin --> UC35
admin --> UC36
admin --> UC37

' Concierge relationships
concierge --> UC30
concierge --> UC31
concierge --> UC32

' Non-Profit relationships
nonprofit --> UC23
nonprofit --> UC26
nonprofit --> UC27
nonprofit --> UC29

' Escrow relationships
escrow --> UC14
escrow --> UC34
escrow --> UC37

' Include relationships
UC6 ..> UC7 : <<include>>
UC18 ..> UC19 : <<include>>
UC18 ..> UC20 : <<include>>
UC33 ..> UC34 : <<include>>

@enduml
```

---

## 2. Class Diagram

```plantuml
@startuml ClassDiagram
skinparam classAttributeIconSize 0

' Enums
enum ExpertCategory {
    CATEGORY_A_SME
    CATEGORY_B_CSUITE
    CATEGORY_C_CELEBRITY
}

enum InteractionType {
    AUCTION
    PROFESSIONAL_FEE
    PRO_BONO
}

enum BookingStatus {
    PENDING
    CONFIRMED
    IN_PROGRESS
    COMPLETED
    CANCELLED
    DISPUTED
}

enum AuctionStatus {
    DRAFT
    OPEN
    CLOSED
    FUNDED
    FULFILLED
}

enum PaymentStatus {
    PENDING
    AUTHORIZED
    IN_ESCROW
    RELEASED
    REFUNDED
}

enum VerificationStatus {
    PENDING
    IN_REVIEW
    VERIFIED
    REJECTED
}

' Abstract User
abstract class User {
    - id: UUID
    - email: String
    - phone: String
    - createdAt: DateTime
    - verificationStatus: VerificationStatus
    + register()
    + login()
    + updateProfile()
}

class Seeker {
    - financialReferences: List<FinancialReference>
    - reputationScore: Float
    - kycStatus: VerificationStatus
    - bidEligibility: Boolean
    + completeKYC()
    + submitFinancialReference()
    + placeBid()
    + bookConsultation()
}

class Expert {
    - category: ExpertCategory
    - credentials: List<Credential>
    - hourlyRate: Decimal
    - availability: List<TimeSlot>
    - securityRequirements: SecurityProfile
    - verifiedAccomplishments: List<Accomplishment>
    + createProfile()
    + setAvailability()
    + createAuctionLot()
    + acceptBooking()
}

class NonProfitOrg {
    - orgName: String
    - taxId: String
    - missionStatement: String
    - verificationDocs: List<Document>
    + defineProject()
    + matchExpert()
    + generateCSRReport()
}

' Verification Classes
class KYCVerification {
    - userId: UUID
    - documentType: String
    - documentNumber: String
    - biometricData: BiometricHash
    - verifiedAt: DateTime
    - expiresAt: DateTime
    + performLivenessCheck()
    + validateDocument()
    + zeroDataStore()
}

class FinancialReference {
    - bankName: String
    - accountType: String
    - referenceNumber: String
    - validatedAmount: Decimal
    - validatedAt: DateTime
}

class Credential {
    - type: String
    - issuingBody: String
    - verificationSource: String
    - verifiedAt: DateTime
}

' Interaction Classes
class AuctionLot {
    - id: UUID
    - expertId: UUID
    - title: String
    - description: String
    - meetingType: String
    - guestLimit: Integer
    - startingBid: Decimal
    - buyNowPrice: Decimal
    - startTime: DateTime
    - endTime: DateTime
    - status: AuctionStatus
    - beneficiaryOrg: NonProfitOrg
    + open()
    + close()
    + determineWinner()
}

class Bid {
    - id: UUID
    - auctionId: UUID
    - seekerId: UUID
    - amount: Decimal
    - isProxyBid: Boolean
    - maxProxyAmount: Decimal
    - placedAt: DateTime
    + validate()
    + notifyOutbid()
}

class Consultation {
    - id: UUID
    - expertId: UUID
    - seekerId: UUID
    - scheduledAt: DateTime
    - duration: Integer
    - rate: Decimal
    - status: BookingStatus
    - meetingType: String
    - virtualHubLink: String
    + book()
    + reschedule()
    + cancel()
    + complete()
}

class ProBonoProject {
    - id: UUID
    - orgId: UUID
    - expertId: UUID
    - title: String
    - description: String
    - deliverables: List<String>
    - estimatedHours: Integer
    - actualHours: Integer
    - status: BookingStatus
    + defineScope()
    + trackHours()
    + calculateValue()
}

' Legal & Compliance
class NDA {
    - id: UUID
    - partyA: UUID
    - partyB: UUID
    - generatedAt: DateTime
    - signedByA: DateTime
    - signedByB: DateTime
    - templateVersion: String
    + generate()
    + sign()
    + store()
}

class MOU {
    - id: UUID
    - projectId: UUID
    - scope: String
    - timeline: String
    - signedAt: DateTime
}

class ComplianceCheck {
    - id: UUID
    - userId: UUID
    - checkType: String
    - sanctionsResult: Boolean
    - pepResult: Boolean
    - adverseMediaResult: Boolean
    - checkedAt: DateTime
    + screenSanctions()
    + checkPEP()
    + scanAdverseMedia()
}

' Payment Classes
class Payment {
    - id: UUID
    - amount: Decimal
    - currency: String
    - status: PaymentStatus
    - method: String
    - createdAt: DateTime
    + authorize()
    + capture()
    + refund()
}

class EscrowAccount {
    - id: UUID
    - transactionId: UUID
    - amount: Decimal
    - fundedAt: DateTime
    - releasedAt: DateTime
    - milestones: List<Milestone>
    + fund()
    + releaseFunds()
    + handleDispute()
}

class Milestone {
    - id: UUID
    - description: String
    - amount: Decimal
    - dueDate: DateTime
    - completedAt: DateTime
    - approved: Boolean
}

' Physical Logistics
class PhysicalMeeting {
    - id: UUID
    - interactionId: UUID
    - venue: Venue
    - scheduledAt: DateTime
    - geofenceId: UUID
    - securityTeam: List<SecurityPersonnel>
    - guestList: List<Guest>
    + scheduleVenue()
    + vetGuests()
    + verifyAttendance()
}

class Venue {
    - id: UUID
    - name: String
    - address: String
    - coordinates: GeoPoint
    - securityRating: String
    - amenities: List<String>
    - preApproved: Boolean
}

class Geofence {
    - id: UUID
    - venueId: UUID
    - boundaryType: String
    - coordinates: List<GeoPoint>
    - radius: Float
    - dwellDuration: Integer
    + monitorEntry()
    + monitorExit()
    + verifyDwell()
}

class GeofenceEvent {
    - id: UUID
    - geofenceId: UUID
    - userId: UUID
    - eventType: String
    - timestamp: DateTime
    - coordinates: GeoPoint
}

class SecurityProfile {
    - level: String
    - requiresEP: Boolean
    - requiresTSCM: Boolean
    - devicePolicy: String
    - bufferZone: Integer
}

class Guest {
    - name: String
    - relationship: String
    - kycVerified: Boolean
    - ndaSigned: Boolean
}

' Relationships
User <|-- Seeker
User <|-- Expert

Seeker "1" *-- "0..*" FinancialReference
Seeker "1" -- "1" KYCVerification
Seeker "1" -- "0..*" Bid
Seeker "1" -- "0..*" Consultation

Expert "1" *-- "0..*" Credential
Expert "1" -- "1" SecurityProfile
Expert "1" -- "0..*" AuctionLot
Expert "1" -- "0..*" Consultation
Expert "1" -- "0..*" ProBonoProject

NonProfitOrg "1" -- "0..*" ProBonoProject
NonProfitOrg "1" -- "0..*" AuctionLot : beneficiary

AuctionLot "1" -- "0..*" Bid
AuctionLot "1" -- "0..1" PhysicalMeeting

Consultation "1" -- "1" NDA
Consultation "1" -- "1" Payment
Consultation "1" -- "0..1" PhysicalMeeting

ProBonoProject "1" -- "1" MOU

Payment "1" -- "0..1" EscrowAccount
EscrowAccount "1" *-- "0..*" Milestone

PhysicalMeeting "1" -- "1" Venue
PhysicalMeeting "1" -- "1" Geofence
PhysicalMeeting "1" *-- "0..*" Guest

Geofence "1" -- "0..*" GeofenceEvent

User "1" -- "0..*" ComplianceCheck

@enduml
```

---

## 3. Sequence Diagram - Auction Flow

```plantuml
@startuml AuctionSequence
title Auction Model - End to End Flow

actor Expert
actor Seeker
participant "Platform\nUI" as UI
participant "Auction\nService" as AuctionSvc
participant "Verification\nService" as VerifySvc
participant "Admin\nDashboard" as Admin
participant "Notification\nService" as NotifySvc
participant "Escrow\nService" as EscrowSvc
participant "Concierge\nService" as ConciergeSvc
participant "Geofence\nService" as GeoSvc
database "Database" as DB

== Lot Creation ==
Expert -> UI: Create Auction Lot
UI -> AuctionSvc: submitLot(details)
AuctionSvc -> DB: saveLot(DRAFT)
AuctionSvc --> UI: lotCreated(lotId)
UI --> Expert: Lot Created Successfully

== Seeker Pre-Registration ==
Seeker -> UI: Register for Premium Bidding
UI -> VerifySvc: initiateKYC(seekerId)
VerifySvc -> VerifySvc: biometricLivenessCheck()
VerifySvc -> VerifySvc: documentAuthentication()
Seeker -> UI: Submit Financial References
UI -> VerifySvc: validateFinancials(bankRef, deposit)
VerifySvc -> DB: updateSeekerStatus(PENDING_APPROVAL)
VerifySvc -> Admin: notifyPendingApproval(seekerId)

== Vetting & Approval ==
Admin -> VerifySvc: reviewSeeker(seekerId)
VerifySvc -> VerifySvc: complianceScreening()
VerifySvc -> VerifySvc: sanctionsCheck()
Admin -> DB: approveSeeker(seekerId)
DB -> NotifySvc: triggerNotification()
NotifySvc --> Seeker: Approved for Premium Bidding

== Bidding Window ==
AuctionSvc -> DB: updateLotStatus(OPEN)
AuctionSvc -> NotifySvc: notifyEligibleBidders(lotId)

loop Bidding Period (3-7 days)
    Seeker -> UI: Place Bid(amount)
    UI -> AuctionSvc: submitBid(lotId, seekerId, amount)
    AuctionSvc -> AuctionSvc: validateBid()
    AuctionSvc -> DB: saveBid()
    AuctionSvc -> NotifySvc: notifyOutbidUsers()
    NotifySvc --> Seeker: You've been outbid!
end

alt Buy Now Option
    Seeker -> UI: Buy Now
    UI -> AuctionSvc: buyNow(lotId, seekerId)
    AuctionSvc -> DB: closeLot(SOLD)
end

== Auction Close ==
AuctionSvc -> AuctionSvc: determineWinner()
AuctionSvc -> DB: updateLotStatus(CLOSED)
AuctionSvc -> NotifySvc: notifyWinner(winnerId)
NotifySvc --> Seeker: Congratulations! You Won!
NotifySvc --> Expert: Auction Completed

== Escrow Funding ==
Seeker -> UI: Fund Escrow
UI -> EscrowSvc: createEscrow(amount)
EscrowSvc -> EscrowSvc: validateFunds()
EscrowSvc -> DB: saveEscrow(FUNDED)
EscrowSvc --> UI: Escrow Funded
EscrowSvc -> ConciergeSvc: triggerLogistics(lotId)

== Logistics Coordination ==
ConciergeSvc -> ConciergeSvc: selectVenue()
ConciergeSvc -> ConciergeSvc: vetSecurityRequirements()
ConciergeSvc -> GeoSvc: createGeofence(venueCoords)

alt Guests Allowed
    Seeker -> UI: Submit Guest List
    UI -> VerifySvc: vetGuests(guestList)
    VerifySvc -> VerifySvc: guestKYC()
    VerifySvc -> VerifySvc: guestNDA()
end

ConciergeSvc -> NotifySvc: sendMeetingDetails()
NotifySvc --> Expert: Meeting Scheduled
NotifySvc --> Seeker: Meeting Confirmed

== Physical Meeting ==
Expert -> GeoSvc: enterGeofence(location)
GeoSvc -> DB: logEvent(ENTER, expertId)
Seeker -> GeoSvc: enterGeofence(location)
GeoSvc -> DB: logEvent(ENTER, seekerId)

GeoSvc -> GeoSvc: monitorDwellTime()

note over Expert, Seeker: Meeting Takes Place

GeoSvc -> GeoSvc: verifyDwellDuration()
GeoSvc -> DB: logEvent(EXIT)
GeoSvc -> EscrowSvc: triggerRelease(meetingVerified)

== Fund Disbursement ==
EscrowSvc -> EscrowSvc: calculateDistribution()
EscrowSvc -> DB: releaseFunds(beneficiary)
EscrowSvc -> NotifySvc: sendTaxDocumentation()
NotifySvc --> Expert: Funds Released to Charity
NotifySvc --> Seeker: Receipt & Tax Documents

@enduml
```

---

## 4. Sequence Diagram - Professional Fee Flow

```plantuml
@startuml ProfessionalFeeSequence
title Professional Fee Model - Consultation Flow

actor Seeker
actor Expert
participant "Platform\nUI" as UI
participant "Search\nService" as SearchSvc
participant "Calendar\nService" as CalendarSvc
participant "Booking\nService" as BookingSvc
participant "Contract\nService" as ContractSvc
participant "Payment\nService" as PaymentSvc
participant "Virtual\nHub" as VirtualHub
participant "Feedback\nService" as FeedbackSvc
database "Database" as DB

== Profile Discovery ==
Seeker -> UI: Search Experts
UI -> SearchSvc: search(filters)
SearchSvc -> DB: queryExperts(industry, skill, rate)
SearchSvc -> SearchSvc: applyAIMatching()
DB --> SearchSvc: expertList
SearchSvc --> UI: rankedResults
UI --> Seeker: Display Expert Profiles

Seeker -> UI: View Expert Profile
UI -> DB: getExpertDetails(expertId)
DB --> UI: expertProfile
UI --> Seeker: Show Profile, Rates, Reviews

== Availability Matching ==
Seeker -> UI: Check Availability
UI -> CalendarSvc: getAvailableSlots(expertId)
CalendarSvc -> CalendarSvc: syncGoogleCalendar()
CalendarSvc -> CalendarSvc: syncOutlookCalendar()
CalendarSvc --> UI: availableSlots
UI --> Seeker: Display Available Time Slots

== Booking & Legal ==
Seeker -> UI: Select Time Slot
UI -> BookingSvc: createBooking(expertId, seekerId, slot)
BookingSvc -> DB: saveBooking(PENDING)

BookingSvc -> ContractSvc: generateNDA(partyA, partyB)
ContractSvc -> ContractSvc: selectTemplate()
ContractSvc -> ContractSvc: populateFields()
ContractSvc --> UI: ndaDocument

UI --> Seeker: Sign NDA
Seeker -> UI: digitalSignature
UI -> ContractSvc: signNDA(seekerId, signature)
ContractSvc -> DB: saveSignature(partyA)

ContractSvc -> Expert: Request NDA Signature
Expert -> UI: Sign NDA
UI -> ContractSvc: signNDA(expertId, signature)
ContractSvc -> DB: saveSignature(partyB)
ContractSvc -> DB: storeNDA(signed)

== Payment Capture ==
BookingSvc -> PaymentSvc: authorizePayment(amount)
PaymentSvc -> PaymentSvc: stripeConnect()
PaymentSvc -> PaymentSvc: holdFunds()
PaymentSvc -> DB: savePayment(AUTHORIZED)
PaymentSvc --> UI: paymentAuthorized

BookingSvc -> DB: updateBooking(CONFIRMED)
BookingSvc --> Seeker: Booking Confirmed
BookingSvc --> Expert: New Booking Notification

== Meeting Execution ==
UI -> VirtualHub: createSecureRoom(bookingId)
VirtualHub -> VirtualHub: generateEncryptedLink()
VirtualHub --> Seeker: Join Link
VirtualHub --> Expert: Join Link

Seeker -> VirtualHub: joinSession()
Expert -> VirtualHub: joinSession()

note over Seeker, Expert: Consultation Session\n(Video/Audio Call)

VirtualHub -> DB: logSessionDuration()
VirtualHub -> VirtualHub: endSession()

== Milestone Approval ==
UI --> Seeker: Confirm Session Completion
Seeker -> UI: confirmCompletion()
UI -> BookingSvc: markComplete(bookingId)
BookingSvc -> DB: updateBooking(COMPLETED)

Seeker -> UI: provideFeedback(rating, comments)
UI -> FeedbackSvc: saveFeedback(bookingId, feedback)
FeedbackSvc -> DB: storeFeedback()
FeedbackSvc -> DB: updateExpertRating()

== Settlement ==
BookingSvc -> PaymentSvc: capturePayment(bookingId)
PaymentSvc -> PaymentSvc: calculateCommission()
PaymentSvc -> PaymentSvc: transferToExpert()
PaymentSvc -> DB: updatePayment(RELEASED)
PaymentSvc -> PaymentSvc: generateInvoice()

PaymentSvc --> Expert: Payment Received + Invoice
PaymentSvc --> Seeker: Receipt

@enduml
```

---

## 5. Sequence Diagram - Pro-Bono Flow

```plantuml
@startuml ProBonoSequence
title Pro-Bono / Social Impact Model Flow

actor "Non-Profit\nOrganization" as NonProfit
actor Expert
participant "Platform\nUI" as UI
participant "Project\nService" as ProjectSvc
participant "Matching\nService" as MatchSvc
participant "Contract\nService" as ContractSvc
participant "Tracking\nService" as TrackingSvc
participant "Reporting\nService" as ReportSvc
database "Database" as DB

== Need Scoping ==
NonProfit -> UI: Create Project
UI -> ProjectSvc: defineProject(details)
ProjectSvc -> ProjectSvc: categorizeNeed()
ProjectSvc -> DB: saveProject(title, deliverables, timeline)
ProjectSvc --> UI: projectCreated
UI --> NonProfit: Project Posted

== Expert Sourcing ==
Expert -> UI: Browse Social Causes
UI -> ProjectSvc: getProjects(filters)
ProjectSvc -> DB: queryOpenProjects()
DB --> ProjectSvc: projectList
ProjectSvc --> UI: availableProjects
UI --> Expert: Display Projects

Expert -> UI: Apply for Project
UI -> MatchSvc: submitApplication(expertId, projectId)
MatchSvc -> DB: saveApplication()
MatchSvc --> NonProfit: New Application Received

== Matching & Vetting ==
NonProfit -> UI: Review Applications
UI -> DB: getApplications(projectId)
DB --> UI: applicationList
UI --> NonProfit: Display Applicants

NonProfit -> UI: Review Expert Portfolio
UI -> DB: getExpertPortfolio(expertId)
DB --> UI: portfolio
UI --> NonProfit: Expert Details

NonProfit -> UI: Schedule Interview
UI -> MatchSvc: scheduleInterview(expertId, time)
MatchSvc --> Expert: Interview Invitation

note over NonProfit, Expert: Cultural Fit Interview

NonProfit -> UI: Select Expert
UI -> MatchSvc: selectExpert(projectId, expertId)
MatchSvc -> DB: updateProjectAssignment()
MatchSvc --> Expert: Selected for Project

== Engagement ==
ContractSvc -> ContractSvc: generateMOU(projectId)
ContractSvc -> ContractSvc: defineScope()
ContractSvc -> ContractSvc: setTimeline()
ContractSvc --> UI: mouDocument

UI --> NonProfit: Review & Sign MOU
NonProfit -> UI: signMOU()
ContractSvc -> DB: saveSignature(nonProfitId)

UI --> Expert: Review & Sign MOU
Expert -> UI: signMOU()
ContractSvc -> DB: saveSignature(expertId)
ContractSvc -> DB: storeMOU(signed)

ContractSvc --> NonProfit: MOU Executed
ContractSvc --> Expert: MOU Executed - Project Begins

== Execution ==
loop Project Duration
    Expert -> UI: Log Work Hours
    UI -> TrackingSvc: logHours(projectId, hours, description)
    TrackingSvc -> DB: saveTimeEntry()

    alt Deliverable Completed
        Expert -> UI: Submit Deliverable
        UI -> ProjectSvc: submitDeliverable(projectId, deliverable)
        ProjectSvc -> DB: saveDeliverable()
        ProjectSvc --> NonProfit: Deliverable Ready for Review

        NonProfit -> UI: Approve Deliverable
        UI -> ProjectSvc: approveDeliverable(deliverableId)
        ProjectSvc -> DB: updateDeliverableStatus(APPROVED)
    end
end

Expert -> UI: Mark Project Complete
UI -> ProjectSvc: completeProject(projectId)
ProjectSvc -> DB: updateProjectStatus(COMPLETED)

== Reporting ==
TrackingSvc -> ReportSvc: calculateProBonoValue(projectId)
ReportSvc -> ReportSvc: applyHourlyValuation()
ReportSvc -> DB: storeValuation()

NonProfit -> UI: Generate CSR Report
UI -> ReportSvc: generateReport(orgId, dateRange)
ReportSvc -> DB: aggregateData()
ReportSvc -> ReportSvc: formatReport()
ReportSvc --> UI: csrReport
UI --> NonProfit: Download CSR Report

Expert -> UI: Request Volunteer Certificate
UI -> ReportSvc: generateCertificate(expertId, projectId)
ReportSvc --> UI: certificate
UI --> Expert: Download Certificate

@enduml
```

---

## 6. Activity Diagram - Verification Process

```plantuml
@startuml VerificationActivity
title User Verification Process

start

:User Initiates Registration;

fork
    :Submit Personal Information;
fork again
    :Upload Identity Documents;
end fork

:System Performs Document Authentication;

if (Documents Valid?) then (yes)
    :Initiate Biometric Liveness Check;

    if (Liveness Verified?) then (yes)
        :Cross-Reference Identity;
    else (no)
        :Request Re-submission;
        stop
    endif
else (no)
    :Reject - Invalid Documents;
    stop
endif

if (User Type?) then (Expert)
    :Credential Verification;

    switch (Expert Category?)
    case (Category A - SME)
        :Verify Academic Credentials;
        :Check Publication Databases;
        :LinkedIn API Integration;
    case (Category B - C-Suite)
        :Verify SEC Filings;
        :Check Company Registries;
        :Conflict of Interest Check;
    case (Category C - Celebrity)
        :Contact Official Representatives;
        :Verify Public Records;
        :Assess Security Requirements;
    endswitch

    :Assign Expert Category;

else (Seeker)
    :Basic KYC Complete;

    if (High-Value Access Required?) then (yes)
        :Request Financial References;
        :Validate Bank Statements;
        :Credit Card Authorization Hold;

        if (Financial Pre-Qualification Passed?) then (yes)
            :Reputation Screening;
            :Social Media Analysis;

            if (Reputation Clear?) then (yes)
                :Mark as Premium Seeker;
            else (no)
                :Flag for Manual Review;
            endif
        else (no)
            :Deny Premium Access;
        endif
    else (no)
        :Standard Seeker Status;
    endif
endif

:Run Compliance Checks;

fork
    :Sanctions List Screening;
fork again
    :PEP (Politically Exposed Person) Check;
fork again
    :Adverse Media Scan;
end fork

if (All Compliance Checks Passed?) then (yes)
    :Approve User;
    :Notify User of Approval;
else (no)
    :Flag for Admin Review;

    :Admin Manual Review;

    if (Admin Approves?) then (yes)
        :Approve with Notes;
    else (no)
        :Reject User;
        stop
    endif
endif

:User Verified and Active;

stop

@enduml
```

---

## 7. Activity Diagram - Geofence Meeting Verification

```plantuml
@startuml GeofenceActivity
title Geofence-Based Meeting Verification

start

:Meeting Scheduled;
:Concierge Selects Venue;

:Define Geofence Boundary;

if (Venue Type?) then (Restaurant/Open)
    :Create Circular Geofence;
    :Set Radius (50-200m);
else (Office Building)
    :Create Polygonal Geofence;
    :Map Building Footprint;
endif

:Set Expected Dwell Duration;
:Configure Battery-Efficient Monitoring;

fork
    :Monitor Expert Location;
fork again
    :Monitor Seeker Location;
end fork

:Switch to Wi-Fi/Cellular Triangulation;

repeat
    :Background Location Monitoring;

    if (User Enters Buffer Zone?) then (yes)
        :Activate High-Precision GPS;
    endif

repeat while (User Near Geofence?) is (no)

:GEOFENCE_TRANSITION_ENTER Detected;
:Log Entry Event with Timestamp;

if (Both Parties Inside Geofence?) then (yes)
    :Send Push Notifications;
    :Start Dwell Timer;
else (no)
    :Wait for Other Party;
endif

:Monitor Dwell Duration;

repeat
    :Track Continuous Presence;

    if (User Exits Temporarily?) then (yes)
        :Log Temporary Exit;
        :Pause Timer;

        if (Returns Within Grace Period?) then (yes)
            :Resume Timer;
        else (no)
            :Flag Incomplete Meeting;
            stop
        endif
    endif

repeat while (Dwell Duration Met?) is (no)

:GEOFENCE_TRANSITION_EXIT Detected;
:Log Exit Event;

if (Required Duration Achieved?) then (yes)
    :Mark Meeting as VERIFIED;
    :Generate Verification Record;

    :Notify Escrow Service;
    :Trigger Fund Release;

    :Send Confirmation to Both Parties;
else (no)
    :Mark Meeting as INCOMPLETE;
    :Flag for Manual Review;

    :Admin Reviews Evidence;

    if (Admin Approves?) then (yes)
        :Override to VERIFIED;
        :Release Funds;
    else (no)
        :Initiate Dispute Process;
    endif
endif

stop

@enduml
```

---

## 8. Component Diagram

```plantuml
@startuml ComponentDiagram
title XpertConnect Platform - Component Architecture

skinparam component {
    BackgroundColor<<external>> LightGray
}

package "Client Applications" {
    [Web Application] as WebApp
    [Mobile App (iOS)] as iOSApp
    [Mobile App (Android)] as AndroidApp
    [Admin Dashboard] as AdminDash
}

package "API Gateway" {
    [API Gateway / Load Balancer] as Gateway
    [Authentication Service] as AuthSvc
    [Rate Limiter] as RateLimiter
}

package "Core Services" {
    [User Service] as UserSvc
    [Expert Service] as ExpertSvc
    [Seeker Service] as SeekerSvc
    [Auction Service] as AuctionSvc
    [Booking Service] as BookingSvc
    [Project Service] as ProjectSvc
}

package "Verification & Compliance" {
    [KYC Service] as KYCSvc
    [Credential Verification] as CredSvc
    [Compliance Engine] as ComplianceSvc
    [Reputation Service] as ReputationSvc
}

package "Financial Services" {
    [Payment Service] as PaymentSvc
    [Escrow Service] as EscrowSvc
    [Invoice Service] as InvoiceSvc
    [Tax Documentation] as TaxSvc
}

package "Communication & Scheduling" {
    [Notification Service] as NotifySvc
    [Calendar Service] as CalendarSvc
    [Virtual Hub] as VirtualHub
    [Messaging Service] as MsgSvc
}

package "Physical Logistics" {
    [Concierge Service] as ConciergeSvc
    [Venue Service] as VenueSvc
    [Geofence Service] as GeoSvc
    [Security Coordination] as SecuritySvc
}

package "Contract & Legal" {
    [NDA Generator] as NDASvc
    [MOU Generator] as MOUSvc
    [Contract Storage] as ContractStore
}

package "Analytics & Reporting" {
    [Analytics Engine] as AnalyticsSvc
    [CSR Reporting] as CSRSvc
    [ROI Calculator] as ROISvc
}

package "AI & Matching" {
    [Smart Matching Engine] as MatchingEngine
    [Recommendation Service] as RecommendSvc
}

package "Data Layer" {
    database "Primary Database\n(PostgreSQL)" as PrimaryDB
    database "Search Index\n(Elasticsearch)" as SearchDB
    database "Cache\n(Redis)" as Cache
    database "Document Store\n(S3)" as DocStore
}

package "External Integrations" <<external>> {
    [Stripe Connect] as Stripe
    [Escrow.com API] as EscrowAPI
    [Google Calendar] as GCal
    [Outlook Calendar] as OCal
    [Ironclad CLM] as Ironclad
    [KYC Provider\n(Onfido/Jumio)] as KYCProvider
    [Sanctions Database] as SanctionsDB
    [LinkedIn API] as LinkedIn
    [SMS Gateway] as SMSGateway
    [Email Service] as EmailSvc
}

' Client to Gateway
WebApp --> Gateway
iOSApp --> Gateway
AndroidApp --> Gateway
AdminDash --> Gateway

' Gateway to Auth
Gateway --> AuthSvc
Gateway --> RateLimiter

' Auth to Services
AuthSvc --> UserSvc

' Core Service Connections
Gateway --> UserSvc
Gateway --> ExpertSvc
Gateway --> SeekerSvc
Gateway --> AuctionSvc
Gateway --> BookingSvc
Gateway --> ProjectSvc

' Verification Connections
UserSvc --> KYCSvc
ExpertSvc --> CredSvc
SeekerSvc --> ComplianceSvc
ComplianceSvc --> ReputationSvc

' Financial Connections
AuctionSvc --> EscrowSvc
BookingSvc --> PaymentSvc
PaymentSvc --> EscrowSvc
EscrowSvc --> InvoiceSvc
InvoiceSvc --> TaxSvc

' Communication Connections
AuctionSvc --> NotifySvc
BookingSvc --> NotifySvc
BookingSvc --> CalendarSvc
BookingSvc --> VirtualHub

' Logistics Connections
AuctionSvc --> ConciergeSvc
ConciergeSvc --> VenueSvc
ConciergeSvc --> GeoSvc
ConciergeSvc --> SecuritySvc

' Legal Connections
BookingSvc --> NDASvc
ProjectSvc --> MOUSvc
NDASvc --> ContractStore
MOUSvc --> ContractStore

' Analytics Connections
ProjectSvc --> CSRSvc
BookingSvc --> AnalyticsSvc
AnalyticsSvc --> ROISvc

' AI Connections
ExpertSvc --> MatchingEngine
SeekerSvc --> RecommendSvc
MatchingEngine --> RecommendSvc

' Data Layer Connections
UserSvc --> PrimaryDB
ExpertSvc --> PrimaryDB
AuctionSvc --> PrimaryDB
BookingSvc --> PrimaryDB
ExpertSvc --> SearchDB
SearchDB --> Cache
ContractStore --> DocStore

' External Integrations
PaymentSvc --> Stripe
EscrowSvc --> EscrowAPI
CalendarSvc --> GCal
CalendarSvc --> OCal
NDASvc --> Ironclad
KYCSvc --> KYCProvider
ComplianceSvc --> SanctionsDB
CredSvc --> LinkedIn
NotifySvc --> SMSGateway
NotifySvc --> EmailSvc

@enduml
```

---

## 9. State Diagram - Auction Lot Lifecycle

```plantuml
@startuml AuctionStateDiagram
title Auction Lot State Machine

[*] --> Draft : Expert Creates Lot

Draft : Lot details being defined
Draft : Meeting parameters set
Draft --> Draft : Edit Details
Draft --> PendingReview : Submit for Review

PendingReview : Awaiting admin approval
PendingReview --> Draft : Rejected (needs changes)
PendingReview --> Scheduled : Approved

Scheduled : Lot approved, waiting for start time
Scheduled --> Open : Start time reached
Scheduled --> Cancelled : Expert cancels

Open : Bidding active
Open : Accepting bids
Open --> Open : New bid placed
Open --> Closed : End time reached
Open --> Sold : Buy Now executed

Closed : Bidding ended
Closed : Winner being determined
Closed --> WinnerSelected : Winning bid identified
Closed --> NoBids : No valid bids received

NoBids : Auction ended without bids
NoBids --> [*]

WinnerSelected : Winning bidder notified
WinnerSelected --> PendingPayment : Winner accepts

Sold : Buy Now completed
Sold --> PendingPayment : Proceed to payment

PendingPayment : Awaiting escrow funding
PendingPayment --> Funded : Escrow funded
PendingPayment --> Forfeited : Payment timeout

Forfeited : Winner failed to pay
Forfeited --> WinnerSelected : Next highest bidder
Forfeited --> [*] : No other bidders

Funded : Payment secured
Funded --> Logistics : Concierge assigned

Logistics : Meeting being coordinated
Logistics : Venue & security arranged
Logistics --> Scheduled_Meeting : Logistics complete

Scheduled_Meeting : Meeting date confirmed
Scheduled_Meeting --> InProgress : Meeting started (geofence entry)

InProgress : Meeting taking place
InProgress : Monitoring dwell time
InProgress --> Completed : Meeting verified

Completed : Meeting successfully held
Completed --> Disbursed : Funds released

Disbursed : Payment distributed
Disbursed : Tax docs generated
Disbursed --> [*]

Cancelled : Lot cancelled
Cancelled --> [*]

state Disputed {
    [*] --> UnderReview
    UnderReview : Admin reviewing case
    UnderReview --> ResolvedForSeeker : Seeker wins dispute
    UnderReview --> ResolvedForExpert : Expert wins dispute
    ResolvedForSeeker --> [*]
    ResolvedForExpert --> [*]
}

InProgress --> Disputed : Issue reported
Completed --> Disputed : Post-meeting dispute

@enduml
```

---

## 10. State Diagram - Consultation Booking Lifecycle

```plantuml
@startuml ConsultationStateDiagram
title Consultation Booking State Machine

[*] --> Initiated : Seeker selects time slot

Initiated : Time slot selected
Initiated : NDA pending
Initiated --> NDA_Pending : Booking created

NDA_Pending : Waiting for signatures
NDA_Pending --> NDA_SeekerSigned : Seeker signs
NDA_Pending --> Cancelled : Timeout or rejection

NDA_SeekerSigned : Seeker has signed
NDA_SeekerSigned --> NDA_FullySigned : Expert signs
NDA_SeekerSigned --> Cancelled : Expert declines

NDA_FullySigned : Both parties signed NDA
NDA_FullySigned --> PaymentPending : Proceed to payment

PaymentPending : Awaiting payment authorization
PaymentPending --> PaymentAuthorized : Payment successful
PaymentPending --> Cancelled : Payment failed

PaymentAuthorized : Funds held
PaymentAuthorized --> Confirmed : Booking confirmed

Confirmed : Meeting scheduled
Confirmed : Calendar invites sent
Confirmed --> Confirmed : Reminder sent
Confirmed --> Rescheduled : Either party reschedules
Confirmed --> InProgress : Meeting time reached
Confirmed --> Cancelled : Either party cancels (within policy)
Confirmed --> NoShow : One party doesn't join

Rescheduled : New time being selected
Rescheduled --> Confirmed : New time confirmed
Rescheduled --> Cancelled : Unable to reschedule

state InProgress {
    [*] --> Connecting
    Connecting : Participants joining
    Connecting --> Active : Both joined
    Active : Session in progress
    Active --> Ending : Time limit approaching
    Ending --> [*] : Session ends
}

InProgress --> PendingCompletion : Session ends

NoShow : Participant failed to attend
NoShow --> PendingCompletion : Handled per policy

PendingCompletion : Awaiting seeker confirmation
PendingCompletion --> Completed : Seeker confirms
PendingCompletion --> Disputed : Issue raised

Completed : Session successfully completed
Completed --> FeedbackPending : Request feedback

FeedbackPending : Awaiting review
FeedbackPending --> Settled : Feedback received
FeedbackPending --> Settled : Feedback timeout

Settled : Payment captured & distributed
Settled --> [*]

Cancelled : Booking cancelled
Cancelled --> Refunded : Refund processed (if applicable)
Cancelled --> [*] : No refund needed

Refunded : Funds returned to seeker
Refunded --> [*]

state Disputed {
    [*] --> ReviewRequested
    ReviewRequested : Admin reviewing
    ReviewRequested --> PartialRefund : Compromise
    ReviewRequested --> FullRefund : Seeker favored
    ReviewRequested --> NoRefund : Expert favored
    PartialRefund --> [*]
    FullRefund --> [*]
    NoRefund --> [*]
}

Disputed --> Settled : Resolution applied

@enduml
```

---

## 11. Entity Relationship Diagram (ERD)

```plantuml
@startuml ERD
title XpertConnect Database Schema (ERD)

skinparam linetype ortho

entity "users" as users {
    * id : UUID <<PK>>
    --
    email : VARCHAR(255)
    phone : VARCHAR(50)
    password_hash : VARCHAR(255)
    user_type : ENUM
    created_at : TIMESTAMP
    updated_at : TIMESTAMP
    verification_status : ENUM
    is_active : BOOLEAN
}

entity "experts" as experts {
    * id : UUID <<PK>>
    --
    * user_id : UUID <<FK>>
    category : ENUM
    hourly_rate : DECIMAL
    bio : TEXT
    headline : VARCHAR(255)
    profile_photo_url : VARCHAR(500)
    is_available : BOOLEAN
    security_level : ENUM
}

entity "seekers" as seekers {
    * id : UUID <<PK>>
    --
    * user_id : UUID <<FK>>
    kyc_status : ENUM
    bid_eligible : BOOLEAN
    reputation_score : DECIMAL
    premium_status : BOOLEAN
}

entity "non_profit_orgs" as nonprofits {
    * id : UUID <<PK>>
    --
    * user_id : UUID <<FK>>
    org_name : VARCHAR(255)
    tax_id : VARCHAR(50)
    mission : TEXT
    verified_at : TIMESTAMP
}

entity "credentials" as credentials {
    * id : UUID <<PK>>
    --
    * expert_id : UUID <<FK>>
    type : VARCHAR(100)
    issuing_body : VARCHAR(255)
    issue_date : DATE
    expiry_date : DATE
    verification_source : VARCHAR(255)
    verified_at : TIMESTAMP
}

entity "kyc_verifications" as kyc {
    * id : UUID <<PK>>
    --
    * user_id : UUID <<FK>>
    document_type : VARCHAR(50)
    document_country : VARCHAR(3)
    biometric_hash : VARCHAR(500)
    verified_at : TIMESTAMP
    expires_at : TIMESTAMP
    provider_ref : VARCHAR(255)
}

entity "financial_references" as financial_refs {
    * id : UUID <<PK>>
    --
    * seeker_id : UUID <<FK>>
    bank_name : VARCHAR(255)
    account_type : VARCHAR(50)
    validated_amount : DECIMAL
    validated_at : TIMESTAMP
}

entity "compliance_checks" as compliance {
    * id : UUID <<PK>>
    --
    * user_id : UUID <<FK>>
    check_type : VARCHAR(50)
    sanctions_clear : BOOLEAN
    pep_clear : BOOLEAN
    adverse_media_clear : BOOLEAN
    checked_at : TIMESTAMP
    notes : TEXT
}

entity "auction_lots" as auctions {
    * id : UUID <<PK>>
    --
    * expert_id : UUID <<FK>>
    beneficiary_org_id : UUID <<FK>>
    title : VARCHAR(255)
    description : TEXT
    meeting_type : VARCHAR(50)
    guest_limit : INTEGER
    starting_bid : DECIMAL
    buy_now_price : DECIMAL
    start_time : TIMESTAMP
    end_time : TIMESTAMP
    status : ENUM
    winning_bid_id : UUID <<FK>>
}

entity "bids" as bids {
    * id : UUID <<PK>>
    --
    * auction_id : UUID <<FK>>
    * seeker_id : UUID <<FK>>
    amount : DECIMAL
    is_proxy : BOOLEAN
    max_proxy_amount : DECIMAL
    placed_at : TIMESTAMP
    is_winning : BOOLEAN
}

entity "consultations" as consultations {
    * id : UUID <<PK>>
    --
    * expert_id : UUID <<FK>>
    * seeker_id : UUID <<FK>>
    scheduled_at : TIMESTAMP
    duration_minutes : INTEGER
    rate : DECIMAL
    status : ENUM
    meeting_type : ENUM
    virtual_hub_link : VARCHAR(500)
    nda_id : UUID <<FK>>
}

entity "probono_projects" as projects {
    * id : UUID <<PK>>
    --
    * org_id : UUID <<FK>>
    expert_id : UUID <<FK>>
    title : VARCHAR(255)
    description : TEXT
    estimated_hours : INTEGER
    actual_hours : DECIMAL
    status : ENUM
    mou_id : UUID <<FK>>
}

entity "ndas" as ndas {
    * id : UUID <<PK>>
    --
    party_a_id : UUID
    party_b_id : UUID
    template_version : VARCHAR(20)
    generated_at : TIMESTAMP
    party_a_signed_at : TIMESTAMP
    party_b_signed_at : TIMESTAMP
    document_url : VARCHAR(500)
}

entity "mous" as mous {
    * id : UUID <<PK>>
    --
    * project_id : UUID <<FK>>
    scope : TEXT
    timeline : TEXT
    signed_at : TIMESTAMP
    document_url : VARCHAR(500)
}

entity "payments" as payments {
    * id : UUID <<PK>>
    --
    * reference_id : UUID
    reference_type : ENUM
    amount : DECIMAL
    currency : VARCHAR(3)
    status : ENUM
    method : VARCHAR(50)
    stripe_payment_id : VARCHAR(255)
    created_at : TIMESTAMP
}

entity "escrow_accounts" as escrow {
    * id : UUID <<PK>>
    --
    * payment_id : UUID <<FK>>
    amount : DECIMAL
    funded_at : TIMESTAMP
    released_at : TIMESTAMP
    status : ENUM
    escrow_provider_ref : VARCHAR(255)
}

entity "milestones" as milestones {
    * id : UUID <<PK>>
    --
    * escrow_id : UUID <<FK>>
    description : VARCHAR(255)
    amount : DECIMAL
    due_date : DATE
    completed_at : TIMESTAMP
    approved : BOOLEAN
}

entity "venues" as venues {
    * id : UUID <<PK>>
    --
    name : VARCHAR(255)
    address : TEXT
    latitude : DECIMAL
    longitude : DECIMAL
    security_rating : ENUM
    is_preapproved : BOOLEAN
}

entity "physical_meetings" as meetings {
    * id : UUID <<PK>>
    --
    * interaction_id : UUID
    interaction_type : ENUM
    * venue_id : UUID <<FK>>
    scheduled_at : TIMESTAMP
    * geofence_id : UUID <<FK>>
    verified : BOOLEAN
}

entity "geofences" as geofences {
    * id : UUID <<PK>>
    --
    * venue_id : UUID <<FK>>
    boundary_type : ENUM
    center_lat : DECIMAL
    center_lng : DECIMAL
    radius_meters : INTEGER
    polygon_coords : JSON
    dwell_duration_mins : INTEGER
}

entity "geofence_events" as geo_events {
    * id : UUID <<PK>>
    --
    * geofence_id : UUID <<FK>>
    * user_id : UUID <<FK>>
    event_type : ENUM
    latitude : DECIMAL
    longitude : DECIMAL
    timestamp : TIMESTAMP
}

entity "guests" as guests {
    * id : UUID <<PK>>
    --
    * meeting_id : UUID <<FK>>
    name : VARCHAR(255)
    relationship : VARCHAR(100)
    kyc_verified : BOOLEAN
    nda_signed : BOOLEAN
}

entity "time_entries" as time_entries {
    * id : UUID <<PK>>
    --
    * project_id : UUID <<FK>>
    * expert_id : UUID <<FK>>
    hours : DECIMAL
    description : TEXT
    logged_at : TIMESTAMP
}

entity "feedback" as feedback {
    * id : UUID <<PK>>
    --
    * consultation_id : UUID <<FK>>
    * seeker_id : UUID <<FK>>
    rating : INTEGER
    comments : TEXT
    created_at : TIMESTAMP
}

' Relationships
users ||--o| experts : has
users ||--o| seekers : has
users ||--o| nonprofits : has
users ||--o{ kyc : undergoes
users ||--o{ compliance : checked

experts ||--o{ credentials : has
experts ||--o{ auctions : creates
experts ||--o{ consultations : provides
experts ||--o{ projects : volunteers

seekers ||--o{ financial_refs : provides
seekers ||--o{ bids : places
seekers ||--o{ consultations : books
seekers ||--o{ feedback : gives

nonprofits ||--o{ projects : creates
nonprofits ||--o{ auctions : benefits

auctions ||--o{ bids : receives
auctions ||--o| meetings : has

consultations ||--|| ndas : requires
consultations ||--|| payments : has
consultations ||--o| meetings : may_have

projects ||--|| mous : requires
projects ||--o{ time_entries : tracks

payments ||--o| escrow : may_have
escrow ||--o{ milestones : has

venues ||--o{ meetings : hosts
venues ||--|| geofences : defines

geofences ||--o{ geo_events : logs

meetings ||--o{ guests : includes

@enduml
```

---

## How to Render These Diagrams

You can render these PlantUML diagrams using:

1. **Online Tools**:
   - [PlantUML Web Server](https://www.plantuml.com/plantuml/uml/)
   - [PlantText](https://www.planttext.com/)

2. **IDE Extensions**:
   - VS Code: "PlantUML" extension
   - IntelliJ: "PlantUML Integration" plugin

3. **Command Line**:
   ```bash
   java -jar plantuml.jar uml-diagrams.md
   ```

4. **Documentation Tools**:
   - Confluence (with PlantUML plugin)
   - GitLab/GitHub (with PlantUML rendering)
