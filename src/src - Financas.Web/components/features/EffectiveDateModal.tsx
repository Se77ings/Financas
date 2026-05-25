'use client';
import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import type { EffectiveDateSummary } from '@/lib/api/types';

interface EffectiveDateModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  editing?: EffectiveDateSummary | null;
}

export function EffectiveDateModal({
  isOpen,
  onClose,
  onSuccess,
  editing,
}: EffectiveDateModalProps) {
  const [month, setMonth] = useState('');
  const [year, setYear] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (editing) {
      setMonth(String(editing.month));
      setYear(String(editing.year));
    } else {
      setMonth('');
      setYear('');
    }
  }, [editing]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      // TODO: Implement create/update
      console.log('Submit', { month, year, editing });
      onSuccess();
      onClose();
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-gray-900 border border-gray-700 rounded-lg p-6 w-full max-w-md">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold">
            {editing ? 'Editar Competência' : 'Nova Competência'}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-white">
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Mês</label>
            <input
              type="number"
              value={month}
              onChange={(e) => setMonth(e.target.value)}
              className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded"
              min="1"
              max="12"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Ano</label>
            <input
              type="number"
              value={year}
              onChange={(e) => setYear(e.target.value)}
              className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded"
              min="2000"
              max="2100"
              required
            />
          </div>
          <div className="flex gap-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-2 bg-gray-700 hover:bg-gray-600 rounded"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 rounded disabled:opacity-50"
            >
              {loading ? 'Salvando...' : editing ? 'Atualizar' : 'Criar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}