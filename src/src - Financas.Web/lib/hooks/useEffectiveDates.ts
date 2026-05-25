'use client';
import { useState, useEffect, useCallback } from 'react';
import { effectiveDateService } from '@/lib/services';
import type { EffectiveDateSummary } from '@/lib/api/types';

export function useEffectiveDates() {
  const [data, setData] = useState<EffectiveDateSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const list = await effectiveDateService.getList();
      const sorted = [...list].sort((a, b) =>
        b.year !== a.year ? b.year - a.year : b.month - a.month
      );
      setData(sorted);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar competências');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  return { data, loading, error, reload: load };
}
