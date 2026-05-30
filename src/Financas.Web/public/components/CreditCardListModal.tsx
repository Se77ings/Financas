import { useEffect, useState } from "react";
import CreditCardView from "@/models/CreditCard/CreditCardView";
import { getCreditCardsAsync, addCreditCardAsync, updateCreditCardAsync, deleteCreditCardAsync, CreditCardRequest } from "@/services/creditCardServices";
import CreditCardForm from "@/forms/CreditCardForm";

interface CreditCardListModalProps {
    onClose: () => void;
}

export default function CreditCardListModal({ onClose }: CreditCardListModalProps) {
    const [cards, setCards] = useState<CreditCardView[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Form modal state
    const [showForm, setShowForm] = useState(false);
    const [editingCard, setEditingCard] = useState<CreditCardView | null>(null);
    const [isSaving, setIsSaving] = useState(false);

    const loadCards = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await getCreditCardsAsync();
            setCards(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Erro ao carregar cartões");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadCards();
    }, []);

    const handleSave = async (data: CreditCardRequest) => {
        setIsSaving(true);
        try {
            if (editingCard) {
                await updateCreditCardAsync(editingCard.id, data);
            } else {
                await addCreditCardAsync(data);
            }
            await loadCards();
            setShowForm(false);
            setEditingCard(null);
        } catch (err) {
            throw err;
        } finally {
            setIsSaving(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (confirm("Tem certeza que deseja remover este cartão? Faturas vinculadas e recorrências serão afetadas pela política do banco de dados.")) {
            try {
                await deleteCreditCardAsync(id);
                await loadCards();
            } catch (err) {
                alert(err instanceof Error ? err.message : "Erro ao remover cartão");
            }
        }
    };

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 backdrop-blur-sm">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-[650px] max-w-full shadow-2xl flex flex-col max-h-[85vh] overflow-hidden animate-in fade-in zoom-in duration-200">
                
                {/* Header */}
                <div className="p-4 border-b border-white/5 flex justify-between items-center bg-[#151413] shrink-0">
                    <h3 className="font-bold text-sm uppercase tracking-wider text-blue-400 flex items-center gap-2">
                        💳 Configuração de Cartões de Crédito
                    </h3>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition cursor-pointer"
                    >
                        ✕
                    </button>
                </div>

                {/* Main Content Area */}
                <div className="flex-grow p-5 overflow-y-auto min-h-0">
                    {error && (
                        <div className="p-3 mb-4 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <div className="flex justify-between items-center mb-4">
                        <p className="text-xs text-gray-400">
                            Configure seus cartões ativos. Faturas e recorrências serão geradas na abertura da competência.
                        </p>
                        <button
                            onClick={() => {
                                setEditingCard(null);
                                setShowForm(true);
                            }}
                            className="px-3 py-1.5 bg-blue-700 hover:bg-blue-600 transition rounded text-xs font-bold shadow cursor-pointer text-white flex items-center gap-1 shrink-0"
                        >
                            ➕ NOVO CARTÃO
                        </button>
                    </div>

                    {isLoading ? (
                        <div className="text-center py-12 text-gray-500 text-sm">Carregando cartões...</div>
                    ) : cards.length === 0 ? (
                        <div className="text-center py-16 border-2 border-dashed border-white/5 rounded-lg text-gray-500 text-sm italic">
                            Nenhum cartão cadastrado no momento.
                        </div>
                    ) : (
                        <div className="border border-white/5 rounded-lg overflow-hidden bg-black/20">
                            <table className="w-full text-left text-sm">
                                <thead className="text-[10px] text-gray-500 uppercase border-b border-white/5 bg-[#151413]">
                                    <tr>
                                        <th className="px-4 py-2.5">Nome</th>
                                        <th className="px-4 py-2.5 text-center">Fechamento / Vencimento</th>
                                        <th className="px-4 py-2.5 text-right">Limite</th>
                                        <th className="px-4 py-2.5 text-center">Status</th>
                                        <th className="px-4 py-2.5 w-20"></th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-white/5 font-mono text-xs">
                                    {cards.map((card) => (
                                        <tr key={card.id} className="hover:bg-white/5">
                                            <td className="px-4 py-3 font-semibold text-gray-200 font-sans">{card.name}</td>
                                            <td className="px-4 py-3 text-center text-gray-400">Dia {card.closingDay} / Dia {card.dueDay}</td>
                                            <td className="px-4 py-3 text-right text-gray-200 font-bold">R$ {card.limit.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</td>
                                            <td className="px-4 py-3 text-center">
                                                {card.isActive ? (
                                                    <span className="px-2 py-0.5 rounded-full text-[9px] bg-green-900/30 text-green-400 border border-green-500/20 uppercase font-sans font-bold">Ativo</span>
                                                ) : (
                                                    <span className="px-2 py-0.5 rounded-full text-[9px] bg-red-900/30 text-red-400 border border-red-500/20 uppercase font-sans font-bold">Inativo</span>
                                                )}
                                            </td>
                                            <td className="px-4 py-3 text-center">
                                                <div className="flex gap-2 justify-center">
                                                    <button
                                                        onClick={() => {
                                                            setEditingCard(card);
                                                            setShowForm(true);
                                                        }}
                                                        className="text-blue-400 hover:text-blue-300 transition text-sm cursor-pointer"
                                                        title="Editar"
                                                    >
                                                        ✏️
                                                    </button>
                                                    <button
                                                        onClick={() => handleDelete(card.id)}
                                                        className="text-red-400 hover:text-red-300 transition text-sm cursor-pointer"
                                                        title="Remover"
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
                    )}
                </div>

                {/* Footer */}
                <div className="p-4 border-t border-white/5 bg-[#151413] flex justify-end shrink-0">
                    <button
                        onClick={onClose}
                        className="px-4 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 transition text-xs font-semibold cursor-pointer"
                    >
                        Fechar
                    </button>
                </div>

                {/* Inner Form Modal */}
                {showForm && (
                    <CreditCardForm
                        initialData={editingCard}
                        onSubmit={handleSave}
                        onClose={() => {
                            setShowForm(false);
                            setEditingCard(null);
                        }}
                        isLoading={isSaving}
                    />
                )}
            </div>
        </div>
    );
}
