import type { ApiError } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5200';

interface RequestConfig extends RequestInit {
  params?: Record<string, string | number | boolean | undefined>;
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  private getToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  private buildUrl(endpoint: string, params?: Record<string, string | number | boolean | undefined>): string {
    const url = new URL(`${this.baseUrl}${endpoint}`);
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined) {
          url.searchParams.append(key, String(value));
        }
      });
    }
    return url.toString();
  }

  private async request<T>(endpoint: string, config: RequestConfig = {}): Promise<T> {
    const { params, ...fetchConfig } = config;
    const url = this.buildUrl(endpoint, params);
    const token = this.getToken();

    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...config.headers,
    };

    if (token) {
      (headers as Record<string, string>)['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, {
      ...fetchConfig,
      headers,
    });

    if (!response.ok) {
      let error: ApiError;
      try {
        error = await response.json();
      } catch {
        error = { message: `HTTP error ${response.status}` };
      }
      throw error;
    }

    // Handle 204 No Content
    if (response.status === 204) {
      return undefined as T;
    }

    return response.json();
  }

  async get<T>(endpoint: string, params?: Record<string, string | number | boolean | undefined>): Promise<T> {
    return this.request<T>(endpoint, { method: 'GET', params });
  }

  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

export const api = new ApiClient(API_BASE_URL);

// Auth API
export const authApi = {
  login: (data: { email: string; password: string }) =>
    api.post<{ token: string; refreshToken: string; expiresAt: string }>('/api/auth/login', data),

  register: (data: { email: string; password: string; confirmPassword: string; firstName: string; lastName: string; userType: string }) =>
    api.post<{ token: string; refreshToken: string; expiresAt: string }>('/api/auth/register', data),

  refreshToken: (data: { token: string; refreshToken: string }) =>
    api.post<{ token: string; refreshToken: string; expiresAt: string }>('/api/auth/refresh-token', data),

  revokeToken: (data: { refreshToken: string }) =>
    api.post('/api/auth/revoke-token', data),

  me: () => api.get<{ id: string; email: string; firstName: string; lastName: string; userType: string }>('/api/auth/me'),
};

// Users API
export const usersApi = {
  getMe: () => api.get('/api/users/me'),
  updateMe: (data: unknown) => api.put('/api/users/me', data),
  changePassword: (data: { currentPassword: string; newPassword: string; confirmNewPassword: string }) =>
    api.post('/api/users/me/change-password', data),
};

// Experts API
export const expertsApi = {
  getAll: (params?: { page?: number; pageSize?: number; category?: string; search?: string }) =>
    api.get('/api/experts', params),
  getById: (id: string) => api.get(`/api/experts/${id}`),
  getMe: () => api.get('/api/experts/me'),
  createProfile: (data: unknown) => api.post('/api/experts/me', data),
  updateProfile: (data: unknown) => api.put('/api/experts/me', data),
};

// Seekers API
export const seekersApi = {
  getMe: () => api.get('/api/seekers/me'),
  createProfile: (data: unknown) => api.post('/api/seekers/me', data),
  updateProfile: (data: unknown) => api.put('/api/seekers/me', data),
};

// Consultations API
export const consultationsApi = {
  getById: (id: string) => api.get(`/api/consultations/${id}`),
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/consultations/my', params),
  getExpert: (params?: { page?: number; pageSize?: number }) => api.get('/api/consultations/expert', params),
  create: (data: unknown) => api.post('/api/consultations', data),
  cancel: (id: string) => api.post(`/api/consultations/${id}/cancel`),
};

// Auctions API
export const auctionsApi = {
  getAll: (params?: { page?: number; pageSize?: number; status?: string }) => api.get('/api/auctions', params),
  getById: (id: string) => api.get(`/api/auctions/${id}`),
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/auctions/my', params),
  create: (data: unknown) => api.post('/api/auctions', data),
  placeBid: (auctionId: string, data: { amount: number; isProxyBid?: boolean; maxProxyAmount?: number }) =>
    api.post(`/api/auctions/${auctionId}/bids`, data),
  getBidHistory: (auctionId: string) => api.get(`/api/auctions/${auctionId}/bids`),
};

// Bids API
export const bidsApi = {
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/bids/my', params),
  getById: (id: string) => api.get(`/api/bids/${id}`),
};

// Projects API (Pro-Bono)
export const projectsApi = {
  getAll: (params?: { page?: number; pageSize?: number; category?: string; status?: string }) =>
    api.get('/api/projects', params),
  getById: (id: string) => api.get(`/api/projects/${id}`),
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/projects/my', params),
  getMyOrg: (params?: { page?: number; pageSize?: number }) => api.get('/api/projects/my-org', params),
  create: (data: unknown) => api.post('/api/projects', data),
  update: (id: string, data: unknown) => api.put(`/api/projects/${id}`, data),
  publish: (id: string) => api.post(`/api/projects/${id}/publish`),
  apply: (id: string) => api.post(`/api/projects/${id}/apply`),
  accept: (id: string, expertId: string) => api.post(`/api/projects/${id}/accept`, { expertId }),
  reject: (id: string, expertId: string) => api.post(`/api/projects/${id}/reject`, { expertId }),
  start: (id: string) => api.post(`/api/projects/${id}/start`),
  complete: (id: string) => api.post(`/api/projects/${id}/complete`),
  cancel: (id: string) => api.post(`/api/projects/${id}/cancel`),
};

// Time Entries API
export const timeEntriesApi = {
  getByProject: (projectId: string) => api.get(`/api/time-entries/project/${projectId}`),
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/time-entries/my', params),
  getById: (id: string) => api.get(`/api/time-entries/${id}`),
  create: (projectId: string, data: { hours: number; description: string }) =>
    api.post(`/api/time-entries/project/${projectId}`, data),
  update: (id: string, data: { hours: number; description: string }) =>
    api.put(`/api/time-entries/${id}`, data),
  delete: (id: string) => api.delete(`/api/time-entries/${id}`),
  getSummary: () => api.get('/api/time-entries/summary'),
};

// Payments API
export const paymentsApi = {
  authorize: (data: { amount: number; currency: string; referenceType: string; referenceId: string }) =>
    api.post('/api/payments/authorize', data),
  capture: (id: string) => api.post(`/api/payments/${id}/capture`),
  refund: (id: string, data?: { amount?: number }) => api.post(`/api/payments/${id}/refund`, data),
  cancel: (id: string) => api.post(`/api/payments/${id}/cancel`),
  getById: (id: string) => api.get(`/api/payments/${id}`),
  getMy: (params?: { page?: number; pageSize?: number }) => api.get('/api/payments/my', params),
};

// Escrow API
export const escrowApi = {
  create: (data: { amount: number; currency: string; consultationId?: string; auctionId?: string }) =>
    api.post('/api/escrow', data),
  getById: (id: string) => api.get(`/api/escrow/${id}`),
  fund: (id: string) => api.post(`/api/escrow/${id}/fund`),
  release: (id: string) => api.post(`/api/escrow/${id}/release`),
  dispute: (id: string, data: { reason: string }) => api.post(`/api/escrow/${id}/dispute`, data),
  addMilestone: (id: string, data: { title: string; amount: number; dueDate?: string }) =>
    api.post(`/api/escrow/${id}/milestones`, data),
  approveMilestone: (milestoneId: string) => api.post(`/api/escrow/milestones/${milestoneId}/approve`),
};
