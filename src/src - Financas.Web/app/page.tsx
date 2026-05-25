'use client';
import { useState, useCallback } from 'react';
import {
  Home,
  TrendingUp,
  Wallet,
  Plus,
  Pencil,
  Trash2,
} from 'lucide-react';
import { effectiveDateService } from '@/lib/services';
import type { EffectiveDateSummary } from '@/lib/api/types';
import { useEffectiveDates } from '@/lib/hooks';
import { BR_CURRENCY, COLORS } from '@/lib/constants';
import { EffectiveDateDetails } from '@/components/features/EffectiveDateDetails';
import { EffectiveDateModal } from '@/components/features/EffectiveDateModal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { CompetencyTabsSkeleton } from '@/components/ui/Skeleton';
import { toast } from '@/components/ui/Toast';

export default function AppPage() {
  const { data: competencies, loading, reload } = useEffectiveDates();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [page, setPage] = useState<'home' | 'details'>('home');
  const [edModal, setEdModal] = useState<{ editing: EffectiveDateSummary | null } | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<EffectiveDateSummary | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const handleNav = (id: string) => {
    setSelectedId(id);
    setPage('details');
  };

  const handleHome = () => {
    setSelectedId(null);
    setPage('home');
  };

  const handleDelete = async () => {
    if (!confirmDelete) return;
    setDeleteLoading(true);
    try {
      await effectiveDateService.delete(confirmDelete.id);
      toast.success('Competência excluída!');
      if (selectedId === confirmDelete.id) handleHome();
      setConfirmDelete(null);
      await reload();
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : 'Erro ao excluir');
    } finally {
      setDeleteLoading(false);
    }
  };

  const handleEdSuccess = useCallback(
    (msg: string) => {
      toast.success(msg);
      reload();
    },
    [reload]
  );

  const selected = competencies.find((c) => c.id === selectedId);

  // Stats para home
  const totalBalance = competencies.reduce((s, c) => s + c.balance, 0);
  const totalPayables = competencies.reduce((s, c) => s + c.payableCount, 0);
  const totalReceivables = competencies.reduce((s, c) => s + c.receivableCount, 0);

  return (
    <div
      className="flex flex-col h-screen text-gray-100"
      style={{ backgroundColor: COLORS.primary }}
    >
      {/* ── HEADER ───────────────────────────────────── */}
      <header
        className="px-5 py-3 border-b shrink-0 flex items-center justify-between"
        style={{ borderColor: COLORS.border + '44' }}
      >
        <div className="flex items-center gap-3">
          <Wallet size={18} className="text-amber-400" />
          <h1 className="text-sm font-bold tracking-tight">
            {page === 'home'
              ? 'Finanças Pessoais'
              : `${selected?.month ?? ''} / ${selected?.year ?? ''}`}
          </h1>
        </div>

        <div className="flex items-center gap-1.5">
          {page === 'details' && selected && (
            <>
              <button
                onClick={() => setEdModal({ editing: selected })}
                title="Editar competência"
                className="flex items-center gap-1 px-2.5 py-1.5 rounded text-xs text-gray-400 hover:text-white hover:bg-white/5 transition"
              >
                <Pencil size={13} />
              </button>
              <button
                onClick={() => setConfirmDelete(selected)}
                title="Excluir competência"
                className="flex items-center gap-1 px-2.5 py-1.5 rounded text-xs text-red-500 hover:text-red-300 hover:bg-red-900/20 transition"
              >
                <Trash2 size={13} />
              </button>
              <div
                className="w-px h-5 mx-1"
                style={{ backgroundColor: COLORS.border + '44' }}
              />
            </>
          )}
          <button
            onClick={() => setEdModal({ editing: null })}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded text-xs font-medium border border-white/15 text-gray-300 hover:bg-white/5 transition"
          >
            <Plus size={13} /> Nova Competência
          </button>
        </div>
      </header>

      {/* ── MAIN ─────────────────────────────────────── */}
      <main className="flex-1 overflow-auto p-5">
        {page === 'home' ? (
          <div className="flex flex-col gap-6 h-full">
            {/* Cards de resumo */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <SummaryCard
                label="Saldo Total (todas)"
                value={BR_CURRENCY.format(totalBalance)}
                valueClass={totalBalance >= 0 ? 'text-green-400' : 'text-orange-400'}
              />
              <SummaryCard
                label="Total Recebíveis"
                value={String(totalReceivables)}
                valueClass="text-blue-400"
              />
              <SummaryCard
                label="Total Obrigações"
                value={String(totalPayables)}
                valueClass="text-red-400"
              />
            </div>

            {/* Grid de competências */}
            {!loading && competencies.length > 0 && (
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
                {competencies.map((c) => (
                  <button
                    key={c.id}
                    onClick={() => handleNav(c.id)}
                    className="flex flex-col items-center p-3 rounded-lg border border-white/5 hover:border-amber-500/30 hover:bg-white/5 transition-all group"
                    style={{ backgroundColor: 'rgba(0,0,0,0.18)' }}
                  >
                    <span className="text-xs text-gray-400 font-medium">{c.display}</span>
                    <span
                      className={`text-sm font-bold font-mono mt-1 ${
                        c.balance >= 0 ? 'text-green-400' : 'text-orange-400'
                      }`}
                    >
                      {BR_CURRENCY.format(c.balance)}
                    </span>
                    <span className="text-xs text-gray-600 mt-0.5 group-hover:text-gray-400 transition-colors">
                      Ver detalhes →
                    </span>
                  </button>
                ))}
              </div>
            )}

            {!loading && competencies.length === 0 && (
              <div
                className="flex flex-col items-center justify-center py-16 gap-3 rounded-2xl border-2 border-dashed"
                style={{ borderColor: COLORS.border + '33' }}
              >
                <Wallet size={36} className="text-gray-600" />
                <p className="text-gray-400 text-sm">Nenhuma competência criada ainda</p>
                <button
                  onClick={() => setEdModal({ editing: null })}
                  className="flex items-center gap-1.5 px-4 py-2 rounded text-xs font-medium bg-amber-700/50 hover:bg-amber-700 text-amber-200 transition"
                >
                  <Plus size={13} /> Criar primeira competência
                </button>
              </div>
            )}

            {/* Área de gráficos (futuro) */}
            {competencies.length > 0 && (
              <div
                className="flex-1 min-h-[100px] border-2 border-dashed rounded-2xl flex flex-col items-center justify-center"
                style={{
                  borderColor: COLORS.border + '22',
                  backgroundColor: 'rgba(0,0,0,0.08)',
                }}
              >
                <TrendingUp size={32} className="text-gray-700 mb-2" />
                <p className="text-gray-600 text-xs">
                  Área reservada para gráficos comparativos
                </p>
              </div>
            )}
          </div>
        ) : selectedId ? (
          <EffectiveDateDetails
            effectiveDateId={selectedId}
            onReloadParent={reload}
          />
        ) : null}
      </main>

      {/* ── FOOTER TABS ──────────────────────────────── */}
      <footer
        className="h-11 border-t flex items-stretch overflow-hidden shrink-0"
        style={{ borderColor: COLORS.border + '44', backgroundColor: 'rgba(0,0,0,0.35)' }}
      >
        {/* Home */}
        <button
          onClick={handleHome}
          className={`flex items-center justify-center px-5 border-r transition-colors ${
            page === 'home'
              ? 'text-amber-400 bg-white/5'
              : 'text-gray-500 hover:text-gray-300 hover:bg-white/5'
          }`}
          style={{ borderColor: COLORS.border + '44' }}
        >
          <Home size={16} />
        </button>

        {/* Tabs de competências */}
        <div className="flex-1 flex overflow-x-auto items-stretch no-scrollbar">
          {loading ? (
            <CompetencyTabsSkeleton count={5} />
          ) : competencies.length === 0 ? (
            <div className="flex items-center px-5 text-xs text-gray-600 italic">
              Sem competências — crie uma nova →
            </div>
          ) : (
            competencies.map((comp) => {
              const isActive = comp.id === selectedId;
              return (
                <button
                  key={comp.id}
                  onClick={() => handleNav(comp.id)}
                  className={`relative flex items-center gap-2 px-5 border-r text-xs font-medium whitespace-nowrap transition-colors ${
                    isActive
                      ? 'text-amber-300 bg-white/5'
                      : 'text-gray-500 hover:text-gray-300 hover:bg-white/5'
                  }`}
                  style={{ borderRightColor: COLORS.border + '33' }}
                >
                  {isActive && (
                    <span className="absolute top-0 left-0 right-0 h-0.5 bg-amber-400" />
                  )}
                  {comp.display}
                  {comp.balance < 0 && (
                    <span
                      className="w-1.5 h-1.5 rounded-full bg-orange-500 shrink-0"
                      title="Saldo negativo"
                    />
                  )}
                  {comp.balance >= 0 && comp.receivableCount > 0 && (
                    <span className="w-1.5 h-1.5 rounded-full bg-green-500/60 shrink-0" />
                  )}
                </button>
              );
            })
          )}
        </div>
      </footer>

      {/* ── MODALS ───────────────────────────────────── */}
      {edModal && (
        <EffectiveDateModal
          isOpen
          onClose={() => setEdModal(null)}
          onSuccess={() =>
            handleEdSuccess(edModal.editing ? 'Competência atualizada!' : 'Competência criada!')
          }
          editing={edModal.editing}
        />
      )}

      <ConfirmDialog
        isOpen={!!confirmDelete}
        onClose={() => setConfirmDelete(null)}
        onConfirm={handleDelete}
        loading={deleteLoading}
        title="Excluir competência"
        message={`Tem certeza que deseja excluir "${confirmDelete?.display}"? A operação falhará se houver lançamentos vinculados.`}
        confirmLabel="Excluir"
      />
    </div>
  );
}

function SummaryCard({
  label,
  value,
  valueClass,
}: {
  label: string;
  value: string;
  valueClass: string;
}) {
  return (
    <div
      className="p-4 rounded-lg border border-white/5"
      style={{ backgroundColor: 'rgba(0,0,0,0.20)' }}
    >
      <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">{label}</p>
      <p className={`text-xl font-bold font-mono ${valueClass}`}>{value}</p>
    </div>
  );
}
