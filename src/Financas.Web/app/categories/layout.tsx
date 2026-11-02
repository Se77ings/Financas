import { Suspense } from 'react';

export default function CategoriesLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <Suspense fallback={<div className="p-8 text-white">Carregando...</div>}>
      {children}
    </Suspense>
  );
}
