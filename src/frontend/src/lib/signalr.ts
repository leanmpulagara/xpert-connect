// SignalR client configuration
// Note: The @microsoft/signalr package needs to be installed

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5200';

export interface AuctionState {
  auctionId: string;
  currentHighBid: number;
  bidCount: number;
  timeRemaining: number;
  status: string;
}

export interface BidNotification {
  auctionId: string;
  amount: number;
  bidderInitials: string;
  placedAt: string;
}

export interface OutbidNotification {
  auctionId: string;
  newAmount: number;
  yourBid: number;
}

// Helper to get token from localStorage
const getToken = (): string | null => {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem('token');
};

// Build connection URL with auth token
export const buildHubUrl = (hubPath: string): string => {
  const token = getToken();
  const baseUrl = `${SIGNALR_HUB_URL}${hubPath}`;
  return token ? `${baseUrl}?access_token=${token}` : baseUrl;
};

// Hub paths
export const AUCTION_HUB = '/hubs/auction';
export const NOTIFICATION_HUB = '/hubs/notifications';

// Usage example (uncomment after installing @microsoft/signalr):
/*
import * as signalR from '@microsoft/signalr';

export const createAuctionConnection = () => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(buildHubUrl(AUCTION_HUB))
    .withAutomaticReconnect()
    .build();

  return connection;
};

export const createNotificationConnection = () => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(buildHubUrl(NOTIFICATION_HUB))
    .withAutomaticReconnect()
    .build();

  return connection;
};
*/

// Placeholder hook for auction updates (to be implemented with actual SignalR)
export const useAuctionRealtime = (
  auctionId: string,
  onBidUpdate?: (bid: BidNotification) => void,
  onOutbid?: (notification: OutbidNotification) => void
) => {
  // This is a placeholder - actual implementation would use SignalR
  // For now, we'll just return the auctionId to avoid unused variable warnings
  console.log('Auction realtime hook initialized for:', auctionId);

  return {
    isConnected: false,
    joinAuction: () => {
      console.log('Would join auction:', auctionId);
    },
    leaveAuction: () => {
      console.log('Would leave auction:', auctionId);
    },
  };
};
