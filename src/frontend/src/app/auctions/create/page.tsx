'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { ArrowLeft, Calendar, DollarSign, Users, Video, MapPin } from 'lucide-react';
import { auctionsApi } from '@/lib/api';
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  Button,
  Input,
  ErrorMessage,
} from '@/components/ui';
import { ProtectedRoute } from '@/components/auth';
import { cn } from '@/lib/utils';

const auctionSchema = z.object({
  title: z.string().min(5, 'Title must be at least 5 characters'),
  description: z.string().min(20, 'Description must be at least 20 characters'),
  meetingType: z.enum(['Virtual', 'InPerson', 'Hybrid']),
  guestLimit: z.number().min(1, 'At least 1 guest required').max(10, 'Maximum 10 guests'),
  startingBid: z.number().min(100, 'Minimum starting bid is $100'),
  buyNowPrice: z.number().optional(),
  startDate: z.string().min(1, 'Start date is required'),
  startTime: z.string().min(1, 'Start time is required'),
  endDate: z.string().min(1, 'End date is required'),
  endTime: z.string().min(1, 'End time is required'),
  charityName: z.string().min(2, 'Charity name is required'),
  charityUrl: z.string().url('Please enter a valid URL').optional().or(z.literal('')),
}).refine((data) => {
  if (data.buyNowPrice && data.buyNowPrice <= data.startingBid) {
    return false;
  }
  return true;
}, {
  message: 'Buy Now price must be higher than starting bid',
  path: ['buyNowPrice'],
});

type AuctionFormData = z.infer<typeof auctionSchema>;

export default function CreateAuctionPage() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<AuctionFormData>({
    resolver: zodResolver(auctionSchema),
    defaultValues: {
      meetingType: 'Virtual',
      guestLimit: 1,
      startingBid: 1000,
    },
  });

  const watchMeetingType = watch('meetingType');

  const onSubmit = async (data: AuctionFormData) => {
    setIsSubmitting(true);
    setError(null);

    try {
      const startTime = new Date(`${data.startDate}T${data.startTime}`).toISOString();
      const endTime = new Date(`${data.endDate}T${data.endTime}`).toISOString();

      await auctionsApi.create({
        title: data.title,
        description: data.description,
        meetingType: data.meetingType,
        guestLimit: data.guestLimit,
        startingBid: data.startingBid,
        buyNowPrice: data.buyNowPrice || null,
        startTime,
        endTime,
        charityName: data.charityName,
        charityUrl: data.charityUrl || null,
      });

      router.push('/auctions?created=true');
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to create auction');
    } finally {
      setIsSubmitting(false);
    }
  };

  // Get tomorrow's date for minimum start date
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  const minDate = tomorrow.toISOString().split('T')[0];

  return (
    <ProtectedRoute allowedRoles={['Expert']}>
      <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Back Link */}
        <Link
          href="/auctions"
          className="inline-flex items-center text-sm text-gray-600 hover:text-gray-900 mb-6"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to auctions
        </Link>

        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Create Charity Auction</h1>
          <p className="mt-1 text-gray-600">
            Host an exclusive experience and donate the proceeds to charity
          </p>
        </div>

        {error && <ErrorMessage message={error} className="mb-6" />}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Basic Info */}
          <Card>
            <CardHeader>
              <CardTitle>Auction Details</CardTitle>
              <CardDescription>Describe what you are offering</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <Input
                label="Auction Title"
                placeholder="e.g., Lunch with [Your Name] - Strategy Session"
                {...register('title')}
                error={errors.title?.message}
              />
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <textarea
                  {...register('description')}
                  rows={4}
                  className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Describe the experience in detail. What will the winner receive? What topics can be discussed?"
                />
                {errors.description && (
                  <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Meeting Type */}
          <Card>
            <CardHeader>
              <CardTitle>Meeting Format</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-3 gap-4">
                {[
                  { value: 'Virtual', icon: Video, label: 'Virtual', desc: 'Video call' },
                  { value: 'InPerson', icon: MapPin, label: 'In Person', desc: 'Meet face-to-face' },
                  { value: 'Hybrid', icon: Users, label: 'Hybrid', desc: 'Winner chooses' },
                ].map((option) => (
                  <button
                    key={option.value}
                    type="button"
                    onClick={() => setValue('meetingType', option.value as 'Virtual' | 'InPerson' | 'Hybrid')}
                    className={cn(
                      'p-4 rounded-lg border-2 transition-colors text-center',
                      watchMeetingType === option.value
                        ? 'border-blue-600 bg-blue-50'
                        : 'border-gray-200 hover:border-gray-300'
                    )}
                  >
                    <option.icon className="h-6 w-6 mx-auto mb-2 text-blue-600" />
                    <p className="font-medium">{option.label}</p>
                    <p className="text-xs text-gray-500">{option.desc}</p>
                  </button>
                ))}
              </div>

              <Input
                label="Guest Limit"
                type="number"
                min={1}
                max={10}
                {...register('guestLimit', { valueAsNumber: true })}
                error={errors.guestLimit?.message}
                helperText="Number of additional guests the winner can bring"
              />
            </CardContent>
          </Card>

          {/* Pricing */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center">
                <DollarSign className="h-5 w-5 mr-2" />
                Pricing
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <Input
                label="Starting Bid ($)"
                type="number"
                min={100}
                step={100}
                {...register('startingBid', { valueAsNumber: true })}
                error={errors.startingBid?.message}
              />
              <Input
                label="Buy Now Price ($) - Optional"
                type="number"
                min={100}
                step={100}
                {...register('buyNowPrice', { valueAsNumber: true })}
                error={errors.buyNowPrice?.message}
                helperText="Allow instant purchase at this price"
              />
            </CardContent>
          </Card>

          {/* Schedule */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center">
                <Calendar className="h-5 w-5 mr-2" />
                Auction Schedule
              </CardTitle>
              <CardDescription>When should bidding start and end?</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <Input
                  label="Start Date"
                  type="date"
                  min={minDate}
                  {...register('startDate')}
                  error={errors.startDate?.message}
                />
                <Input
                  label="Start Time"
                  type="time"
                  {...register('startTime')}
                  error={errors.startTime?.message}
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <Input
                  label="End Date"
                  type="date"
                  min={minDate}
                  {...register('endDate')}
                  error={errors.endDate?.message}
                />
                <Input
                  label="End Time"
                  type="time"
                  {...register('endTime')}
                  error={errors.endTime?.message}
                />
              </div>
            </CardContent>
          </Card>

          {/* Charity */}
          <Card>
            <CardHeader>
              <CardTitle>Benefiting Charity</CardTitle>
              <CardDescription>100% of proceeds go to the charity you choose</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <Input
                label="Charity Name"
                placeholder="e.g., American Red Cross"
                {...register('charityName')}
                error={errors.charityName?.message}
              />
              <Input
                label="Charity Website (Optional)"
                type="url"
                placeholder="https://..."
                {...register('charityUrl')}
                error={errors.charityUrl?.message}
              />
            </CardContent>
          </Card>

          {/* Submit */}
          <div className="flex gap-4">
            <Button
              type="button"
              variant="outline"
              className="flex-1"
              onClick={() => router.back()}
            >
              Cancel
            </Button>
            <Button type="submit" className="flex-1" isLoading={isSubmitting}>
              Create Auction
            </Button>
          </div>
        </form>
      </div>
    </ProtectedRoute>
  );
}
