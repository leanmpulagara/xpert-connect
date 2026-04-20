'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Calendar, Clock, Video, MapPin, CreditCard, ArrowLeft } from 'lucide-react';
import Link from 'next/link';
import { expertsApi, consultationsApi } from '@/lib/api';
import { formatCurrency } from '@/lib/utils';
import { useAuthStore } from '@/stores/auth';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Button,
  Input,
  Avatar,
  Badge,
  LoadingPage,
  ErrorMessage,
} from '@/components/ui';
import { ProtectedRoute } from '@/components/auth';
import { cn } from '@/lib/utils';
import type { Expert } from '@/types';

const bookingSchema = z.object({
  date: z.string().min(1, 'Please select a date'),
  time: z.string().min(1, 'Please select a time'),
  duration: z.number().min(30, 'Minimum duration is 30 minutes'),
  meetingType: z.enum(['Virtual', 'InPerson']),
  notes: z.string().optional(),
});

type BookingFormData = z.infer<typeof bookingSchema>;

const durations = [
  { value: 30, label: '30 minutes' },
  { value: 60, label: '1 hour' },
  { value: 90, label: '1.5 hours' },
  { value: 120, label: '2 hours' },
];

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function BookExpertPage() {
  const params = useParams();
  const router = useRouter();
  const { user } = useAuthStore();
  const [expert, setExpert] = useState<Expert | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isBooking, setIsBooking] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedDuration, setSelectedDuration] = useState(60);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<BookingFormData>({
    resolver: zodResolver(bookingSchema),
    defaultValues: {
      duration: 60,
      meetingType: 'Virtual',
    },
  });

  const watchDate = watch('date');
  const watchMeetingType = watch('meetingType');

  useEffect(() => {
    const fetchExpert = async () => {
      if (!params.expertId) return;

      setIsLoading(true);
      try {
        const data = await expertsApi.getById(params.expertId as string) as Expert;
        setExpert(data);
      } catch {
        setError('Failed to load expert profile');
      } finally {
        setIsLoading(false);
      }
    };

    fetchExpert();
  }, [params.expertId]);

  const getAvailableTimesForDate = (dateStr: string) => {
    if (!expert?.availability || !dateStr) return [];

    const date = new Date(dateStr);
    const dayOfWeek = date.getDay();
    const availableSlots = expert.availability.filter(a => a.dayOfWeek === dayOfWeek);

    const times: string[] = [];
    availableSlots.forEach(slot => {
      const [startHour, startMin] = slot.startTime.split(':').map(Number);
      const [endHour, endMin] = slot.endTime.split(':').map(Number);

      let currentHour = startHour;
      let currentMin = startMin;

      while (currentHour < endHour || (currentHour === endHour && currentMin < endMin)) {
        times.push(`${String(currentHour).padStart(2, '0')}:${String(currentMin).padStart(2, '0')}`);
        currentMin += 30;
        if (currentMin >= 60) {
          currentMin = 0;
          currentHour++;
        }
      }
    });

    return times;
  };

  const availableTimes = watchDate ? getAvailableTimesForDate(watchDate) : [];

  const calculateTotal = () => {
    if (!expert?.hourlyRate) return 0;
    return (expert.hourlyRate / 60) * selectedDuration;
  };

  const onSubmit = async (data: BookingFormData) => {
    if (!expert) return;

    setIsBooking(true);
    setError(null);

    try {
      const scheduledAt = new Date(`${data.date}T${data.time}`).toISOString();

      await consultationsApi.create({
        expertId: expert.id,
        scheduledAt,
        durationMinutes: data.duration,
        meetingType: data.meetingType,
        notes: data.notes,
      });

      router.push('/consultations?booked=true');
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to book consultation');
    } finally {
      setIsBooking(false);
    }
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  if (!expert) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-16 text-center">
        <h1 className="text-2xl font-bold">Expert Not Found</h1>
        <Link href="/experts">
          <Button className="mt-4">Browse Experts</Button>
        </Link>
      </div>
    );
  }

  return (
    <ProtectedRoute allowedRoles={['Seeker']}>
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Back Link */}
        <Link
          href={`/experts/${expert.id}`}
          className="inline-flex items-center text-sm text-gray-600 hover:text-gray-900 mb-6"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to profile
        </Link>

        <h1 className="text-2xl font-bold text-gray-900 mb-8">Book a Consultation</h1>

        {error && <ErrorMessage message={error} className="mb-6" />}

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Booking Form */}
          <div className="lg:col-span-2">
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              {/* Date Selection */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center">
                    <Calendar className="h-5 w-5 mr-2" />
                    Select Date
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <Input
                    type="date"
                    {...register('date')}
                    error={errors.date?.message}
                    min={new Date().toISOString().split('T')[0]}
                  />
                  {watchDate && expert.availability && (
                    <p className="mt-2 text-sm text-gray-500">
                      {dayNames[new Date(watchDate).getDay()]} -
                      {expert.availability.find(a => a.dayOfWeek === new Date(watchDate).getDay())
                        ? ' Available'
                        : ' Not available on this day'}
                    </p>
                  )}
                </CardContent>
              </Card>

              {/* Time Selection */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center">
                    <Clock className="h-5 w-5 mr-2" />
                    Select Time
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {availableTimes.length > 0 ? (
                    <div className="grid grid-cols-4 gap-2">
                      {availableTimes.map((time) => (
                        <button
                          key={time}
                          type="button"
                          onClick={() => setValue('time', time)}
                          className={cn(
                            'p-2 text-sm rounded-lg border transition-colors',
                            watch('time') === time
                              ? 'border-blue-600 bg-blue-50 text-blue-700'
                              : 'border-gray-200 hover:border-gray-300'
                          )}
                        >
                          {time}
                        </button>
                      ))}
                    </div>
                  ) : (
                    <p className="text-gray-500 text-sm">
                      {watchDate ? 'No available times on this date' : 'Please select a date first'}
                    </p>
                  )}
                  {errors.time && (
                    <p className="mt-2 text-sm text-red-600">{errors.time.message}</p>
                  )}
                </CardContent>
              </Card>

              {/* Duration */}
              <Card>
                <CardHeader>
                  <CardTitle>Duration</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                    {durations.map((d) => (
                      <button
                        key={d.value}
                        type="button"
                        onClick={() => {
                          setSelectedDuration(d.value);
                          setValue('duration', d.value);
                        }}
                        className={cn(
                          'p-3 text-sm rounded-lg border transition-colors',
                          selectedDuration === d.value
                            ? 'border-blue-600 bg-blue-50 text-blue-700'
                            : 'border-gray-200 hover:border-gray-300'
                        )}
                      >
                        {d.label}
                      </button>
                    ))}
                  </div>
                </CardContent>
              </Card>

              {/* Meeting Type */}
              <Card>
                <CardHeader>
                  <CardTitle>Meeting Type</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    <button
                      type="button"
                      onClick={() => setValue('meetingType', 'Virtual')}
                      className={cn(
                        'p-4 rounded-lg border-2 transition-colors flex flex-col items-center',
                        watchMeetingType === 'Virtual'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-200 hover:border-gray-300'
                      )}
                    >
                      <Video className="h-8 w-8 mb-2 text-blue-600" />
                      <span className="font-medium">Virtual</span>
                      <span className="text-sm text-gray-500">Video call</span>
                    </button>
                    <button
                      type="button"
                      onClick={() => setValue('meetingType', 'InPerson')}
                      className={cn(
                        'p-4 rounded-lg border-2 transition-colors flex flex-col items-center',
                        watchMeetingType === 'InPerson'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-200 hover:border-gray-300'
                      )}
                    >
                      <MapPin className="h-8 w-8 mb-2 text-blue-600" />
                      <span className="font-medium">In Person</span>
                      <span className="text-sm text-gray-500">Meet in person</span>
                    </button>
                  </div>
                </CardContent>
              </Card>

              {/* Notes */}
              <Card>
                <CardHeader>
                  <CardTitle>Additional Notes (Optional)</CardTitle>
                </CardHeader>
                <CardContent>
                  <textarea
                    {...register('notes')}
                    rows={4}
                    className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Tell the expert what you'd like to discuss..."
                  />
                </CardContent>
              </Card>

              {/* Submit */}
              <Button type="submit" size="lg" className="w-full" isLoading={isBooking}>
                <CreditCard className="h-5 w-5 mr-2" />
                Proceed to Payment
              </Button>
            </form>
          </div>

          {/* Summary Sidebar */}
          <div>
            <Card className="sticky top-24">
              <CardHeader>
                <CardTitle>Booking Summary</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Expert Info */}
                <div className="flex items-center gap-3">
                  <Avatar
                    src={expert.profilePhotoUrl}
                    firstName={expert.firstName}
                    lastName={expert.lastName}
                    size="md"
                  />
                  <div>
                    <p className="font-medium">{expert.firstName} {expert.lastName}</p>
                    {expert.headline && (
                      <p className="text-sm text-gray-500">{expert.headline}</p>
                    )}
                  </div>
                </div>

                <hr />

                {/* Details */}
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Duration</span>
                    <span>{selectedDuration} minutes</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Rate</span>
                    <span>{formatCurrency(expert.hourlyRate || 0, expert.currency)}/hr</span>
                  </div>
                </div>

                <hr />

                {/* Total */}
                <div className="flex justify-between items-center">
                  <span className="font-medium">Total</span>
                  <span className="text-xl font-bold text-blue-600">
                    {formatCurrency(calculateTotal(), expert.currency)}
                  </span>
                </div>

                <p className="text-xs text-gray-500">
                  Payment will be held in escrow until the consultation is complete.
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
