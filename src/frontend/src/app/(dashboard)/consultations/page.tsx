'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { Calendar, Clock, Video, MapPin, CheckCircle, XCircle, AlertCircle } from 'lucide-react';
import { consultationsApi } from '@/lib/api';
import { formatCurrency, formatDateTime } from '@/lib/utils';
import { useAuthStore } from '@/stores/auth';
import {
  Card,
  CardContent,
  Button,
  Avatar,
  Badge,
  LoadingPage,
  ErrorMessage,
} from '@/components/ui';
import type { Consultation, PagedResult } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info' }> = {
  Pending: { label: 'Pending', variant: 'warning' },
  Confirmed: { label: 'Confirmed', variant: 'info' },
  InProgress: { label: 'In Progress', variant: 'info' },
  Completed: { label: 'Completed', variant: 'success' },
  Cancelled: { label: 'Cancelled', variant: 'danger' },
  NoShow: { label: 'No Show', variant: 'danger' },
};

export default function ConsultationsPage() {
  const searchParams = useSearchParams();
  const { user } = useAuthStore();
  const [consultations, setConsultations] = useState<Consultation[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showSuccess, setShowSuccess] = useState(false);

  useEffect(() => {
    if (searchParams.get('booked') === 'true') {
      setShowSuccess(true);
      setTimeout(() => setShowSuccess(false), 5000);
    }
  }, [searchParams]);

  useEffect(() => {
    const fetchConsultations = async () => {
      setIsLoading(true);
      try {
        const endpoint = user?.userType === 'Expert'
          ? consultationsApi.getExpert
          : consultationsApi.getMy;
        const data = await endpoint({ pageSize: 50 }) as PagedResult<Consultation>;
        setConsultations(data.items);
      } catch (err) {
        const apiError = err as { message?: string };
        setError(apiError.message || 'Failed to load consultations');
      } finally {
        setIsLoading(false);
      }
    };

    if (user) {
      fetchConsultations();
    }
  }, [user]);

  const handleCancel = async (id: string) => {
    if (!confirm('Are you sure you want to cancel this consultation?')) return;

    try {
      await consultationsApi.cancel(id);
      setConsultations(prev =>
        prev.map(c => c.id === id ? { ...c, status: 'Cancelled' } : c)
      );
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to cancel consultation');
    }
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  const upcomingConsultations = consultations.filter(
    c => ['Pending', 'Confirmed'].includes(c.status)
  );
  const pastConsultations = consultations.filter(
    c => ['Completed', 'Cancelled', 'NoShow'].includes(c.status)
  );

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Consultations</h1>
          <p className="mt-1 text-gray-600">
            {user?.userType === 'Expert' ? 'Manage your scheduled sessions' : 'Your booked consultations'}
          </p>
        </div>
        {user?.userType === 'Seeker' && (
          <Link href="/experts">
            <Button>
              <Calendar className="h-4 w-4 mr-2" />
              Book New
            </Button>
          </Link>
        )}
      </div>

      {showSuccess && (
        <div className="mb-6 p-4 rounded-lg bg-green-50 border border-green-200 flex items-center gap-3">
          <CheckCircle className="h-5 w-5 text-green-600" />
          <span className="text-green-700">Consultation booked successfully!</span>
        </div>
      )}

      {error && <ErrorMessage message={error} className="mb-6" />}

      {/* Upcoming */}
      <section className="mb-12">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Upcoming ({upcomingConsultations.length})
        </h2>
        {upcomingConsultations.length > 0 ? (
          <div className="space-y-4">
            {upcomingConsultations.map((consultation) => (
              <ConsultationCard
                key={consultation.id}
                consultation={consultation}
                userType={user?.userType}
                onCancel={handleCancel}
              />
            ))}
          </div>
        ) : (
          <Card>
            <CardContent className="p-8 text-center">
              <Calendar className="h-12 w-12 mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500">No upcoming consultations</p>
              {user?.userType === 'Seeker' && (
                <Link href="/experts">
                  <Button className="mt-4" variant="outline">
                    Browse Experts
                  </Button>
                </Link>
              )}
            </CardContent>
          </Card>
        )}
      </section>

      {/* Past */}
      <section>
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Past ({pastConsultations.length})
        </h2>
        {pastConsultations.length > 0 ? (
          <div className="space-y-4">
            {pastConsultations.map((consultation) => (
              <ConsultationCard
                key={consultation.id}
                consultation={consultation}
                userType={user?.userType}
                isPast
              />
            ))}
          </div>
        ) : (
          <p className="text-gray-500 text-sm">No past consultations</p>
        )}
      </section>
    </div>
  );
}

interface ConsultationCardProps {
  consultation: Consultation;
  userType?: string;
  isPast?: boolean;
  onCancel?: (id: string) => void;
}

function ConsultationCard({ consultation, userType, isPast, onCancel }: ConsultationCardProps) {
  const status = statusConfig[consultation.status] || { label: consultation.status, variant: 'default' as const };
  const isSeeker = userType === 'Seeker';

  return (
    <Card>
      <CardContent className="p-6">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          {/* Person Info */}
          <div className="flex items-center gap-4">
            <Avatar
              src={consultation.expertProfilePhotoUrl}
              firstName={isSeeker ? consultation.expertName.split(' ')[0] : consultation.seekerName.split(' ')[0]}
              lastName={isSeeker ? consultation.expertName.split(' ')[1] : consultation.seekerName.split(' ')[1]}
              size="lg"
            />
            <div>
              <h3 className="font-semibold text-gray-900">
                {isSeeker ? consultation.expertName : consultation.seekerName}
              </h3>
              <div className="flex items-center gap-4 mt-1 text-sm text-gray-500">
                <span className="flex items-center">
                  <Calendar className="h-4 w-4 mr-1" />
                  {formatDateTime(consultation.scheduledAt)}
                </span>
                <span className="flex items-center">
                  <Clock className="h-4 w-4 mr-1" />
                  {consultation.durationMinutes} min
                </span>
              </div>
              <div className="flex items-center gap-2 mt-2">
                <Badge variant={status.variant}>{status.label}</Badge>
                {consultation.meetingType === 'Virtual' ? (
                  <span className="flex items-center text-xs text-gray-500">
                    <Video className="h-3 w-3 mr-1" />
                    Virtual
                  </span>
                ) : (
                  <span className="flex items-center text-xs text-gray-500">
                    <MapPin className="h-3 w-3 mr-1" />
                    In Person
                  </span>
                )}
              </div>
            </div>
          </div>

          {/* Price & Actions */}
          <div className="flex flex-col items-end gap-2">
            <p className="text-lg font-semibold text-gray-900">
              {formatCurrency(consultation.totalAmount, consultation.currency)}
            </p>

            <div className="flex gap-2">
              {!isPast && consultation.virtualHubLink && (
                <a href={consultation.virtualHubLink} target="_blank" rel="noopener noreferrer">
                  <Button size="sm">
                    <Video className="h-4 w-4 mr-2" />
                    Join Call
                  </Button>
                </a>
              )}
              <Link href={`/consultations/${consultation.id}`}>
                <Button size="sm" variant="outline">
                  View Details
                </Button>
              </Link>
              {!isPast && ['Pending', 'Confirmed'].includes(consultation.status) && onCancel && (
                <Button
                  size="sm"
                  variant="ghost"
                  className="text-red-600 hover:text-red-700"
                  onClick={() => onCancel(consultation.id)}
                >
                  <XCircle className="h-4 w-4" />
                </Button>
              )}
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
