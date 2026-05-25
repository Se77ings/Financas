'use client';
import { useEffect, useState, useCallback, useRef } from 'react';
import { CheckCircle, XCircle, X } from 'lucide-react';

export type ToastType = 'success' | 'error';

export interface ToastMessage {
  id: number;
  type: ToastType;
  message: string;
}

let _dispatch: ((t: Omit<ToastMessage, 'id'>) => void) | null = null;

/** Chame de qualquer lugar para exibir um toast */
export const toast = {
  success: (message: string) => _dispatch?.({ type: 'success', message }),
  error: (message: string) => _dispatch?.({ type: 'error', message }),
};

export function ToastProvider() {
  const [toasts, setToasts] = useState<ToastMessage[]>([]);
  const counter = useRef(0);

  const add = useCallback((t: Omit<ToastMessage, 'id'>) => {
    const id = ++counter.current;
    setToasts((prev) => [...prev, { ...t, id }]);
    setTimeout(() => {
      setToasts((prev) => prev.filter((x) => x.id !== id));
    }, 3500);
  }, []);

  useEffect(() => {
    _dispatch = add;
    return () => {
      _dispatch = null;
    };
  }, [add]);

  if (toasts.length === 0) return null;

  return (
    <div className="fixed bottom-14 right-4 z-[200] flex flex-col gap-2 items-end pointer-events-none">
      {toasts.map((t) => (
        <div
          key={t.id}
          className="flex items-center gap-3 px-4 py-3 rounded-lg shadow-xl border border-white/10 text-sm font-medium pointer-events-auto"
          style={{
            backgroundColor: t.type === 'success' ? '#14532d' : '#450a0a',
            color: t.type === 'success' ? '#86efac' : '#fca5a5',
            animation: 'slideIn 0.2s ease-out',
          }}
        >
          {t.type === 'success' ? (
            <CheckCircle size={16} className="shrink-0" />
          ) : (
            <XCircle size={16} className="shrink-0" />
          )}
          <span>{t.message}</span>
          <button
            onClick={() => setToasts((prev) => prev.filter((x) => x.id !== t.id))}
            className="ml-1 opacity-60 hover:opacity-100 transition-opacity"
          >
            <X size={14} />
          </button>
        </div>
      ))}
      <style>{`
        @keyframes slideIn {
          from { opacity: 0; transform: translateX(12px); }
          to   { opacity: 1; transform: translateX(0); }
        }
      `}</style>
    </div>
  );
}
