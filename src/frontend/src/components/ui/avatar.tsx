'use client';

import { cn, getInitials } from '@/lib/utils';
import Image from 'next/image';
import { useState } from 'react';

interface AvatarProps {
  src?: string | null;
  alt?: string;
  firstName?: string;
  lastName?: string;
  size?: 'sm' | 'md' | 'lg' | 'xl';
  className?: string;
}

const sizes = {
  sm: 'h-8 w-8 text-xs',
  md: 'h-10 w-10 text-sm',
  lg: 'h-12 w-12 text-base',
  xl: 'h-16 w-16 text-lg',
};

const imageSizes = {
  sm: 32,
  md: 40,
  lg: 48,
  xl: 64,
};

export function Avatar({ src, alt, firstName, lastName, size = 'md', className }: AvatarProps) {
  const [imageError, setImageError] = useState(false);
  const initials = getInitials(firstName, lastName);

  if (src && !imageError) {
    return (
      <div className={cn('relative rounded-full overflow-hidden bg-gray-100', sizes[size], className)}>
        <Image
          src={src}
          alt={alt || `${firstName} ${lastName}`}
          width={imageSizes[size]}
          height={imageSizes[size]}
          className="object-cover"
          onError={() => setImageError(true)}
        />
      </div>
    );
  }

  return (
    <div
      className={cn(
        'flex items-center justify-center rounded-full bg-blue-100 text-blue-600 font-medium',
        sizes[size],
        className
      )}
    >
      {initials}
    </div>
  );
}
