'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Search, Star, BadgeCheck } from 'lucide-react';
import { expertsApi } from '@/lib/api';
import { formatCurrency } from '@/lib/utils';
import { Input, Card, CardContent, Avatar, Badge, LoadingPage, Button } from '@/components/ui';
import type { ExpertListItem, PagedResult } from '@/types';

const categories = [
  { id: 0, label: 'All' },
  { id: 1, label: 'Subject Matter Expert' },
  { id: 2, label: 'C-Suite Executive' },
  { id: 3, label: 'Celebrity/High-Profile' },
];

export default function ExpertsPage() {
  const [experts, setExperts] = useState<ExpertListItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const fetchExperts = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const params: Record<string, string | number> = {
          page,
          pageSize: 12,
        };
        if (search) params.search = search;
        if (category !== 0) params.category = category;

        const result = await expertsApi.getAll(params) as PagedResult<ExpertListItem>;
        setExperts(result.items);
        setTotalPages(result.totalPages);
      } catch (err) {
        setError('Failed to load experts. Please try again.');
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchExperts();
  }, [page, search, category]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
  };

  if (isLoading && experts.length === 0) {
    return <LoadingPage />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Find an Expert</h1>
        <p className="mt-2 text-gray-600">
          Connect with industry leaders and accomplished professionals
        </p>
      </div>

      {/* Search and Filters */}
      <div className="mb-8 space-y-4">
        <form onSubmit={handleSearch} className="flex gap-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
            <Input
              type="text"
              placeholder="Search experts by name, expertise, or keywords..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-10"
            />
          </div>
          <Button type="submit">Search</Button>
        </form>

        {/* Category Filters */}
        <div className="flex flex-wrap gap-2">
          {categories.map((cat) => (
            <button
              key={cat.id}
              onClick={() => {
                setCategory(cat.id);
                setPage(1);
              }}
              className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
                category === cat.id
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              {cat.label}
            </button>
          ))}
        </div>
      </div>

      {/* Error State */}
      {error && (
        <div className="text-center py-12">
          <p className="text-red-600">{error}</p>
          <Button
            variant="outline"
            className="mt-4"
            onClick={() => window.location.reload()}
          >
            Try Again
          </Button>
        </div>
      )}

      {/* Experts Grid */}
      {!error && (
        <>
          {experts.length === 0 ? (
            <div className="text-center py-12">
              <p className="text-gray-600">No experts found matching your criteria.</p>
            </div>
          ) : (
            <div className="grid sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {experts.map((expert) => (
                <Link key={expert.id} href={`/experts/${expert.id}`}>
                  <Card className="h-full hover:shadow-md transition-shadow cursor-pointer">
                    <CardContent className="p-6">
                      <div className="flex flex-col items-center text-center">
                        <Avatar
                          src={expert.profilePhotoUrl}
                          firstName={expert.firstName}
                          lastName={expert.lastName}
                          size="xl"
                        />
                        <div className="mt-4">
                          <div className="flex items-center justify-center gap-1">
                            <h3 className="font-semibold text-gray-900">
                              {expert.firstName} {expert.lastName}
                            </h3>
                            {expert.verificationStatus === 2 && (
                              <BadgeCheck className="h-4 w-4 text-blue-600" />
                            )}
                          </div>
                          {expert.headline && (
                            <p className="mt-1 text-sm text-gray-600 line-clamp-2">
                              {expert.headline}
                            </p>
                          )}
                        </div>
                        {expert.categoryName && (
                          <Badge variant="info" className="mt-3">
                            {expert.categoryName}
                          </Badge>
                        )}
                        <div className="mt-4 flex items-center gap-4 text-sm">
                          {expert.averageRating && (
                            <div className="flex items-center text-yellow-500">
                              <Star className="h-4 w-4 fill-current" />
                              <span className="ml-1 text-gray-700">
                                {expert.averageRating.toFixed(1)}
                              </span>
                              <span className="text-gray-500 ml-1">
                                ({expert.totalReviews})
                              </span>
                            </div>
                          )}
                        </div>
                        {expert.hourlyRate && (
                          <p className="mt-3 text-lg font-semibold text-gray-900">
                            {formatCurrency(expert.hourlyRate, expert.currency)}
                            <span className="text-sm font-normal text-gray-500">/hr</span>
                          </p>
                        )}
                      </div>
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8 flex justify-center gap-2">
              <Button
                variant="outline"
                disabled={page === 1}
                onClick={() => setPage(page - 1)}
              >
                Previous
              </Button>
              <span className="flex items-center px-4 text-gray-600">
                Page {page} of {totalPages}
              </span>
              <Button
                variant="outline"
                disabled={page === totalPages}
                onClick={() => setPage(page + 1)}
              >
                Next
              </Button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
