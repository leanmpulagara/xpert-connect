import { act, renderHook } from '@testing-library/react';
import { useAuthStore } from '../auth';

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {};
  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value.toString();
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
})();

Object.defineProperty(window, 'localStorage', { value: localStorageMock });

describe('Auth Store', () => {
  beforeEach(() => {
    // Reset store state before each test
    localStorageMock.clear();
    const { result } = renderHook(() => useAuthStore());
    act(() => {
      result.current.logout();
    });
  });

  it('initializes with null user and token', () => {
    const { result } = renderHook(() => useAuthStore());

    expect(result.current.user).toBeNull();
    expect(result.current.token).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('sets user and token on login', () => {
    const { result } = renderHook(() => useAuthStore());

    const mockUser = {
      id: '123',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      userType: 'Seeker' as const,
    };
    const mockToken = 'jwt-token-123';

    act(() => {
      result.current.login(mockUser, mockToken);
    });

    expect(result.current.user).toEqual(mockUser);
    expect(result.current.token).toBe(mockToken);
    expect(result.current.isAuthenticated).toBe(true);
  });

  it('clears user and token on logout', () => {
    const { result } = renderHook(() => useAuthStore());

    // First login
    act(() => {
      result.current.login(
        {
          id: '123',
          email: 'test@example.com',
          firstName: 'Test',
          lastName: 'User',
          userType: 'Seeker',
        },
        'token'
      );
    });

    // Then logout
    act(() => {
      result.current.logout();
    });

    expect(result.current.user).toBeNull();
    expect(result.current.token).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('updates user data', () => {
    const { result } = renderHook(() => useAuthStore());

    // Login first
    act(() => {
      result.current.login(
        {
          id: '123',
          email: 'test@example.com',
          firstName: 'Test',
          lastName: 'User',
          userType: 'Seeker',
        },
        'token'
      );
    });

    // Update user
    act(() => {
      result.current.setUser({
        id: '123',
        email: 'updated@example.com',
        firstName: 'Updated',
        lastName: 'User',
        userType: 'Seeker',
      });
    });

    expect(result.current.user?.email).toBe('updated@example.com');
    expect(result.current.user?.firstName).toBe('Updated');
  });
});
