"use client";

import { useState, useEffect, useMemo, Suspense } from 'react';
import { useSearchParams, useRouter } from "next/navigation";

// Imports das Views/Models conforme solicitado
import EffectiveDateView from '@/models/EffectiveDate/EffectiveDateView';
import ReceivableSummaryView from '@/models/Receivable/ReceivableSummary';
import ImmediateExpenseSummary from '@/models/ImmediateExpense/ImmediateExpenseSummary';
import CardExpenseSummary from '@/models/CardExpense/CardExpenseSummary';
import CardExpenseItemSummary from '@/models/CardExpense/Item/CardExpenseItemSummary';
import { getEffectiveDateByIdAsync, syncMissingCardInvoicesAsync, getEligibleEffectiveDatesForTransferAsync } from '@/services/effectiveDateServices';
import { getCardExpenseAsync, addCardExpenseAsync, updateCardExpenseAsync, deleteCardExpenseAsync, addCardExpenseItemAsync, updateCardExpenseItemAsync, deleteCardExpenseItemAsync, transferCardExpenseItemsAsync, CardExpenseRequest, CardExpenseItemRequest } from '@/services/cardExpenseServices';
import EffectiveDateSummary from '@/models/EffectiveDate/EffectiveDateSummary';
import { addReceivableAsync, updateReceivableAsync, deleteReceivableAsync, ReceivableRequest } from '@/services/receivableServices';
import { addImmediateExpenseAsync, updateImmediateExpenseAsync, deleteImmediateExpenseAsync, ImmediateExpenseRequest } from '@/services/immediateExpenseServices';
import ReceivableForm from '@/forms/ReceivableForm';
import ImmediateExpenseForm from '@/forms/ImmediateExpenseForm';
import CardExpenseForm from '@/forms/CardExpenseForm';
import CardExpenseItemForm from '@/forms/CardExpenseItemForm';
import { formatDateBR } from '@/utils/dateFormat';

// Definição dos SVGs para evitar dependências externas
const Icons = {
    Plus: () => (
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
    ),
    ArrowUp: () => (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#22c55e" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="16 12 12 8 8 12"></polyline><line x1="12" y1="16" x2="12" y2="8"></line></svg>
    ),
    ArrowDown: () => (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#ef4444" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="8 12 12 16 16 12"></polyline><line x1="12" y1="8" x2="12" y2="16"></line></svg>
    ),
    Card: (props: { color?: string }) => (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke={props.color || "#60a5fa"} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="2" y="5" width="20" height="14" rx="2"></rect><line x1="2" y1="10" x2="22" y2="10"></line></svg>
    ),
    ChevronRight: () => (
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
    ),
    X: () => (
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
    ),
    Calendar: () => (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
    )
};

export default function EffectiveDateDetails() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const id = searchParams?.get("id");

    const [viewData, setViewData] = useState<EffectiveDateView | null>(null);
    const [selectedCardId, setSelectedCardId] = useState<string | null>(null);
    const [selectedCardData, setSelectedCardData] = useState<CardExpenseSummary | null>(null);
    const [isLoadingCard, setIsLoadingCard] = useState(false);
    
    // Multi-selection & Transfer states
    const [selectedItemIds, setSelectedItemIds] = useState<string[]>([]);
    const [showTransferModal, setShowTransferModal] = useState(false);
    const [eligibleCompetencies, setEligibleCompetencies] = useState<EffectiveDateSummary[]>([]);
    const [selectedTargetCompetencyId, setSelectedTargetCompetencyId] = useState<string>("");
    const [isTransferring, setIsTransferring] = useState(false);
    
    // Form states
    const [showReceivableForm, setShowReceivableForm] = useState(false);
    const [editingReceivable, setEditingReceivable] = useState<any>(null);
    const [showExpenseForm, setShowExpenseForm] = useState(false);
    const [editingExpense, setEditingExpense] = useState<any>(null);
    const [isOperating, setIsOperating] = useState(false);
    
    // Card states
    const [showCardForm, setShowCardForm] = useState(false);
    const [editingCard, setEditingCard] = useState<any>(null);
    const [showCardItemForm, setShowCardItemForm] = useState(false);
    const [editingCardItem, setEditingCardItem] = useState<any>(null);
    const [isSyncing, setIsSyncing] = useState(false);

    const handleSyncCards = async () => {
        if (!id) return;
        setIsSyncing(true);
        try {
            await syncMissingCardInvoicesAsync(id);
            await reloadData();
            alert("Cartões sincronizados com sucesso!");
        } catch (error) {
            alert(`Erro ao sincronizar cartões: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
        } finally {
            setIsSyncing(false);
        }
    };

    const reloadData = async () => {
        if (id) {
            try {
                const view = await getEffectiveDateByIdAsync(id);
                setViewData(view);
            } catch (error) {
                console.error('Erro ao recarregar dados:', error);
            }
        }
    };

    useEffect(() => {
        setSelectedCardId(null);
        setSelectedCardData(null);
        setSelectedItemIds([]);
        reloadData();
    }, [id]);

    // Método para mockar os dados da View conforme as classes fornecidas
    function loadEffectiveDateData(id: string) {
        // Simulando Recebimentos
        const recs = [
            new ReceivableSummaryView("r1", "Salário Mensal", 5500.00, "05/03/2026"),
            new ReceivableSummaryView("r2", "Reembolso", 150.00, "10/03/2026"),
        ];

        // Simulando Gastos à Vista
        const imms = [
            new ImmediateExpenseSummary("i1", "Internet", 89.90, "07/12/2022", 89.90, 0, "", id),
            new ImmediateExpenseSummary("i2", "Água e Luz", 157.75, "07/12/2022", 157.75, 0, "", id),
            new ImmediateExpenseSummary("i3", "Presente Marina", 120.00, "", 120.00, 0, "", id),
            new ImmediateExpenseSummary("i4", "Rd Saude (Lidocaina)", 11.69, "13/04/2026", 11.69, 0, "", id),
        ];

        // Simulando Itens de Cartão (Inter)
        const cardItemsInter = [
            new CardExpenseItemSummary("ci1", "Amazon", 19.90, "27/02/2026", 0, "", 1, 1, true, "Amazon"),
            new CardExpenseItemSummary("ci2", "Vivo Easy", 38.00, "01/03/2026", 0, "", 1, 1, true, "Vivo Easy"),
            new CardExpenseItemSummary("ci3", "AppleBill", 9.99, "02/03/2026", 0, "", 1, 1, true, "AppleBill"),
        ];

        // Simulando Resumo de Cartões
        const cards = [
            new CardExpenseSummary("c1", "Inter", 1023.14, "07/12/2023", 1023.14, 0, "", cardItemsInter),
            new CardExpenseSummary("c2", "PicpayCard", 334.90, "07/12/2023", 334.90, 0, "", []),
            new CardExpenseSummary("c3", "C6 Bank", 540.51, "07/12/2026", 540.51, 0, "", []),
        ];

        const view = new EffectiveDateView(id, 3, 2026, recs, imms, cards);
        setViewData(view);
    }

    const reloadSelectedCard = async (cardId: string) => {
        try {
            const cardData = await getCardExpenseAsync(cardId);
            setSelectedCardData(cardData);
        } catch (error) {
            console.error('Erro ao recarregar cartão:', error);
        }
    };

    const handleCardClick = async (cardId: string) => {
        if (selectedCardId === cardId) {
            setSelectedCardId(null);
            setSelectedCardData(null);
            setSelectedItemIds([]);
            return;
        }
        
        setSelectedCardId(cardId);
        setSelectedItemIds([]);
        setIsLoadingCard(true);
        try {
            const cardData = await getCardExpenseAsync(cardId);
            setSelectedCardData(cardData);
        } catch (error) {
            console.error('Erro ao carregar cartão:', error);
            setSelectedCardId(null);
        } finally {
            setIsLoadingCard(false);
        }
    };

    const handleOpenTransferModal = async () => {
        try {
            const list = await getEligibleEffectiveDatesForTransferAsync();
            // Filter out current competency
            setEligibleCompetencies(list.filter(c => c.id !== id));
            setShowTransferModal(true);
        } catch (error) {
            alert(`Erro ao buscar competências: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
        }
    };

    const handleConfirmTransfer = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedTargetCompetencyId || selectedItemIds.length === 0) return;
        
        setIsTransferring(true);
        try {
            await transferCardExpenseItemsAsync(selectedItemIds, selectedTargetCompetencyId);
            
            await reloadData();
            if (selectedCardId) {
                await reloadSelectedCard(selectedCardId);
            }
            
            setSelectedItemIds([]);
            setShowTransferModal(false);
            setSelectedTargetCompetencyId("");
            alert("Itens transferidos com sucesso!");
        } catch (error) {
            alert(`Erro ao transferir itens: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
        } finally {
            setIsTransferring(false);
        }
    };

    const handleSaveReceivable = async (data: ReceivableRequest) => {
        try {
            if (editingReceivable?.id) {
                await updateReceivableAsync(editingReceivable.id, data);
            } else {
                await addReceivableAsync(data);
            }
            await reloadData();
        } catch (error) {
            throw error;
        }
    };

    const handleDeleteReceivable = async (itemId: string) => {
        if (confirm('Tem certeza que deseja deletar este recebimento?')) {
            setIsOperating(true);
            try {
                await deleteReceivableAsync(itemId);
                await reloadData();
            } catch (error) {
                alert(`Erro ao deletar: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
            } finally {
                setIsOperating(false);
            }
        }
    };

    const handleSaveExpense = async (data: ImmediateExpenseRequest) => {
        try {
            if (editingExpense?.id) {
                await updateImmediateExpenseAsync(editingExpense.id, data);
            } else {
                await addImmediateExpenseAsync(data);
            }
            await reloadData();
        } catch (error) {
            throw error;
        }
    };

    const handleDeleteExpense = async (itemId: string) => {
        if (confirm('Tem certeza que deseja deletar este gasto?')) {
            setIsOperating(true);
            try {
                await deleteImmediateExpenseAsync(itemId);
                await reloadData();
            } catch (error) {
                alert(`Erro ao deletar: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
            } finally {
                setIsOperating(false);
            }
        }
    };

    const handleSaveCardItem = async (data: CardExpenseItemRequest) => {
        if (!selectedCardId) return;
        try {
            if (editingCardItem?.id) {
                await updateCardExpenseItemAsync(selectedCardId, editingCardItem.id, data);
            } else {
                await addCardExpenseItemAsync(selectedCardId, data);
            }
            await reloadData();
            await reloadSelectedCard(selectedCardId);
        } catch (error) {
            throw error;
        }
    };

    const handleSaveCard = async (data: CardExpenseRequest) => {
        const cardId = editingCard?.id;
        try {
            if (cardId) {
                await updateCardExpenseAsync(cardId, data);
            } else {
                await addCardExpenseAsync(data);
            }
            await reloadData();
            if (cardId && selectedCardId === cardId) {
                await reloadSelectedCard(cardId);
            }
        } catch (error) {
            throw error;
        }
    };

    const handleDeleteCardItem = async (itemId: string) => {
        if (!selectedCardId) return;
        if (confirm('Tem certeza que deseja deletar este item da fatura?')) {
            setIsOperating(true);
            try {
                await deleteCardExpenseItemAsync(selectedCardId, itemId);
                await reloadData();
                await reloadSelectedCard(selectedCardId);
            } catch (error) {
                alert(`Erro ao deletar: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
            } finally {
                setIsOperating(false);
            }
        }
    };

    const selectedCard = selectedCardData || viewData?.cardExpenses.find(c => c.id === selectedCardId);

    const totals = useMemo(() => {
        if (!viewData) return { received: 0, spent: 0, balance: 0 };
        const received = viewData.receivables.reduce((acc, curr) => acc + curr.total, 0);
        const spent = viewData.immediateExpenses.reduce((acc, curr) => acc + curr.total, 0) +
            viewData.cardExpenses.reduce((acc, curr) => acc + curr.total, 0);
        return { received, spent, balance: received - spent };
    }, [viewData]);

    const selectedTotal = useMemo(() => {
        if (!selectedCard?.items) return 0;
        return selectedCard.items
            .filter(item => selectedItemIds.includes(item.id))
            .reduce((sum, item) => sum + item.total, 0);
    }, [selectedCard, selectedItemIds]);

    if (!id) return <div className="p-8 text-white">ID não informado.</div>;
    if (!viewData) return <div className="p-8 text-white">Carregando...</div>;

    return (
        <div className="flex flex-col flex-1 min-h-0 w-full min-w-0 h-full overflow-hidden bg-[#262521] text-gray-100">

            <header className="shrink-0 w-full p-4 flex flex-wrap items-center justify-between gap-4 border-b border-white/5 bg-[#1a1917]">
                <h1 className="text-xl font-bold flex items-center gap-2">
                    Competência: <span className="text-gray-400">{viewData.monthAbbreviation}-{viewData.year.toString().slice(-2)}</span>
                </h1>

                <div className="flex gap-2">
                    <button
                        onClick={() => {
                            setEditingReceivable(null);
                            setShowReceivableForm(true);
                        }}
                        className="flex items-center gap-2 px-4 py-1.5 rounded bg-green-700 hover:bg-green-600 transition text-[11px] font-bold shadow-lg"
                    >
                        <Icons.Plus /> RECEBIMENTO
                    </button>
                    <button
                        onClick={() => {
                            setEditingExpense(null);
                            setShowExpenseForm(true);
                        }}
                        className="flex items-center gap-2 px-4 py-1.5 rounded bg-[#59161C] hover:bg-[#7a1e26] transition text-[11px] font-bold shadow-lg"
                    >
                        <Icons.Plus /> GASTO
                    </button>
                    <button
                        onClick={handleSyncCards}
                        disabled={isSyncing}
                        className="flex items-center gap-2 px-4 py-1.5 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed transition text-[11px] font-bold shadow-lg text-white cursor-pointer"
                    >
                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" className={isSyncing ? "animate-spin" : ""}>
                            <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.67"/>
                        </svg>
                        {isSyncing ? 'SINCRONIZANDO...' : 'SINCRONIZAR CARTÕES'}
                    </button>
                </div>
            </header>

            {/* ÁREA DE TABELAS */}
            <div className="flex flex-1 min-h-0 w-full min-w-0 flex-col lg:flex-row gap-6 overflow-hidden p-4 md:p-6">

                {/* TABELA ESQUERDA: MOVIMENTAÇÃO */}
                <div className="flex flex-1 min-h-0 min-w-0 w-full flex-col rounded-xl bg-[#1a1917] border border-white/5 shadow-2xl overflow-hidden">
                    <div className="shrink-0 p-4 border-b border-white/5 flex items-center gap-2">
                        <Icons.Calendar />
                        <h3 className="font-semibold text-sm uppercase tracking-wider text-gray-300">Movimentação do Mês</h3>
                    </div>

                     <div className="flex-1 min-h-0 min-w-0 overflow-y-auto overflow-x-auto bg-[#1a1917]">
                        <table className="w-full min-w-full text-left text-sm">
                            <thead className="text-[10px] text-gray-500 uppercase border-b border-white/5 bg-[#1a1917]">
                                <tr>
                                    <th className="px-4 py-3">Descrição</th>
                                    <th className="px-4 py-3">Data</th>
                                    <th className="px-4 py-3 text-right">Valor</th>
                                    <th className="px-4 py-3 w-16"></th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-white/5 ">
                                {/* Recebimentos */}
                                {viewData.receivables.map(item => (
                                    <tr key={item.id} className="hover:bg-white/5 group">
                                        <td className="px-4 py-4 flex items-center gap-3">
                                            <Icons.ArrowUp /> <span className="font-medium">{item.description}</span>
                                        </td>
                                        <td className="px-4 py-4 text-gray-500 font-mono">{formatDateBR(item.date)}</td>
                                        <td className="px-4 py-4 text-right font-bold text-green-500 font-mono">
                                            + R$ {item.total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                        </td>
                                        <td className="px-4 py-4 text-center">
                                            <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition justify-center">
                                                <button
                                                    type="button"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        setEditingReceivable(item);
                                                        setShowReceivableForm(true);
                                                    }}
                                                    className="p-1 text-blue-400 hover:bg-blue-400/10 rounded transition"
                                                    title="Editar"
                                                >
                                                    ✏️
                                                </button>
                                                <button
                                                    type="button"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleDeleteReceivable(item.id);
                                                    }}
                                                    disabled={isOperating}
                                                    className="p-1 text-red-400 hover:bg-red-400/10 rounded transition disabled:opacity-50"
                                                    title="Deletar"
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                                {/* Gastos */}
                                {viewData.immediateExpenses.map(item => (
                                    <tr key={item.id} className="hover:bg-white/5 group" >
                                        <td className="px-4 py-4 flex items-center gap-3">
                                            <Icons.ArrowDown /> <span className="font-medium">{item.description}</span>
                                        </td>
                                        <td className="px-4 py-4 text-gray-500 font-mono">{formatDateBR(item.dueDate)}</td>
                                        <td className="px-4 py-4 text-right font-bold text-white font-mono">
                                            - R$ {item.total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                        </td>
                                        <td className="px-4 py-4 text-center">
                                            <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition justify-center">
                                                <button
                                                    type="button"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        setEditingExpense(item);
                                                        setShowExpenseForm(true);
                                                    }}
                                                    className="p-1 text-blue-400 hover:bg-blue-400/10 rounded transition"
                                                    title="Editar"
                                                >
                                                    ✏️
                                                </button>
                                                <button
                                                    type="button"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleDeleteExpense(item.id);
                                                    }}
                                                    disabled={isOperating}
                                                    className="p-1 text-red-400 hover:bg-red-400/10 rounded transition disabled:opacity-50"
                                                    title="Deletar"
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                                {/* Cartões */}
                                {viewData.cardExpenses.map(item => (
                                    <tr
                                        key={item.id}
                                        onClick={() => handleCardClick(item.id)}
                                        className={`cursor-pointer group ${selectedCardId === item.id ? 'bg-[#1e293b]' : 'hover:bg-white/5'}`}
                                    >
                                        <td className="px-4 py-4 flex items-center gap-3">
                                            <Icons.Card /> <span className="font-medium">{item.description}</span>
                                        </td>
                                        <td className="px-4 py-4 text-gray-500 font-mono">{formatDateBR(item.dueDate)}</td>
                                        <td className="px-4 py-4 text-right font-bold text-white font-mono">
                                            <div className="flex items-center justify-end gap-2">
                                                - R$ {item.total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                                <div className={selectedCardId === item.id ? 'rotate-90' : ''}><Icons.ChevronRight /></div>
                                            </div>
                                        </td>
                                        <td className="px-4 py-4 text-center" onClick={(e) => e.stopPropagation()}>
                                            <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition justify-center">
                                                <button
                                                    type="button"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        setEditingCard(item);
                                                        setShowCardForm(true);
                                                    }}
                                                    className="p-1 text-blue-400 hover:bg-blue-400/10 rounded transition"
                                                    title="Editar fatura"
                                                >
                                                    ✏️
                                                </button>
                                                <button
                                                    type="button"
                                                    onClick={async (e) => {
                                                        e.stopPropagation();
                                                        if (!confirm('Tem certeza que deseja deletar esta fatura de cartão?')) return;
                                                        setIsOperating(true);
                                                        try {
                                                            await deleteCardExpenseAsync(item.id);
                                                            if (selectedCardId === item.id) {
                                                                setSelectedCardId(null);
                                                                setSelectedCardData(null);
                                                            }
                                                            await reloadData();
                                                        } catch (error) {
                                                            alert(`Erro ao deletar: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
                                                        } finally {
                                                            setIsOperating(false);
                                                        }
                                                    }}
                                                    disabled={isOperating}
                                                    className="p-1 text-red-400 hover:bg-red-400/10 rounded transition disabled:opacity-50"
                                                    title="Deletar fatura"
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    {/* RODAPÉ TOTAIS — shrink-0 mantém altura fixa; só a tabela rola acima */}
                    <div className="shrink-0 p-4 bg-black/40 border-t border-white/10 grid grid-cols-3 gap-2">
                        <div className="text-center">
                            <p className="text-[10px] text-gray-500 uppercase">Recebido</p>
                            <p className="text-green-500 font-bold text-sm">R$ {totals.received.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</p>
                        </div>
                        <div className="text-center border-x border-white/5">
                            <p className="text-[10px] text-gray-500 uppercase">Gasto</p>
                            <p className="text-red-500 font-bold text-sm">R$ {totals.spent.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</p>
                        </div>
                        <div className="text-center">
                            <p className="text-[10px] text-gray-500 uppercase">Saldo</p>
                            <p className="text-blue-400 font-bold text-sm">R$ {totals.balance.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</p>
                        </div>
                    </div>
                </div>

                {/* TABELA DIREITA: DETALHE CARTÃO */}
                {selectedCard || isLoadingCard ? (
                    <div className="flex flex-1 min-h-0 min-w-0 w-full flex-col rounded-xl bg-[#1a1917] border border-blue-500/20 shadow-2xl overflow-hidden">
                        <div className="shrink-0 p-4 bg-blue-900/10 border-b border-white/5 flex justify-between items-center gap-2">
                            <h3 className="font-semibold text-sm uppercase tracking-wider text-blue-300 flex items-center gap-2">
                                <Icons.Card color="#93c5fd" /> Detalhes Fatura: {selectedCard?.description}
                            </h3>
                            <div className="flex items-center gap-2">
                                {selectedItemIds.length > 0 && (
                                    <button
                                        type="button"
                                        onClick={handleOpenTransferModal}
                                        className="flex items-center justify-center p-1 rounded bg-gray-800 hover:bg-gray-700 text-gray-300 hover:text-white transition text-[11px]"
                                        title="Transferência entre competências"
                                    >
                                        ⚙️
                                    </button>
                                )}
                                <button
                                    type="button"
                                    onClick={() => {
                                        setEditingCardItem(null);
                                        setShowCardItemForm(true);
                                    }}
                                    className="flex items-center gap-1 px-3 py-1 rounded bg-blue-700 hover:bg-blue-600 transition text-[10px] font-bold"
                                >
                                    <Icons.Plus /> ITEM
                                </button>
                                <button
                                    type="button"
                                    onClick={() => {
                                        setSelectedCardId(null);
                                        setSelectedCardData(null);
                                    }}
                                    className="text-gray-500 hover:text-white"
                                >
                                    <Icons.X />
                                </button>
                            </div>
                        </div>

                        <div className="flex-1 min-h-0 min-w-0 overflow-y-auto overflow-x-auto">
                            {isLoadingCard ? (
                                <div className="flex items-center justify-center h-32 text-gray-500">
                                    <p>Carregando detalhes...</p>
                                </div>
                            ) : selectedCard ? (
                                <table className="w-full min-w-full text-left text-sm">
                                    <thead className="text-[10px] text-gray-500 uppercase border-b border-white/5 sticky top-0 ">
                                        <tr>
                                            <th className="px-4 py-3 w-8">
                                                <input
                                                    type="checkbox"
                                                    checked={selectedCard.items && selectedCard.items.length > 0 && selectedCard.items.every(item => selectedItemIds.includes(item.id))}
                                                    onChange={(e) => {
                                                        if (e.target.checked) {
                                                            setSelectedItemIds(selectedCard.items?.map(item => item.id) || []);
                                                        } else {
                                                            setSelectedItemIds([]);
                                                        }
                                                    }}
                                                    className="rounded border-white/10 bg-white/5 text-blue-600 focus:ring-blue-500 cursor-pointer"
                                                />
                                            </th>
                                            <th className="px-4 py-3">Item</th>
                                            <th className="px-4 py-3">Data</th>
                                            <th className="px-4 py-3 text-right">Valor</th>
                                            <th className="px-4 py-3 w-16"></th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-white/5 font-mono">
                                        {selectedCard.items && selectedCard.items.length > 0 ? (
                                            selectedCard.items.map(item => (
                                                <tr key={item.id} className="hover:bg-white/5 group">
                                                    <td className="px-4 py-4 w-8">
                                                        <input
                                                            type="checkbox"
                                                            checked={selectedItemIds.includes(item.id)}
                                                            onChange={(e) => {
                                                                if (e.target.checked) {
                                                                    setSelectedItemIds(prev => [...prev, item.id]);
                                                                } else {
                                                                    setSelectedItemIds(prev => prev.filter(id => id !== item.id));
                                                                }
                                                            }}
                                                            className="rounded border-white/10 bg-white/5 text-blue-600 focus:ring-blue-500 cursor-pointer"
                                                        />
                                                    </td>
                                                    <td className="px-4 py-4 text-gray-200">
                                                        {item.displayDescription}
                                                        {item.quantityInstallments > 1 && (
                                                            <span className="ml-2 text-[10px] bg-white/5 px-1.5 py-0.5 rounded text-gray-400">
                                                                {item.currentInstallments}/{item.quantityInstallments}
                                                            </span>
                                                        )}
                                                    </td>
                                                    <td className="px-4 py-4 text-gray-500">{formatDateBR(item.dueDate)}</td>
                                                    <td className="px-4 py-4 text-right">R$ {item.total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</td>
                                                    <td className="px-4 py-4 text-center">
                                                        <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition justify-center">
                                                            <button
                                                                type="button"
                                                                onClick={() => {
                                                                    setEditingCardItem(item);
                                                                    setShowCardItemForm(true);
                                                                }}
                                                                className="p-1 text-blue-400 hover:bg-blue-400/10 rounded transition"
                                                                title="Editar item"
                                                            >
                                                                ✏️
                                                            </button>
                                                            <button
                                                                type="button"
                                                                onClick={() => handleDeleteCardItem(item.id)}
                                                                disabled={isOperating}
                                                                className="p-1 text-red-400 hover:bg-red-400/10 rounded transition disabled:opacity-50"
                                                                title="Deletar item"
                                                            >
                                                                🗑️
                                                            </button>
                                                        </div>
                                                    </td>
                                                </tr>
                                            ))
                                        ) : (
                                            <tr>
                                                <td colSpan={5} className="px-4 py-8 text-center text-gray-500">
                                                    Nenhum item nesta fatura
                                                </td>
                                            </tr>
                                        )}
                                    </tbody>
                                </table>
                            ) : null}
                        </div>

                        {selectedCard && (
                            <div className="shrink-0 p-5 bg-blue-950/20 border-t border-white/10 flex justify-between items-center">
                                <div className="flex flex-col gap-0.5">
                                    <span className="text-xs uppercase font-bold text-blue-400">Total Fatura</span>
                                    {selectedItemIds.length > 0 && (
                                        <span className="text-[10px] text-gray-400 font-medium">
                                            Selecionados: {selectedItemIds.length} item(ns) (R$ {selectedTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })})
                                        </span>
                                    )}
                                </div>
                                <span className="text-xl font-bold font-mono">R$ {selectedCard.total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</span>
                            </div>
                        )}
                    </div>
                ) : (
                    <div className="hidden lg:flex flex-1 min-h-0 min-w-0 w-full flex-col items-center justify-center border-2 border-dashed border-white/5 rounded-xl text-gray-600">
                        <Icons.Card color="currentColor" />
                        <p className="text-sm italic mt-2">Clique em um cartão para detalhar</p>
                    </div>
                )}

            </div>

            {/* FORMULÁRIOS MODAIS */}
            {showReceivableForm && (
                <ReceivableForm
                    key={editingReceivable?.id ?? 'new-receivable'}
                    effectiveDateId={id || ''}
                    initialData={editingReceivable}
                    onSubmit={handleSaveReceivable}
                    onClose={() => {
                        setShowReceivableForm(false);
                        setEditingReceivable(null);
                    }}
                    isLoading={isOperating}
                />
            )}

            {showExpenseForm && (
                <ImmediateExpenseForm
                    key={editingExpense?.id ?? 'new-expense'}
                    effectiveDateId={id || ''}
                    initialData={editingExpense}
                    onSubmit={handleSaveExpense}
                    onClose={() => {
                        setShowExpenseForm(false);
                        setEditingExpense(null);
                    }}
                    isLoading={isOperating}
                />
            )}

            {showCardForm && (
                <CardExpenseForm
                    key={editingCard?.id ?? 'new-card'}
                    effectiveDateId={id || ''}
                    initialData={editingCard}
                    onSubmit={handleSaveCard}
                    onClose={() => {
                        setShowCardForm(false);
                        setEditingCard(null);
                    }}
                    isLoading={isOperating}
                />
            )}

            {showCardItemForm && selectedCardId && (
                <CardExpenseItemForm
                    key={editingCardItem?.id ?? 'new-card-item'}
                    cardExpenseId={selectedCardId}
                    creditCardId={selectedCard?.creditCardId}
                    initialData={editingCardItem}
                    onSubmit={handleSaveCardItem}
                    onClose={() => {
                        setShowCardItemForm(false);
                        setEditingCardItem(null);
                    }}
                    isLoading={isOperating}
                />
            )}

            {showTransferModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
                    <div className="w-full max-w-md bg-[#1a1917] border border-white/10 rounded-xl overflow-hidden shadow-2xl">
                        <header className="px-5 py-4 border-b border-white/5 bg-[#141312] flex justify-between items-center">
                            <h3 className="font-bold text-sm uppercase text-gray-200">Transferência de Competência</h3>
                            <button onClick={() => setShowTransferModal(false)} className="text-gray-500 hover:text-white transition">
                                <Icons.X />
                            </button>
                        </header>
                        
                        <form onSubmit={handleConfirmTransfer} className="p-5 flex flex-col gap-4">
                            <div>
                                <label className="block text-xs uppercase font-bold text-gray-400 mb-2">
                                    Selecione a competência de destino
                                </label>
                                <select
                                    value={selectedTargetCompetencyId}
                                    onChange={(e) => setSelectedTargetCompetencyId(e.target.value)}
                                    required
                                    className="w-full px-3 py-2 rounded bg-black/40 border border-white/10 text-gray-100 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
                                >
                                    <option value="" disabled>Selecione...</option>
                                    {eligibleCompetencies.map(comp => (
                                        <option key={comp.id} value={comp.id}>
                                            {comp.display}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            
                            <div className="text-xs text-gray-500 leading-relaxed bg-white/5 p-3 rounded">
                                <p className="font-semibold text-gray-400 mb-1">Atenção:</p>
                                Os {selectedItemIds.length} item(ns) selecionado(s) serão movidos para a fatura do cartão <strong>{selectedCard?.description}</strong> na competência selecionada, mantendo seus valores e datas originais.
                            </div>
                            
                            <div className="flex justify-end gap-2 mt-2">
                                <button
                                    type="button"
                                    onClick={() => setShowTransferModal(false)}
                                    className="px-4 py-2 rounded border border-white/10 text-xs font-semibold hover:bg-white/5 transition text-gray-300"
                                >
                                    CANCELAR
                                </button>
                                <button
                                    type="submit"
                                    disabled={isTransferring || !selectedTargetCompetencyId}
                                    className="px-4 py-2 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed transition text-xs font-semibold text-white shadow-lg flex items-center gap-1"
                                >
                                    {isTransferring ? 'TRANSFERINDO...' : 'CONFIRMAR'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}