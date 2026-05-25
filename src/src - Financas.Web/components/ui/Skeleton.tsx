interface SkeletonProps {
  className?: string;
}

export function Skeleton({ className = '' }: SkeletonProps) {
  return (
    <div
      className={`animate-pulse rounded bg-white/8 ${className}`}
      style={{ backgroundColor: 'rgba(255,255,255,0.06)' }}
    />
  );
}

export function TableSkeleton({ rows = 5 }: { rows?: number }) {
  return (
    <div className="flex flex-col divide-y divide-white/5">
      {Array.from({ length: rows }).map((_, i) => (
        <div key={i} className="flex items-center gap-3 px-4 py-3">
          <Skeleton className="w-4 h-4 rounded-full shrink-0" />
          <Skeleton className="h-3 flex-1" />
          <Skeleton className="h-3 w-20" />
          <Skeleton className="h-3 w-16 ml-auto" />
        </div>
      ))}
    </div>
  );
}

export function CompetencyTabsSkeleton({ count = 5 }: { count?: number }) {
  return (
    <div className="flex items-center gap-0">
      {Array.from({ length: count }).map((_, i) => (
        <div key={i} className="flex items-center px-6 h-full">
          <Skeleton className="h-3 w-14" />
        </div>
      ))}
    </div>
  );
}
