'use client';

import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Camera, Save, X, Edit2 } from 'lucide-react';
import { useAuthStore } from '@/stores/auth';
import { usersApi, expertsApi, seekersApi } from '@/lib/api';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Button,
  Input,
  Avatar,
  ErrorMessage,
} from '@/components/ui';

const profileSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  phoneNumber: z.string().optional(),
  profilePhotoUrl: z.string().url().optional().or(z.literal('')),
});

const expertProfileSchema = z.object({
  headline: z.string().min(1, 'Headline is required'),
  bio: z.string().min(10, 'Bio must be at least 10 characters'),
  hourlyRate: z.number().min(0, 'Hourly rate must be positive').optional(),
  yearsOfExperience: z.number().min(0, 'Years of experience must be positive').optional(),
});

const seekerProfileSchema = z.object({
  company: z.string().optional(),
  jobTitle: z.string().optional(),
});

type ProfileFormData = z.infer<typeof profileSchema>;
type ExpertProfileData = z.infer<typeof expertProfileSchema>;
type SeekerProfileData = z.infer<typeof seekerProfileSchema>;

export default function ProfilePage() {
  const { user, fetchUser } = useAuthStore();
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [expertProfile, setExpertProfile] = useState<ExpertProfileData | null>(null);
  const [seekerProfile, setSeekerProfile] = useState<SeekerProfileData | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      firstName: user?.firstName || '',
      lastName: user?.lastName || '',
      phoneNumber: '',
      profilePhotoUrl: user?.profilePhotoUrl || '',
    },
  });

  const {
    register: registerExpert,
    handleSubmit: handleSubmitExpert,
    reset: resetExpert,
    formState: { errors: expertErrors },
  } = useForm<ExpertProfileData>({
    resolver: zodResolver(expertProfileSchema),
  });

  const {
    register: registerSeeker,
    handleSubmit: handleSubmitSeeker,
    reset: resetSeeker,
    formState: { errors: seekerErrors },
  } = useForm<SeekerProfileData>({
    resolver: zodResolver(seekerProfileSchema),
  });

  useEffect(() => {
    if (user) {
      reset({
        firstName: user.firstName,
        lastName: user.lastName,
        phoneNumber: '',
        profilePhotoUrl: user.profilePhotoUrl || '',
      });

      // Fetch role-specific profile
      if (user.userType === 'Expert') {
        expertsApi.getMe().then((data) => {
          const profile = data as ExpertProfileData;
          setExpertProfile(profile);
          resetExpert(profile);
        }).catch(() => {});
      } else if (user.userType === 'Seeker') {
        seekersApi.getMe().then((data) => {
          const profile = data as SeekerProfileData;
          setSeekerProfile(profile);
          resetSeeker(profile);
        }).catch(() => {});
      }
    }
  }, [user, reset, resetExpert, resetSeeker]);

  const onSubmit = async (data: ProfileFormData) => {
    setIsSaving(true);
    setError(null);
    setSuccess(null);

    try {
      await usersApi.updateMe(data);
      await fetchUser();
      setSuccess('Profile updated successfully');
      setIsEditing(false);
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  const onSubmitExpert = async (data: ExpertProfileData) => {
    setIsSaving(true);
    setError(null);
    setSuccess(null);

    try {
      await expertsApi.updateProfile(data);
      setExpertProfile(data);
      setSuccess('Expert profile updated successfully');
      setIsEditing(false);
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to update expert profile');
    } finally {
      setIsSaving(false);
    }
  };

  const onSubmitSeeker = async (data: SeekerProfileData) => {
    setIsSaving(true);
    setError(null);
    setSuccess(null);

    try {
      await seekersApi.updateProfile(data);
      setSeekerProfile(data);
      setSuccess('Profile updated successfully');
      setIsEditing(false);
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setIsEditing(false);
    setError(null);
    if (user) {
      reset({
        firstName: user.firstName,
        lastName: user.lastName,
        phoneNumber: '',
        profilePhotoUrl: user.profilePhotoUrl || '',
      });
    }
    if (expertProfile) {
      resetExpert(expertProfile);
    }
    if (seekerProfile) {
      resetSeeker(seekerProfile);
    }
  };

  if (!user) {
    return null;
  }

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Profile</h1>
        <p className="mt-1 text-gray-600">
          Manage your personal information and preferences
        </p>
      </div>

      {error && <ErrorMessage message={error} className="mb-6" />}

      {success && (
        <div className="mb-6 p-4 rounded-lg bg-green-50 border border-green-200 text-sm text-green-700">
          {success}
        </div>
      )}

      {/* Profile Header */}
      <Card className="mb-6">
        <CardContent className="p-6">
          <div className="flex items-center gap-6">
            <div className="relative">
              <Avatar
                src={user.profilePhotoUrl}
                firstName={user.firstName}
                lastName={user.lastName}
                size="xl"
              />
              {isEditing && (
                <button
                  type="button"
                  className="absolute bottom-0 right-0 p-2 bg-blue-600 rounded-full text-white hover:bg-blue-700 transition-colors"
                >
                  <Camera className="h-4 w-4" />
                </button>
              )}
            </div>
            <div className="flex-1">
              <h2 className="text-xl font-semibold text-gray-900">
                {user.firstName} {user.lastName}
              </h2>
              <p className="text-gray-500">{user.email}</p>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 mt-2">
                {user.userType}
              </span>
            </div>
            {!isEditing && (
              <Button onClick={() => setIsEditing(true)} variant="outline">
                <Edit2 className="h-4 w-4 mr-2" />
                Edit Profile
              </Button>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Basic Information */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle>Basic Information</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input
                label="First Name"
                {...register('firstName')}
                error={errors.firstName?.message}
                disabled={!isEditing}
              />
              <Input
                label="Last Name"
                {...register('lastName')}
                error={errors.lastName?.message}
                disabled={!isEditing}
              />
            </div>
            <Input
              label="Phone Number"
              type="tel"
              placeholder="+1 (555) 000-0000"
              {...register('phoneNumber')}
              error={errors.phoneNumber?.message}
              disabled={!isEditing}
            />
            <Input
              label="Profile Photo URL"
              type="url"
              placeholder="https://example.com/photo.jpg"
              {...register('profilePhotoUrl')}
              error={errors.profilePhotoUrl?.message}
              disabled={!isEditing}
            />

            {isEditing && (
              <div className="flex gap-3 pt-4">
                <Button type="submit" isLoading={isSaving}>
                  <Save className="h-4 w-4 mr-2" />
                  Save Changes
                </Button>
                <Button type="button" variant="outline" onClick={handleCancel}>
                  <X className="h-4 w-4 mr-2" />
                  Cancel
                </Button>
              </div>
            )}
          </form>
        </CardContent>
      </Card>

      {/* Expert-specific fields */}
      {user.userType === 'Expert' && (
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Expert Profile</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmitExpert(onSubmitExpert)} className="space-y-4">
              <Input
                label="Headline"
                placeholder="e.g., Former CEO at Fortune 500"
                {...registerExpert('headline')}
                error={expertErrors.headline?.message}
                disabled={!isEditing}
              />
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Bio
                </label>
                <textarea
                  {...registerExpert('bio')}
                  rows={4}
                  className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 shadow-sm placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-50 disabled:text-gray-500"
                  placeholder="Tell seekers about your expertise and experience..."
                  disabled={!isEditing}
                />
                {expertErrors.bio && (
                  <p className="mt-1 text-sm text-red-600">{expertErrors.bio.message}</p>
                )}
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                  label="Hourly Rate ($)"
                  type="number"
                  placeholder="250"
                  {...registerExpert('hourlyRate', { valueAsNumber: true })}
                  error={expertErrors.hourlyRate?.message}
                  disabled={!isEditing}
                />
                <Input
                  label="Years of Experience"
                  type="number"
                  placeholder="15"
                  {...registerExpert('yearsOfExperience', { valueAsNumber: true })}
                  error={expertErrors.yearsOfExperience?.message}
                  disabled={!isEditing}
                />
              </div>

              {isEditing && (
                <div className="flex gap-3 pt-4">
                  <Button type="submit" isLoading={isSaving}>
                    <Save className="h-4 w-4 mr-2" />
                    Save Expert Profile
                  </Button>
                </div>
              )}
            </form>
          </CardContent>
        </Card>
      )}

      {/* Seeker-specific fields */}
      {user.userType === 'Seeker' && (
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Professional Information</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmitSeeker(onSubmitSeeker)} className="space-y-4">
              <Input
                label="Company"
                placeholder="Your company name"
                {...registerSeeker('company')}
                error={seekerErrors.company?.message}
                disabled={!isEditing}
              />
              <Input
                label="Job Title"
                placeholder="Your job title"
                {...registerSeeker('jobTitle')}
                error={seekerErrors.jobTitle?.message}
                disabled={!isEditing}
              />

              {isEditing && (
                <div className="flex gap-3 pt-4">
                  <Button type="submit" isLoading={isSaving}>
                    <Save className="h-4 w-4 mr-2" />
                    Save Professional Info
                  </Button>
                </div>
              )}
            </form>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
