'use client';
import { useState, useEffect, useCallback } from 'react';
import { effectiveDateService } from '@/lib/services';
import type { EffectiveDateView } from '@/lib/api/types';

export function useEffectiveDateView(id: string | null) {
  const [data, setData] = useState<EffectiveDateView | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      const view = await effectiveDateService.getById(id);
      setData(view);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar competência');
      setData(null);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    load();
  }, [load]);

  return { data, loading, error, reload: load };
}
