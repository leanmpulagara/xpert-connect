// User types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  profilePhotoUrl?: string;
  userType: 'Expert' | 'Seeker' | 'NonProfit' | 'Admin';
  isActive: boolean;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phone?: string;
  userType: 'Expert' | 'Seeker' | 'NonProfit';
}

// Expert types
export interface Expert {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  profilePhotoUrl?: string;
  headline?: string;
  bio?: string;
  category?: string;
  hourlyRate?: number;
  currency: string;
  linkedInUrl?: string;
  isVerified: boolean;
  averageRating?: number;
  totalReviews: number;
  credentials: Credential[];
  availability: Availability[];
}

export interface Credential {
  id: string;
  type: string;
  title: string;
  issuer?: string;
  issuedDate?: string;
  isVerified: boolean;
}

export interface Availability {
  id: string;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
}

export interface ExpertListItem {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  profilePhotoUrl?: string;
  headline?: string;
  category: number;
  categoryName: string;
  hourlyRate?: number;
  currency: string;
  isAvailable: boolean;
  verificationStatus: number;
  verificationStatusName: string;
  credentialCount: number;
  averageRating?: number;
  totalReviews?: number;
}

// Seeker types
export interface Seeker {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  profilePhotoUrl?: string;
  company?: string;
  jobTitle?: string;
  linkedInUrl?: string;
}

// Consultation types
export interface Consultation {
  id: string;
  expertId: string;
  expertName: string;
  expertProfilePhotoUrl?: string;
  seekerId: string;
  seekerName: string;
  scheduledAt: string;
  durationMinutes: number;
  rate: number;
  currency: string;
  totalAmount: number;
  status: string;
  meetingType: string;
  virtualHubLink?: string;
  notes?: string;
  hasFeedback: boolean;
}

// Auction types
export interface Auction {
  id: string;
  expertId: string;
  expertName: string;
  expertPhotoUrl?: string;
  title: string;
  description?: string;
  meetingType: string;
  guestLimit: number;
  startingBid: number;
  buyNowPrice?: number;
  currentHighBid?: number;
  startTime: string;
  endTime: string;
  status: string;
  bidCount: number;
  isWinner?: boolean;
  winnerName?: string;
  winningAmount?: number;
}

export interface Bid {
  id: string;
  auctionId: string;
  amount: number;
  isProxyBid: boolean;
  placedAt: string;
  isWinning: boolean;
}

// Pagination
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Pro-Bono Project types
export interface Project {
  id: string;
  title: string;
  description: string;
  organizationId: string;
  organizationName: string;
  organizationLogoUrl?: string;
  expertId?: string;
  expertName?: string;
  category: string;
  requiredSkills: string[];
  estimatedHours: number;
  loggedHours: number;
  deadline?: string;
  status: string;
  createdAt: string;
}

export interface TimeEntry {
  id: string;
  projectId: string;
  projectTitle: string;
  expertId: string;
  expertName: string;
  hours: number;
  description: string;
  loggedAt: string;
}

// Payment types
export interface Payment {
  id: string;
  amount: number;
  currency: string;
  status: string;
  paymentType: string;
  description?: string;
  referenceType?: string;
  referenceId?: string;
  createdAt: string;
  completedAt?: string;
}

export interface EscrowAccount {
  id: string;
  amount: number;
  currency: string;
  status: string;
  consultationId?: string;
  auctionId?: string;
  createdAt: string;
  milestones: Milestone[];
}

export interface Milestone {
  id: string;
  escrowAccountId: string;
  title: string;
  amount: number;
  status: string;
  dueDate?: string;
  approvedAt?: string;
}

// API Error
export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
