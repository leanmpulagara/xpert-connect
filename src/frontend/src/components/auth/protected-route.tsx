'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/stores/auth';
import { LoadingPage } from '@/components/ui';

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: string[];
}

export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const router = useRouter();
  const { user, token, fetchUser, isLoading } = useAuthStore();

  useEffect(() => {
    if (!token) {
      router.push('/login');
      return;
    }

    if (!user) {
      fetchUser().catch(() => {
        router.push('/login');
      });
    }
  }, [token, user, fetchUser, router]);

  useEffect(() => {
    if (user && allowedRoles && allowedRoles.length > 0) {
      if (!allowedRoles.includes(user.userType)) {
        router.push('/dashboard');
      }
    }
  }, [user, allowedRoles, router]);

  if (isLoading || !user) {
    return <LoadingPage />;
  }

  if (allowedRoles && allowedRoles.length > 0 && !allowedRoles.includes(user.userType)) {
    return <LoadingPage />;
  }

  return <>{children}</>;
}
