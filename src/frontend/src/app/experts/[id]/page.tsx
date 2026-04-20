'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { Star, BadgeCheck, Calendar, Clock, MapPin, ExternalLink, Award } from 'lucide-react';
import { expertsApi } from '@/lib/api';
import { formatCurrency } from '@/lib/utils';
import { Card, CardHeader, CardTitle, CardContent, Avatar, Badge, Button, LoadingPage } from '@/components/ui';
import type { Expert } from '@/types';

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function ExpertProfilePage() {
  const params = useParams();
  const [expert, setExpert] = useState<Expert | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchExpert = async () => {
      if (!params.id) return;

      setIsLoading(true);
      setError(null);
      try {
        const data = await expertsApi.getById(params.id as string) as Expert;
        setExpert(data);
      } catch (err) {
        setError('Failed to load expert profile. Please try again.');
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchExpert();
  }, [params.id]);

  if (isLoading) {
    return <LoadingPage />;
  }

  if (error || !expert) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">Expert Not Found</h1>
        <p className="mt-2 text-gray-600">{error || 'The expert profile you are looking for does not exist.'}</p>
        <Link href="/experts">
          <Button className="mt-4">Browse Experts</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="grid lg:grid-cols-3 gap-8">
        {/* Left Column - Profile Info */}
        <div className="lg:col-span-2 space-y-6">
          {/* Profile Header */}
          <Card>
            <CardContent className="p-8">
              <div className="flex flex-col sm:flex-row items-start gap-6">
                <Avatar
                  src={expert.profilePhotoUrl}
                  firstName={expert.firstName}
                  lastName={expert.lastName}
                  size="xl"
                  className="h-24 w-24"
                />
                <div className="flex-1">
                  <div className="flex items-center gap-2">
                    <h1 className="text-2xl font-bold text-gray-900">
                      {expert.firstName} {expert.lastName}
                    </h1>
                    {expert.isVerified && (
                      <BadgeCheck className="h-6 w-6 text-blue-600" />
                    )}
                  </div>
                  {expert.headline && (
                    <p className="mt-1 text-lg text-gray-600">{expert.headline}</p>
                  )}
                  <div className="mt-3 flex flex-wrap items-center gap-4">
                    {expert.category && (
                      <Badge variant="info">{expert.category}</Badge>
                    )}
                    {expert.averageRating && (
                      <div className="flex items-center text-yellow-500">
                        <Star className="h-5 w-5 fill-current" />
                        <span className="ml-1 font-medium text-gray-900">
                          {expert.averageRating.toFixed(1)}
                        </span>
                        <span className="text-gray-500 ml-1">
                          ({expert.totalReviews} reviews)
                        </span>
                      </div>
                    )}
                    {expert.linkedInUrl && (
                      <a
                        href={expert.linkedInUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="flex items-center text-blue-600 hover:text-blue-700"
                      >
                        <ExternalLink className="h-5 w-5" />
                        <span className="ml-1 text-sm">LinkedIn</span>
                      </a>
                    )}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Bio */}
          {expert.bio && (
            <Card>
              <CardHeader>
                <CardTitle>About</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-700 whitespace-pre-line">{expert.bio}</p>
              </CardContent>
            </Card>
          )}

          {/* Credentials */}
          {expert.credentials && expert.credentials.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Credentials</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {expert.credentials.map((credential) => (
                    <div
                      key={credential.id}
                      className="flex items-start gap-4 p-4 rounded-lg bg-gray-50"
                    >
                      <Award className="h-6 w-6 text-blue-600 flex-shrink-0 mt-0.5" />
                      <div>
                        <div className="flex items-center gap-2">
                          <h4 className="font-medium text-gray-900">
                            {credential.title}
                          </h4>
                          {credential.isVerified && (
                            <Badge variant="success">Verified</Badge>
                          )}
                        </div>
                        {credential.issuer && (
                          <p className="text-sm text-gray-600">{credential.issuer}</p>
                        )}
                        {credential.issuedDate && (
                          <p className="text-sm text-gray-500">
                            Issued: {new Date(credential.issuedDate).toLocaleDateString()}
                          </p>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Right Column - Booking */}
        <div className="space-y-6">
          {/* Booking Card */}
          <Card className="sticky top-24">
            <CardHeader>
              <CardTitle>Book a Consultation</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Rate */}
              <div className="text-center p-4 bg-gray-50 rounded-lg">
                <p className="text-3xl font-bold text-gray-900">
                  {expert.hourlyRate
                    ? formatCurrency(expert.hourlyRate, expert.currency)
                    : 'Contact for pricing'}
                </p>
                {expert.hourlyRate && (
                  <p className="text-gray-500">per hour</p>
                )}
              </div>

              {/* Availability */}
              {expert.availability && expert.availability.length > 0 && (
                <div>
                  <h4 className="font-medium text-gray-900 mb-3 flex items-center">
                    <Clock className="h-4 w-4 mr-2" />
                    Availability
                  </h4>
                  <div className="space-y-2">
                    {expert.availability.map((slot) => (
                      <div
                        key={slot.id}
                        className="flex justify-between text-sm"
                      >
                        <span className="text-gray-600">
                          {dayNames[slot.dayOfWeek]}
                        </span>
                        <span className="text-gray-900">
                          {slot.startTime} - {slot.endTime}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Book Button */}
              <Link href={`/book/${expert.id}`} className="block">
                <Button className="w-full" size="lg">
                  <Calendar className="h-5 w-5 mr-2" />
                  Schedule Consultation
                </Button>
              </Link>

              <p className="text-xs text-center text-gray-500">
                Free cancellation up to 24 hours before the session
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
