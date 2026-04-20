'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import {
  Clock,
  Building2,
  Calendar,
  ArrowLeft,
  Heart,
  CheckCircle,
  User,
  FileText,
} from 'lucide-react';
import { projectsApi, timeEntriesApi } from '@/lib/api';
import { formatDateTime, formatRelativeTime } from '@/lib/utils';
import { useAuthStore } from '@/stores/auth';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Button,
  Badge,
  Avatar,
  LoadingPage,
  ErrorMessage,
  Modal,
  ModalFooter,
  Input,
} from '@/components/ui';
import type { Project, TimeEntry } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info'; description: string }> = {
  Draft: { label: 'Draft', variant: 'default', description: 'Project is being prepared' },
  Open: { label: 'Open', variant: 'success', description: 'Accepting expert applications' },
  Matching: { label: 'Matching', variant: 'info', description: 'Reviewing applications' },
  InProgress: { label: 'In Progress', variant: 'info', description: 'Expert is working on the project' },
  Completed: { label: 'Completed', variant: 'success', description: 'Project has been completed' },
  Cancelled: { label: 'Cancelled', variant: 'danger', description: 'Project was cancelled' },
};

export default function ProjectDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { user } = useAuthStore();
  const [project, setProject] = useState<Project | null>(null);
  const [timeEntries, setTimeEntries] = useState<TimeEntry[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isApplying, setIsApplying] = useState(false);
  const [showTimeModal, setShowTimeModal] = useState(false);
  const [timeHours, setTimeHours] = useState('');
  const [timeDescription, setTimeDescription] = useState('');
  const [isLoggingTime, setIsLoggingTime] = useState(false);

  useEffect(() => {
    const fetchProject = async () => {
      if (!params.id) return;

      setIsLoading(true);
      try {
        const projectData = await projectsApi.getById(params.id as string) as Project;
        setProject(projectData);

        // Fetch time entries if project is in progress
        if (['InProgress', 'Completed'].includes(projectData.status)) {
          const entries = await timeEntriesApi.getByProject(params.id as string) as TimeEntry[];
          setTimeEntries(entries);
        }
      } catch {
        setError('Failed to load project details');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProject();
  }, [params.id]);

  const handleApply = async () => {
    if (!project || !confirm('Apply to volunteer for this project?')) return;

    setIsApplying(true);
    setError(null);

    try {
      await projectsApi.apply(project.id);
      // Refresh project data
      const updated = await projectsApi.getById(project.id) as Project;
      setProject(updated);
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to apply');
    } finally {
      setIsApplying(false);
    }
  };

  const handleLogTime = async () => {
    if (!project) return;

    setIsLoggingTime(true);
    try {
      await timeEntriesApi.create(project.id, {
        hours: parseFloat(timeHours),
        description: timeDescription,
      });

      // Refresh time entries
      const entries = await timeEntriesApi.getByProject(project.id) as TimeEntry[];
      setTimeEntries(entries);

      // Refresh project for updated hours
      const updated = await projectsApi.getById(project.id) as Project;
      setProject(updated);

      setShowTimeModal(false);
      setTimeHours('');
      setTimeDescription('');
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to log time');
    } finally {
      setIsLoggingTime(false);
    }
  };

  if (isLoading) {
    return <LoadingPage />;
  }

  if (!project) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-16 text-center">
        <h1 className="text-2xl font-bold">Project Not Found</h1>
        <Link href="/projects">
          <Button className="mt-4">Browse Projects</Button>
        </Link>
      </div>
    );
  }

  const status = statusConfig[project.status] || { label: project.status, variant: 'default' as const, description: '' };
  const isExpert = user?.userType === 'Expert';
  const isAssignedExpert = project.expertId && user?.id === project.expertId;
  const canApply = isExpert && project.status === 'Open';
  const canLogTime = isAssignedExpert && project.status === 'InProgress';

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Back Link */}
      <Link
        href="/projects"
        className="inline-flex items-center text-sm text-gray-600 hover:text-gray-900 mb-6"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Back to projects
      </Link>

      {error && <ErrorMessage message={error} className="mb-6" />}

      <div className="grid lg:grid-cols-3 gap-8">
        {/* Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Header Card */}
          <Card>
            <CardContent className="p-6">
              <div className="flex items-start gap-4">
                <div className="w-16 h-16 rounded-lg bg-blue-100 flex items-center justify-center">
                  {project.organizationLogoUrl ? (
                    <img
                      src={project.organizationLogoUrl}
                      alt={project.organizationName}
                      className="w-16 h-16 rounded-lg object-cover"
                    />
                  ) : (
                    <Building2 className="h-8 w-8 text-blue-600" />
                  )}
                </div>
                <div className="flex-1">
                  <Badge variant={status.variant} className="mb-2">
                    {status.label}
                  </Badge>
                  <h1 className="text-2xl font-bold text-gray-900">{project.title}</h1>
                  <p className="text-gray-600 mt-1">{project.organizationName}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Description */}
          <Card>
            <CardHeader>
              <CardTitle>Project Description</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-gray-700 whitespace-pre-line">{project.description}</p>
            </CardContent>
          </Card>

          {/* Required Skills */}
          {project.requiredSkills.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Required Skills</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex flex-wrap gap-2">
                  {project.requiredSkills.map((skill) => (
                    <span
                      key={skill}
                      className="px-3 py-1.5 bg-blue-100 text-blue-700 rounded-full text-sm font-medium"
                    >
                      {skill}
                    </span>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Time Entries */}
          {timeEntries.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <span>Time Log</span>
                  <span className="text-sm font-normal text-gray-500">
                    {project.loggedHours} / {project.estimatedHours} hours
                  </span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {timeEntries.map((entry) => (
                    <div
                      key={entry.id}
                      className="flex items-start justify-between p-3 bg-gray-50 rounded-lg"
                    >
                      <div>
                        <p className="font-medium">{entry.hours} hours</p>
                        <p className="text-sm text-gray-600">{entry.description}</p>
                        <p className="text-xs text-gray-400 mt-1">
                          {formatDateTime(entry.loggedAt)}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Project Details */}
          <Card>
            <CardHeader>
              <CardTitle>Project Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Clock className="h-5 w-5 text-blue-600" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Estimated Hours</p>
                  <p className="font-medium">{project.estimatedHours} hours</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <FileText className="h-5 w-5 text-blue-600" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Category</p>
                  <p className="font-medium">{project.category}</p>
                </div>
              </div>

              {project.deadline && (
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <Calendar className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Deadline</p>
                    <p className="font-medium">{formatDateTime(project.deadline)}</p>
                  </div>
                </div>
              )}

              {project.expertName && (
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-green-100 rounded-lg">
                    <User className="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Assigned Expert</p>
                    <p className="font-medium">{project.expertName}</p>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Progress */}
          {project.status === 'InProgress' && (
            <Card>
              <CardHeader>
                <CardTitle>Progress</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Hours Logged</span>
                    <span className="font-medium">{project.loggedHours} / {project.estimatedHours}</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-2">
                    <div
                      className="bg-blue-600 h-2 rounded-full transition-all"
                      style={{
                        width: `${Math.min((project.loggedHours / project.estimatedHours) * 100, 100)}%`,
                      }}
                    />
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Actions */}
          <Card>
            <CardContent className="p-4 space-y-3">
              {canApply && (
                <Button className="w-full" onClick={handleApply} isLoading={isApplying}>
                  <Heart className="h-4 w-4 mr-2" />
                  Apply to Volunteer
                </Button>
              )}

              {canLogTime && (
                <Button className="w-full" onClick={() => setShowTimeModal(true)}>
                  <Clock className="h-4 w-4 mr-2" />
                  Log Time
                </Button>
              )}

              {!user && (
                <div className="text-center">
                  <p className="text-sm text-gray-500 mb-3">Sign in to apply</p>
                  <Link href="/login">
                    <Button className="w-full">Sign In</Button>
                  </Link>
                </div>
              )}

              {user && !isExpert && user.userType !== 'NonProfit' && (
                <p className="text-sm text-gray-500 text-center">
                  Only experts can apply for pro bono projects
                </p>
              )}
            </CardContent>
          </Card>

          {/* Info */}
          <Card>
            <CardContent className="p-4">
              <div className="space-y-2 text-sm">
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  100% volunteer work
                </p>
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  CSR hours tracked
                </p>
                <p className="flex items-center text-gray-600">
                  <CheckCircle className="h-4 w-4 mr-2 text-green-500" />
                  Certificate upon completion
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Log Time Modal */}
      <Modal
        isOpen={showTimeModal}
        onClose={() => setShowTimeModal(false)}
        title="Log Time"
        description="Record your volunteer hours"
      >
        <div className="space-y-4">
          <Input
            label="Hours"
            type="number"
            step="0.5"
            min="0.5"
            value={timeHours}
            onChange={(e) => setTimeHours(e.target.value)}
            placeholder="e.g., 2.5"
          />
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Description
            </label>
            <textarea
              value={timeDescription}
              onChange={(e) => setTimeDescription(e.target.value)}
              rows={3}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="What did you work on?"
            />
          </div>
        </div>
        <ModalFooter>
          <Button variant="outline" onClick={() => setShowTimeModal(false)}>
            Cancel
          </Button>
          <Button
            onClick={handleLogTime}
            isLoading={isLoggingTime}
            disabled={!timeHours || !timeDescription}
          >
            Log Time
          </Button>
        </ModalFooter>
      </Modal>
    </div>
  );
}
