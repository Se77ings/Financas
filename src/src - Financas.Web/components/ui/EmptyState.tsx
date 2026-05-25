import type { ReactNode } from 'react';

interface EmptyStateProps {
  icon: ReactNode;
  title: string;
  description?: string;
  action?: ReactNode;
}

export function EmptyState({ icon, title, description, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-14 gap-3 text-center">
      <div className="text-gray-600">{icon}</div>
      <div>
        <p className="text-gray-400 text-sm font-medium">{title}</p>
        {description && (
          <p className="text-gray-600 text-xs mt-0.5">{description}</p>
        )}
      </div>
      {action && <div className="mt-1">{action}</div>}
    </div>
  );
}
