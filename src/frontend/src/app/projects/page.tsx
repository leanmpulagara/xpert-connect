'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Heart, Clock, Building2, Calendar, Plus, Search } from 'lucide-react';
import { projectsApi } from '@/lib/api';
import { formatRelativeTime } from '@/lib/utils';
import { useAuthStore } from '@/stores/auth';
import {
  Card,
  CardContent,
  Button,
  Badge,
  Input,
  LoadingPage,
  ErrorMessage,
} from '@/components/ui';
import { cn } from '@/lib/utils';
import type { Project, PagedResult } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info' }> = {
  Draft: { label: 'Draft', variant: 'default' },
  Open: { label: 'Open', variant: 'success' },
  Matching: { label: 'Matching', variant: 'info' },
  InProgress: { label: 'In Progress', variant: 'info' },
  Completed: { label: 'Completed', variant: 'success' },
  Cancelled: { label: 'Cancelled', variant: 'danger' },
};

const categories = [
  'All Categories',
  'Business Strategy',
  'Technology',
  'Marketing',
  'Finance',
  'Legal',
  'Healthcare',
  'Education',
];

export default function ProjectsPage() {
  const { user } = useAuthStore();
  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState('All Categories');
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    const fetchProjects = async () => {
      setIsLoading(true);
      try {
        const params: { pageSize: number; category?: string; status?: string } = { pageSize: 50 };
        if (selectedCategory !== 'All Categories') {
          params.category = selectedCategory;
        }
        const data = await projectsApi.getAll(params) as PagedResult<Project>;
        setProjects(data.items);
      } catch (err) {
        const apiError = err as { message?: string };
        setError(apiError.message || 'Failed to load projects');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProjects();
  }, [selectedCategory]);

  const filteredProjects = projects.filter(project =>
    project.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    project.description.toLowerCase().includes(searchQuery.toLowerCase()) ||
    project.organizationName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  if (isLoading) {
    return <LoadingPage />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Pro Bono Projects</h1>
          <p className="mt-1 text-gray-600">
            Volunteer your expertise to help non-profit organizations
          </p>
        </div>
        {user?.userType === 'NonProfit' && (
          <Link href="/projects/create">
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Post Project
            </Button>
          </Link>
        )}
      </div>

      {/* Search & Filters */}
      <div className="mb-6 space-y-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
          <Input
            type="text"
            placeholder="Search projects..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-10"
          />
        </div>

        <div className="flex flex-wrap gap-2">
          {categories.map((category) => (
            <button
              key={category}
              onClick={() => setSelectedCategory(category)}
              className={cn(
                'px-4 py-2 rounded-full text-sm font-medium transition-colors',
                selectedCategory === category
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              )}
            >
              {category}
            </button>
          ))}
        </div>
      </div>

      {error && <ErrorMessage message={error} className="mb-6" />}

      {/* Projects Grid */}
      {filteredProjects.length > 0 ? (
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredProjects.map((project) => {
            const status = statusConfig[project.status] || { label: project.status, variant: 'default' as const };

            return (
              <Link key={project.id} href={`/projects/${project.id}`}>
                <Card className="h-full hover:shadow-lg transition-shadow cursor-pointer">
                  <CardContent className="p-6">
                    {/* Organization */}
                    <div className="flex items-center gap-3 mb-4">
                      <div className="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                        {project.organizationLogoUrl ? (
                          <img
                            src={project.organizationLogoUrl}
                            alt={project.organizationName}
                            className="w-10 h-10 rounded-lg object-cover"
                          />
                        ) : (
                          <Building2 className="h-5 w-5 text-blue-600" />
                        )}
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-900">
                          {project.organizationName}
                        </p>
                        <Badge variant={status.variant}>{status.label}</Badge>
                      </div>
                    </div>

                    {/* Title */}
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                      {project.title}
                    </h3>

                    {/* Description */}
                    <p className="text-sm text-gray-600 mb-4 line-clamp-2">
                      {project.description}
                    </p>

                    {/* Skills */}
                    {project.requiredSkills.length > 0 && (
                      <div className="flex flex-wrap gap-1 mb-4">
                        {project.requiredSkills.slice(0, 3).map((skill) => (
                          <span
                            key={skill}
                            className="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded-full"
                          >
                            {skill}
                          </span>
                        ))}
                        {project.requiredSkills.length > 3 && (
                          <span className="px-2 py-1 text-gray-500 text-xs">
                            +{project.requiredSkills.length - 3} more
                          </span>
                        )}
                      </div>
                    )}

                    {/* Stats */}
                    <div className="flex items-center justify-between text-sm text-gray-500 pt-4 border-t">
                      <span className="flex items-center">
                        <Clock className="h-4 w-4 mr-1" />
                        ~{project.estimatedHours}h
                      </span>
                      {project.deadline && (
                        <span className="flex items-center">
                          <Calendar className="h-4 w-4 mr-1" />
                          Due {formatRelativeTime(project.deadline)}
                        </span>
                      )}
                    </div>
                  </CardContent>
                </Card>
              </Link>
            );
          })}
        </div>
      ) : (
        <Card>
          <CardContent className="p-12 text-center">
            <Heart className="h-12 w-12 mx-auto text-gray-300 mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No projects found</h3>
            <p className="text-gray-500">
              {searchQuery || selectedCategory !== 'All Categories'
                ? 'Try adjusting your search or filters'
                : 'Check back later for new volunteer opportunities'}
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
