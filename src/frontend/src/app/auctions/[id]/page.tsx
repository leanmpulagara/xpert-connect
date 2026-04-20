'use client';

import { useEffect, useState, useCallback } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  Clock,
  Users,
  Gavel,
  ArrowLeft,
  Trophy,
  AlertCircle,
  CheckCircle,
  DollarSign,
} from 'lucide-react';
import { auctionsApi } from '@/lib/api';
import { formatCurrency, formatDateTime } from '@/lib/utils';
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
import { cn } from '@/lib/utils';
import type { Auction } from '@/types';

interface BidHistory {
  id: string;
  bidderInitials: string;
  amount: number;
  placedAt: string;
  isWinning: boolean;
}

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info' }> = {
  Draft: { label: 'Draft', variant: 'default' },
  Scheduled: { label: 'Scheduled', variant: 'info' },
  Open: { label: 'Live', variant: 'success' },
  Closed: { label: 'Closed', variant: 'default' },
  WinnerSelected: { label: 'Won', variant: 'success' },
  Cancelled: { label: 'Cancelled', variant: 'danger' },
};

export default function AuctionDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { user } = useAuthStore();
  const [auction, setAuction] = useState<Auction | null>(null);
  const [bidHistory, setBidHistory] = useState<BidHistory[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isBidding, setIsBidding] = useState(false);
  const [bidSuccess, setBidSuccess] = useState(false);
  const [timeRemaining, setTimeRemaining] = useState<string>('');

  const bidSchema = z.object({
    amount: z.number().min(
      (auction?.currentHighBid || auction?.startingBid || 0) + 1,
      `Bid must be higher than ${formatCurrency((auction?.currentHighBid || auction?.startingBid || 0) + 1)}`
    ),
    isProxyBid: z.boolean().optional(),
    maxProxyAmount: z.number().optional(),
  });

  type BidFormData = z.infer<typeof bidSchema>;

  const {
    register,
    handleSubmit,
    watch,
    reset,
    formState: { errors },
  } = useForm<BidFormData>({
    resolver: zodResolver(bidSchema),
    defaultValues: {
      amount: (auction?.currentHighBid || auction?.startingBid || 0) + 100,
      isProxyBid: false,
    },
  });

  const watchIsProxyBid = watch('isProxyBid');

  const updateTimeRemaining = useCallback(() => {
    if (!auction?.endTime) return;

    const end = new Date(auction.endTime).getTime();
    const now = Date.now();
    const diff = end - now;

    if (diff <= 0) {
      setTimeRemaining('Ended');
      return;
    }

    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);

    if (days > 0) {
      setTimeRemaining(`${days}d ${hours}h ${minutes}m`);
    } else if (hours > 0) {
      setTimeRemaining(`${hours}h ${minutes}m ${seconds}s`);
    } else {
      setTimeRemaining(`${minutes}m ${seconds}s`);
    }
  }, [auction?.endTime]);

  useEffect(() => {
    const fetchAuction = async () => {
      if (!params.id) return;

      setIsLoading(true);
      try {
        const [auctionData, bidsData] = await Promise.all([
          auctionsApi.getById(params.id as string) as Promise<Auction>,
          auctionsApi.getBidHistory(params.id as string) as Promise<BidHistory[]>,
        ]);
        setAuction(auctionData);
        setBidHistory(bidsData);
      } catch {
        setError('Failed to load auction details');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAuction();
  }, [params.id]);

  useEffect(() => {
    if (auction?.status === 'Open') {
      updateTimeRemaining();
      const interval = setInterval(updateTimeRemaining, 1000);
      return () => clearInterval(interval);
    }
  }, [auction?.status, updateTimeRemaining]);

  const onSubmitBid = async (data: BidFormData) => {
    if (!auction) return;

    setIsBidding(true);
    setError(null);
    setBidSuccess(false);

    try {
      await auctionsApi.placeBid(auction.id, {
        amount: data.amount,
        isProxyBid: data.isProxyBid,
        maxProxyAmount: data.maxProxyAmount,
      });

      // Refresh auction data
      const [auctionData, bidsData] = await Promise.all([
        auctionsApi.getById(auction.id) as Promise<Auction>,
        auctionsApi.getBidHistory(auction.id) as Promise<BidHistory[]>,
      ]);
      setAuction(auctionData);
      setBidHistory(bidsData);
      setBidSuccess(true);
      reset({ amount: auctionData.currentHighBid! + 100 });
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to place bid');
    } finally {
      setIsBidding(false);
    }
  };

  const handleBuyNow = async () => {
    if (!auction?.buyNowPrice || !confirm(`Buy now for ${formatCurrency(auction.buyNowPrice)}?`)) return;

    setIsBidding(true);
    try {
      await auctionsApi.placeBid(auction.id, {
        amount: auction.buyNowPrice,
      });
      router.push('/consultations?booked=true');
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to complete purchase');
    } finally {
      setIsBidding(false);
    }
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  if (!auction) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-16 text-center">
        <h1 className="text-2xl font-bold">Auction Not Found</h1>
        <Link href="/auctions">
          <Button className="mt-4">Browse Auctions</Button>
        </Link>
      </div>
    );
  }

  const status = statusConfig[auction.status] || { label: auction.status, variant: 'default' as const };
  const isLive = auction.status === 'Open';
  const canBid = isLive && user?.userType === 'Seeker';
  const minimumBid = (auction.currentHighBid || auction.startingBid) + 1;

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Back Link */}
      <Link
        href="/auctions"
        className="inline-flex items-center text-sm text-gray-600 hover:text-gray-900 mb-6"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Back to auctions
      </Link>

      {error && <ErrorMessage message={error} className="mb-6" />}

      {bidSuccess && (
        <div className="mb-6 p-4 rounded-lg bg-green-50 border border-green-200 flex items-center gap-3">
          <CheckCircle className="h-5 w-5 text-green-600" />
          <span className="text-green-700">Bid placed successfully!</span>
        </div>
      )}

      <div className="grid lg:grid-cols-3 gap-8">
        {/* Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Header Card */}
          <Card>
            <CardContent className="p-6">
              <div className="flex items-start gap-4">
                <Avatar
                  src={auction.expertPhotoUrl}
                  firstName={auction.expertName.split(' ')[0]}
                  lastName={auction.expertName.split(' ')[1]}
                  size="xl"
                />
                <div className="flex-1">
                  <Badge variant={status.variant} className="mb-2">
                    {status.label}
                  </Badge>
                  <h1 className="text-2xl font-bold text-gray-900">{auction.title}</h1>
                  <p className="text-gray-600 mt-1">Hosted by {auction.expertName}</p>
                </div>
              </div>

              {auction.description && (
                <p className="mt-4 text-gray-700">{auction.description}</p>
              )}

              {/* Time Remaining Banner */}
              {isLive && (
                <div className="mt-6 p-4 bg-red-50 rounded-lg flex items-center justify-center gap-3">
                  <Clock className="h-6 w-6 text-red-600 animate-pulse" />
                  <div className="text-center">
                    <p className="text-sm text-red-600">Time Remaining</p>
                    <p className="text-2xl font-bold text-red-700">{timeRemaining}</p>
                  </div>
                </div>
              )}

              {/* Winner Banner */}
              {auction.status === 'WinnerSelected' && auction.winnerName && (
                <div className="mt-6 p-4 bg-yellow-50 rounded-lg flex items-center gap-3">
                  <Trophy className="h-6 w-6 text-yellow-600" />
                  <div>
                    <p className="font-medium text-yellow-800">
                      Won by {auction.winnerName}
                    </p>
                    <p className="text-sm text-yellow-600">
                      Winning bid: {formatCurrency(auction.winningAmount || 0)}
                    </p>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Auction Details */}
          <Card>
            <CardHeader>
              <CardTitle>Auction Details</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-500">Meeting Type</p>
                  <p className="font-medium">{auction.meetingType}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Guest Limit</p>
                  <p className="font-medium">{auction.guestLimit} guests</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Start Time</p>
                  <p className="font-medium">{formatDateTime(auction.startTime)}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">End Time</p>
                  <p className="font-medium">{formatDateTime(auction.endTime)}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Bid History */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center">
                <Gavel className="h-5 w-5 mr-2" />
                Bid History ({bidHistory.length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              {bidHistory.length > 0 ? (
                <div className="space-y-3">
                  {bidHistory.map((bid, index) => (
                    <div
                      key={bid.id}
                      className={cn(
                        'flex items-center justify-between p-3 rounded-lg',
                        index === 0 ? 'bg-yellow-50' : 'bg-gray-50'
                      )}
                    >
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center font-medium">
                          {bid.bidderInitials}
                        </div>
                        <div>
                          <p className="font-medium">
                            {formatCurrency(bid.amount)}
                            {index === 0 && (
                              <Badge variant="success" className="ml-2">Leading</Badge>
                            )}
                          </p>
                          <p className="text-sm text-gray-500">
                            {formatDateTime(bid.placedAt)}
                          </p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-gray-500 text-center py-4">No bids yet. Be the first!</p>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Bidding Sidebar */}
        <div className="space-y-6">
          {/* Current Bid Card */}
          <Card>
            <CardContent className="p-6">
              <div className="text-center mb-6">
                <p className="text-sm text-gray-500">Current High Bid</p>
                <p className="text-4xl font-bold text-gray-900">
                  {formatCurrency(auction.currentHighBid || auction.startingBid)}
                </p>
                <p className="text-sm text-gray-500 mt-1">
                  <Users className="h-4 w-4 inline mr-1" />
                  {auction.bidCount} bids
                </p>
              </div>

              {/* Buy Now Button */}
              {auction.buyNowPrice && isLive && (
                <div className="mb-6 p-4 bg-green-50 rounded-lg">
                  <p className="text-sm text-green-700 mb-2">Buy Now Price</p>
                  <p className="text-2xl font-bold text-green-600 mb-3">
                    {formatCurrency(auction.buyNowPrice)}
                  </p>
                  <Button
                    className="w-full bg-green-600 hover:bg-green-700"
                    onClick={handleBuyNow}
                    disabled={!canBid || isBidding}
                  >
                    <DollarSign className="h-4 w-4 mr-2" />
                    Buy Now
                  </Button>
                </div>
              )}

              {/* Bid Form */}
              {canBid ? (
                <form onSubmit={handleSubmit(onSubmitBid)} className="space-y-4">
                  <Input
                    label="Your Bid"
                    type="number"
                    step="1"
                    min={minimumBid}
                    {...register('amount', { valueAsNumber: true })}
                    error={errors.amount?.message}
                    helperText={`Minimum bid: ${formatCurrency(minimumBid)}`}
                  />

                  <label className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      {...register('isProxyBid')}
                      className="h-4 w-4 rounded border-gray-300 text-blue-600"
                    />
                    <span className="text-sm text-gray-600">Enable proxy bidding</span>
                  </label>

                  {watchIsProxyBid && (
                    <Input
                      label="Maximum Proxy Amount"
                      type="number"
                      step="1"
                      {...register('maxProxyAmount', { valueAsNumber: true })}
                      helperText="System will bid up to this amount automatically"
                    />
                  )}

                  <Button type="submit" className="w-full" size="lg" isLoading={isBidding}>
                    <Gavel className="h-5 w-5 mr-2" />
                    Place Bid
                  </Button>
                </form>
              ) : (
                <div className="p-4 bg-gray-50 rounded-lg text-center">
                  {!user ? (
                    <>
                      <AlertCircle className="h-8 w-8 mx-auto text-gray-400 mb-2" />
                      <p className="text-gray-600 mb-3">Sign in to place bids</p>
                      <Link href="/login">
                        <Button>Sign In</Button>
                      </Link>
                    </>
                  ) : user.userType !== 'Seeker' ? (
                    <>
                      <AlertCircle className="h-8 w-8 mx-auto text-gray-400 mb-2" />
                      <p className="text-gray-600">Only seekers can place bids</p>
                    </>
                  ) : (
                    <>
                      <AlertCircle className="h-8 w-8 mx-auto text-gray-400 mb-2" />
                      <p className="text-gray-600">
                        {auction.status === 'Scheduled' ? 'Auction has not started yet' : 'Auction has ended'}
                      </p>
                    </>
                  )}
                </div>
              )}
            </CardContent>
          </Card>

          {/* Info Card */}
          <Card>
            <CardContent className="p-4">
              <div className="space-y-2 text-sm">
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  100% goes to charity
                </p>
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  Secure payment via escrow
                </p>
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  Verified expert identity
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
