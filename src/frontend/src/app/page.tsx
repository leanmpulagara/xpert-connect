import Link from 'next/link';
import { Button } from '@/components/ui';
import { ArrowRight, Users, Gavel, Heart } from 'lucide-react';

const features = [
  {
    icon: Users,
    title: 'Professional Consultations',
    description: 'Book one-on-one sessions with industry leaders and experts at competitive rates.',
  },
  {
    icon: Gavel,
    title: 'Charity Auctions',
    description: 'Participate in exclusive auctions for once-in-a-lifetime experiences with accomplished individuals.',
  },
  {
    icon: Heart,
    title: 'Pro Bono Projects',
    description: 'Connect non-profits with experts willing to volunteer their time and expertise.',
  },
];

export default function HomePage() {
  return (
    <div>
      {/* Hero Section */}
      <section className="relative bg-gradient-to-br from-blue-600 to-blue-800 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24 lg:py-32">
          <div className="max-w-3xl">
            <h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold tracking-tight">
              Connect with Accomplished Minds
            </h1>
            <p className="mt-6 text-xl text-blue-100">
              Access expertise from industry leaders, executives, and celebrities through consultations, exclusive auctions, and pro-bono projects.
            </p>
            <div className="mt-10 flex flex-col sm:flex-row gap-4">
              <Link href="/experts">
                <Button size="lg" className="bg-white text-blue-600 hover:bg-blue-50">
                  Find an Expert
                  <ArrowRight className="ml-2 h-5 w-5" />
                </Button>
              </Link>
              <Link href="/register">
                <Button size="lg" variant="outline" className="border-white text-white hover:bg-blue-700">
                  Become an Expert
                </Button>
              </Link>
            </div>
          </div>
        </div>
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-10" />
      </section>

      {/* Features Section */}
      <section className="py-24 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h2 className="text-3xl font-bold text-gray-900">Three Ways to Connect</h2>
            <p className="mt-4 text-lg text-gray-600">
              Choose the model that works best for your needs
            </p>
          </div>
          <div className="mt-16 grid md:grid-cols-3 gap-8">
            {features.map((feature) => (
              <div
                key={feature.title}
                className="bg-white rounded-xl p-8 shadow-sm border border-gray-200 hover:shadow-md transition-shadow"
              >
                <div className="h-12 w-12 rounded-lg bg-blue-100 flex items-center justify-center">
                  <feature.icon className="h-6 w-6 text-blue-600" />
                </div>
                <h3 className="mt-6 text-xl font-semibold text-gray-900">
                  {feature.title}
                </h3>
                <p className="mt-2 text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-24">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="bg-blue-600 rounded-2xl px-8 py-16 text-center">
            <h2 className="text-3xl font-bold text-white">
              Ready to get started?
            </h2>
            <p className="mt-4 text-lg text-blue-100 max-w-2xl mx-auto">
              Join thousands of seekers and experts who are already connecting on XpertConnect.
            </p>
            <div className="mt-8 flex justify-center gap-4">
              <Link href="/register">
                <Button size="lg" className="bg-white text-blue-600 hover:bg-blue-50">
                  Create Free Account
                </Button>
              </Link>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
