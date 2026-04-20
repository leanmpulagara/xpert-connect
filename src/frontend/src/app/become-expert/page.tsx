'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  ArrowLeft,
  ArrowRight,
  CheckCircle,
  Briefcase,
  FileText,
  DollarSign,
  Award,
} from 'lucide-react';
import { useAuthStore } from '@/stores/auth';
import { expertsApi } from '@/lib/api';
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

const categories = [
  { id: 'business', label: 'Business & Strategy', icon: Briefcase },
  { id: 'technology', label: 'Technology & Engineering', icon: FileText },
  { id: 'finance', label: 'Finance & Investment', icon: DollarSign },
  { id: 'leadership', label: 'Leadership & Management', icon: Award },
  { id: 'marketing', label: 'Marketing & Sales', icon: Briefcase },
  { id: 'legal', label: 'Legal & Compliance', icon: FileText },
  { id: 'healthcare', label: 'Healthcare & Life Sciences', icon: Award },
  { id: 'other', label: 'Other', icon: Briefcase },
];

const expertSchema = z.object({
  category: z.string().min(1, 'Please select a category'),
  headline: z.string().min(5, 'Headline must be at least 5 characters'),
  bio: z.string().min(50, 'Bio must be at least 50 characters'),
  hourlyRate: z.number().min(50, 'Minimum hourly rate is $50'),
  yearsOfExperience: z.number().min(1, 'Years of experience is required'),
  credentials: z.string().optional(),
  linkedInUrl: z.string().url('Please enter a valid URL').optional().or(z.literal('')),
});

type ExpertFormData = z.infer<typeof expertSchema>;

const steps = [
  { id: 1, name: 'Category', description: 'Select your expertise' },
  { id: 2, name: 'Profile', description: 'Tell us about yourself' },
  { id: 3, name: 'Credentials', description: 'Your qualifications' },
  { id: 4, name: 'Pricing', description: 'Set your rate' },
];

export default function BecomeExpertPage() {
  const router = useRouter();
  const { user } = useAuthStore();
  const [currentStep, setCurrentStep] = useState(1);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    setValue,
    trigger,
    formState: { errors },
  } = useForm<ExpertFormData>({
    resolver: zodResolver(expertSchema),
    defaultValues: {
      hourlyRate: 100,
      yearsOfExperience: 1,
    },
  });

  const handleCategorySelect = (categoryId: string) => {
    setSelectedCategory(categoryId);
    setValue('category', categoryId);
  };

  const handleNext = async () => {
    let fieldsToValidate: (keyof ExpertFormData)[] = [];

    switch (currentStep) {
      case 1:
        fieldsToValidate = ['category'];
        break;
      case 2:
        fieldsToValidate = ['headline', 'bio'];
        break;
      case 3:
        fieldsToValidate = ['credentials', 'linkedInUrl'];
        break;
      case 4:
        fieldsToValidate = ['hourlyRate', 'yearsOfExperience'];
        break;
    }

    const isValid = await trigger(fieldsToValidate);
    if (isValid) {
      setCurrentStep((prev) => Math.min(prev + 1, 4));
    }
  };

  const handleBack = () => {
    setCurrentStep((prev) => Math.max(prev - 1, 1));
  };

  const onSubmit = async (data: ExpertFormData) => {
    setIsSubmitting(true);
    setError(null);

    try {
      await expertsApi.createProfile({
        category: data.category,
        headline: data.headline,
        bio: data.bio,
        hourlyRate: data.hourlyRate,
        yearsOfExperience: data.yearsOfExperience,
        credentials: data.credentials,
        linkedInUrl: data.linkedInUrl,
      });
      router.push('/dashboard');
    } catch (err) {
      const apiError = err as { message?: string };
      setError(apiError.message || 'Failed to create expert profile');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <ProtectedRoute allowedRoles={['Seeker']}>
      <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Become an Expert</h1>
          <p className="mt-2 text-gray-600">
            Share your expertise and connect with seekers worldwide
          </p>
        </div>

        {/* Progress Steps */}
        <nav className="mb-8">
          <ol className="flex items-center justify-center">
            {steps.map((step, index) => (
              <li key={step.id} className="flex items-center">
                <div
                  className={cn(
                    'flex items-center',
                    index !== steps.length - 1 && 'pr-8 sm:pr-20'
                  )}
                >
                  <div
                    className={cn(
                      'w-10 h-10 rounded-full flex items-center justify-center text-sm font-medium',
                      currentStep > step.id
                        ? 'bg-blue-600 text-white'
                        : currentStep === step.id
                        ? 'bg-blue-600 text-white'
                        : 'bg-gray-200 text-gray-600'
                    )}
                  >
                    {currentStep > step.id ? (
                      <CheckCircle className="h-5 w-5" />
                    ) : (
                      step.id
                    )}
                  </div>
                  <span
                    className={cn(
                      'ml-3 text-sm font-medium hidden sm:block',
                      currentStep >= step.id ? 'text-gray-900' : 'text-gray-500'
                    )}
                  >
                    {step.name}
                  </span>
                </div>
                {index !== steps.length - 1 && (
                  <div
                    className={cn(
                      'h-0.5 w-12 sm:w-20',
                      currentStep > step.id ? 'bg-blue-600' : 'bg-gray-200'
                    )}
                  />
                )}
              </li>
            ))}
          </ol>
        </nav>

        {error && <ErrorMessage message={error} className="mb-6" />}

        <form onSubmit={handleSubmit(onSubmit)}>
          {/* Step 1: Category Selection */}
          {currentStep === 1 && (
            <Card>
              <CardHeader>
                <CardTitle>Select Your Category</CardTitle>
                <CardDescription>
                  Choose the primary area where you can offer expert advice
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  {categories.map((category) => (
                    <button
                      key={category.id}
                      type="button"
                      onClick={() => handleCategorySelect(category.id)}
                      className={cn(
                        'p-4 rounded-lg border-2 text-center transition-all',
                        selectedCategory === category.id
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-200 hover:border-gray-300'
                      )}
                    >
                      <category.icon
                        className={cn(
                          'h-8 w-8 mx-auto mb-2',
                          selectedCategory === category.id
                            ? 'text-blue-600'
                            : 'text-gray-400'
                        )}
                      />
                      <span
                        className={cn(
                          'text-sm font-medium',
                          selectedCategory === category.id
                            ? 'text-blue-600'
                            : 'text-gray-700'
                        )}
                      >
                        {category.label}
                      </span>
                    </button>
                  ))}
                </div>
                {errors.category && (
                  <p className="mt-2 text-sm text-red-600">
                    {errors.category.message}
                  </p>
                )}
              </CardContent>
            </Card>
          )}

          {/* Step 2: Profile Information */}
          {currentStep === 2 && (
            <Card>
              <CardHeader>
                <CardTitle>Tell Us About Yourself</CardTitle>
                <CardDescription>
                  This information will be visible to seekers browsing experts
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <Input
                  label="Professional Headline"
                  placeholder="e.g., Former CEO at Fortune 500 Company"
                  {...register('headline')}
                  error={errors.headline?.message}
                  helperText="A brief title that describes your expertise"
                />
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Bio
                  </label>
                  <textarea
                    {...register('bio')}
                    rows={6}
                    className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 shadow-sm placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    placeholder="Tell seekers about your background, experience, and what value you can provide..."
                  />
                  {errors.bio && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.bio.message}
                    </p>
                  )}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Step 3: Credentials */}
          {currentStep === 3 && (
            <Card>
              <CardHeader>
                <CardTitle>Your Credentials</CardTitle>
                <CardDescription>
                  Add your qualifications to build trust with seekers
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Credentials & Certifications
                  </label>
                  <textarea
                    {...register('credentials')}
                    rows={4}
                    className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 shadow-sm placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    placeholder="List your degrees, certifications, awards, and notable achievements..."
                  />
                  {errors.credentials && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.credentials.message}
                    </p>
                  )}
                </div>
                <Input
                  label="LinkedIn Profile URL (Optional)"
                  type="url"
                  placeholder="https://linkedin.com/in/yourprofile"
                  {...register('linkedInUrl')}
                  error={errors.linkedInUrl?.message}
                />
              </CardContent>
            </Card>
          )}

          {/* Step 4: Pricing */}
          {currentStep === 4 && (
            <Card>
              <CardHeader>
                <CardTitle>Set Your Rate</CardTitle>
                <CardDescription>
                  Choose your hourly rate for consultations
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <Input
                  label="Hourly Rate ($)"
                  type="number"
                  placeholder="100"
                  {...register('hourlyRate', { valueAsNumber: true })}
                  error={errors.hourlyRate?.message}
                  helperText="XpertConnect takes a 15% platform fee"
                />
                <Input
                  label="Years of Experience"
                  type="number"
                  placeholder="10"
                  {...register('yearsOfExperience', { valueAsNumber: true })}
                  error={errors.yearsOfExperience?.message}
                />

                <div className="bg-gray-50 rounded-lg p-4 mt-6">
                  <h4 className="font-medium text-gray-900 mb-2">
                    Your Earnings Breakdown
                  </h4>
                  <div className="space-y-1 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Your hourly rate</span>
                      <span className="font-medium">$100.00</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Platform fee (15%)</span>
                      <span className="text-red-600">-$15.00</span>
                    </div>
                    <hr className="my-2" />
                    <div className="flex justify-between">
                      <span className="font-medium text-gray-900">
                        You receive
                      </span>
                      <span className="font-medium text-green-600">
                        $85.00/hr
                      </span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Navigation Buttons */}
          <div className="flex justify-between mt-6">
            <Button
              type="button"
              variant="outline"
              onClick={handleBack}
              disabled={currentStep === 1}
            >
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back
            </Button>

            {currentStep < 4 ? (
              <Button type="button" onClick={handleNext}>
                Next
                <ArrowRight className="h-4 w-4 ml-2" />
              </Button>
            ) : (
              <Button type="submit" isLoading={isSubmitting}>
                Complete Registration
              </Button>
            )}
          </div>
        </form>
      </div>
    </ProtectedRoute>
  );
}
