'use client';
import { useState, useEffect, useCallback } from 'react';
import {
  ArrowUpCircle,
  ArrowDownCircle,
  CreditCard,
  ChevronRight,
  Calendar,
  X,
  Pencil,
  Plus,
  Filter,
} from 'lucide-react';
import { effectiveDateService } from '@/lib/services';
import type {
  EffectiveDateView,
  ImmediateExpenseView,
  CardExpenseView,
  CardExpenseItemView,
  ReceivableView,
} from '@/lib/api/types';
import { BR_DATE, BR_CURRENCY } from '@/lib/constants';
// import { ReceivableModal } from '@/components/features/ReceivableModal';
// import { ImmediateExpenseModal } from '@/components/features/ImmediateExpenseModal';
// import { CardExpenseModal } from '@/components/features/CardExpenseModal';
// import { CardExpenseItemModal } from '@/components/features/CardExpenseItemModal';
import { toast } from '@/components/ui/Toast';

interface Props {
  effectiveDateId: string;
  onReloadParent?: () => void;
}

type TransactionRow =
  | { kind: 'receivable'; data: ReceivableView }
  | { kind: 'payable'; data: ImmediateExpenseView }
  | { kind: 'card'; data: CardExpenseView };

type FilterKind = 'all' | 'receivable' | 'payable' | 'card';

type ModalState =
  | { type: 'receivable'; editing: ReceivableView | null }
  | { type: 'payable'; editing: ImmediateExpenseView | null }
  | { type: 'card'; editing: CardExpenseView | null }
  | { type: 'cardItem'; cardId: string; editing: CardExpenseItemView | null }
  | null;

const FILTER_LABELS: Record<FilterKind, string> = {
  all: 'Todos',
  receivable: 'Recebimentos',
  payable: 'Gastos',
  card: 'Cartão',
};

export function EffectiveDateDetails({ effectiveDateId, onReloadParent }: Props) {
  const [view, setView] = useState<EffectiveDateView | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedCard, setSelectedCard] = useState<CardExpenseView | null>(null);
  const [modal, setModal] = useState<ModalState>(null);
  const [filter, setFilter] = useState<FilterKind>('all');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await effectiveDateService.getById(effectiveDateId);
      setView(data);
      if (selectedCard) {
        const refreshed = data.cardExpense.find((c) => c.id === selectedCard.id);
        setSelectedCard(refreshed ?? null);
      }
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : 'Erro ao carregar competência');
    } finally {
      setLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [effectiveDateId]);

  useEffect(() => {
    load();
  }, [load]);

  const handleSuccess = useCallback(() => {
    toast.success('Salvo com sucesso!');
    load();
    onReloadParent?.();
  }, [load, onReloadParent]);

  const closeModal = () => setModal(null);

  if (loading && !view) {
    return (
      <div className="flex-1 flex items-center justify-center py-20">
        <div className="w-6 h-6 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!view) return null;

  const allRows: TransactionRow[] = [
    ...view.receivable.map((r) => ({ kind: 'receivable' as const, data: r })),
    ...view.payable.map((p) => ({ kind: 'payable' as const, data: p })),
    ...view.cardExpense.map((c) => ({ kind: 'card' as const, data: c })),
  ];

  const rows =
    filter === 'all' ? allRows : allRows.filter((r) => r.kind === filter);

  const totalReceivable = view.receivable.reduce((s, r) => s + r.total, 0);
  const totalPayable =
    view.payable.reduce((s, p) => s + p.total, 0) +
    view.cardExpense.reduce((s, c) => s + c.total, 0);
  const balance = totalReceivable - totalPayable;

  return (
    <>
      {/* Action bar */}
      <div className="flex flex-wrap items-center gap-2 mb-4">
        <button
          onClick={() => setModal({ type: 'receivable', editing: null })}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded bg-green-800 hover:bg-green-700 transition text-xs font-medium text-white"
        >
          <Plus size={13} /> Recebimento
        </button>
        <button
          onClick={() => setModal({ type: 'payable', editing: null })}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded bg-red-900 hover:bg-red-800 transition text-xs font-medium text-white"
        >
          <Plus size={13} /> Despesa
        </button>
        <button
          onClick={() => setModal({ type: 'card', editing: null })}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded border border-white/20 hover:bg-white/5 transition text-xs font-medium text-gray-300"
        >
          <CreditCard size={13} /> Cartão
        </button>

        <div className="ml-auto flex items-center gap-1">
          <Filter size={13} className="text-gray-500 mr-1" />
          {(Object.keys(FILTER_LABELS) as FilterKind[]).map((k) => (
            <button
              key={k}
              onClick={() => setFilter(k)}
              className={`px-2.5 py-1 rounded text-xs font-medium transition ${
                filter === k
                  ? 'bg-amber-700/60 text-amber-200'
                  : 'text-gray-500 hover:text-gray-300 hover:bg-white/5'
              }`}
            >
              {FILTER_LABELS[k]}
            </button>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5 items-start">
        {/* LEFT — Extrato */}
        <div
          className="rounded-xl border border-white/5 overflow-hidden flex flex-col shadow-2xl"
          style={{ backgroundColor: 'rgba(0,0,0,0.22)' }}
        >
          <div
            className="px-4 py-3 border-b border-white/5 flex items-center gap-2"
            style={{ backgroundColor: 'rgba(255,255,255,0.03)' }}
          >
            <Calendar size={15} className="text-gray-400" />
            <span className="text-sm font-semibold">
              Movimentação — {view.month}/{view.year}
            </span>
            <span className="ml-auto text-xs text-gray-500">
              {rows.length} item{rows.length !== 1 ? 's' : ''}
            </span>
          </div>

          {rows.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-14 gap-2">
              <Calendar size={32} className="text-gray-600" />
              <p className="text-gray-500 text-sm">Nenhuma movimentação</p>
              <p className="text-gray-600 text-xs">
                Use os botões acima para adicionar.
              </p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead
                  className="text-xs text-gray-400 border-b border-white/5"
                  style={{ backgroundColor: 'rgba(255,255,255,0.02)' }}
                >
                  <tr>
                    <th className="px-4 py-2.5 font-medium">Descrição</th>
                    <th className="px-4 py-2.5 font-medium">Data</th>
                    <th className="px-4 py-2.5 font-medium text-right">Total</th>
                    <th className="w-8" />
                  </tr>
                </thead>
                <tbody className="divide-y divide-white/5">
                  {rows.map((row) => {
                    const isCard = row.kind === 'card';
                    const isReceivable = row.kind === 'receivable';
                    const isSelected =
                      isCard && selectedCard?.id === (row.data as CardExpenseView).id;

                    const date =
                      row.kind === 'receivable'
                        ? BR_DATE((row.data as ReceivableView).date)
                        : BR_DATE(
                            (row.data as ImmediateExpenseView | CardExpenseView).dueDate
                          );

                    return (
                      <tr
                        key={row.data.id}
                        onClick={() =>
                          isCard &&
                          setSelectedCard(
                            isSelected ? null : (row.data as CardExpenseView)
                          )
                        }
                        className={`transition-colors group ${isCard ? 'cursor-pointer' : ''} ${
                          isSelected
                            ? 'bg-blue-900/30'
                            : isCard
                            ? 'bg-blue-900/5 hover:bg-blue-900/15'
                            : 'hover:bg-white/5'
                        }`}
                      >
                        <td className="px-4 py-2.5">
                          <div className="flex items-center gap-2">
                            {isReceivable ? (
                              <ArrowUpCircle size={14} className="text-green-500 shrink-0" />
                            ) : isCard ? (
                              <CreditCard size={14} className="text-blue-400 shrink-0" />
                            ) : (
                              <ArrowDownCircle size={14} className="text-red-500 shrink-0" />
                            )}
                            <span className="font-medium text-sm truncate max-w-[160px]">
                              {row.data.description}
                            </span>
                          </div>
                        </td>
                        <td className="px-4 py-2.5 text-xs text-gray-400 whitespace-nowrap">
                          {date}
                        </td>
                        <td
                          className={`px-4 py-2.5 text-right text-xs font-mono font-semibold whitespace-nowrap ${
                            isReceivable ? 'text-green-400' : 'text-gray-100'
                          }`}
                        >
                          {isReceivable ? '+' : '-'} {BR_CURRENCY.format(row.data.total)}
                          {isCard && (
                            <ChevronRight size={11} className="inline ml-0.5 text-gray-500" />
                          )}
                        </td>
                        <td className="pr-3">
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              if (row.kind === 'receivable')
                                setModal({
                                  type: 'receivable',
                                  editing: row.data as ReceivableView,
                                });
                              else if (row.kind === 'payable')
                                setModal({
                                  type: 'payable',
                                  editing: row.data as ImmediateExpenseView,
                                });
                              else
                                setModal({
                                  type: 'card',
                                  editing: row.data as CardExpenseView,
                                });
                            }}
                            className="opacity-0 group-hover:opacity-100 text-gray-400 hover:text-white transition-opacity"
                          >
                            <Pencil size={12} />
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}

          {/* Footer */}
          <div
            className="px-4 py-3 border-t border-white/10 grid grid-cols-3 gap-2 text-xs"
            style={{ backgroundColor: 'rgba(0,0,0,0.32)' }}
          >
            <div className="text-center">
              <p className="text-gray-400 mb-0.5">Recebido</p>
              <p className="text-green-400 font-bold font-mono">
                {BR_CURRENCY.format(totalReceivable)}
              </p>
            </div>
            <div className="text-center">
              <p className="text-gray-400 mb-0.5">Gasto</p>
              <p className="text-red-400 font-bold font-mono">
                {BR_CURRENCY.format(totalPayable)}
              </p>
            </div>
            <div className="text-center">
              <p className="text-gray-400 mb-0.5">Saldo</p>
              <p
                className={`font-bold font-mono ${
                  balance >= 0 ? 'text-blue-400' : 'text-orange-400'
                }`}
              >
                {BR_CURRENCY.format(balance)}
              </p>
            </div>
          </div>
        </div>

        {/* RIGHT — Card detail panel */}
        <div
          className={`transition-all duration-300 ${
            selectedCard
              ? 'opacity-100 translate-x-0'
              : 'opacity-0 translate-x-6 pointer-events-none'
          }`}
        >
          {selectedCard && (
            <div
              className="rounded-xl border overflow-hidden flex flex-col shadow-2xl"
              style={{ backgroundColor: 'rgba(0,0,0,0.22)', borderColor: '#3b82f622' }}
            >
              <div
                className="px-4 py-3 border-b border-white/5 flex items-center gap-2"
                style={{ backgroundColor: 'rgba(59,130,246,0.07)' }}
              >
                <CreditCard size={15} className="text-blue-400 shrink-0" />
                <span className="text-sm font-semibold text-blue-200 truncate flex-1">
                  {selectedCard.description}
                </span>
                <span className="text-xs text-gray-500 whitespace-nowrap">
                  Venc. {BR_DATE(selectedCard.dueDate)}
                </span>
                <button
                  onClick={() =>
                    setModal({
                      type: 'cardItem',
                      cardId: selectedCard.id,
                      editing: null,
                    })
                  }
                  className="flex items-center gap-1 text-xs px-2 py-1 rounded bg-blue-900/40 hover:bg-blue-800/50 text-blue-300 transition ml-2 whitespace-nowrap"
                >
                  <Plus size={11} /> Item
                </button>
                <button
                  onClick={() => setModal({ type: 'card', editing: selectedCard })}
                  className="text-gray-500 hover:text-white transition ml-1"
                >
                  <Pencil size={13} />
                </button>
                <button
                  onClick={() => setSelectedCard(null)}
                  className="text-gray-500 hover:text-white transition ml-1"
                >
                  <X size={15} />
                </button>
              </div>

              {selectedCard.items.length === 0 ? (
                <div className="flex flex-col items-center justify-center py-12 gap-2">
                  <CreditCard size={28} className="text-gray-600" />
                  <p className="text-gray-500 text-sm">Nenhum item na fatura</p>
                  <button
                    onClick={() =>
                      setModal({
                        type: 'cardItem',
                        cardId: selectedCard.id,
                        editing: null,
                      })
                    }
                    className="mt-1 text-xs text-blue-400 hover:text-blue-300 transition"
                  >
                    + Adicionar primeiro item
                  </button>
                </div>
              ) : (
                <div className="max-h-[420px] overflow-auto">
                  <table className="w-full text-left text-sm">
                    <thead
                      className="text-xs text-gray-400 border-b border-white/5 sticky top-0"
                      style={{ backgroundColor: '#0f0d0b' }}
                    >
                      <tr>
                        <th className="px-4 py-2.5 font-medium">Item</th>
                        <th className="px-4 py-2.5 font-medium">Data</th>
                        <th className="px-4 py-2.5 font-medium text-right">Total</th>
                        <th className="w-8" />
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-white/5">
                      {selectedCard.items.map((item) => {
                        const label =
                          item.quantityInstallments > 1
                            ? `${item.description} (${item.currentInstallment}/${item.quantityInstallments})`
                            : item.description;
                        return (
                          <tr
                            key={item.id}
                            className="hover:bg-white/5 transition-colors group"
                          >
                            <td
                              className="px-4 py-2.5 text-xs truncate max-w-[160px]"
                              title={item.description}
                            >
                              {label}
                            </td>
                            <td className="px-4 py-2.5 text-xs text-gray-400 whitespace-nowrap">
                              {BR_DATE(item.dueDate)}
                            </td>
                            <td className="px-4 py-2.5 text-right text-xs font-mono text-gray-200 whitespace-nowrap">
                              {BR_CURRENCY.format(item.total)}
                            </td>
                            <td className="pr-3">
                              <button
                                onClick={() =>
                                  setModal({
                                    type: 'cardItem',
                                    cardId: selectedCard.id,
                                    editing: item,
                                  })
                                }
                                className="opacity-0 group-hover:opacity-100 text-gray-400 hover:text-white transition-opacity"
                              >
                                <Pencil size={11} />
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              )}

              <div
                className="px-4 py-3 border-t border-white/10 flex items-center justify-between"
                style={{ backgroundColor: 'rgba(30,58,138,0.12)' }}
              >
                <span className="text-xs text-blue-300">
                  {selectedCard.items.length} item
                  {selectedCard.items.length !== 1 ? 's' : ''}
                </span>
                <div className="text-right">
                  <p className="text-xs text-gray-400">Total da Fatura</p>
                  <p className="text-base font-bold font-mono text-white">
                    {BR_CURRENCY.format(selectedCard.total)}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Modals */}
      {modal?.type === 'receivable' && (
        <ReceivableModal
          isOpen
          onClose={closeModal}
          onSuccess={handleSuccess}
          effectiveDateId={effectiveDateId}
          editing={modal.editing}
        />
      )}
      {modal?.type === 'payable' && (
        <ImmediateExpenseModal
          isOpen
          onClose={closeModal}
          onSuccess={handleSuccess}
          effectiveDateId={effectiveDateId}
          editing={modal.editing}
        />
      )}
      {modal?.type === 'card' && (
        <CardExpenseModal
          isOpen
          onClose={closeModal}
          onSuccess={handleSuccess}
          effectiveDateId={effectiveDateId}
          editing={modal.editing}
        />
      )}
      {modal?.type === 'cardItem' && (
        <CardExpenseItemModal
          isOpen
          onClose={closeModal}
          onSuccess={handleSuccess}
          cardExpenseId={modal.cardId}
          editing={modal.editing}
        />
      )}
    </>
  );
}
