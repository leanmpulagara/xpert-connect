'use client';

import Link from 'next/link';
import { Calendar, DollarSign, Users, Clock, ArrowRight } from 'lucide-react';
import { useAuthStore } from '@/stores/auth';
import { Card, CardHeader, CardTitle, CardContent, Button } from '@/components/ui';

const stats = [
  { name: 'Upcoming Consultations', value: '3', icon: Calendar, href: '/consultations' },
  { name: 'Active Bids', value: '2', icon: DollarSign, href: '/bids' },
  { name: 'Pro Bono Hours', value: '12', icon: Clock, href: '/projects' },
  { name: 'Network', value: '24', icon: Users, href: '/network' },
];

export default function DashboardPage() {
  const { user } = useAuthStore();

  if (!user) {
    return null;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Welcome Header */}
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">
          Welcome back, {user.firstName}!
        </h1>
        <p className="mt-1 text-gray-600">
          Here&apos;s what&apos;s happening with your account today.
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        {stats.map((stat) => (
          <Card key={stat.name} className="hover:shadow-md transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <stat.icon className="h-8 w-8 text-blue-600" />
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-gray-500">{stat.name}</p>
                  <p className="text-2xl font-semibold text-gray-900">{stat.value}</p>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="grid md:grid-cols-2 gap-6">
        {/* For Seekers */}
        {user.userType === 'Seeker' && (
          <>
            <Card>
              <CardHeader>
                <CardTitle>Find an Expert</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Browse our network of industry leaders and book a consultation.
                </p>
                <Link href="/experts">
                  <Button>
                    Browse Experts
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Active Auctions</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Participate in exclusive charity auctions for unique experiences.
                </p>
                <Link href="/auctions">
                  <Button variant="outline">
                    View Auctions
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
          </>
        )}

        {/* For Experts */}
        {user.userType === 'Expert' && (
          <>
            <Card>
              <CardHeader>
                <CardTitle>Your Consultations</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Manage your upcoming consultations and availability.
                </p>
                <Link href="/consultations">
                  <Button>
                    View Consultations
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Create an Auction</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Host a charity auction and make a difference.
                </p>
                <Link href="/auctions/create">
                  <Button variant="outline">
                    Create Auction
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
          </>
        )}

        {/* For Non-Profits */}
        {user.userType === 'NonProfit' && (
          <>
            <Card>
              <CardHeader>
                <CardTitle>Your Projects</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Manage your pro-bono projects and expert applications.
                </p>
                <Link href="/projects">
                  <Button>
                    View Projects
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Create a Project</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-4">
                  Post a new pro-bono project and find expert volunteers.
                </p>
                <Link href="/projects/create">
                  <Button variant="outline">
                    Create Project
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
          </>
        )}
      </div>
    </div>
  );
}
