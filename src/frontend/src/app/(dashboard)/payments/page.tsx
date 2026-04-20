'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { CreditCard, DollarSign, ArrowUpRight, ArrowDownLeft, Clock, CheckCircle, XCircle } from 'lucide-react';
import { paymentsApi } from '@/lib/api';
import { formatCurrency, formatDateTime } from '@/lib/utils';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Badge,
  LoadingPage,
  ErrorMessage,
} from '@/components/ui';
import type { Payment, PagedResult } from '@/types';

const statusConfig: Record<string, { label: string; variant: 'default' | 'success' | 'warning' | 'danger' | 'info'; icon: typeof CheckCircle }> = {
  Pending: { label: 'Pending', variant: 'warning', icon: Clock },
  Authorized: { label: 'Authorized', variant: 'info', icon: Clock },
  Captured: { label: 'Completed', variant: 'success', icon: CheckCircle },
  Refunded: { label: 'Refunded', variant: 'default', icon: ArrowDownLeft },
  Cancelled: { label: 'Cancelled', variant: 'danger', icon: XCircle },
  Failed: { label: 'Failed', variant: 'danger', icon: XCircle },
};

export default function PaymentsPage() {
  const [payments, setPayments] = useState<Payment[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPayments = async () => {
      setIsLoading(true);
      try {
        const data = await paymentsApi.getMy({ pageSize: 50 }) as PagedResult<Payment>;
        setPayments(data.items);
      } catch (err) {
        const apiError = err as { message?: string };
        setError(apiError.message || 'Failed to load payment history');
      } finally {
        setIsLoading(false);
      }
    };

    fetchPayments();
  }, []);

  // Calculate totals
  const totalSpent = payments
    .filter((p) => p.status === 'Captured' && p.paymentType !== 'Refund')
    .reduce((sum, p) => sum + p.amount, 0);

  const totalRefunded = payments
    .filter((p) => p.status === 'Refunded' || p.paymentType === 'Refund')
    .reduce((sum, p) => sum + p.amount, 0);

  if (isLoading) {
    return <LoadingPage />;
  }

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Payment History</h1>
        <p className="mt-1 text-gray-600">View your transactions and receipts</p>
      </div>

      {error && <ErrorMessage message={error} className="mb-6" />}

      {/* Summary Cards */}
      <div className="grid md:grid-cols-3 gap-6 mb-8">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-blue-100 rounded-lg">
                <DollarSign className="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <p className="text-sm text-gray-500">Total Spent</p>
                <p className="text-2xl font-bold text-gray-900">
                  {formatCurrency(totalSpent)}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-green-100 rounded-lg">
                <ArrowDownLeft className="h-6 w-6 text-green-600" />
              </div>
              <div>
                <p className="text-sm text-gray-500">Total Refunded</p>
                <p className="text-2xl font-bold text-gray-900">
                  {formatCurrency(totalRefunded)}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-purple-100 rounded-lg">
                <CreditCard className="h-6 w-6 text-purple-600" />
              </div>
              <div>
                <p className="text-sm text-gray-500">Transactions</p>
                <p className="text-2xl font-bold text-gray-900">{payments.length}</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Transactions List */}
      <Card>
        <CardHeader>
          <CardTitle>Transactions</CardTitle>
        </CardHeader>
        <CardContent>
          {payments.length > 0 ? (
            <div className="divide-y divide-gray-100">
              {payments.map((payment) => {
                const status = statusConfig[payment.status] || {
                  label: payment.status,
                  variant: 'default' as const,
                  icon: Clock,
                };
                const StatusIcon = status.icon;
                const isRefund = payment.paymentType === 'Refund' || payment.status === 'Refunded';

                return (
                  <div
                    key={payment.id}
                    className="flex items-center justify-between py-4"
                  >
                    <div className="flex items-center gap-4">
                      <div
                        className={`p-2 rounded-lg ${
                          isRefund ? 'bg-green-100' : 'bg-gray-100'
                        }`}
                      >
                        {isRefund ? (
                          <ArrowDownLeft className="h-5 w-5 text-green-600" />
                        ) : (
                          <ArrowUpRight className="h-5 w-5 text-gray-600" />
                        )}
                      </div>
                      <div>
                        <p className="font-medium text-gray-900">
                          {payment.description || payment.paymentType}
                        </p>
                        <p className="text-sm text-gray-500">
                          {formatDateTime(payment.createdAt)}
                        </p>
                        {payment.referenceType && payment.referenceId && (
                          <Link
                            href={`/${payment.referenceType.toLowerCase()}s/${payment.referenceId}`}
                            className="text-sm text-blue-600 hover:underline"
                          >
                            View {payment.referenceType}
                          </Link>
                        )}
                      </div>
                    </div>
                    <div className="text-right">
                      <p
                        className={`text-lg font-semibold ${
                          isRefund ? 'text-green-600' : 'text-gray-900'
                        }`}
                      >
                        {isRefund ? '+' : '-'}
                        {formatCurrency(payment.amount, payment.currency)}
                      </p>
                      <Badge variant={status.variant}>
                        <StatusIcon className="h-3 w-3 mr-1" />
                        {status.label}
                      </Badge>
                    </div>
                  </div>
                );
              })}
            </div>
          ) : (
            <div className="text-center py-8">
              <CreditCard className="h-12 w-12 mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500">No transactions yet</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
