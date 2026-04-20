'use client';

import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { authApi } from '@/lib/api';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: string;
  profilePhotoUrl?: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
  expiresAt: string | null;
  isLoading: boolean;
  error: string | null;

  // Actions
  login: (email: string, password: string) => Promise<void>;
  register: (data: {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    userType: string;
  }) => Promise<void>;
  logout: () => void;
  fetchUser: () => Promise<void>;
  clearError: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      refreshToken: null,
      expiresAt: null,
      isLoading: false,
      error: null,

      login: async (email: string, password: string) => {
        set({ isLoading: true, error: null });
        try {
          const response = await authApi.login({ email, password });

          // Store token in localStorage for API client
          localStorage.setItem('token', response.token);

          set({
            token: response.token,
            refreshToken: response.refreshToken,
            expiresAt: response.expiresAt,
            isLoading: false,
          });

          // Fetch user data
          await get().fetchUser();
        } catch (err: unknown) {
          const error = err as { message?: string };
          set({
            error: error.message || 'Login failed',
            isLoading: false,
          });
          throw err;
        }
      },

      register: async (data) => {
        set({ isLoading: true, error: null });
        try {
          const response = await authApi.register(data);

          // Store token in localStorage for API client
          localStorage.setItem('token', response.token);

          set({
            token: response.token,
            refreshToken: response.refreshToken,
            expiresAt: response.expiresAt,
            isLoading: false,
          });

          // Fetch user data
          await get().fetchUser();
        } catch (err: unknown) {
          const error = err as { message?: string };
          set({
            error: error.message || 'Registration failed',
            isLoading: false,
          });
          throw err;
        }
      },

      logout: () => {
        localStorage.removeItem('token');
        set({
          user: null,
          token: null,
          refreshToken: null,
          expiresAt: null,
          error: null,
        });
      },

      fetchUser: async () => {
        const { token } = get();
        if (!token) return;

        try {
          const user = await authApi.me();
          set({ user: user as User });
        } catch (err) {
          // Token might be invalid
          get().logout();
          throw err;
        }
      },

      clearError: () => set({ error: null }),
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        token: state.token,
        refreshToken: state.refreshToken,
        expiresAt: state.expiresAt,
        user: state.user,
      }),
    }
  )
);
