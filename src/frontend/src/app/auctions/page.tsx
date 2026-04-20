'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Clock, Users, DollarSign, Gavel, Plus } from 'lucide-react';
import { auctionsApi } from '@/lib/api';
import { formatCurrency, formatRelativeTime } from '@/lib/utils';
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
import { cn } from '@/lib/utils';
import type { Auction, PagedResult } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info' }> = {
  Draft: { label: 'Draft', variant: 'default' },
  Scheduled: { label: 'Scheduled', variant: 'info' },
  Open: { label: 'Live', variant: 'success' },
  Closed: { label: 'Closed', variant: 'default' },
  WinnerSelected: { label: 'Won', variant: 'success' },
  Cancelled: { label: 'Cancelled', variant: 'danger' },
};

export default function AuctionsPage() {
  const { user } = useAuthStore();
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<'all' | 'live' | 'upcoming' | 'ended'>('all');

  useEffect(() => {
    const fetchAuctions = async () => {
      setIsLoading(true);
      try {
        const statusParam = filter === 'live' ? 'Open' : filter === 'upcoming' ? 'Scheduled' : filter === 'ended' ? 'Closed' : undefined;
        const data = await auctionsApi.getAll({ pageSize: 50, status: statusParam }) as PagedResult<Auction>;
        setAuctions(data.items);
      } catch (err) {
        const apiError = err as { message?: string };
        setError(apiError.message || 'Failed to load auctions');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAuctions();
  }, [filter]);

  const getTimeRemaining = (endTime: string) => {
    const end = new Date(endTime).getTime();
    const now = Date.now();
    const diff = end - now;

    if (diff <= 0) return 'Ended';

    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

    if (days > 0) return `${days}d ${hours}h left`;
    if (hours > 0) return `${hours}h ${minutes}m left`;
    return `${minutes}m left`;
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Charity Auctions</h1>
          <p className="mt-1 text-gray-600">
            Bid on exclusive experiences with industry leaders
          </p>
        </div>
        {user?.userType === 'Expert' && (
          <Link href="/auctions/create">
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Create Auction
            </Button>
          </Link>
        )}
      </div>

      {/* Filters */}
      <div className="flex gap-2 mb-6">
        {(['all', 'live', 'upcoming', 'ended'] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={cn(
              'px-4 py-2 rounded-full text-sm font-medium transition-colors',
              filter === f
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            )}
          >
            {f === 'all' ? 'All Auctions' : f.charAt(0).toUpperCase() + f.slice(1)}
          </button>
        ))}
      </div>

      {error && <ErrorMessage message={error} className="mb-6" />}

      {/* Auctions Grid */}
      {auctions.length > 0 ? (
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {auctions.map((auction) => {
            const status = statusConfig[auction.status] || { label: auction.status, variant: 'default' as const };
            const isLive = auction.status === 'Open';

            return (
              <Link key={auction.id} href={`/auctions/${auction.id}`}>
                <Card className="h-full hover:shadow-lg transition-shadow cursor-pointer">
                  <CardContent className="p-6">
                    {/* Expert */}
                    <div className="flex items-center gap-3 mb-4">
                      <Avatar
                        src={auction.expertPhotoUrl}
                        firstName={auction.expertName.split(' ')[0]}
                        lastName={auction.expertName.split(' ')[1]}
                        size="md"
                      />
                      <div>
                        <p className="font-medium text-gray-900">{auction.expertName}</p>
                        <Badge variant={status.variant}>{status.label}</Badge>
                      </div>
                    </div>

                    {/* Title */}
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                      {auction.title}
                    </h3>

                    {/* Description */}
                    {auction.description && (
                      <p className="text-sm text-gray-600 mb-4 line-clamp-2">
                        {auction.description}
                      </p>
                    )}

                    {/* Stats */}
                    <div className="space-y-2 mb-4">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-gray-500 flex items-center">
                          <Gavel className="h-4 w-4 mr-1" />
                          Current Bid
                        </span>
                        <span className="font-semibold text-gray-900">
                          {auction.currentHighBid
                            ? formatCurrency(auction.currentHighBid)
                            : formatCurrency(auction.startingBid)}
                        </span>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-gray-500 flex items-center">
                          <Users className="h-4 w-4 mr-1" />
                          Bids
                        </span>
                        <span className="font-medium">{auction.bidCount}</span>
                      </div>
                      {auction.buyNowPrice && (
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-gray-500 flex items-center">
                            <DollarSign className="h-4 w-4 mr-1" />
                            Buy Now
                          </span>
                          <span className="font-semibold text-green-600">
                            {formatCurrency(auction.buyNowPrice)}
                          </span>
                        </div>
                      )}
                    </div>

                    {/* Time Remaining */}
                    {isLive && (
                      <div className="flex items-center justify-center gap-2 p-3 bg-red-50 rounded-lg">
                        <Clock className="h-4 w-4 text-red-600 animate-pulse" />
                        <span className="text-sm font-medium text-red-600">
                          {getTimeRemaining(auction.endTime)}
                        </span>
                      </div>
                    )}

                    {auction.status === 'Scheduled' && (
                      <div className="flex items-center justify-center gap-2 p-3 bg-blue-50 rounded-lg">
                        <Clock className="h-4 w-4 text-blue-600" />
                        <span className="text-sm font-medium text-blue-600">
                          Starts {formatRelativeTime(auction.startTime)}
                        </span>
                      </div>
                    )}
                  </CardContent>
                </Card>
              </Link>
            );
          })}
        </div>
      ) : (
        <Card>
          <CardContent className="p-12 text-center">
            <Gavel className="h-12 w-12 mx-auto text-gray-300 mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No auctions found</h3>
            <p className="text-gray-500">
              {filter === 'all'
                ? 'Check back later for new charity auctions.'
                : `No ${filter} auctions at the moment.`}
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
