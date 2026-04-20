'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  Calendar,
  Clock,
  Video,
  MapPin,
  ArrowLeft,
  Star,
  MessageSquare,
  FileText,
  CheckCircle,
} from 'lucide-react';
import { consultationsApi, api } from '@/lib/api';
import { formatCurrency, formatDateTime } from '@/lib/utils';
import { useAuthStore } from '@/stores/auth';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Button,
  Avatar,
  Badge,
  Input,
  LoadingPage,
  ErrorMessage,
  Modal,
  ModalFooter,
} from '@/components/ui';
import { cn } from '@/lib/utils';
import type { Consultation } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info'; description: string }> = {
  Pending: { label: 'Pending', variant: 'warning', description: 'Waiting for expert confirmation' },
  Confirmed: { label: 'Confirmed', variant: 'info', description: 'The consultation is confirmed' },
  InProgress: { label: 'In Progress', variant: 'info', description: 'Consultation is happening now' },
  Completed: { label: 'Completed', variant: 'success', description: 'Consultation completed' },
  Cancelled: { label: 'Cancelled', variant: 'danger', description: 'Consultation was cancelled' },
  NoShow: { label: 'No Show', variant: 'danger', description: 'One party did not attend' },
};

const feedbackSchema = z.object({
  rating: z.number().min(1).max(5),
  comment: z.string().min(10, 'Please provide at least 10 characters of feedback'),
});

type FeedbackFormData = z.infer<typeof feedbackSchema>;

export default function ConsultationDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { user } = useAuthStore();
  const [consultation, setConsultation] = useState<Consultation | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showFeedbackModal, setShowFeedbackModal] = useState(false);
  const [selectedRating, setSelectedRating] = useState(0);
  const [isSubmittingFeedback, setIsSubmittingFeedback] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<FeedbackFormData>({
    resolver: zodResolver(feedbackSchema),
  });

  useEffect(() => {
    const fetchConsultation = async () => {
      if (!params.id) return;

      setIsLoading(true);
      try {
        const data = await consultationsApi.getById(params.id as string) as Consultation;
        setConsultation(data);
      } catch {
        setError('Failed to load consultation details');
      } finally {
        setIsLoading(false);
      }
    };

    fetchConsultation();
  }, [params.id]);

  const handleCancel = async () => {
    if (!consultation || !confirm('Are you sure you want to cancel this consultation?')) return;

    try {
      await consultationsApi.cancel(consultation.id);
      setConsultation({ ...consultation, status: 'Cancelled' });
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to cancel consultation');
    }
  };

  const onSubmitFeedback = async (data: FeedbackFormData) => {
    if (!consultation) return;

    setIsSubmittingFeedback(true);
    try {
      await api.post(`/api/consultations/${consultation.id}/feedback`, {
        rating: data.rating,
        comment: data.comment,
      });
      setConsultation({ ...consultation, hasFeedback: true });
      setShowFeedbackModal(false);
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to submit feedback');
    } finally {
      setIsSubmittingFeedback(false);
    }
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  if (!consultation) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-16 text-center">
        <h1 className="text-2xl font-bold">Consultation Not Found</h1>
        <Link href="/consultations">
          <Button className="mt-4">Back to Consultations</Button>
        </Link>
      </div>
    );
  }

  const status = statusConfig[consultation.status] || { label: consultation.status, variant: 'default' as const, description: '' };
  const isSeeker = user?.userType === 'Seeker';
  const canCancel = ['Pending', 'Confirmed'].includes(consultation.status);
  const canLeaveFeedback = consultation.status === 'Completed' && !consultation.hasFeedback && isSeeker;

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Back Link */}
      <Link
        href="/consultations"
        className="inline-flex items-center text-sm text-gray-600 hover:text-gray-900 mb-6"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Back to consultations
      </Link>

      {error && <ErrorMessage message={error} className="mb-6" />}

      <div className="grid md:grid-cols-3 gap-8">
        {/* Main Content */}
        <div className="md:col-span-2 space-y-6">
          {/* Header Card */}
          <Card>
            <CardContent className="p-6">
              <div className="flex items-start justify-between">
                <div className="flex items-center gap-4">
                  <Avatar
                    src={consultation.expertProfilePhotoUrl}
                    firstName={isSeeker ? consultation.expertName.split(' ')[0] : consultation.seekerName.split(' ')[0]}
                    lastName={isSeeker ? consultation.expertName.split(' ')[1] : consultation.seekerName.split(' ')[1]}
                    size="xl"
                  />
                  <div>
                    <h1 className="text-xl font-bold text-gray-900">
                      Consultation with {isSeeker ? consultation.expertName : consultation.seekerName}
                    </h1>
                    <Badge variant={status.variant} className="mt-2">
                      {status.label}
                    </Badge>
                    <p className="text-sm text-gray-500 mt-1">{status.description}</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Details Card */}
          <Card>
            <CardHeader>
              <CardTitle>Session Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <Calendar className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Date & Time</p>
                    <p className="font-medium">{formatDateTime(consultation.scheduledAt)}</p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <Clock className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Duration</p>
                    <p className="font-medium">{consultation.durationMinutes} minutes</p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    {consultation.meetingType === 'Virtual' ? (
                      <Video className="h-5 w-5 text-blue-600" />
                    ) : (
                      <MapPin className="h-5 w-5 text-blue-600" />
                    )}
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Meeting Type</p>
                    <p className="font-medium">{consultation.meetingType}</p>
                  </div>
                </div>
              </div>

              {consultation.virtualHubLink && ['Confirmed', 'InProgress'].includes(consultation.status) && (
                <div className="mt-6 p-4 bg-blue-50 rounded-lg">
                  <p className="text-sm text-blue-700 mb-2">Your virtual meeting room is ready</p>
                  <a href={consultation.virtualHubLink} target="_blank" rel="noopener noreferrer">
                    <Button>
                      <Video className="h-4 w-4 mr-2" />
                      Join Video Call
                    </Button>
                  </a>
                </div>
              )}

              {consultation.notes && (
                <div className="mt-4">
                  <h4 className="text-sm font-medium text-gray-700 mb-2 flex items-center">
                    <MessageSquare className="h-4 w-4 mr-2" />
                    Notes
                  </h4>
                  <p className="text-gray-600 bg-gray-50 p-3 rounded-lg">{consultation.notes}</p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Feedback Card */}
          {consultation.hasFeedback && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center">
                  <Star className="h-5 w-5 mr-2 text-yellow-500" />
                  Feedback Submitted
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center gap-2">
                  <CheckCircle className="h-5 w-5 text-green-500" />
                  <span className="text-gray-600">You have already left feedback for this consultation.</span>
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Payment Summary */}
          <Card>
            <CardHeader>
              <CardTitle>Payment Summary</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Rate</span>
                <span>{formatCurrency(consultation.rate, consultation.currency)}/hr</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Duration</span>
                <span>{consultation.durationMinutes} min</span>
              </div>
              <hr />
              <div className="flex justify-between">
                <span className="font-medium">Total</span>
                <span className="text-xl font-bold text-blue-600">
                  {formatCurrency(consultation.totalAmount, consultation.currency)}
                </span>
              </div>
            </CardContent>
          </Card>

          {/* Actions */}
          <Card>
            <CardHeader>
              <CardTitle>Actions</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {canLeaveFeedback && (
                <Button className="w-full" onClick={() => setShowFeedbackModal(true)}>
                  <Star className="h-4 w-4 mr-2" />
                  Leave Feedback
                </Button>
              )}
              {canCancel && (
                <Button variant="danger" className="w-full" onClick={handleCancel}>
                  Cancel Consultation
                </Button>
              )}
              <Button variant="outline" className="w-full">
                <FileText className="h-4 w-4 mr-2" />
                Download Receipt
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Feedback Modal */}
      <Modal
        isOpen={showFeedbackModal}
        onClose={() => setShowFeedbackModal(false)}
        title="Leave Feedback"
        description="Share your experience with this consultation"
      >
        <form onSubmit={handleSubmit(onSubmitFeedback)}>
          <div className="space-y-4">
            {/* Star Rating */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rating
              </label>
              <div className="flex gap-2">
                {[1, 2, 3, 4, 5].map((star) => (
                  <button
                    key={star}
                    type="button"
                    onClick={() => {
                      setSelectedRating(star);
                      setValue('rating', star);
                    }}
                    className="p-1"
                  >
                    <Star
                      className={cn(
                        'h-8 w-8 transition-colors',
                        star <= selectedRating
                          ? 'text-yellow-400 fill-yellow-400'
                          : 'text-gray-300'
                      )}
                    />
                  </button>
                ))}
              </div>
              {errors.rating && (
                <p className="mt-1 text-sm text-red-600">Please select a rating</p>
              )}
            </div>

            {/* Comment */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Your Feedback
              </label>
              <textarea
                {...register('comment')}
                rows={4}
                className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Share your experience..."
              />
              {errors.comment && (
                <p className="mt-1 text-sm text-red-600">{errors.comment.message}</p>
              )}
            </div>
          </div>

          <ModalFooter>
            <Button variant="outline" onClick={() => setShowFeedbackModal(false)}>
              Cancel
            </Button>
            <Button type="submit" isLoading={isSubmittingFeedback}>
              Submit Feedback
            </Button>
          </ModalFooter>
        </form>
      </Modal>
    </div>
  );
}
